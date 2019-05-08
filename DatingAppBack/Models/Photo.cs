using System;

namespace DatingAppBack.Models
{
    public class Photo
    {
        public int Id { get; set; }
        public string Url { get; set; }
        public string Description { get; set; }
        public DateTime DateAdded { get; set; }
        public bool IsMain { get; set; }
        //*for cascade delete only, otherwise efcore creates the props automatically*/
        public User User { get; set; }
        public int UserId { get; set; }
    }
}