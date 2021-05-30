using System;
using System.Collections.Generic;

#nullable disable

namespace FastWXCallBackend.Models
{
    public partial class Contact
    {
        public string Id { get; set; }
        public string UserId { get; set; }
        public string Wxname { get; set; }
        public sbyte HeadImgId { get; set; }
        public string PhotoImagePath { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpdateTime { get; set; }
        public ulong? Enable { get; set; }
    }
}
