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

    }
}
