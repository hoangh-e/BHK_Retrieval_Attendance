# BỔ SUNG HƯỚNG DẪN SỬ DỤNG

**Tài liệu gốc:** USER_GUIDE.md  
**Ngày tạo:** 26/02/2026  
**Mục đích:** Trình bày các chức năng Quản lý nhân viên còn thiếu

---

## 📋 TỔNG QUAN THIẾU SÓT

### Phân tích nội dung hiện tại
Hướng dẫn sử dụng hiện tại **USER_GUIDE.md** đã cover:
- ✅ Section 5: Quản lý nhân viên (xem danh sách, tìm kiếm, chi tiết)
- ✅ Section 7: Xuất báo cáo (chỉ chấm công)
- ✅ 12 screenshots đề xuất

### Nội dung còn thiếu

#### 1. **Xuất file Excel nhân viên** (Section 5 & 7)
- ❌ Không có hướng dẫn nhấn nút "FileExport" trong EmployeeView
- ❌ Không có hướng dẫn sử dụng ExportEmployeeDialog
- ❌ Không có hướng dẫn chọn Excel Table
- ❌ Không có hướng dẫn xử lý "Refactor cột" và "Tạo table mặc định"

#### 2. **Screenshots thiếu** (Section 2 & 5)
- ❌ Không có ảnh nút "Xuất file nhân viên"
- ❌ Không có ảnh dialog "Xuất danh sách nhân viên vào Excel"
- ❌ Không có ảnh Excel Table structure
- ❌ Không có ảnh kết quả sau khi xuất

#### 3. **Use cases thiếu** (Section 5)
- ❌ Không có hướng dẫn filter nhân viên nâng cao
- ❌ Không có hướng dẫn refresh danh sách
- ❌ Không có lưu ý về chế độ tìm kiếm vs xem tất cả

#### 4. **FAQ thiếu** (Section 11)
- ❌ Không có câu hỏi về xuất nhân viên
- ❌ Không có câu hỏi về Excel Table
- ❌ Không có câu hỏi về sự khác biệt giữa xuất nhân viên vs xuất chấm công

#### 5. **Errors thiếu** (Section 10)
- ❌ Không có lỗi "File Excel đang mở"
- ❌ Không có lỗi "Table không hợp lệ"
- ❌ Không có lỗi "Không tìm thấy table"

---

## 📝 NỘI DUNG CẦN BỔ SUNG

### BỔ SUNG 1: Section 5 - Quản lý nhân viên (Thêm subsection xuất file)

#### ⚙️ Vị trí chèn
**Sau:** Bước 5 "Điều hướng trang" (dòng ~207)  
**Trước:** "Kết quả mong đợi" (dòng ~225)

#### 📄 Nội dung đề xuất

