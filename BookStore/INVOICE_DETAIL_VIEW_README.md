# Màn Hình Xem Hóa ??n Chi Ti?t

## T?ng Quan
Màn hình **Xem Hóa ??n Chi Ti?t** cho phép xem danh sách chi ti?t t?t c? các hóa ??n ?ã thanh toán trong h? th?ng. Màn hình này hi?n th? thông tin chi ti?t t? b?ng `InvoiceDetail` và include thông tin t? b?ng `Invoice` và `User`.

## V? Trí
- **Sidebar**: Section "BÁN HÀNG"
- **Button**: ?? Xem Hóa ??n
- **View**: `Team4.BookStore/Views/InvoiceDetailView.xaml`

## C?u Trúc D? Án

### 1. Backend Layer (DAL & BLL)

#### InvoiceDetailRepo.cs
**???ng d?n**: `BookStore.DAL/Repositories/InvoiceDetailRepo.cs`

**Ch?c n?ng**:
- `GetAll()`: L?y t?t c? invoice details v?i Invoice và User (Staff) included
- `GetById(int)`: L?y invoice detail theo ID
- `GetByInvoiceId(int)`: L?y t?t c? invoice details c?a m?t hóa ??n
- `Search(string)`: Tìm ki?m theo tên sách, tên nhân viên, ho?c mã hóa ??n
- `GetByDateRange(DateTime, DateTime)`: L?c theo kho?ng th?i gian
- `GetByStaffId(int)`: L?c theo nhân viên

**Include Strategy**:
```csharp
_ctx.InvoiceDetails
    .Include(id => id.Invoice)
        .ThenInclude(i => i.Staff)  // User entity
    .Include(id => id.Book)
```

#### InvoiceDetailService.cs
**???ng d?n**: `BookStore.BLL/Services/InvoiceDetailService.cs`

**Ch?c n?ng**:
- T?ng business logic wrapper cho Repository
- X? lý logic nghi?p v? và validation
- Các methods t??ng ?ng v?i Repository

### 2. Frontend Layer (WPF)

#### InvoiceDetailView.xaml
**???ng d?n**: `Team4.BookStore/Views/InvoiceDetailView.xaml`

**Giao Di?n**:
- Header v?i tiêu ?? và mô t?
- Search box ?? tìm ki?m
- Filter buttons: "Hôm nay", "Tháng này"
- Refresh button
- DataGrid hi?n th? d? li?u
- Summary section: T?ng s? b?n ghi và t?ng doanh thu

**DataGrid Columns**:
1. ID Chi Ti?t
2. Mã Hóa ??n
3. Ngày T?o (dd/MM/yyyy HH:mm)
4. Nhân Viên (Staff.FullName)
5. Tên Sách (Book.BookName)
6. S? L??ng
7. ??n Giá (formatted with VND)
8. Thành Ti?n (Subtotal, formatted with VND)
9. Ph??ng Th?c (PaymentMethod)
10. Tr?ng Thái (Status)

#### InvoiceDetailView.xaml.cs
**???ng d?n**: `Team4.BookStore/Views/InvoiceDetailView.xaml.cs`

**Ch?c N?ng**:
- `LoadAllInvoiceDetails()`: Load t?t c? d? li?u khi kh?i ??ng
- `SearchTextBox_TextChanged()`: Tìm ki?m real-time
- `TodayButton_Click()`: Filter hóa ??n hôm nay
- `ThisMonthButton_Click()`: Filter hóa ??n tháng này
- `RefreshButton_Click()`: Làm m?i d? li?u
- `UpdateSummary()`: C?p nh?t th?ng kê t?ng quan

### 3. Integration v?i MainWindow

#### MainWindow.xaml
- Thêm button "?? Xem Hóa ??n" vào sidebar, section "BÁN HÀNG"

#### MainWindow.xaml.cs
- Thêm field `_invoiceDetailView`
- Thêm method `LoadInvoiceDetailView()`
- Thêm click handler `InvoiceDetailButton_Click()`
- C?p nh?t highlight logic cho button m?i

## C? S? D? Li?u

### B?ng Liên Quan
1. **InvoiceDetail** (chính)
   - InvoiceDetailId
   - InvoiceId (FK)
   - BookId (FK)
   - Quantity
   - UnitPrice
   - Subtotal

2. **Invoice** (included)
   - InvoiceId
   - InvoiceDate
   - StaffId (FK)
   - TotalAmount
   - PaymentMethod
   - Status

