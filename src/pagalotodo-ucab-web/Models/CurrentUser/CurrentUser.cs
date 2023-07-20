namespace UCABPagaloTodoWeb.Models.CurrentUser;

public static class CurrentUser
{
    private static LoginModel? _loginModel;
    private static readonly object lockObject = new (); 

    public static void SetUser(LoginModel user)
    {
        lock (lockObject) 
        {
            _loginModel = user;
        }
    }

    public static LoginModel? GetUser()
    {
        return _loginModel;
    }

    public static void EmptyUser()
    {
        _loginModel = null;
    }
}