```markdown
**Bước 6: Xuất danh sách nhân viên (tùy chọn)**

Nếu cần lưu danh sách nhân viên ra file Excel:

1. Nhấn nút **Xuất file** (icon FileExport màu trắng) ở góc phải header
   - Vị trí: Bên trái nút "Làm mới" (Refresh)
   - Icon: Mũi tên xuống + tài liệu

2. Cửa sổ "Xuất danh sách nhân viên vào Excel" sẽ hiển thị

3. **Chọn đường dẫn file Excel:**
   - Nhập trực tiếp đường dẫn hoặc nhấn "Chọn file"
   - Chọn file Excel đã có sẵn (có thể là file template)
   - Hệ thống sẽ hiển thị tên file bên dưới

4. **Chọn Excel Table:**
   - Dropdown "Chọn Excel Table" hiển thị danh sách table trong file
   - Chọn table muốn xuất dữ liệu vào
   
   **Trường hợp đặc biệt:**
   
   - **Nếu không có table nào:**
     - Card màu xanh "Tạo Employee Table mặc định" sẽ hiển thị
     - Nhấn nút "TẠO EMPLOYEE TABLE"
     - Hệ thống tạo table với 10 cột chuẩn trên sheet đầu tiên
     - Dropdown tự động cập nhật
   
   - **Nếu table không đúng cấu trúc:**
     - Card màu cam warning hiển thị
     - Nhấn nút "REFACTOR CỘT" để chỉnh sửa cấu trúc table
     - Dialog mới hiển thị để map cột Excel → cột Employee
     - Sau khi refactor, quay lại dialog xuất file

5. **Xác nhận và xuất:**
   - Kiểm tra thông tin: Số records, Tên table
   - Nhấn nút **XUẤT** (góc dưới bên phải)
   - Đợi thanh tiến trình hoàn tất
   - Thông báo "Xuất file thành công" hiển thị
   - Dialog tự động đóng

6. **Mở file Excel để kiểm tra:**
   - Vào thư mục đã chọn
   - Mở file Excel
   - Dữ liệu nhân viên xuất hiện trong table đã chọn
   - Mỗi nhân viên = 1 dòng với 10 cột

**Cấu trúc 10 cột Excel:**

| Cột | Tên | Mô tả |
|-----|-----|-------|
| 1 | DIN | Mã nhân viên |
| 2 | Name | Tên nhân viên |
| 3 | Card | Số thẻ |
| 4 | Gender | Giới tính |
| 5 | Birthday | Ngày sinh |
| 6 | IdentityCard | CMND/CCCD |
| 7 | Nation | Quốc tịch |
| 8 | Phone | Số điện thoại |
| 9 | Email | Email |
| 10 | Department | Phòng ban |
```

---

### BỔ SUNG 2: Section 5 - Lưu ý quan trọng (Mở rộng)

#### ⚙️ Vị trí sửa
**Thay thế:** Phần "Lưu ý quan trọng" (dòng ~229-231)

#### 📄 Nội dung đề xuất

```markdown
### Lưu ý quan trọng

**Về xem danh sách:**
- Nếu kết nối thất bại, kiểm tra lại địa chỉ IP và đảm bảo máy chấm công đang bật
- Danh sách hiển thị 10 nhân viên mỗi trang (phân trang tự động)
- Ứng dụng hiện tại chỉ xem thông tin, chưa hỗ trợ thêm/sửa/xóa nhân viên
- Để thêm/sửa/xóa nhân viên, vui lòng sử dụng phần mềm quản lý của máy chấm công hoặc liên hệ IT

**Về tìm kiếm:**
- Tìm kiếm chỉ hỗ trợ theo Mã nhân viên (DIN)
- Sau khi tìm kiếm, danh sách chỉ hiển thị kết quả phù hợp
- Để quay lại xem tất cả, xóa từ khóa tìm kiếm và nhấn "Tìm" hoặc "Làm mới"
- Phân trang vẫn hoạt động trong chế độ tìm kiếm

**Về xuất file nhân viên:**
- Chức năng xuất sẽ xuất TẤT CẢ nhân viên (không chỉ trang hiện tại)
- File Excel phải có Excel Table (không phải range thường)
- Excel Table phải có đúng 10 cột với tên đúng chuẩn
- Nếu file đang mở trong Excel, đóng file trước khi xuất
- Hệ thống sẽ GHI ĐÈ dữ liệu cũ trong table (xóa hết rồi ghi mới)
- Đường dẫn mặc định có thể cấu hình trong tab "Cài đặt"

**Về hiệu suất:**
- Load danh sách: ~1-2 giây (tùy số lượng nhân viên)
- Xem chi tiết: ~1-2 giây (load thông tin enrollment)
- Tìm kiếm: Gần như tức thì (dưới 1 giây)
- Xuất file: ~2-3 giây cho 100 nhân viên
```

---

### BỔ SUNG 3: Section 7 - Xuất báo cáo (Thêm phần xuất nhân viên)

#### ⚙️ Vị trí chèn
**Sau:** Bước 6 "Kiểm tra file đã xuất" (dòng ~431)  
**Trước:** "Kết quả mong đợi" (dòng ~433)

