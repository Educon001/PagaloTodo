namespace UCABPagaloTodoMS.Core.Entities
{
    public class ValoresEntity : BaseEntity
    {
        public string? Nombre { get; set; }
        public string? Apellido { get; set; }
        public string Identificacion { get; set; } = string.Empty;
    }
}
