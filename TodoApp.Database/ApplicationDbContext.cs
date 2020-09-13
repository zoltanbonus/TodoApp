using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TodoApp.Interfaces.Database;
using TodoApp.Model.Database;

namespace TodoApp.Host.Data
{
    public class ApplicationDbContext : IdentityDbContext, IApplicationDbContext
    {
        public virtual DbSet<TodoItem> TodoItems { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
                : base(options)
        {

        }

        public override int SaveChanges()
        {
            return base.SaveChanges();
        }
    }
}