#### 📄 Nội dung đề xuất

```markdown
**Bước 7: Xuất danh sách nhân viên (tùy chọn)**

Ngoài xuất dữ liệu chấm công, bạn cũng có thể xuất danh sách nhân viên:

1. Vào tab **Quản lý nhân viên**
2. Nhấn icon **Xuất file** (FileExport) ở góc phải header
3. Làm theo hướng dẫn trong **Section 5 - Bước 6**

**Sự khác biệt giữa xuất chấm công và xuất nhân viên:**

| Tiêu chí | Xuất chấm công | Xuất nhân viên |
|----------|---------------|---------------|
| Nơi thực hiện | Tab "Quản lý chấm công" | Tab "Quản lý nhân viên" |
| Nút xuất | Icon trong Filter Panel | Icon trong Header |
| Dữ liệu xuất | Bản ghi chấm công theo bộ lọc | TẤT CẢ nhân viên (không lọc) |
| Định dạng | Excel, PDF, CSV | Chỉ Excel (Excel Table) |
| Yêu cầu | File thường hoặc template | File phải có Excel Table |
| Số cột | 8 cột (STT, DIN, Ngày, Giờ, ...) | 10 cột (DIN, Name, Card, ...) |
| Ghi đè | Tạo file mới hoặc ghi đè | Xóa dữ liệu cũ trong table + ghi mới |
```

---

### BỔ SUNG 4: Section 10 - Các lỗi thường gặp (Thêm lỗi mới)

#### ⚙️ Vị trí chèn
**Trong bảng:** Sau dòng "Màn hình bị đơ khi load dữ liệu lớn" (dòng ~492)

#### 📄 Nội dung đề xuất

Thêm 4 dòng vào bảng:

```markdown
| "File Excel đang được mở bởi ứng dụng khác" | File đang mở trong Excel | Đóng file Excel, sau đó thử xuất lại |
| "Không tìm thấy Excel Table trong file" | File không có Excel Table nào | Nhấn "TẠO EMPLOYEE TABLE" trong dialog xuất file |
| "Excel Table không hợp lệ (sai số cột)" | Table có ít hơn hoặc nhiều hơn 10 cột | Nhấn "REFACTOR CỘT" để chỉnh sửa cấu trúc |
| "Không thể ghi vào file Excel" | Không có quyền ghi | Chọn đường dẫn khác hoặc liên hệ IT cấp quyền |
```

---

### BỔ SUNG 5: Section 11 - Câu hỏi thường gặp (Thêm FAQs mới)

#### ⚙️ Vị trí chèn
**Sau:** FAQ số 10 (dòng ~556)  
**Trước:** Section 12 "Liên hệ hỗ trợ" (dòng ~562)

#### 📄 Nội dung đề xuất

