using BookStore.DAL.Entities;
using BookStore.DAL.Repositories;
using System;
using System.Collections.Generic;

namespace BookStore.BLL.Services
{
    /// <summary>
    /// Service for managing invoice details
    /// </summary>
    public class InvoiceDetailService
    {
        private readonly InvoiceDetailRepo _repo = new();

        /// <summary>
        /// Get all invoice details
        /// </summary>
        public List<InvoiceDetail> GetAllInvoiceDetails()
        {
            return _repo.GetAll();
        }

        /// <summary>
        /// Get invoice detail by ID
        /// </summary>
        public InvoiceDetail? GetInvoiceDetailById(int invoiceDetailId)
        {
            return _repo.GetById(invoiceDetailId);
        }

        /// <summary>
        /// Get invoice details by Invoice ID
        /// </summary>
        public List<InvoiceDetail> GetInvoiceDetailsByInvoiceId(int invoiceId)
        {
            return _repo.GetByInvoiceId(invoiceId);
        }

        /// <summary>
        /// Search invoice details
        /// </summary>
        public List<InvoiceDetail> SearchInvoiceDetails(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return GetAllInvoiceDetails();
            }
            return _repo.Search(searchTerm);
        }

        /// <summary>
        /// Get invoice details by date range
        /// </summary>
        public List<InvoiceDetail> GetInvoiceDetailsByDateRange(DateTime startDate, DateTime endDate)
        {
            return _repo.GetByDateRange(startDate, endDate);
        }

        /// <summary>
        /// Get invoice details by staff ID
        /// </summary>
        public List<InvoiceDetail> GetInvoiceDetailsByStaffId(int staffId)
        {
            return _repo.GetByStaffId(staffId);
        }
    }
}
