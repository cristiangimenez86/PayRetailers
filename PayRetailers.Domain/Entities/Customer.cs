namespace PayRetailers.Domain.Entities
{
    public class Customer
    {
        public required string Name { get; set; }
        public required string Surname { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
    }
}
