using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using Kita.Domain.Expenses;

namespace Kita.Application.Services
{
    public interface IExpenseService
    {
        Task<ApiResult<ExpenseDTO>> GetExpensesAsync(QueryParameter parameters);
        Task<Expense> GetExpenseByIdAsync(int id);
        Task<ExpenseDTO> CreateExpenseAsync(ExpenseCreateDTO expenseDTO);
        Task UpdateExpenseAsync(int id, ExpenseUpdateDTO expenseDTO);
        Task DeleteExpenseAsync(int id);
    }
}
