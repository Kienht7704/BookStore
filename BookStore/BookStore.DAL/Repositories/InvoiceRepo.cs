using BookStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.DAL.Repositories
{
    public class InvoiceRepo
    {
        private BookStoreDbContext _ctx;

        /// <summary>
        /// Create a new invoice with its details
        /// </summary>
        public Invoice Create(Invoice invoice)
        {
            _ctx = new();
            _ctx.Invoices.Add(invoice);
            _ctx.SaveChanges();
            return invoice;
        }

        /// <summary>
        /// Get invoice by ID with details and book info
        /// </summary>
        public Invoice? GetById(int invoiceId)
        {
            _ctx = new();
            return _ctx.Invoices
                .Include(i => i.Staff)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Book)
                .FirstOrDefault(i => i.InvoiceId == invoiceId);
        }

        /// <summary>
        /// Get all invoices for a specific date
        /// </summary>
        public List<Invoice> GetByDate(DateTime date)
        {
            _ctx = new();
            return _ctx.Invoices
                .Include(i => i.Staff)
                .Include(i => i.InvoiceDetails)
                .Where(i => i.InvoiceDate.Date == date.Date)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        public List<Invoice> GetAll()
        {
            _ctx = new();
            return _ctx.Invoices
                .Include(i => i.Staff)
                .Include(i => i.InvoiceDetails)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }

        /// <summary>
        /// Get invoices by staff ID
        /// </summary>
        public List<Invoice> GetByStaffId(int staffId)
        {
            _ctx = new();
            return _ctx.Invoices
                .Include(i => i.InvoiceDetails)
                .Where(i => i.StaffId == staffId)
                .OrderByDescending(i => i.InvoiceDate)
                .ToList();
        }
    }
}
