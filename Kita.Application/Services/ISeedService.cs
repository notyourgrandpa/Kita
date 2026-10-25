using Kita.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Application.Services
{
    public interface ISeedService
    {
        Task<SeedResultDTO> Import(Stream fileStream);
        Task CreateDefaultUser();
    }
}
