using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Categories.Commands
{
    public record UnarchiveCategoryCommand(int Id) : IRequest<Unit>;
}
