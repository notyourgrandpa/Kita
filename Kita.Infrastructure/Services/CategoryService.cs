using Kita.Application.Common.Interfaces;
using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.Services;
using Kita.Domain.Abstractions;
using Kita.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using OfficeOpenXml.FormulaParsing.Excel.Functions.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using static FluentValidation.Validators.PredicateValidator;
using static OfficeOpenXml.ExcelErrorValue;

namespace Kita.Infrastructure.Services
{
    public class CategoryService: ICategoryService
    {
        private readonly IApplicationDbContext _context;

        public CategoryService(IApplicationDbContext context) 
        { 
            _context = context;
        }

        public async Task<bool> IsDupeField<T>(int categoryId, string fieldName, string fieldValue) where T : class
        {

            if (!ApiResult<Category>.IsValidProperty(fieldName, true))
                return false;

            string predicate = string.Format("{0} == @0 && Id != @1", fieldName);

            object[] values = { fieldValue, categoryId };

            return await _context.Set<T>().Where(predicate, values).AnyAsync();
        }
    }
}
