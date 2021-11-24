using FluentValidation;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using ValidationException = Ordering.Application.Exceptions.ValidationException;

namespace Ordering.Application.Behaviours
{
    public class ValidationBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validator;

        public ValidationBehaviour(IEnumerable<IValidator<TRequest>> validator)
        {
            _validator = validator ?? throw new ArgumentNullException(nameof(validator));
        }

        public async Task<TResponse> Handle(TRequest request, 
                                            CancellationToken cancellationToken, 
                                            RequestHandlerDelegate<TResponse> next)
        {
            if (_validator.Any())
            {
                var context = new ValidationContext<TRequest>(request);
                var validationResult = await Task.WhenAll(
                    _validator.Select(o => o.ValidateAsync(context, cancellationToken)));
                var failures = validationResult
                                    .SelectMany(o => o.Errors)
                                    .Where(o => o != null)
                                    .ToList();
                if (failures.Count != 0)
                {
                    throw new ValidationException(failures);
                }
            }
            return await next();
        }
    }
}
