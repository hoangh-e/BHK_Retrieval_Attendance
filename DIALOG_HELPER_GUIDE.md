# Dialog Helper Usage Guide

## Tổng quan

`DialogHelper` là một utility class cung cấp các phương thức hiển thị dialog thống nhất trong toàn bộ ứng dụng. Nó sử dụng `Ookii.Dialogs.Wpf.TaskDialog` để tạo ra các dialog đẹp và hiện đại, với fallback về `MessageBox` khi cần thiết.

## Khi nào nên sử dụng Dialog

### ✅ **NÊN sử dụng Dialog khi:**

1. **Thông báo kết quả thao tác**
   - Kết nối thiết bị thành công/thất bại
   - Lưu dữ liệu thành công/thất bại
   - Import/Export dữ liệu hoàn thành

2. **Xác nhận từ người dùng**
   - Xóa dữ liệu quan trọng
   - Ghi đè file có sẵn
   - Thoát ứng dụng khi có thay đổi chưa lưu

3. **Cảnh báo người dùng**
   - Dữ liệu không hợp lệ
   - Cài đặt không đúng
   - Lỗi không nghiêm trọng

4. **Lỗi cần người dùng biết**
   - Lỗi kết nối mạng
   - File không tồn tại
   - Quyền truy cập bị từ chối

### ❌ **KHÔNG nên sử dụng Dialog khi:**

1. **Validation thời gian thực**
   - Sử dụng Border màu đỏ, tooltip thay vì dialog

2. **Thông tin trạng thái liên tục**
   - Sử dụng StatusBar, ProgressBar thay vì dialog

3. **Thông báo debug/technical**
   - Chỉ log ra file/console, không hiện dialog cho user

## Các loại Dialog và cách sử dụng

### 1. Success Dialog - Thông báo thành công

```csharp
// Sử dụng cơ bản
DialogHelper.ShowSuccess("Kết nối thiết bị thành công!");

// Với title tùy chỉnh
DialogHelper.ShowSuccess(
    "Kết nối đến thiết bị ZKTeco U160 thành công!\nSeri: ABC123456",
    "Kết nối thành công"
);
```

**Khi sử dụng:**
- Kết nối thiết bị thành công
- Lưu cài đặt thành công
- Export dữ liệu hoàn thành
- Import dữ liệu thành công

### 2. Error Dialog - Thông báo lỗi

```csharp
// Lỗi đơn giản
DialogHelper.ShowError("Không thể kết nối đến thiết bị");

// Lỗi với chi tiết kỹ thuật
DialogHelper.ShowError(
    "Không thể kết nối đến thiết bị",
    "Timeout after 5 seconds. Device may be offline or IP address is incorrect.",
    "Lỗi kết nối"
);

// Từ Exception
catch (Exception ex)
{
    DialogHelper.ShowError(
        "Lỗi khi lưu dữ liệu",
        ex.Message,
        "Lỗi hệ thống"
    );
}
```

**Khi sử dụng:**
- Kết nối thiết bị thất bại
- Lỗi đọc/ghi file
- Lỗi validation nghiêm trọng
- Exception không xử lý được

### 3. Warning Dialog - Cảnh báo

```csharp
// Cảnh báo đơn giản
DialogHelper.ShowWarning("Thiết bị đang được sử dụng bởi ứng dụng khác");

// Cảnh báo với hướng dẫn
DialogHelper.ShowWarning(
    "Dữ liệu trên thiết bị đã đầy",
    "Vui lòng xóa dữ liệu cũ hoặc sao lưu trước khi tiếp tục",
    "Cảnh báo dung lượng"
);
```

**Khi sử dụng:**
- Dữ liệu có thể không chính xác
- Cài đặt không tối ưu
- Tình huống không lý tưởng nhưng có thể tiếp tục

### 4. Confirmation Dialog - Xác nhận

```csharp
// Xác nhận đơn giản (Yes/No)
if (DialogHelper.ShowConfirmation("Bạn có muốn xóa tất cả dữ liệu?"))
{
    // User chọn Yes
    DeleteAllData();
}

// Xác nhận với button text tùy chỉnh
if (DialogHelper.ShowConfirmation(
    "Tìm thấy dữ liệu mới trên thiết bị. Bạn muốn tải về?",
    "Tải về ngay",
    "Bỏ qua",
    "Dữ liệu mới"))
{
    // User chọn "Tải về ngay"
    DownloadData();
}
```

**Khi sử dụng:**
- Xóa dữ liệu quan trọng
- Ghi đè file có sẵn
- Thoát khi có thay đổi chưa lưu
- Thao tác không thể undo

### 5. Information Dialog - Thông tin

```csharp
DialogHelper.ShowInformation("Ứng dụng đã được cập nhật lên phiên bản 2.1.0");

DialogHelper.ShowInformation(
    $"Thiết bị: {deviceInfo.Name}\\nSeri: {deviceInfo.Serial}\\nFirmware: {deviceInfo.Version}",
    "Thông tin thiết bị"
);
```

**Khi sử dụng:**
- Hiển thị thông tin thiết bị
- Thông báo cập nhật
- Hướng dẫn sử dụng

