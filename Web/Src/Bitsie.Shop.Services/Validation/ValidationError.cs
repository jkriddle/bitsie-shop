using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bitsie.Shop.Services
{
    /// <summary>
    /// Basic wrapper for validation errors
    /// </summary>
    public class ValidationError
    {
        /// <summary>
        /// Property name of object being validated.
        /// </summary>
        public string PropertyName { get; set; }

        /// <summary>
        /// Error message of property
        /// </summary>
        public string Message { get; set; }

        public ValidationError(string propertyName, string message)
        {
            PropertyName = propertyName;
            Message = message;
        }
    }
}
