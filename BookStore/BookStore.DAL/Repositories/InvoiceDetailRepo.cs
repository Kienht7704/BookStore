using BookStore.DAL.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.DAL.Repositories
{
    public class InvoiceDetailRepo
    {
        private BookStoreDbContext _ctx;

        /// <summary>
        /// Get all invoice details with Invoice and User included
        /// </summary>
        public List<InvoiceDetail> GetAll()
        {
            _ctx = new();
            return _ctx.InvoiceDetails
                .Include(id => id.Invoice)
                    .ThenInclude(i => i.Staff)
                .Include(id => id.Book)
                .OrderByDescending(id => id.Invoice.InvoiceDate)
                .ToList();
        }

        /// <summary>
        /// Get invoice detail by ID
        /// </summary>
        public InvoiceDetail? GetById(int invoiceDetailId)
        {
            _ctx = new();
            return _ctx.InvoiceDetails
                .Include(id => id.Invoice)
                    .ThenInclude(i => i.Staff)
                .Include(id => id.Book)
                .FirstOrDefault(id => id.InvoiceDetailId == invoiceDetailId);
        }

        /// <summary>
        /// Get invoice details by Invoice ID
        /// </summary>
        public List<InvoiceDetail> GetByInvoiceId(int invoiceId)
        {
            _ctx = new();
            return _ctx.InvoiceDetails
                .Include(id => id.Invoice)
                    .ThenInclude(i => i.Staff)
                .Include(id => id.Book)
                .Where(id => id.InvoiceId == invoiceId)
                .ToList();
        }

        /// <summary>
        /// Search invoice details by book name, staff name, or invoice ID
        /// </summary>
        public List<InvoiceDetail> Search(string searchTerm)
        {
            _ctx = new();
            var lowerSearch = searchTerm.ToLower();
            
            return _ctx.InvoiceDetails
                .Include(id => id.Invoice)
                    .ThenInclude(i => i.Staff)
                .Include(id => id.Book)
                .Where(id => 
                    id.Book.BookName.ToLower().Contains(lowerSearch) ||
                    id.Invoice.Staff.FullName.ToLower().Contains(lowerSearch) ||
                    id.InvoiceId.ToString().Contains(searchTerm))
                .OrderByDescending(id => id.Invoice.InvoiceDate)
                .ToList();
        }

        /// <summary>
        /// Get invoice details by date range
        /// </summary>
        public List<InvoiceDetail> GetByDateRange(DateTime startDate, DateTime endDate)
        {
            _ctx = new();
            return _ctx.InvoiceDetails
                .Include(id => id.Invoice)
                    .ThenInclude(i => i.Staff)
                .Include(id => id.Book)
                .Where(id => id.Invoice.InvoiceDate.Date >= startDate.Date 
                    && id.Invoice.InvoiceDate.Date <= endDate.Date)
                .OrderByDescending(id => id.Invoice.InvoiceDate)
                .ToList();
        }

        /// <summary>
        /// Get invoice details by staff ID
        /// </summary>
        public List<InvoiceDetail> GetByStaffId(int staffId)
        {
            _ctx = new();
            return _ctx.InvoiceDetails
                .Include(id => id.Invoice)
                    .ThenInclude(i => i.Staff)
                .Include(id => id.Book)
                .Where(id => id.Invoice.StaffId == staffId)
                .OrderByDescending(id => id.Invoice.InvoiceDate)
                .ToList();
        }
    }
}
