namespace WorkshopsGov.Models.Common
{
    public abstract class AuditableEntityBase
    {
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}