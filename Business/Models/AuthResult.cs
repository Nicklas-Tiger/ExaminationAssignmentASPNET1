namespace Business.Models;

public class AuthResult : ServiceResult
{
    public string? SuccessMessage { get; set; } 
}

public class AuthResult<T> : ServiceResult
{
    public T? Result { get; set; }
}

