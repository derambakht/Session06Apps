namespace Infrastructure.Common;

public class MellatActionResult<T>
{
    public bool IsSuccess { get; set;}
    public string Message { get; set;}
    public T Data {get; set;}
}