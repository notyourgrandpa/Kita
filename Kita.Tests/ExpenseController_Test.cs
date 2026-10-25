using Kita.Application.DTOs;
using Kita.Application.Services;
using Kita.Server.Controllers;
using Microsoft.AspNetCore.Mvc;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Tests
{
    public class ExpenseController_Test
    {
        private readonly Mock<IExpenseService> _serviceMock;
        private readonly ExpensesController _controller;

        public ExpenseController_Test()
        {
            _serviceMock = new Mock<IExpenseService>();
            _controller = new ExpensesController(_serviceMock.Object);
        }

        [Fact]
        public async Task CreateExpense_ReturnsOk_WhenSuccessfullyInserted()
        {
            //Arrange
            var expense = new ExpenseCreateDTO
            {
                Category = "New Category",
                Description = "Description",
                Amount = 12.0M,
                Date = DateTime.Now
            };

            var expenseDTO = new ExpenseDTO
            {
                Id = 10,
                Category = "New Category",
                Description = "Description",
                Amount = 12.0M,
                Date = DateTime.Now
            };

            _serviceMock.Setup(s => s.CreateExpenseAsync(expense)).ReturnsAsync(expenseDTO);
            var result = await _controller.CreateExpenseAsync(expense);

            var created = Assert.IsType<CreatedAtActionResult>(result.Result);
            Assert.Equal("GetExpenseById", created.ActionName);
            Assert.Equal(expenseDTO, created.Value);
        }
    }
}
