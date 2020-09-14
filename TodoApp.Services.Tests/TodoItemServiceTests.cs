using NUnit.Framework;
using Moq;
using TodoApp.Interfaces.Database;
using TodoApp.Model.Database;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace TodoApp.Services.Tests
{
    [TestFixture]
    public class TodoItemServiceTests
    {
        private const int defaultPage = 0;
        private const int defaultPageSize = 25;

        private const string invalidUserName = "invalidUser";
        private const int invalidTodoItemId = 0;
        private const string newDescription = "new item";
        private const string emptySearchString = "";

        private const string firstUser = "a@company.com";
        private const string firstDescription = "description 1";
        private const bool firstIsCompleted = false;

        private const string secondUser = "b@company.com";
        private const string secondDescription = "description 2";
        private const bool secondIsCompleted = true;

        private const string thirdDescription = "description 3";
        private const bool thirdIsCompleted = true;

        private Mock<IApplicationDbContext> dbContextMock;
        private Mock<DbSet<TodoItem>> todoItemsMock;
        private TodoItemService todoItemService;

        private List<TodoItem> todoItems;

        [SetUp]
        public void Setup()
        {
            todoItems = new List<TodoItem>()
            {
                new TodoItem { Id = 1, UserName = firstUser, Description = firstDescription, IsCompleted = firstIsCompleted },
                new TodoItem { Id = 2, UserName = secondUser, Description = secondDescription, IsCompleted = secondIsCompleted },
                new TodoItem { Id = 3, UserName = secondUser, Description = thirdDescription, IsCompleted = thirdIsCompleted },

            };
            var queryable = todoItems.AsQueryable();

            todoItemsMock = new Mock<DbSet<TodoItem>>();
            todoItemsMock.As<IQueryable<TodoItem>>().Setup(m => m.Provider).Returns(queryable.Provider);
            todoItemsMock.As<IQueryable<TodoItem>>().Setup(m => m.Expression).Returns(queryable.Expression);
            todoItemsMock.As<IQueryable<TodoItem>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            todoItemsMock.As<IQueryable<TodoItem>>().Setup(m => m.GetEnumerator()).Returns(queryable.GetEnumerator());
            todoItemsMock.Setup(dataset => dataset.Add(It.IsAny<TodoItem>())).Callback<TodoItem>((item) =>
            {
                todoItems.Add(item);
            });
            todoItemsMock.Setup(dataset => dataset.Remove(It.IsAny<TodoItem>())).Callback<TodoItem>((item) =>
            {
                todoItems.Remove(item);
            });

            dbContextMock = new Mock<IApplicationDbContext>();
            dbContextMock.Setup(mock => mock.TodoItems).Returns(todoItemsMock.Object);

            todoItemService = new TodoItemService(dbContextMock.Object);
        }

        [TestCase(firstUser, defaultPage, defaultPageSize, emptySearchString, null, 1, TestName = "Test filter count by user name (single item)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, emptySearchString, null, 2, TestName = "Test filter count by user name (multiple items)")]
        [TestCase(invalidUserName, defaultPage, defaultPageSize, emptySearchString, null, 0, TestName = "Test filter count by user name (invalid user)")]
        [TestCase(secondUser, 1, 1, emptySearchString, null, 1, TestName = "Test filter count by user name (second page)")]
        [TestCase(firstUser, defaultPage, defaultPageSize, emptySearchString, true, 0, TestName = "Test filter county by IsCompleted (no items)")]
        [TestCase(firstUser, defaultPage, defaultPageSize, emptySearchString, false, 1, TestName = "Test filter county by IsCompleted (single item)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, emptySearchString, true, 2, TestName = "Test filter county by IsCompleted (multiple items)")]
        [TestCase(secondUser, 1, 1, emptySearchString, true, 1, TestName = "Test filter county by IsCompleted (second page)")]
        [TestCase(firstUser, defaultPage, defaultPageSize, "1", null, 1, TestName = "Test search todo item count (single hit)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, "description", null, 2, TestName = "Test search todo item count (multiple hits)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, "DESCRIPTION", null, 2, TestName = "Test search todo item count (ignore case)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, "no hit", null, 0, TestName = "Test search todo item count (no hits)")]
        [TestCase(secondUser, 1, 1, "description", null, 1, TestName = "Test search todo item count (second page)")]
        public void TestGetTodoItemCount(string userName, int page, int pageSize, string searchString, bool? isCompleted, int expectedItemCount)
        {
            Assert.That(todoItemService.GetTodoItems(userName, page, pageSize, searchString, isCompleted).Count(), Is.EqualTo(expectedItemCount));
        }

        [TestCase(firstUser, defaultPage, defaultPageSize, emptySearchString, null, TestName = "Test filter by user name (single item)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, emptySearchString, null, TestName = "Test filter by user name (multiple items)")]
        [TestCase(invalidUserName, defaultPage, defaultPageSize, emptySearchString, null, TestName = "Test filter  by user name (invalid user)")]
        [TestCase(secondUser, 1, 1, emptySearchString, null, TestName = "Test filter by user name (second page)")]
        [TestCase(firstUser, defaultPage, defaultPageSize, emptySearchString, true, TestName = "Test filter by IsCompleted (no items)")]
        [TestCase(firstUser, defaultPage, defaultPageSize, emptySearchString, false, TestName = "Test filter by IsCompleted (single item)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, emptySearchString, true, TestName = "Test filter by IsCompleted (multiple items)")]
        [TestCase(secondUser, 1, 1, emptySearchString, true, TestName = "Test filter by IsCompleted (second page)")]
        [TestCase(firstUser, defaultPage, defaultPageSize, "1", null, TestName = "Test search todo item (single hit)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, "description", null, TestName = "Test search todo item (multiple hits)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, "DESCRIPTION", null, TestName = "Test search todo item (ignore case)")]
        [TestCase(secondUser, defaultPage, defaultPageSize, "no hit", null, TestName = "Test search todo item (no hits)")]
        [TestCase(secondUser, 1, 1, "description", null, TestName = "Test search todo item (second page)")]
        public void TestGetTodoItems(string userName, int page, int pageSize, string searchString, bool? isCompleted)
        {
            IEnumerable<TodoItem> todos = todoItemService.GetTodoItems(userName, page, pageSize, searchString, isCompleted);

            Assert.That(todos
                .All(todo => todo.UserName == userName),
                Is.True
            );

            Assert.That(todos
                .All(todo => string.IsNullOrEmpty(searchString) ? true : todo.Description.ToLower().Contains(searchString.ToLower())),
                Is.True
            );

            Assert.That(todos
                .All(todo => isCompleted == null ? true : todo.IsCompleted == isCompleted),
                Is.True
            );
        }

        [TestCase(TestName = "Test create todo item count")]
        public void TestCreateTodoItemCount()
        {
            int initialTodoCount = todoItems.Count();
            todoItemService.CreateTodoItem(firstUser, firstDescription);
            int todoCountAfterCreate = todoItems.Count();

            Assert.That(initialTodoCount + 1, Is.EqualTo(todoCountAfterCreate));
        }

        [TestCase(TestName = "Test create todo item save")]
        public void TestCreateTodoItemSave()
        {
            TodoItem newItem = todoItemService.CreateTodoItem(firstUser, newDescription);
            dbContextMock.Verify(mock => mock.SaveChanges(), Times.Once);
        }

        [TestCase(1, TestName = "Test complete todo item (incomplete item)")]
        [TestCase(2, TestName = "Test complete todo item (completed item)")]
        public void TestCompleteTodoItem(int id)
        {
            todoItemService.CompleteTodoItem(id);
            Assert.That(todoItems.FirstOrDefault(todo => todo.Id == id).IsCompleted, Is.EqualTo(true));
        }

        [TestCase(TestName = "Test complete todo item (invalid id)")]
        public void TestCompleteTodoItem()
        {
            todoItemService.CompleteTodoItem(invalidTodoItemId);
            Assert.That(todoItems[0].IsCompleted, Is.EqualTo(firstIsCompleted));
            Assert.That(todoItems[1].IsCompleted, Is.EqualTo(secondIsCompleted));
            Assert.That(todoItems[2].IsCompleted, Is.EqualTo(thirdIsCompleted));
        }

        [TestCase(TestName = "Test complete todo item save")]
        public void TestCompleteTodoItemSave()
        {
            todoItemService.CompleteTodoItem(1);
            dbContextMock.Verify(mock => mock.SaveChanges(), Times.Once);
        }

        [TestCase(TestName = "Test delete todo item (valid id)")]
        public void TestDeleteTodoItem()
        {
            int initialTodoCount = todoItems.Count();
            todoItemService.DeleteTodoItem(1);
            int todoCountAfterDeletion = todoItems.Count();
            Assert.That(todoCountAfterDeletion, Is.EqualTo(initialTodoCount - 1));
        }

        [TestCase(TestName = "Test delete todo item (invalid id)")]
        public void TestDeleteTodoItemInvalidId()
        {
            int initialTodoCount = todoItems.Count();
            todoItemService.DeleteTodoItem(invalidTodoItemId);
            int todoCountAfterDeletion = todoItems.Count();
            Assert.That(todoCountAfterDeletion, Is.EqualTo(initialTodoCount));
        }

        [TestCase(TestName = "Test delete todo item save")]
        public void TestDeleteTodoItemSave()
        {
            todoItemService.DeleteTodoItem(1);
            dbContextMock.Verify(mock => mock.SaveChanges(), Times.Once);
        }

        [TestCase(TestName = "Test update todo item description (valid id)")]
        public void TestUpdateTodoDescription()
        {
            todoItemService.UpdateTodoDescription(1, newDescription);
            Assert.That(todoItems[0].Description, Is.EqualTo(newDescription));
            Assert.That(todoItems[1].Description, Is.EqualTo(secondDescription));
            Assert.That(todoItems[2].Description, Is.EqualTo(thirdDescription));
        }

        [TestCase(TestName = "Test update todo item description (invalid id")]
        public void TestUpdateTodoDescriptionInvalidId()
        {
            todoItemService.UpdateTodoDescription(invalidTodoItemId, newDescription);
            Assert.That(todoItems[0].Description, Is.EqualTo(firstDescription));
            Assert.That(todoItems[1].Description, Is.EqualTo(secondDescription));
            Assert.That(todoItems[2].Description, Is.EqualTo(thirdDescription));
        }

        [TestCase(TestName = "Test update todo item description save")]
        public void TestUpdateTodoItemDescriptionSave()
        {
            todoItemService.UpdateTodoDescription(1, newDescription);
            dbContextMock.Verify(mock => mock.SaveChanges(), Times.Once);
        }
    }
}