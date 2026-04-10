using System.ComponentModel.DataAnnotations;
using ChitMeo.Mediator;

namespace ChitMeo.Shared.Behaviors;

public class ValidationBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    public async Task<TResponse> HandleAsync(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(request);
        bool isValid = Validator.TryValidateObject(request, context, results, true);

        if (!isValid)
        {
            var messages = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new ValidationException(messages);
        }

        var response = await next();
        return response;
    }
}
