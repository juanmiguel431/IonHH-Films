namespace Films.Core.Domain;

public class OperationResult
{
    private const string ErrorMessage = "Something went wrong, please try again";
    private const string SuccessMessage = "Operation completed successfully";
    
    public bool IsSuccess { get; set; }
    public string Message { get; set; }
    public object? Data { get; set; }
    
    public static OperationResult CreateSuccessMessage(string message = SuccessMessage, object? data = null)
    {
        return new OperationResult
        {
            Message = message,
            IsSuccess = true,
            Data = data
        };
    }
    
    public static OperationResult CreateSuccessMessage(object data)
    {
        return CreateSuccessMessage(SuccessMessage, data);
    }
    
    public static OperationResult CreateErrorMessage(string message = ErrorMessage)
    {
        return new OperationResult
        {
            Message = message,
            IsSuccess = false
        };
    }
}