namespace Ecommerce.CrudApi.Shared
{
    public sealed class Result
    {
        public bool IsSuccess { get; }
        public string? ErrorCode { get; }
        public string? ErrorMessage { get; }

        private Result(bool isSuccess, string? errorCode, string? errorMessage)
        {
            IsSuccess = isSuccess;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public static Result Success() => new(true, null, null);

        public static Result Failure(string code, string message) => new(false, code, message);
    }

    public sealed class Result<T>
    {
        public bool IsSuccess { get; }
        public T? Value { get; }
        public string? ErrorCode { get; }
        public string? ErrorMessage { get; }

        private Result(bool isSuccess, T? value, string? errorCode, string? errorMessage)
        {
            IsSuccess = isSuccess;
            Value = value;
            ErrorCode = errorCode;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T value) => new(true, value, null, null);

        public static Result<T> Failure(string code, string message) => new(false, default, code, message);
    }
}
