using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Ordering.Application.Exceptions
{
    public class ValidationException: ApplicationException
    {
        public IDictionary<string, string[]> Errors { get; }

        public ValidationException()
            :base("One or more validation failures have occurred.")
        {
            Errors = new Dictionary<string, string[]>();
        }

        public ValidationException(IEnumerable<ValidationFailure> failures)
            :this()
        {
            Errors = failures
                .GroupBy(o => o.PropertyName, o => o.ErrorMessage)
                .ToDictionary(o => o.Key, o => o.ToArray());
        }
    }
}
