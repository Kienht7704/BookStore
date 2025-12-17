using BookStore.DAL.Entities;
using BookStore.DAL.Repositories;
using System;
using System.Collections.Generic;

namespace BookStore.BLL.Services
{
    /// <summary>
    /// Service for managing invoices
    /// </summary>
    public class InvoiceService
    {
        private readonly InvoiceRepo _repo = new();

        /// <summary>
        /// Get invoice by ID with all details
        /// </summary>
        public Invoice? GetInvoiceById(int invoiceId)
        {
            return _repo.GetById(invoiceId);
        }

        /// <summary>
        /// Get all invoices for a specific date
        /// </summary>
        public List<Invoice> GetInvoicesByDate(DateTime date)
        {
            return _repo.GetByDate(date);
        }

        /// <summary>
        /// Get all invoices
        /// </summary>
        public List<Invoice> GetAllInvoices()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Get invoices by staff ID
        /// </summary>
        public List<Invoice> GetInvoicesByStaff(int staffId)
        {
            return _repo.GetByStaffId(staffId);
        }

        /// <summary>
        /// Get today's sales summary
        /// </summary>
        public (int totalOrders, double totalRevenue) GetTodaySummary()
        {
            var todayInvoices = _repo.GetByDate(DateTime.Today);
            int totalOrders = todayInvoices.Count;
            double totalRevenue = 0;
            foreach (var invoice in todayInvoices)
            {
                totalRevenue += invoice.TotalAmount;
            }
            return (totalOrders, totalRevenue);
        }
    }
}
