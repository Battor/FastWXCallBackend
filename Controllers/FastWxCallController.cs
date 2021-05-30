using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

using FastWXCallBackend.Models;
using Newtonsoft.Json;
using System.IO;
using Microsoft.EntityFrameworkCore;
using log4net;

namespace FastWXCallBackend.Controllers
{
    [ApiController]
    [Route("api/FastWxCall/[action]")]
    public class FastWxCallController : TemplateController
    {
        private FastwxcallContext context;
        private JwtHelper jwtHelper;

        private string IMAGE_DIRECTORY = Directory.GetCurrentDirectory() + Path.DirectorySeparatorChar + "PhotoImages" ;

        public FastWxCallController(FastwxcallContext context, JwtHelper jwtHelper)
        {
            this.context = context;
            this.jwtHelper = jwtHelper;
        }

        [HttpGet]
        public List<User> GetUsers()
        {
            return context.Set<Models.User>().Where(a => 1 == 1).ToList();
        }

        /// <summary>
        /// 注册
        /// </summary>
        /// <param name="userName">用户名</param>
        /// <param name="password">密码</param>
        /// <returns></returns>
        [HttpPost]
        public Result Regist([FromForm]string userName, [FromForm]string password)
        {
            if(string.IsNullOrWhiteSpace(userName) || string.IsNullOrWhiteSpace(password))
            {
                result.Code = 1;
                result.Data = "用户名或密码为空";
                return result;
            }

            var isUserNameExists = context.Set<Models.User>().Any(user => user.UserName == userName);
            if (isUserNameExists)
            {
                result.Code = 1;
                result.Data = "用户名已经被使用，请重新输入";
                return result;
            }

            string secretKey = Guid.NewGuid().ToString().Replace("-", "").Substring(0, 10);

            Models.User userEntity = new User()
            {
                Id = Guid.NewGuid().ToString(),
                UserName = userName,
                Password = CalculateMD5(password + secretKey),
                SecretKey = secretKey,
                CreateTime = DateTime.Now,
                UpdateTime = null,
                LastSignInTime = DateTime.Now,
                Enable = 1,
            };

            context.Set<User>().Add(userEntity);
            context.SaveChanges();

            return result;
        }

        [HttpPost]
        public Result Login([FromForm] string userName, [FromForm] string password)
        {
            Models.User userEntity = context.Set<Models.User>().Where(u => u.UserName == userName).FirstOrDefault();
            if(userEntity == null)
            {
                result.Code = 1;
                result.Data = "用户名或密码错误";
                return result;
            }

            string tmpPasswordMD5 = CalculateMD5(password + userEntity.SecretKey);
            if(userEntity.Password != tmpPasswordMD5)
            {
                result.Code = 1;
                result.Data = "用户名或密码错误";
                return result;
            }

            result.Data = jwtHelper.CreateToken(userEntity.Id);

            return result;
        }
        
        [HttpPut]
        [Authorize]
        public Result SaveUserContacts([FromForm] string contactsJson, [FromForm] List<IFormFile> photoImages)
        {
            string userId = User.Identity.Name;
            List<Contact> contacts = JsonConvert.DeserializeObject<List<Contact>>(contactsJson);

            if (!Directory.Exists(IMAGE_DIRECTORY))
            {
                Directory.CreateDirectory(IMAGE_DIRECTORY);
            }

            List<Contact> existsContactEntities = context.Set<Contact>().Where(contact => contact.UserId == userId).ToList();
            List<string> existsContactIds = existsContactEntities.Select(a => a.Id).ToList();

            var nowContactList = contacts.Select((contact, index) => new 
            {
                Contact = contact,
                PhotoImage = photoImages[index]
            });
            List<string> nowContactListIds = nowContactList.Select(a => a.Contact.Id).ToList();

            List<Contact> toDelContacts = existsContactEntities.Where(a => !nowContactListIds.Contains(a.Id)).ToList();
            List<dynamic> toAddContacts = nowContactList.Where(a => !existsContactIds.Contains(a.Contact.Id)).ToList<dynamic>();
            List<dynamic> toUpdateContacts = nowContactList.Where(a => existsContactIds.Contains(a.Contact.Id)).ToList<dynamic>();

            foreach(var contact in toDelContacts)
            {
                System.IO.File.Delete(contact.PhotoImagePath);
                context.Set<Contact>().Remove(contact);
            }

            foreach(var item in toAddContacts)
            {
                string nowPhotoImagePath = IMAGE_DIRECTORY + Path.DirectorySeparatorChar + item.PhotoImage.FileName;
                using (FileStream fileStream = System.IO.File.Create(nowPhotoImagePath))
                {
                    item.PhotoImage.CopyTo(fileStream);
                }
                
                Contact contact = item.Contact;
                //contact.Id
                contact.UserId = userId;
                //contact.Wxname
                //contact.HeadImgId
                contact.PhotoImagePath = nowPhotoImagePath;
                contact.CreateTime = DateTime.Now;
                contact.Enable = 1;
                context.Set<Contact>().Add(contact);
            }

            foreach(var item in toUpdateContacts)
            {
                var existsContact = existsContactEntities.Where(contact => contact.Id == item.Contact.Id).FirstOrDefault();
                string nowPhotoImagePath = IMAGE_DIRECTORY + Path.DirectorySeparatorChar + item.PhotoImage.FileName;

                if (existsContact.PhotoImagePath != nowPhotoImagePath)
                { 
                    System.IO.File.Delete(existsContact.PhotoImagePath);

                    using FileStream fileStream = System.IO.File.Create(nowPhotoImagePath);
                    item.PhotoImage.CopyTo(fileStream);
                    existsContact.PhotoImagePath = nowPhotoImagePath;
                }

                existsContact.Wxname = item.Contact.Wxname;
                existsContact.HeadImgId = item.Contact.HeadImgId;
                existsContact.UpdateTime = DateTime.Now;

                context.Set<Contact>().Attach(existsContact);
                context.Entry(existsContact).State = EntityState.Modified;
            }

            context.SaveChanges();
            
            return result;
        }

        /// <summary>
        /// 测试程序是否正常工作
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Result Test()
        {
            result.Data = "Hello World!";
            return result;
        }

        /// <summary>
        /// 测试日志是否正常记录
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Result TestLog()
        {
            Logger.Debug("日志程序正常工作");
            return result;
        }

        /// <summary>
        /// 测试异常能否处理
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public Result TestError()
        {
            throw new Exception("异常情况测试");
        }

        private string CalculateMD5(string str)
        {
            //将字符串编码为字节序列
            byte[] bt = Encoding.UTF8.GetBytes(str);
            //创建默认实现的实例
            var md5 = MD5.Create();
            //计算指定字节数组的哈希值。
            var md5bt = md5.ComputeHash(bt);
            //将byte数组转换为字符串
            StringBuilder builder = new StringBuilder();
            foreach (var item in md5bt)
            {
                builder.Append(item.ToString("X2"));
            }
            string md5Str = builder.ToString();
            return builder.ToString();
        }
    }
}
