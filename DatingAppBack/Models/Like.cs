using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppBack.Models
{
    public class Like
    {
        public int LikerId { get; set; } // user who likes
        public int LikeeId { get; set; } // user being liked
        public User Liker { get; set; }
        public User Likee { get; set; }
    }
}
