using System;
using System.Collections.Generic;

namespace BookStore.DAL.Entities;

public partial class Invoice
{
    public int InvoiceId { get; set; }

    public DateTime InvoiceDate { get; set; }

    public int StaffId { get; set; }

    public double TotalAmount { get; set; }

    public string PaymentMethod { get; set; } = null!;

    public string Status { get; set; } = "Completed";

    public virtual User Staff { get; set; } = null!;

    public virtual ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
}
