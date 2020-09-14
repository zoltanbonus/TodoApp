using System.Collections.Generic;
using System.Linq;
using TodoApp.Interfaces.Database;
using TodoApp.Interfaces.Services;
using TodoApp.Model.Database;

namespace TodoApp.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly IApplicationDbContext dbContext;

        public TodoItemService(IApplicationDbContext dbContext)
        {
            this.dbContext = dbContext;
        }

        public void CompleteTodoItem(int id)
        {
            TodoItem todo = dbContext.TodoItems.FirstOrDefault(todo => todo.Id == id);
            if (todo == null)
            {
                return;
            }

            todo.IsCompleted = true;
            dbContext.SaveChanges();
        }

        public TodoItem CreateTodoItem(string userName, string description)
        {
            if (string.IsNullOrEmpty(description))
            {
                return null;
            }

            TodoItem newItem = new TodoItem
            {
                UserName = userName,
                Description = description
            };

            dbContext.TodoItems.Add(newItem);
            dbContext.SaveChanges();
            return newItem;
        }

        public void DeleteTodoItem(int id)
        {
            TodoItem todoItem = dbContext.TodoItems.FirstOrDefault(todo => todo.Id == id);
            if (todoItem == null)
            {
                return;
            }

            dbContext.TodoItems.Remove(todoItem);
            dbContext.SaveChanges();
        }

        public void UpdateTodoDescription(int id, string description)
        {
            TodoItem todoItem = dbContext.TodoItems.FirstOrDefault(todo => todo.Id == id);
            if (todoItem == null)
            {
                return;
            }

            todoItem.Description = description;
            dbContext.SaveChanges();
        }

        public int GetTodoItemCount(string userName, string searchString, bool? isCompleted)
        {
            return GetTodoItemQuery(userName, searchString, isCompleted).Count();
        }

        public IEnumerable<TodoItem> GetTodoItems(string userName, int page, int pageSize, string searchString, bool? isCompleted)
        {
            if (page < 0)
            {
                page = 0;
            }

            return GetTodoItemQuery(userName, searchString, isCompleted)
               .Skip(page * pageSize)
               .Take(pageSize)
               .ToList();
        }

        private IQueryable<TodoItem> GetTodoItemQuery(string userName, string searchString, bool? isCompleted)
        {
            return dbContext.TodoItems
               .Where(todo => todo.UserName == userName)
               .Where(todo => string.IsNullOrEmpty(searchString) ? true : todo.Description.ToLower().Contains(searchString.ToLower()))
               .Where(todo => isCompleted == null ? true : todo.IsCompleted == isCompleted);
        }
    }
}