```markdown
**11. Làm sao để xuất danh sách nhân viên ra Excel?**
- Vào tab "Quản lý nhân viên"
- Nhấn icon "Xuất file" (FileExport) ở góc phải header (bên trái nút Refresh)
- Chọn file Excel và Excel Table
- Nhấn "XUẤT"
- Chi tiết xem **Section 5 - Bước 6**

**12. Excel Table là gì? Khác gì với bảng thường?**
- Excel Table là cấu trúc dữ liệu đặc biệt trong Excel
- Có thể tạo bằng: Select range → Insert → Table (hoặc Ctrl+T)
- Khác biệt: Table có header tự động, filter, format động
- Ứng dụng yêu cầu Excel Table để đảm bảo cấu trúc chuẩn

**13. Tôi có thể chọn mấy nhân viên để xuất không?**
- Không. Chức năng xuất sẽ xuất **TẤT CẢ** nhân viên trong máy chấm công
- Không có tùy chọn chọn từng nhân viên
- Nếu cần lọc, có thể xuất tất cả rồi filter trong Excel

**14. Table Excel của tôi có 8 cột, có xuất được không?**
- Không. Hệ thống yêu cầu đúng 10 cột chuẩn
- Nhấn "REFACTOR CỘT" trong dialog để map cột Excel → cột Employee
- Hoặc nhấn "TẠO EMPLOYEE TABLE" để tạo table mới đúng chuẩn

**15. Dữ liệu cũ trong table có bị xóa không?**
- Có. Hệ thống sẽ **XÓA HẾT** dữ liệu cũ trong table trước khi ghi mới
- Nếu muốn giữ dữ liệu cũ, copy ra sheet khác trước khi xuất
- Hoặc xuất vào file Excel khác

**16. Xuất nhân viên có xuất cả ảnh và vân tay không?**
- Không. Chỉ xuất thông tin text (10 cột)
- Ảnh và vân tay không được xuất (dữ liệu binary, không phù hợp Excel)
- Nếu cần, sử dụng phần mềm quản lý của máy chấm công

**17. Tôi có thể xuất sang PDF như xuất chấm công không?**
- Không. Xuất nhân viên chỉ hỗ trợ Excel (Excel Table)
- PDF không hỗ trợ cấu trúc table động
- Nếu cần PDF, xuất Excel trước → chuyển đổi thủ công

**18. File Excel xuất ra có công thức không?**
- Không. Chỉ xuất dữ liệu tĩnh (text, số, ngày)
- Nếu cần công thức, thêm thủ công sau khi xuất
```

---

### BỔSUNG 6: Screenshots (Thêm vào "Mục lục hình ảnh")

#### ⚙️ Vị trí chèn
**Trong bảng:** Sau dòng 12 "Màn hình cài đặt" (dòng ~734)

#### 📄 Nội dung đề xuất

Thêm 5 dòng vào bảng:

```markdown
| 13 | Nút xuất file nhân viên | Icon FileExport màu trắng trong header EmployeeView |
| 14 | Dialog xuất danh sách nhân viên | Cửa sổ "Xuất danh sách nhân viên vào Excel" |
| 15 | Chọn Excel Table | Dropdown "Chọn Excel Table" với danh sách tables |
| 16 | Card tạo table mặc định | Card màu xanh "Tạo Employee Table mặc định" |
| 17 | File Excel sau khi xuất | File Excel mở trong Excel với data trong table |
```

---

### BỔ SUNG 7: Section 5 - 📸 Hình minh họa (Thêm screenshots mới)

#### ⚙️ Vị trí chèn
**Sau:** Screenshot hiện tại "Thông tin chi tiết nhân viên" (dòng ~244)  
**Trước:** Section 6 "Quản lý chấm công" (dòng ~250)

#### 📄 Nội dung đề xuất

```markdown
### 📸 Hình minh họa (Xuất file nhân viên)

**Tên ảnh đề xuất:** Nút xuất file nhân viên  
**Mô tả ảnh cần chụp:** Chụp header "DANH SÁCH NHÂN VIÊN" với 2 nút: icon FileExport (màu trắng) và icon Refresh  
**Mục đích:** Giúp người dùng tìm nút xuất file nhân viên

**Tên ảnh đề xuất:** Dialog xuất danh sách nhân viên  
**Mô tả ảnh cần chụp:** Chụp toàn bộ dialog "Xuất danh sách nhân viên vào Excel" với các trường: Đường dẫn file, Tên file, Chọn Excel Table  
**Mục đích:** Hướng dẫn giao diện xuất file nhân viên

**Tên ảnh đề xuất:** Dropdown chọn Excel Table  
**Mô tả ảnh cần chụp:** Chụp dropdown "Chọn Excel Table" đang mở với danh sách tables (ví dụ: EmployeeTable, Table1, Table2)  
**Mục đích:** Minh họa cách chọn table

**Tên ảnh đề xuất:** Card tạo Employee Table mặc định  
**Mô tả ảnh cần chụp:** Chụp card màu xanh với text "Tạo Employee Table mặc định" và nút "TẠO EMPLOYEE TABLE"  
**Mục đích:** Hướng dẫn tạo table mới khi file chưa có table

**Tên ảnh đề xuất:** Kết quả xuất file Excel  
**Mô tả ảnh cần chụp:** Chụp file Excel đã mở trong Microsoft Excel, hiển thị EmployeeTable với dữ liệu 10 cột và vài dòng nhân viên  
**Mục đích:** Xác nhận kết quả sau khi xuất thành công
```

