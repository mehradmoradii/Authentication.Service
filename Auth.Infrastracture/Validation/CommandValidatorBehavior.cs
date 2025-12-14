using FluentValidation;
using MediatR;


namespace Auth.Infrastracture.Validation
{
    public class CommandValidatorBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse> where TRequest : class, IRequest<TResponse>
    {
        private readonly IEnumerable<IValidator<TRequest>> _validator;

        public CommandValidatorBehavior(IEnumerable<IValidator<TRequest>> validator) => _validator = validator;

        public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> Text , CancellationToken cancellationToken)
        {
            {
                if (!_validator.Any())
                {
                    return await Text();
                }

                var cnt = new ValidationContext<TRequest>(request);

                var Errors = _validator
                    .Select(o => o.Validate(cnt))
                    .SelectMany(o => o.Errors)
                    .Where(o => o != null)
                    .GroupBy(
                    o => o.ErrorMessage,
                    o => o.PropertyName,
                    (errorMessage, propertyName) => new
                    {
                        Key = errorMessage,
                        Value = propertyName
                    })
                    .ToDictionary(o => o.Key, o => o.Value);

                if (Errors.Any())
                {
                    throw new ValidationException(Errors.ToString());
                }
                return await Text();
            }
        }
    
    }
}
