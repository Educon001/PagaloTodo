namespace UCABPagaloTodoMS.Application.Responses;

public class ForgotPasswordResponse
{
    public const string Successful = "Correo enviado exitosamente";
    public const string Unsuccessful = "Correo no se pudo enviar";
    public const string NotFound = "El correo ingresado no está registrado";

    public string Response { get; set; }

    public ForgotPasswordResponse(string response)
    {
        Response = response;
    }
}