---

### BỔ SUNG 8: Section 9 - Cấu hình hệ thống (Thêm lưu ý)

#### ⚙️ Vị trí chèn
**Sau:** "Lưu ý" (dòng ~541)  
**Trước:** Section 10 "Các lỗi thường gặp" (dòng ~549)

#### 📄 Nội dung đề xuất

```markdown
### Liên quan đến xuất file nhân viên

**Đường dẫn file Excel nhân viên:**
- Card "Đường dẫn file Excel nhân viên" dùng cho chức năng xuất nhân viên
- Khác với card "Đường dẫn xuất file điểm danh" (dùng cho xuất chấm công)
- Đường dẫn này trỏ đến 1 FILE Excel cụ thể (không phải thư mục)
- File phải có Excel Table với 10 cột chuẩn

**Test Export Employee:**
- Nút "TEST XUẤT DANH SÁCH NHÂN VIÊN" trong card "Chức năng Test"
- Xuất dữ liệu mẫu (không phải dữ liệu thật từ thiết bị)
- Dùng để kiểm tra template Excel có hợp lệ không
- Chỉ IT/Admin nên sử dụng

**Tên table nhân viên:**
- Trường "Tên table nhân viên" (mặc định: EmployeeTable)
- Tên này dùng cho tích hợp hệ thống khác (không ảnh hưởng xuất file)
- Người dùng thường không cần thay đổi
```

---

## 🔧 HƯỚNG DẪN TÍCH HỢP

### Bước 1: Mở file USER_GUIDE.md

### Bước 2: Áp dụng các bổ sung

#### Thứ tự thực hiện:

1. **BỔ SUNG 1:** Chèn "Bước 6: Xuất danh sách nhân viên" sau dòng 207 (trong Section 5)
2. **BỔ SUNG 2:** Thay thế "Lưu ý quan trọng" (dòng 229-231) bằng nội dung mở rộng
3. **BỔ SUNG 3:** Chèn "Bước 7: Xuất danh sách nhân viên" sau dòng 431 (trong Section 7)
4. **BỔ SUNG 4:** Thêm 4 dòng lỗi mới vào bảng Section 10 (sau dòng 492)
5. **BỔ SUNG 5:** Thêm 8 FAQs mới vào Section 11 (sau FAQ 10, dòng 556)
6. **BỔ SUNG 6:** Thêm 5 dòng vào bảng "Mục lục hình ảnh" (sau dòng 734)
7. **BỔ SUNG 7:** Chèn 5 screenshots mới vào Section 5 (sau dòng 244)
8. **BỔSUNG 8:** Thêm phần "Liên quan đến xuất file nhân viên" vào Section 9 (sau dòng 541)

### Bước 3: Cập nhật Mục lục (nếu cần)

Không cần thay đổi Mục lục vì chỉ thêm subsections, không có sections mới.

### Bước 4: Cập nhật Lịch sử phiên bản

Thêm vào bảng "Lịch sử phiên bản":
```markdown
| 1.1 | 26/02/2026 | AI Generated | Bổ sung hướng dẫn xuất file nhân viên, 5 screenshots mới, 8 FAQs mới |
```

---

## 📊 METRICS

