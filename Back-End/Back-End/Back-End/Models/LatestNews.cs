using System;
using System.Collections.Generic;

namespace Back_End.Models;

public partial class LatestNews
{
    public int Id { get; set; }

    public string ImageUrl { get; set; } = null!;

    public string NewsType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime? PublishedAt { get; set; }
}
