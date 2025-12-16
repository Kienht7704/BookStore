using System;
using System.Collections.Generic;

namespace BookStore.DAL.Entities;

public partial class Book
{
    public int BookId { get; set; }

    public string BookName { get; set; } = null!;

    public string Description { get; set; } = null!;

    public DateTime PublicationDate { get; set; }

    public int Quantity { get; set; }

    public double Price { get; set; }

    public string Author { get; set; } = null!;

    public int CategoryId { get; set; }

    public string? ImageUrl { get; set; }

    public virtual Category Category { get; set; } = null!;
}
