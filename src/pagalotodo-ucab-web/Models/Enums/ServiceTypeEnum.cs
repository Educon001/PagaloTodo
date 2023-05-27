using System.ComponentModel.DataAnnotations;

namespace UCABPagaloTodoMS.Core.Enums;

public enum ServiceTypeEnum
{
    Directo,
    [Display(Name="Por Confirmacion")]
    PorConfirmacion
}