using FluentValidation;
using MediatR;

namespace API.Behavirous;

public class ValidatorPipelineBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : IBaseRequest
{
    private readonly IEnumerable<IValidator<TRequest>> _validators;

    public ValidatorPipelineBehaviour(IEnumerable<IValidator<TRequest>> validator)
    {
        _validators = validator;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        // There is always going to be one validator at least now. The IEnumerable is to handle requests that don't have any validators
        if (_validators.Any())
        {
            var validator = _validators.First();
            var validationResult = validator.Validate(request);

            if (!validationResult.IsValid)
            {
                throw new ValidationException(validationResult.Errors);
            }
        }

        return await next();
    }
}
