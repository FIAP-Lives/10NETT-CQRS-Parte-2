namespace Ecommerce.CrudApi.Data.Read.Models
{
    public sealed class OrderReadModel
    {
        public Guid OrderId { get; set; }
        public string CustomerName { get; set; } = "";
        public string ShippingAddress { get; set; } = "";
        public string Status { get; set; } = "";
        public decimal Total { get; set; }
        public string ItemsJson { get; set; } = "[]";
    }
}
