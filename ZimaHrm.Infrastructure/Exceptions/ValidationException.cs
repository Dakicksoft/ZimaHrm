using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace ZimaHrm.Core.Infrastructure.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationException(Type target, IList<ValidationResult> validationResults)
        {
            TargetType = target;
            ValidationResults = validationResults;
        }

        public ValidationException() : base()
        {
        }

        public ValidationException(string message) : base(message)
        {
        }

        public ValidationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        public IList<ValidationResult> ValidationResults { get; }

        public Type TargetType { get; }

        public override string Message
        {
            get
            {
                return string.Concat(TargetType.ToString(), ": ", string.Join(";", ValidationResults.Select(x => $"{x.ErrorMessage}")));
            }
        }
    }
}
