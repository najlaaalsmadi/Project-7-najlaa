using System;
using System.Collections.Generic;

namespace Back_End.Models;

public partial class Coupon
{
    public int Id { get; set; }

    public string? Name { get; set; }

    public decimal? DiscountAmount { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public string? Status { get; set; }
}
