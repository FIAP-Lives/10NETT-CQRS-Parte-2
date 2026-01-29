namespace Ecommerce.Shared.Exceptions;

public class ValidationException : Exception
{
    public string Code { get; }
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public ValidationException(string code, string message, IReadOnlyDictionary<string, string[]> errors)
        : base(message)
    {
        Code = code;
        Errors = errors;
    }
}
