using System;
using System.Collections.Generic;
using System.Text;
using ZimaHrm.Core.Infrastructure.Result;
using FluentValidation;

namespace ZimaHrm.Core.Infrastructure.Validation
{
    public abstract class Validator<T> : AbstractValidator<T>
    {
        protected Validator()
        {
        }

        protected Validator(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }

        private string ErrorMessage { get; }

        public IResult Valid(T instance)
        {
            if (Equals(instance, default(T)))
            {
                return new ErrorResult(ErrorMessage);
            }

            var result = Validate(instance);

            if (result.IsValid)
            {
                return new SuccessResult();
            }

            return new ErrorResult(ErrorMessage ?? result.ToString());
        }
    }
}
