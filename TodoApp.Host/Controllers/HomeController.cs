using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TodoApp.Host.Models;
using TodoApp.Interfaces.Services;
using TodoApp.Model.Database;

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
        public IActionResult AddTodo(string newItemDescription)
        {
            try
            {
                todoItemService.CreateTodoItem(this.HttpContext.User.Identity.Name, newItemDescription);
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // TODO add logging
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
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
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public IActionResult UpdateTodo(int id, string description)
        {
            try
            {
                todoItemService.UpdateTodoDescription(id, description);
                return new JsonResult("OK");
            }
            catch (Exception ex)
            {
                return new JsonResult("ERROR");
            }
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
