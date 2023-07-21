// Espacio de nombres para la clase CustomException.
namespace UCABPagaloTodoMS.Application.Exceptions;

// La clase CustomException hereda de Exception.
public class CustomException : Exception
{
    // Constructor que toma una cadena de mensaje.
    public CustomException(string message) : base(message)
    {
    }

    // Constructor que toma una cadena de mensaje y un objeto Exception.
    public CustomException(string message, Exception ex) : base(message, ex)
    {
    }

    // Constructor que toma un objeto Exception.
    public CustomException(Exception ex) : base(SetMessage(ex), ex)
    {
    }

    // Método privado estático que devuelve una cadena que contiene el mensaje del objeto Exception,
    // más el mensaje del objeto InnerException si no es nulo.
    private static string SetMessage(Exception ex)
    {
        return ex.Message + (ex.InnerException != null ? "\n" + ex.InnerException?.Message : "");
    }
}