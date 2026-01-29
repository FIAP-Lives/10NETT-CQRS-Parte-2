namespace Ecommerce.CrudApi.Data.Write.Entities
{
    public sealed class OutboxMessage
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public required string Type { get; set; }
        public required string Payload { get; set; }
        public DateTime OccurredAtUtc { get; set; } = DateTime.UtcNow;
        public DateTime? ProcessedAtUtc { get; set; }
    }
}
