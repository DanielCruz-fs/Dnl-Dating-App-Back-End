using Microsoft.EntityFrameworkCore;
using DatingAppBack.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DatingAppBack.Data
{   //we created this class which is the data access layer
    public class DataContext: DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base (options) { }

        public DbSet<Value> Values { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Photo> Photos { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<Like>().HasKey(l => new { l.LikerId, l.LikeeId });
            builder.Entity<Like>()
                   .HasOne<User>(l => l.Likee)
                   .WithMany(u => u.Likers)
                   .HasForeignKey(l => l.LikeeId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Like>()
                   .HasOne<User>(l => l.Liker)
                   .WithMany(u => u.Likees)
                   .HasForeignKey(l => l.LikerId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                   .HasOne<User>(m => m.Sender)
                   .WithMany(u => u.MessagesSent)
                   .HasForeignKey(m => m.SenderId)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                   .HasOne<User>(m => m.Recipient)
                   .WithMany(u => u.MessagesReceived)
                   .HasForeignKey(m => m.RecipientId)
                   .OnDelete(DeleteBehavior.Restrict);

        }

    }
}
