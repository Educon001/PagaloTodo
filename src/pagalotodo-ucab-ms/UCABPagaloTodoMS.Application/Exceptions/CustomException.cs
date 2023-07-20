namespace UCABPagaloTodoMS.Application.Exceptions;

public class CustomException : Exception
{
    public CustomException(string message, Exception ex) : base(message,ex)
    {
    }

    public CustomException(Exception ex) : base(SetMessage(ex),ex)
    {
    }
    
    public CustomException(string message) : base(message)
    {
    }
    
    private static string SetMessage(Exception ex)
    {
        return ex.Message + (ex.InnerException is not null? "\n"  + ex.InnerException?.Message : "");
    } 
}