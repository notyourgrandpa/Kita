using Kita.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kita.Server.Data
{
    public class ApiLoginResult
    {
        /// <summary>
        /// TRUE if the login attempt is successful, FALSE otherwise.
        /// </summary>
        public bool Success { get; set; }
        /// <summary>
        /// Login attempt result message
        /// </summary>
        public required string Message { get; set; }
        /// <summary>
        /// The JWT token if the login attempt is successful, or NULL if not
        /// </summary>
        public string? Token { get; set; }

        /// <summary>
        /// Stores User information
        /// </summary>
        public UserDTO? User { get; set; }
    }
}
