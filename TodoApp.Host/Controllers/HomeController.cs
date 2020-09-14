using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Diagnostics;
using TodoApp.Host.Models;
using TodoApp.Interfaces.Services;

namespace TodoApp.Host.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ITodoItemService todoItemService;

        public HomeController(ILogger<HomeController> logger, ITodoItemService todoItemService)
        {
            _logger = logger;
            this.todoItemService = todoItemService;
        }

        public IActionResult Index(string searchString = "", bool? isCompleted = null, int page = Constants.DefaultPage, int pageSize = Constants.DefaultPageSize)
        {
            page = page < 0 ? 0 : page;
            pageSize = pageSize < 0 ? 1 : pageSize;

            return View("Index", model: new IndexViewModel
            {
                TodoItems = todoItemService.GetTodoItems(this.HttpContext.User.Identity.Name, page, pageSize, searchString, isCompleted),
                TotalNumberOfItems = todoItemService.GetTodoItemCount(this.HttpContext.User.Identity.Name, searchString, isCompleted),
                SearchString = searchString,
                IsCompleted = isCompleted,
                Page = page,
                PageSize = (pageSize > 0 ? pageSize : 1),
            }); ;
        }

        [HttpPost]
        public IActionResult AddTodo(string newItemDescription, string searchString, bool? isCompleted, int page, int pageSize)
        {
            try
            {
                todoItemService.CreateTodoItem(this.HttpContext.User.Identity.Name, newItemDescription);
                return Index(searchString, isCompleted, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Error();
            }
        }

        [HttpPost]
        public IActionResult CompleteTodo(int id, string searchString, bool? isCompleted, int page, int pageSize)
        {
            try
            {
                todoItemService.CompleteTodoItem(id);
                return Index(searchString, isCompleted, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Error();
            }
        }

        [HttpPost]
        public IActionResult DeleteTodo(int id, string searchString, bool? isCompleted, int page, int pageSize)
        {
            try
            {
                todoItemService.DeleteTodoItem(id);
                return Index(searchString, isCompleted, page, pageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return Error();
            }
        }

        [HttpPost]
        public IActionResult UpdateTodo(int id, string description)
        {
            try
            {
                todoItemService.UpdateTodoDescription(id, description);
                return new JsonResult(Constants.SuccessJsonResultText);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                return new JsonResult(Constants.ErrorJsonResultText);
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error", new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
