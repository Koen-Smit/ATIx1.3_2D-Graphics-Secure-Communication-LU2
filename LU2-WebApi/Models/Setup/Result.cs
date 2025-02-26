public class Result
{
    public bool IsSuccess { get; private set; }
    public string? ErrorMessage { get; private set; }
    public string? SuccessMessage { get; private set; }

    public Result(bool isSuccess, string? successMessage = null, string? errorMessage = null)
    {
        IsSuccess = isSuccess;
        SuccessMessage = successMessage;
        ErrorMessage = errorMessage;
    }

    public static Result Success(string? successMessage = null) => new Result(true, successMessage);
    public static Result Failure(string errorMessage) => new Result(false, errorMessage: errorMessage);
}
