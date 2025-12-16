using BookStore.DAL.Entities;
using BookStore.DAL.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;

namespace BookStore.BLL.Services
{
    /// <summary>
    /// Represents an item in the current POS order (in-memory, not saved to DB yet)
    /// </summary>
    public class OrderItem
    {
        public Book Book { get; set; } = null!;
        public int Quantity { get; set; }
        public double UnitPrice => Book.Price;
        public double Subtotal => UnitPrice * Quantity;
    }

    /// <summary>
    /// POS Service for managing checkout operations
    /// </summary>
    public class POSService
    {
        private readonly BookRepo _bookRepo = new();
        private readonly InvoiceRepo _invoiceRepo = new();
        
        // Current order items (in-memory cart)
        public List<OrderItem> CurrentOrder { get; private set; } = new();

        /// <summary>
        /// Add a book to the current order by BookId
        /// </summary>
        public (bool success, string message) AddToOrder(int bookId, int quantity = 1)
        {
            var book = _bookRepo.GetById(bookId);
            
            if (book == null)
            {
                return (false, $"Book with ID {bookId} not found.");
            }

            if (book.Quantity < quantity)
            {
                return (false, $"Not enough stock. Available: {book.Quantity}");
            }

            // Check if book already in order
            var existingItem = CurrentOrder.FirstOrDefault(o => o.Book.BookId == bookId);
            if (existingItem != null)
            {
                // Check stock for total quantity
                if (book.Quantity < existingItem.Quantity + quantity)
                {
                    return (false, $"Not enough stock. Available: {book.Quantity}, In cart: {existingItem.Quantity}");
                }
                existingItem.Quantity += quantity;
            }
            else
            {
                CurrentOrder.Add(new OrderItem { Book = book, Quantity = quantity });
            }

            return (true, $"Added {book.BookName} to order.");
        }

        /// <summary>
        /// Update quantity of an item in the order
        /// </summary>
        public (bool success, string message) UpdateQuantity(int bookId, int newQuantity)
        {
            var item = CurrentOrder.FirstOrDefault(o => o.Book.BookId == bookId);
            if (item == null)
            {
                return (false, "Item not found in order.");
            }

            if (newQuantity <= 0)
            {
                return RemoveFromOrder(bookId);
            }

            if (item.Book.Quantity < newQuantity)
            {
                return (false, $"Not enough stock. Available: {item.Book.Quantity}");
            }

            item.Quantity = newQuantity;
            return (true, "Quantity updated.");
        }

        /// <summary>
        /// Remove an item from the order
        /// </summary>
        public (bool success, string message) RemoveFromOrder(int bookId)
        {
            var item = CurrentOrder.FirstOrDefault(o => o.Book.BookId == bookId);
            if (item == null)
            {
                return (false, "Item not found in order.");
            }

            CurrentOrder.Remove(item);
            return (true, "Item removed from order.");
        }

        /// <summary>
        /// Calculate the subtotal (before tax)
        /// </summary>
        public double CalculateSubtotal()
        {
            return CurrentOrder.Sum(o => o.Subtotal);
        }

        /// <summary>
        /// Calculate tax amount (optional, default 0%)
        /// </summary>
        public double CalculateTax(double taxRate = 0)
        {
            return CalculateSubtotal() * taxRate;
        }

        /// <summary>
        /// Calculate the total amount
        /// </summary>
        public double CalculateTotal(double taxRate = 0)
        {
            return CalculateSubtotal() + CalculateTax(taxRate);
        }

        /// <summary>
        /// Clear the current order
        /// </summary>
        public void ClearOrder()
        {
            CurrentOrder.Clear();
        }

        /// <summary>
        /// Process payment and create invoice
        /// </summary>
        public (bool success, string message, Invoice? invoice) ProcessPayment(int staffId, string paymentMethod, double taxRate = 0)
        {
            if (CurrentOrder.Count == 0)
            {
                return (false, "No items in order.", null);
            }

            try
            {
                // Create invoice
                var invoice = new Invoice
                {
                    InvoiceDate = DateTime.Now,
                    StaffId = staffId,
                    TotalAmount = CalculateTotal(taxRate),
                    PaymentMethod = paymentMethod,
                    Status = "Completed",
                    InvoiceDetails = CurrentOrder.Select(o => new InvoiceDetail
                    {
                        BookId = o.Book.BookId,
                        Quantity = o.Quantity,
                        UnitPrice = o.UnitPrice,
                        Subtotal = o.Subtotal
                    }).ToList()
                };

                // Save invoice to database
                _invoiceRepo.Create(invoice);

                // Update book stock
                foreach (var item in CurrentOrder)
                {
                    _bookRepo.UpdateStock(item.Book.BookId, item.Quantity);
                }

                // Clear the order
                ClearOrder();

                return (true, "Payment processed successfully.", invoice);
            }
            catch (Exception ex)
            {
                return (false, $"Error processing payment: {ex.Message}", null);
            }
        }
    }
}
