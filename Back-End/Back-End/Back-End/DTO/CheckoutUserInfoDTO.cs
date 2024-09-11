namespace Back_End.DTO
{
    public class CheckoutUserInfoDTO
    {
        public string? Name { get; set; }

        public string? Email { get; set; }

        public string? PhoneNumber { get; set; }

        public List<OrderItemsDTO> Orders { get; set; }

    }
}
