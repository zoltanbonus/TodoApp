using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TodoApp.Model.Database;

namespace TodoApp.Interfaces.Database
{
    public interface IApplicationDbContext
    {
        int SaveChanges();

        DbSet<TodoItem> TodoItems { get; set; }
    }
}
