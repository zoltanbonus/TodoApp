using System.Collections.Generic;
using TodoApp.Model.Database;

namespace TodoApp.Host.Models
{
    public class IndexViewModel
    {
        public IEnumerable<TodoItem> TodoItems { get; set; }

        public int TotalNumberOfItems { get; set; }

        public int Page { get; set; }

        public int PageSize { get; set; }

        public string SearchString { get; set; }

        public bool? IsCompleted { get; set; }
    }
}
