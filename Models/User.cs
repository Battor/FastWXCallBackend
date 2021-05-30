using System;
using System.Collections.Generic;

#nullable disable

namespace FastWXCallBackend.Models
{
    public partial class User
    {
        public string Id { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string SecretKey { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public DateTime? LastSignInTime { get; set; }
        public ulong? Enable { get; set; }
    }
}
