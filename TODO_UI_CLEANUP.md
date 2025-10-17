# Danh sách công việc cần thực hiện

## ✅ Đã hoàn thành:
1. ✅ Xóa section "Connection Information" trong DeviceConnectionView.xaml
2. ✅ Xóa nút "TẠO TABLE" trong ExportAttendanceDialog.xaml
3. ✅ Xóa IsTableSelected binding trong nút XUẤT

## 🔄 Đang thực hiện - Xóa emoji icons:

### Files cần xóa emoji (UI-facing):

#### 1. **ExportEmployeeDialogViewModel.cs** - CẦN SỬA:
- Dòng 152: `StatusMessage = $"✅ Đã tạo..."` → Xóa ✅
- Dòng 160: `StatusMessage = $"❌ Lỗi..."` → Xóa ❌  
- Dòng 198: `StatusMessage = $"✅ Đã xuất..."` → Xóa ✅
- Dòng 201: `MessageBox.Show($"🎉 Xuất thành công..📊..📁..✨"` → Xóa tất cả emoji
- Dòng 215: `StatusMessage = $"❌ Lỗi..."` → Xóa ❌
- Dòng 288: `StatusMessage = $"✅ Tìm thấy..."` → Xóa ✅
- Dòng 298: `StatusMessage = "📋 File không..."` → Xóa 📋
- Dòng 303: `StatusMessage = $"❌ Lỗi..."` → Xóa ❌
- Dòng 335: `TableValidationMessage = $"⚠️ Cấu trúc..📋"` → Xóa ⚠️📋
- Dòng 342: `TableValidationMessage = $"✅ Cấu trúc..📋"` → Xóa ✅📋
- Dòng 351: `StatusMessage = $"📊 Excel table...✅"` → Xóa 📊✅
- Dòng 358: `TableValidationMessage = $"❌ Lỗi..."` → Xóa ❌
- Dòng 397: `StatusMessage = $"✅ Đã tạo..."` → Xóa ✅
- Dòng 405: `StatusMessage = $"❌ Lỗi..."` → Xóa ❌
- Dòng 448: `StatusMessage = $"❌ Lỗi..."` → Xóa ❌

#### 2. **RefactorColumnsDialogViewModel.cs** - CẦN SỬA:
- Dòng 98: `"✅ Đã refactor..."` → Xóa ✅
- Dòng 109: `$"❌ Lỗi..."` → Xóa ❌

#### 3. **ExportEmployeeDialog.xaml** - CẦN KIỂM TRA:
- Tìm các text note có emoji

## 📝 Còn lại cần làm:

### 4. Xóa text note không cần thiết:
- [ ] Xóa "Hoặc chọn table khác phù hợp"
- [ ] Xóa "(Excel Table khác với Worksheet..."

### 5. Chỉnh màu chữ trong ExportEmployeeDialog.xaml:
- [ ] Tìm các TextBlock có Foreground trắng/sáng
- [ ] Đổi sang màu dễ đọc hơn (MaterialDesignBody)

### 6. Quản lý nhân viên - Chỉ tìm kiếm theo ID:
- [ ] Mở EmployeeView.xaml  
- [ ] Xóa các TextBox tìm kiếm không phải ID
- [ ] Mở EmployeeViewModel.cs
- [ ] Sửa SearchCommand chỉ search theo ID

### 7. Thêm tab "Thông tin phần mềm":
- [ ] Mở MainWindow.xaml
- [ ] Thêm TabItem mới "Thông tin phần mềm" 
- [ ] Tạo AboutViewModel.cs
- [ ] Tạo AboutView.xaml với UI tương tự HistoryView
- [ ] Đăng ký ViewModel vào DI
- [ ] Bind navigation

## Hướng dẫn thực hiện tiếp:

### Bước 1: Xóa emoji trong ViewModels
```bash
# Chạy find & replace trong VS Code:
Find: (StatusMessage|TableValidationMessage|MessageBox\.Show)\s*=?\s*\$?"[^"]*([📊✅⚠️❌📁📋🎉💾🔄✨]+)[^"]*"
Replace: Manually review and remove emojis
```

### Bước 2: Kiểm tra XAML
```bash
# Tìm trong các Dialog.xaml:
Find: Text="[^"]*([📊✅⚠️❌📁📋🎉💾🔄✨]+)[^"]*"
```

### Bước 3: Test
- Build project
- Chạy app
- Kiểm tra tất cả dialogs
- Verify không còn emoji nào

---

**Lưu ý**: Comments trong code (// ✅) có thể giữ lại vì không ảnh hưởng UI.
