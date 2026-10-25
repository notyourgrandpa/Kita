using Kita.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Commands
{ 
    public record CreateCategoryCommand(string Name): IRequest<CategoryDTO> { }
}