## Tích hợp với Result Pattern

### Cách sử dụng kết hợp

```csharp
public async Task ConnectToDeviceAsync()
{
    var result = await _deviceService.ConnectAsync(ipAddress, port);
    
    if (result.IsSuccess)
    {
        // ✅ Hiển thị thành công
        DialogHelper.ShowSuccess(
            result.Message,
            "Kết nối thành công"
        );
        
        // Enable các control khác
        EnableDeviceControls();
    }
    else
    {
        // ❌ Hiển thị lỗi với chi tiết
        DialogHelper.ShowError(
            result.Message,
            result.ErrorDetails,
            "Kết nối thất bại"
        );
        
        // Vẫn để form mở để user thử lại
    }
}
```

### Xử lý Exception

```csharp
try
{
    var result = await SomeOperationAsync();
    
    if (result.IsSuccess)
    {
        DialogHelper.ShowSuccess(result.Message);
    }
    else
    {
        DialogHelper.ShowError(result.Message, result.ErrorDetails);
    }
}
catch (Exception ex)
{
    // Log exception
    _logger.LogError(ex, "Unexpected error in operation");
    
    // Show user-friendly error
    DialogHelper.ShowError(
        "Đã xảy ra lỗi không mong muốn",
        ex.Message,
        "Lỗi hệ thống"
    );
}
```

## Best Practices

### ✅ **Nên làm:**

1. **Sử dụng tiếng Việt cho user message**
   ```csharp
   DialogHelper.ShowError("Không thể kết nối đến thiết bị", technicalDetails);
   ```

2. **Đưa chi tiết kỹ thuật vào ErrorDetails**
   ```csharp
   DialogHelper.ShowError(
       "Lỗi khi lưu file",           // User-friendly message
       $"IOException: {ex.Message}", // Technical details
       "Lỗi I/O"
   );
   ```

3. **Sử dụng title có ý nghĩa**
   ```csharp
   DialogHelper.ShowSuccess("Dữ liệu đã được lưu", "Lưu thành công");
   ```

4. **Combine với logging**
   ```csharp
   _logger.LogError(ex, "Connection failed");
   DialogHelper.ShowError("Kết nối thất bại", ex.Message);
   ```

### ❌ **Không nên làm:**

1. **Hiển thị dialog liên tục**
   ```csharp
   // ❌ Không làm như này
   foreach (var item in items)
   {
       if (item.IsInvalid)
           DialogHelper.ShowError($"Item {item.Id} invalid");
   }
   ```

2. **Sử dụng dialog cho debug**
   ```csharp
   // ❌ Không làm như này
   DialogHelper.ShowInformation($"Debug: Variable x = {x}");
   ```

3. **Dialog quá dài**
   ```csharp
   // ❌ Không làm như này
   DialogHelper.ShowError("Very very very long message that spans multiple lines...");
   ```

## Ví dụ thực tế

### Device Connection

```csharp
private async Task ConnectAsync()
{
    try
    {
        IsConnecting = true;
        StatusMessage = "Đang kết nối...";

        var result = await _deviceService.ConnectAsync(IpAddress, Port);

        if (result.IsSuccess)
        {
            DialogHelper.ShowSuccess(
                $"Kết nối thành công đến {IpAddress}:{Port}",
                "Kết nối thiết bị"
            );
            EnableDeviceFeatures();
        }
        else
        {
            DialogHelper.ShowError(
                "Không thể kết nối đến thiết bị",
                $"IP: {IpAddress}, Port: {Port}\\nChi tiết: {result.ErrorDetails}",
                "Lỗi kết nối"
            );
        }
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Connection exception");
        DialogHelper.ShowError("Lỗi khi kết nối thiết bị", ex.Message);
    }
    finally
    {
        IsConnecting = false;
    }
}
```

### Data Export

```csharp
private async Task ExportDataAsync()
{
    if (DialogHelper.ShowConfirmation(
        "Xuất dữ liệu sẽ mất vài phút. Bạn có muốn tiếp tục?",
        "Xuất dữ liệu"))
    {
        try
        {
            var result = await _exportService.ExportAsync();
            
            if (result.IsSuccess)
            {
                DialogHelper.ShowSuccess(
                    $"Đã xuất {result.Data.RecordCount} bản ghi thành công",
                    "Xuất dữ liệu"
                );
            }
            else
            {
                DialogHelper.ShowError(result.Message, result.ErrorDetails);
            }
        }
        catch (Exception ex)
        {
            DialogHelper.ShowError("Lỗi khi xuất dữ liệu", ex.Message);
        }
    }
}
```

---

## Tóm tắt

- **Success**: Thao tác hoàn thành thành công
- **Error**: Lỗi cần user biết và xử lý
- **Warning**: Cảnh báo nhưng có thể tiếp tục
- **Confirmation**: Cần xác nhận trước khi thực hiện
- **Information**: Thông tin bổ sung cho user

**Nguyên tắc chung**: Dialog phải có giá trị cho user, không spam, và luôn có hành động rõ ràng sau khi đóng dialog.