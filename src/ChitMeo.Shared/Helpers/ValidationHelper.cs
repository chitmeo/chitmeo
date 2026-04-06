using System.ComponentModel.DataAnnotations;

namespace ChitMeo.Shared.Helpers;

public static class ValidationHelper
{
    public static void ValidateAndThrow(object model)
    {
        var results = new List<ValidationResult>();
        var context = new ValidationContext(model);
        bool isValid = Validator.TryValidateObject(model, context, results, true);

        if (!isValid)
        {
            var messages = string.Join("; ", results.Select(r => r.ErrorMessage));
            throw new ValidationException(messages);
        }
    }
}
