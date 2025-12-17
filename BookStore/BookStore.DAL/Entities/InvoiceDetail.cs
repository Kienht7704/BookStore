using System;
using System.Collections.Generic;

namespace BookStore.DAL.Entities;

public partial class InvoiceDetail
{
    public int InvoiceDetailId { get; set; }

    public int InvoiceId { get; set; }

    public int BookId { get; set; }

    public int Quantity { get; set; }

    public double UnitPrice { get; set; }

    public double Subtotal { get; set; }

    public virtual Invoice Invoice { get; set; } = null!;

    public virtual Book Book { get; set; } = null!;
}
