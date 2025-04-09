namespace Business.Models;

public class ProjectResult<T> : ServiceResult
{
   public T? Result { get; set; }
}
public class ProjectResult : ServiceResult
{
    public string? ErrorMessage { get; set; }
}
