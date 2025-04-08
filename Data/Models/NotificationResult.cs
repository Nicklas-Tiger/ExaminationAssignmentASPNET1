namespace Data.Models;

public class NotificationResult : ResponseResult
{

}

public class NotificationResult<T> : NotificationResult
{
    public T? Result { get; set; }
}