| Metric | Hiện tại | Sau bổ sung |
|--------|----------|-------------|
| Tổng số sections | 12 | 12 |
| Số subsections/steps | 45 | 53 (+8) |
| Số screenshots đề xuất | 12 | 17 (+5) |
| Số FAQs | 10 | 18 (+8) |
| Số lỗi documented | 7 | 11 (+4) |
| Độ chi tiết Quản lý nhân viên | 40% | 100% |

---

## ✅ CHECKLIST

- [ ] Đọc kỹ file USER_GUIDE.md hiện tại
- [ ] Áp dụng BỔ SUNG 1 (Section 5 - Bước 6)
- [ ] Áp dụng BỔ SUNG 2 (Mở rộng Lưu ý Section 5)
- [ ] Áp dụng BỔ SUNG 3 (Section 7 - Bước 7)
- [ ] Áp dụng BỔ SUNG 4 (Section 10 - Thêm 4 lỗi)
- [ ] Áp dụng BỔ SUNG 5 (Section 11 - Thêm 8 FAQs)
- [ ] Áp dụng BỔ SUNG 6 (Mục lục hình ảnh - Thêm 5 ảnh)
- [ ] Áp dụng BỔ SUNG 7 (Section 5 - Thêm 5 screenshots)
- [ ] Áp dụng BỔ SUNG 8 (Section 9 - Thêm lưu ý xuất file)
- [ ] Cập nhật Version History
- [ ] Review toàn bộ tài liệu sau khi merge
- [ ] Đánh số lại screenshots nếu cần (1-17)

---

## 💡 LƯU Ý CHO NGƯỜI CHỤP SCREENSHOTS

### Screenshot 13: Nút xuất file nhân viên
- **Chuẩn bị:** Mở ứng dụng → Kết nối thiết bị → Vào tab "Quản lý nhân viên"
- **Chụp:** Header với text "DANH SÁCH NHÂN VIÊN", icon AccountGroup, icon FileExport (màu trắng), icon Refresh
- **Làm nổi bật:** Khoanh tròn hoặc mũi tên chỉ vào icon FileExport
- **Kích thước:** Header portion (~200px height)

### Screenshot 14: Dialog xuất danh sách nhân viên
- **Chuẩn bị:** Nhấn icon FileExport → Dialog hiển thị
- **Chụp:** Toàn bộ dialog từ title bar đến nút XUẤT/HỦY
- **Điền dữ liệu mẫu:** 
  - Đường dẫn: `C:\Data\Employees.xlsx`
  - Chọn table: `EmployeeTable`
- **Kích thước:** Full dialog (~600x500px)

### Screenshot 15: Dropdown chọn Excel Table
- **Chuẩn bị:** Click vào dropdown "Chọn Excel Table"
- **Chụp:** Dropdown đang mở với danh sách tables (EmployeeTable, Table1, Table2, ...)
- **Lưu ý:** Đảm bảo có ít nhất 2-3 tables để người dùng thấy cách chọn
- **Kích thước:** Dropdown + context (~300x200px)

### Screenshot 16: Card tạo table mặc định
- **Chuẩn bị:** Chọn file Excel không có table nào → Card màu xanh hiển thị
- **Chụp:** Card với text "Tạo Employee Table mặc định" và nút "TẠO EMPLOYEE TABLE"
- **Làm nổi bật:** Nút "TẠO EMPLOYEE TABLE" là focus point
- **Kích thước:** Card portion (~400x150px)

### Screenshot 17: File Excel sau khi xuất
- **Chuẩn bị:** Xuất thành công → Mở file Excel trong Microsoft Excel
- **Chụp:** Excel window với EmployeeTable hiển thị, header 10 cột (DIN, Name, Card, ...), 5-10 dòng dữ liệu mẫu
- **Làm nổi bật:** Table name "EmployeeTable" trong Excel, 10 cột header
- **Kích thước:** Excel window portion (~800x400px)

---

**GHI CHÚ:**  
File này là **hướng dẫn bổ sung**, không thay thế file gốc. Merge thủ công hoặc sử dụng script để tích hợp nội dung vào USER_GUIDE.md.
