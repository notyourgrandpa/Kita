using Kita.Application.Common.Paging;
using Kita.Application.DTOs;
using Kita.Application.QueryParameters;
using Kita.Domain.Categories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Services
{
    public interface ICategoryService
    {
        Task<bool> IsDupeField<T>(int categoryId, string fieldName, string fieldValue) where T : class;
    }
}