3. **User** (included via Invoice.Staff)
   - MemberId
   - FullName
   - EmailAddress
   - RoleId

4. **Book** (included)
   - BookId
   - BookName
   - Price

## Tính N?ng

### 1. View-Only Mode
- Màn hình ch? cho phép **xem** d? li?u, không có ch?c n?ng thêm/s?a/xóa
- DataGrid ???c set `IsReadOnly="True"`

### 2. Tìm Ki?m
- Tìm ki?m theo:
  - Tên sách
  - Tên nhân viên
  - Mã hóa ??n
- Real-time search khi gõ

### 3. L?c D? Li?u
- **Hôm nay**: Hi?n th? hóa ??n ngày hôm nay
- **Tháng này**: Hi?n th? hóa ??n tháng hi?n t?i

### 4. Th?ng Kê
- T?ng s? b?n ghi ?ang hi?n th?
- T?ng doanh thu (tính t? Subtotal)
- C?p nh?t theo filter/search

### 5. Làm M?i
- Button "Làm m?i" ?? reload toàn b? d? li?u
- Clear search box và filter

## Styling

### Color Scheme
- Header: `#34495e` (dark gray-blue)
- Primary button: `#3498db` (blue)
- Success color: `#27ae60` (green) - cho revenue và status
- Info color: `#16a085` (teal)
- Secondary: `#95a5a6` (gray)

### Typography
- Header: 24px, Bold
- Content: 14px
- Summary: 16px, Bold (for revenue)

## Cách S? D?ng

1. **M? Màn Hình**:
   - Click vào button "?? Xem Hóa ??n" trong sidebar

2. **Xem D? Li?u**:
   - Danh sách hi?n th? t?t c? invoice details
   - Scroll ?? xem thêm

3. **Tìm Ki?m**:
   - Nh?p t? khóa vào search box
   - K?t qu? t? ??ng l?c

4. **L?c Theo Ngày**:
   - Click "Hôm nay" ?? xem hóa ??n hôm nay
   - Click "Tháng này" ?? xem hóa ??n tháng này

5. **Làm M?i**:
   - Click "Làm m?i" ?? reset v? tr?ng thái ban ??u

6. **Xem Th?ng Kê**:
   - Ki?m tra ph?n d??i cùng ?? xem t?ng s? b?n ghi và doanh thu

## Testing

### Test Cases
1. Load data khi m? màn hình
2. Search functionality
3. Filter by today
4. Filter by this month
5. Refresh data
6. Summary calculations
7. DataGrid display v?i format ?úng

### Sample Test
```csharp
// Test search
SearchTextBox.Text = "Harry Potter";
// Verify filtered results

// Test today filter
TodayButton_Click(null, null);
// Verify only today's records

// Test summary
var details = _service.GetAllInvoiceDetails();
var expectedRevenue = details.Sum(d => d.Subtotal);
// Verify TotalRevenueLabel shows correct value
```

## Dependencies

### NuGet Packages
- Entity Framework Core (cho Include và queries)
- WPF framework

### Internal Dependencies
- `BookStore.DAL.Entities`
- `BookStore.DAL.Repositories`
- `BookStore.BLL.Services`

## Future Enhancements

### Possible Features
1. Export to Excel
2. Print invoice details
3. Advanced filtering (by payment method, status)
4. Date range picker
5. Sort columns
6. Group by invoice
7. Detail view popup khi click vào row

## Troubleshooting

### Common Issues

1. **Data not loading**
   - Check database connection
   - Verify tables exist
   - Check Include statements

2. **Search not working**
   - Verify searchTerm is lowercase
   - Check entity properties are not null

3. **Summary not updating**
   - Ensure UpdateSummary() is called after each data change
   - Verify LINQ sum calculation

## Code Examples

### Add Include in Repository
```csharp
return _ctx.InvoiceDetails
    .Include(id => id.Invoice)
        .ThenInclude(i => i.Staff)
    .Include(id => id.Book)
    .ToList();
```

### Format Currency in XAML
```xml
Binding="{Binding Subtotal, StringFormat='{}{0:N0} VND'}"
```

### Filter by Date Range
```csharp
DateTime firstDay = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
DateTime lastDay = firstDay.AddMonths(1).AddDays(-1);
var result = _service.GetInvoiceDetailsByDateRange(firstDay, lastDay);
```

---

**Created**: January 2025  
**Version**: 1.0  
**Author**: BookStore Development Team
