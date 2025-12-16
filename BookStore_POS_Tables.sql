-- ===================================================
-- BookStoreDB - POS Tables Addition Script
-- Run this AFTER the main BookStoreDB.sql script
-- ===================================================

USE BookStoreDB
GO

-- =====================
-- INVOICE TABLE
-- =====================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Invoice')
BEGIN
    CREATE TABLE Invoice (
        InvoiceId int IDENTITY(1,1) PRIMARY KEY,
        InvoiceDate datetime NOT NULL DEFAULT GETDATE(),
        StaffId int NOT NULL,
        TotalAmount float NOT NULL,
        PaymentMethod nvarchar(20) NOT NULL,
        Status nvarchar(20) NOT NULL DEFAULT 'Completed'
    )

    ALTER TABLE Invoice
    ADD CONSTRAINT FK_Invoice_User
    FOREIGN KEY (StaffId) REFERENCES [User](MemberID)

    PRINT 'Invoice table created successfully.'
END
ELSE
BEGIN
    PRINT 'Invoice table already exists.'
END
GO

-- =====================
-- INVOICE DETAIL TABLE
-- =====================
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'InvoiceDetail')
BEGIN
    CREATE TABLE InvoiceDetail (
        InvoiceDetailId int IDENTITY(1,1) PRIMARY KEY,
        InvoiceId int NOT NULL,
        BookId int NOT NULL,
        Quantity int NOT NULL,
        UnitPrice float NOT NULL,
        Subtotal float NOT NULL
    )

    ALTER TABLE InvoiceDetail
    ADD CONSTRAINT FK_InvoiceDetail_Invoice
    FOREIGN KEY (InvoiceId) REFERENCES Invoice(InvoiceId)
    ON DELETE CASCADE

    ALTER TABLE InvoiceDetail
    ADD CONSTRAINT FK_InvoiceDetail_Book
    FOREIGN KEY (BookId) REFERENCES Book(BookId)

    PRINT 'InvoiceDetail table created successfully.'
END
ELSE
BEGIN
    PRINT 'InvoiceDetail table already exists.'
END
GO

-- ===================================================
-- VERIFY TABLES
-- ===================================================
SELECT 'Invoice Table:' AS Info, COUNT(*) AS Count FROM Invoice
SELECT 'InvoiceDetail Table:' AS Info, COUNT(*) AS Count FROM InvoiceDetail
GO

PRINT ''
PRINT '====================================================='
PRINT 'POS Tables Setup Complete!'
PRINT 'You can now use the POS system in the BookStore app.'
PRINT '====================================================='
GO
