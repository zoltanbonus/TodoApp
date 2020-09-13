using System.Collections.Generic;
using TodoApp.Model.Database;

namespace TodoApp.Interfaces.Services
{
    public interface ITodoItemService
    {
        TodoItem CreateTodoItem(string userName, string description);

        IEnumerable<TodoItem> GetTodoItems(string userName, int page, int pageSize, string searchString, bool? isCompleted);

        int GetTodoItemCount(string userName, string searchString, bool? isCompleted);

        void UpdateTodoDescription(int id, string description);

        void CompleteTodoItem(int id);

        void DeleteTodoItem(int id);
    }
}
