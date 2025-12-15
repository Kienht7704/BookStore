using System;
using System.Collections.Generic;

namespace BookStore.DAL.Entities;

public partial class Category
{
    public int CategoryId { get; set; }

    public string BookGenreType { get; set; } = null!;

    public string Description { get; set; } = null!;

    public virtual ICollection<Book> Books { get; set; } = new List<Book>();
}
