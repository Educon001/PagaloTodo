using System.ComponentModel.DataAnnotations;
using UCABPagaloTodoMS.Core.Enums;

namespace UCABPagaloTodoWeb.Models;

public class PaymentRequest
{
    [Required(ErrorMessage = "El monto es requerido")]
    [Range(0, int.MaxValue, ErrorMessage = "El monto debe ser mayor a 0")]
    public float? Amount { get; set; }
    
    [Required(ErrorMessage = "Numero de tarjeta es requerido")]
    [RegularExpression(@"^\d{13,18}$", ErrorMessage = "Introduzca un numero de tarjeta valido")] 
    public string? CardNumber { get; set; }
    
    [Required(ErrorMessage = "Mes de expiracion obligatorio")]
    [Range(1, 12, ErrorMessage = "Introduzca un mes valido")]
    public int? ExpirationMonth { get; set; }
    
    [Required(ErrorMessage = "Año de expiracion obligatorio")]
    [Range(0, 99, ErrorMessage = "Año de expiracion invalido")]
    public int? ExpirationYear { get; set; }
    
    [Required(ErrorMessage = "Nombre de tarjetaHabiente requerido")]
    [RegularExpression(@"^[A-Za-z ]+$", ErrorMessage = "Nombre de tarjetahabiente solo debe tener letras")]
    public string? CardholderName { get; set; }
    
    [Required(ErrorMessage = "Cvv requerido")]
    [RegularExpression(@"^\d{3,4}$", ErrorMessage = "CVV debe tener 3 o 4 digitos")]
    public string? CardSecurityCode { get; set; }
    
    public PaymentStatusEnum? PaymentStatus { get; set; }
    public Guid? Consumer { get; set; }
    public Guid? Service { get; set; }
    //Para los pagos por confirmacion
    
    [Required(ErrorMessage = "El identificador es obligatorio")]
    public string? Identifier { get; set; }
}