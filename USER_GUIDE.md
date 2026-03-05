# Hệ thống Quản lý Chấm công BHK

**Hướng dẫn sử dụng (User Guide)**
- Phiên bản tài liệu: 1.0
- Ngày cập nhật: 26/02/2026
- Đối tượng: Người dùng cuối

---
PAGE BREAK
---

## Mục lục

1. [Bắt đầu nhanh](#1-bắt-đầu-nhanh)
2. [Tổng quan giao diện](#2-tổng-quan-giao-diện)
3. [Kết nối thiết bị chấm công](#3-kết-nối-thiết-bị-chấm-công)
4. [Xem thông tin thiết bị](#4-xem-thông-tin-thiết-bị)
5. [Quản lý nhân viên](#5-quản-lý-nhân-viên)
6. [Quản lý chấm công](#6-quản-lý-chấm-công)
7. [Xuất báo cáo](#7-xuất-báo-cáo)
8. [Xem lịch sử hoạt động](#8-xem-lịch-sử-hoạt-động)
9. [Cấu hình hệ thống](#9-cấu-hình-hệ-thống)
10. [Các lỗi thường gặp](#10-các-lỗi-thường-gặp)
11. [Câu hỏi thường gặp](#11-câu-hỏi-thường-gặp)
12. [Liên hệ hỗ trợ](#12-liên-hệ-hỗ-trợ)

---
PAGE BREAK
---

## Lịch sử phiên bản

| Phiên bản | Ngày | Người thực hiện | Ghi chú |
|----------|------|---------------|--------|
| 1.0 | 26/02/2026 | AI Generated | Tài liệu hướng dẫn sử dụng ban đầu |

---
PAGE BREAK
---

## Thông tin hệ thống

| Thành phần | Thông tin |
|------------|----------|
| Tên hệ thống | Hệ thống Quản lý Chấm công BHK |
| Nền tảng | Ứng dụng Desktop (Windows) |
| Yêu cầu hệ điều hành | Windows 10 trở lên |
| Yêu cầu phần mềm | .NET 8.0 Runtime (tự động cài đặt cùng ứng dụng) |
| Thiết bị hỗ trợ | Máy chấm công Realand/ZKTeco (Model: ZDC2911) |
| Phạm vi sử dụng | Quản lý chấm công nội bộ doanh nghiệp |
| Định dạng xuất file | Excel (.xlsx, .csv), PDF |
| Kênh hỗ trợ | Liên hệ bộ phận IT nội bộ |

---
PAGE BREAK
---

## 1. Bắt đầu nhanh

### Mục đích
Hướng dẫn 5 bước cơ bản để bắt đầu sử dụng hệ thống.

### Các bước thực hiện

**Bước 1: Khởi động ứng dụng**
- Nhấn đúp vào icon "BHK Attendance System" trên màn hình desktop
- Ứng dụng sẽ mở màn hình kết nối thiết bị

**Bước 2: Nhập thông tin thiết bị**
- Nhập Địa chỉ IP của máy chấm công (ví dụ: 192.168.10.249)
- Nhập Cổng UDP (mặc định: 5500)
- Nhập Số hiệu thiết bị (DN) (thường là 1)
- Nhấn nút "KẾT NỐI"

**Bước 3: Kiểm tra kết nối**
- Đợi thông báo "Đã kết nối" xuất hiện
- Màn hình tự động chuyển sang trang chủ

**Bước 4: Xem thông tin**
- Tab "Thông tin thiết bị": Xem thông tin máy chấm công đã kết nối
- Tab "Quản lý nhân viên": Xem danh sách nhân viên trong máy
- Tab "Quản lý chấm công": Xem dữ liệu chấm công

**Bước 5: Xuất báo cáo (nếu cần)**
- Vào tab "Quản lý chấm công"
- Chọn khoảng thời gian cần xuất
- Nhấn biểu tượng "Xuất file" (icon xuất)
- Chọn định dạng Excel hoặc PDF và lưu file

### Kết quả mong đợi
Sau 5 bước trên, bạn đã:
- Kết nối thành công với máy chấm công
- Xem được thông tin thiết bị và dữ liệu chấm công
- Có thể xuất báo cáo khi cần

---
PAGE BREAK
---

## 2. Tổng quan giao diện

### Màn hình kết nối thiết bị (Màn hình đầu tiên)

Khi khởi động ứng dụng, bạn sẽ thấy màn hình "Kết nối thiết bị TCP/IP" với các phần:

**Phần nhập liệu:**
- **Địa chỉ IP*** (bắt buộc): Địa chỉ IP của máy chấm công
- **Cổng UDP*** (bắt buộc): Cổng kết nối (mặc định 5500)
- **Số hiệu thiết bị (DN)*** (bắt buộc): Số định danh thiết bị
- **Tên thiết bị**: Tự động hiển thị sau khi kết nối
- **Mật khẩu**: Mật khẩu thiết bị (nếu có cấu hình)
- **Mẫu thiết bị**: Hiển thị model thiết bị

**Phần trạng thái:**
- Hiển thị trạng thái kết nối (Chưa kết nối / Đã kết nối)
- Thanh tiến trình khi đang xử lý

**Các nút chức năng:**
- **KẾT NỐI** (màu xanh): Kết nối tới thiết bị
- **NGẮT KẾT NỐI** (màu cam): Ngắt kết nối
- **QUẢN LÝ THIẾT BỊ** (màu tím): Mở danh sách thiết bị đã lưu

### Màn hình chính (Sau khi kết nối)

Màn hình chính có tiêu đề "HỆ THỐNG QUẢN LÝ CHẤM CÔNG" với các tab bên trái:

1. **Thông tin thiết bị** (icon màn hình): Xem thông tin máy chấm công đã kết nối
2. **Quản lý nhân viên** (icon người dùng): Xem danh sách nhân viên
3. **Quản lý chấm công** (icon lịch): Xem và lọc dữ liệu chấm công
4. **Lịch sử hoạt động** (icon lịch sử): Xem nhật ký các thao tác đã thực hiện
5. **Cài đặt** (icon bánh răng): Cấu hình đường dẫn xuất file

Góc trên phải có nút **NGẮT KẾT NỐI** (màu cam) để quay lại màn hình kết nối.

### 📸 Hình minh họa

**Tên ảnh đề xuất:** Giao diện màn hình kết nối  
**Mô tả ảnh cần chụp:** Chụp toàn bộ màn hình "Kết nối thiết bị TCP/IP" với các trường nhập liệu và 3 nút chức năng  
**Mục đích:** Giúp người dùng nhận diện màn hình đầu tiên khi mở ứng dụng

**Tên ảnh đề xuất:** Giao diện màn hình chính  
**Mô tả ảnh cần chụp:** Chụp màn hình chính với tiêu đề "HỆ THỐNG QUẢN LÝ CHẤM CÔNG" và các tab bên trái  
**Mục đích:** Giúp người dùng nhận biết cấu trúc điều hướng chính

---
PAGE BREAK
---

## 3. Kết nối thiết bị chấm công

### Mục đích
Thiết lập kết nối giữa ứng dụng và máy chấm công để lấy dữ liệu.

### Điều kiện tiên quyết
- Máy chấm công đã được cài đặt và kết nối mạng LAN
- Biết địa chỉ IP của máy chấm công (hỏi bộ phận IT nếu chưa biết)
- Máy tính đang kết nối cùng mạng LAN với máy chấm công

### Các bước thực hiện

**Bước 1: Mở ứng dụng**
- Nhấn đúp vào icon "BHK Attendance System"
- Màn hình "Kết nối thiết bị TCP/IP" sẽ hiển thị

**Bước 2: Nhập thông tin kết nối**
- **Địa chỉ IP**: Nhập địa chỉ IP máy chấm công (ví dụ: 192.168.10.249)
- **Cổng UDP**: Nhập cổng (mặc định 5500, thường không cần thay đổi)
- **Số hiệu thiết bị (DN)**: Nhập số định danh (thường là 1)
- **Mật khẩu**: Để trống nếu thiết bị không có mật khẩu

**Bước 3: Kết nối**
- Nhấn nút **KẾT NỐI** (màu xanh)
- Thanh tiến trình sẽ hiển thị trong khi đang kết nối
- Đợi 5-10 giây

**Bước 4: Kiểm tra trạng thái**
- Nếu thành công: Trạng thái hiển thị "Đã kết nối" (chấm xanh)
- Màn hình tự động chuyển sang trang chủ
- Thông tin thiết bị (Tên thiết bị, Mẫu thiết bị) sẽ tự động cập nhật

### Kết quả mong đợi
- Trạng thái kết nối: "Đã kết nối"
- Chấm tròn màu xanh hiển thị bên cạnh trạng thái
- Ứng dụng chuyển sang màn hình chính với tab "Thông tin thiết bị"

### Lưu ý quan trọng
- Nếu kết nối thất bại, kiểm tra lại địa chỉ IP và đảm bảo máy chấm công đang bật
- Các trường có dấu * (sao đỏ) là bắt buộc
- Không được thay đổi thông tin kết nối khi đã kết nối (các trường sẽ bị vô hiệu hóa)

### 📸 Hình minh họa

**Tên ảnh đề xuất:** Nhập thông tin kết nối  
**Mô tả ảnh cần chụp:** Chụp màn hình với các trường IP, Port, DN đã được điền thông tin mẫu và nút "KẾT NỐI" được làm nổi bật  
**Mục đích:** Hướng dẫn người dùng cách nhập thông tin chính xác

**Tên ảnh đề xuất:** Trạng thái kết nối thành công  
**Mô tả ảnh cần chụp:** Chụp phần hiển thị trạng thái với chấm tròn xanh và text "Đã kết nối"  
**Mục đích:** Giúp người dùng nhận biết kết nối thành công

---
PAGE BREAK
---

## 4. Xem thông tin thiết bị

### Mục đích
Xem thông tin chi tiết về máy chấm công đã kết nối.

### Điều kiện tiên quyết
- Đã kết nối thành công với máy chấm công (xem phần 3)

### Các bước thực hiện

**Bước 1: Vào tab Thông tin thiết bị**
- Sau khi kết nối thành công, ứng dụng tự động hiển thị tab này
- Hoặc nhấn vào tab **Thông tin thiết bị** (icon màn hình) bên trái

**Bước 2: Xem các thông tin**

Tab này hiển thị 10 thẻ thông tin được bố trí theo lưới 2 cột:

**Cột trái:**
- **Địa chỉ IP**: IP của máy chấm công
- **Số thiết bị**: Device Number (DN)
- **Serial Number**: Số serial của thiết bị
- **Số lượng nhân viên**: Tổng số nhân viên đã đăng ký
- **Bộ nhớ sử dụng**: Dung lượng bộ nhớ đang dùng

**Cột phải:**
- **Cổng**: Port kết nối
- **Model thiết bị**: Loại máy chấm công
- **Phiên bản Firmware**: Phiên bản phần mềm thiết bị
- **Bản ghi chấm công (30 ngày)**: Số lượng bản ghi trong 30 ngày gần nhất
- **Thời gian kết nối**: Thời điểm kết nối thành công và trạng thái

**Bước 3: Cập nhật thông tin (nếu cần)**
- Các thông tin này được load tự động khi kết nối
- Để làm mới, nhấn vào tab khác rồi quay lại tab "Thông tin thiết bị"

### Kết quả mong đợi
- Tất cả 10 thẻ thông tin hiển thị đầy đủ dữ liệu
- Trạng thái kết nối hiển thị màu xanh và text "Đã kết nối"

### Chế độ thử nghiệm
Nếu thấy cảnh báo màu đỏ "CHẾ ĐỘ THỬ NGHIỆM - Kết nối mô phỏng", nghĩa là:
- Ứng dụng đang chạy ở chế độ test, không kết nối thiết bị thật
- Dữ liệu hiển thị là dữ liệu mẫu
- Tùy cấu hình bởi IT

### 📸 Hình minh họa

**Tên ảnh đề xuất:** Màn hình thông tin thiết bị  
**Mô tả ảnh cần chụp:** Chụp toàn bộ tab "Thông tin thiết bị" với 10 thẻ thông tin hiển thị đầy đủ dữ liệu  
**Mục đích:** Giúp người dùng hiểu cách đọc và hiển thị thông tin thiết bị

---
PAGE BREAK
---

## 5. Quản lý nhân viên

### Mục đích
Xem danh sách nhân viên đã đăng ký trong máy chấm công và thông tin chi tiết của từng người.

### Điều kiện tiên quyết
- Đã kết nối thành công với máy chấm công

### Các bước thực hiện

**Bước 1: Mở tab Quản lý nhân viên**
- Nhấn vào tab **Quản lý nhân viên** (icon người dùng) bên trái
- Danh sách nhân viên sẽ tự động load

**Bước 2: Xem danh sách nhân viên**
- Màn hình chia làm 2 phần:
  - **Bên trái**: Danh sách nhân viên (dạng bảng)
  - **Bên phải**: Thông tin chi tiết nhân viên đã chọn

**Bước 3: Tìm kiếm nhân viên (nếu cần)**
- Nhập tên hoặc mã nhân viên vào ô tìm kiếm phía trên
- Nhấn nút **Tìm**
- Danh sách sẽ lọc theo từ khóa

**Bước 4: Xem chi tiết nhân viên**
- Nhấn vào một nhân viên trong danh sách
- Bên phải sẽ hiển thị:
  - **Thông tin cơ bản**: CMND/CCCD, Giới tính, Ngày sinh, Phòng ban
  - **Quyền và phân quyền**: Quyền hạn, Nhóm người dùng
  - **Kiểm soát truy cập**: Kiểm soát khóa, vùng thời gian truy cập
  - **Thông tin đăng ký**: Các phương thức xác thực (vân tay, thẻ, mật khẩu)

**Bước 5: Điều hướng trang (nếu có nhiều nhân viên)**
- Phía dưới danh sách có các nút phân trang:
  - **<<**: Trang đầu
  - **<**: Trang trước
  - **>**: Trang sau
  - **>>**: Trang cuối
- Hiển thị tổng số nhân viên và trang hiện tại

### Kết quả mong đợi
- Danh sách nhân viên hiển thị đầy đủ
- Khi chọn một nhân viên, thông tin chi tiết hiển thị bên phải
- Có thể tìm kiếm và lọc nhân viên

### Lưu ý
- Dữ liệu nhân viên được lấy trực tiếp từ máy chấm công
- Ứng dụng hiện tại chỉ xem thông tin, chưa hỗ trợ thêm/sửa/xóa nhân viên
- Để thêm/sửa/xóa nhân viên, vui lòng sử dụng phần mềm quản lý của máy chấm công hoặc liên hệ IT

### 📸 Hình minh họa

**Tên ảnh đề xuất:** Danh sách nhân viên  
**Mô tả ảnh cần chụp:** Chụp tab "Quản lý nhân viên" với danh sách bên trái và một nhân viên được chọn, thông tin chi tiết hiển thị bên phải  
**Mục đích:** Giúp người dùng hiểu cách xem và chọn nhân viên

**Tên ảnh đề xuất:** Thông tin chi tiết nhân viên  
**Mô tả ảnh cần chụp:** Chụp phần bên phải với tiêu đề "THÔNG TIN CHI TIẾT" và các section thông tin cơ bản, quyền hạn  
**Mục đích:** Minh họa cách hiển thị thông tin chi tiết

---
PAGE BREAK
---

## 6. Quản lý chấm công

### Mục đích
Xem, lọc và tìm kiếm dữ liệu chấm công của nhân viên.

### Điều kiện tiên quyết
- Đã kết nối thành công với máy chấm công

### Các bước thực hiện

**Bước 1: Mở tab Quản lý chấm công**
- Nhấn vào tab **Quản lý chấm công** (icon lịch) bên trái
- Tiêu đề "QUẢN LÝ CHẤM CÔNG" sẽ hiển thị

**Bước 2: Chọn cách lọc dữ liệu**

Có 3 cách lọc:

**Cách 1: Khoảng có sẵn** (Predefined Range)
- Chọn radio button "Khoảng có sẵn"
- Chọn từ dropdown: Hôm nay, Hôm qua, 7 ngày qua, 30 ngày qua, v.v.
- Nhấn nút **Lọc**

**Cách 2: Ngày cụ thể** (Single Date)
- Chọn radio button "Ngày cụ thể"
- Chọn ngày từ Date Picker
- Nhấn nút **Lọc**

**Cách 3: Khoảng tùy chỉnh** (Date Range)
- Chọn radio button "Khoảng tùy chỉnh"
- Chọn "Từ ngày" và "Đến ngày"
- Nhấn nút **Lọc**

**Bước 3: Lọc theo giờ (tùy chọn)**
- Chọn từ dropdown "Giờ": Tất cả, Buổi sáng, Buổi chiều, v.v.
- Nhấn nút **Lọc**

**Bước 4: Xem kết quả**
- Bảng dữ liệu hiển thị với các cột:
  - **STT**: Số thứ tự
  - **DIN**: Mã nhân viên
  - **Ngày**: Ngày chấm công
  - **Giờ**: Giờ chấm công
  - **Loại**: Loại chấm công
  - **Thiết bị**: Số thiết bị (DN)
  - **Hành động**: Hành động (vào/ra)
  - **Ghi chú**: Ghi chú (nếu có)

**Bước 5: Làm mới dữ liệu (nếu cần)**
- Nhấn nút **Làm mới** (icon vòng tròn xoay)

### Phân trang
- Phía dưới bảng hiển thị tổng số bản ghi và thời gian load
- Sử dụng các nút **<<**, **<**, **>**, **>>** để điều hướng trang

### Kết quả mong đợi
- Dữ liệu chấm công hiển thị theo bộ lọc đã chọn
- Có thể xem nhiều trang nếu dữ liệu lớn
- Thông tin tổng số bản ghi và thời gian load hiển thị phía dưới

### 📸 Hình minh họa

**Tên ảnh đề xuất:** Bộ lọc chấm công  
**Mô tả ảnh cần chụp:** Chụp phần Filter Panel với 3 radio button và các trường chọn ngày, nút "Lọc" được làm nổi bật  
**Mục đích:** Hướng dẫn người dùng cách lọc dữ liệu

**Tên ảnh đề xuất:** Bảng dữ liệu chấm công  
**Mô tả ảnh cần chụp:** Chụp bảng DataGrid với các cột STT, DIN, Ngày, Giờ, Loại, Thiết bị và vài dòng dữ liệu mẫu  
**Mục đích:** Minh họa cách hiển thị kết quả

---
PAGE BREAK
---

## 7. Xuất báo cáo

### Mục đích
Xuất dữ liệu chấm công ra file Excel hoặc PDF để lưu trữ hoặc chia sẻ.

### Điều kiện tiên quyết
- Đã kết nối thành công với máy chấm công
- Đã lọc dữ liệu chấm công cần xuất (xem phần 6)

### Các bước thực hiện

**Bước 1: Lọc dữ liệu cần xuất**
- Vào tab **Quản lý chấm công**
- Chọn khoảng thời gian cần xuất (theo hướng dẫn phần 6)
- Nhấn **Lọc** để xem trước dữ liệu

**Bước 2: Mở hộp thoại xuất file**
- Nhấn nút **Xuất file** (icon xuất, bên phải nút "Làm mới")
- Hộp thoại cấu hình xuất file sẽ hiển thị

**Bước 3: Chọn định dạng file**
- Chọn định dạng mong muốn:
  - **Excel** (.xlsx): Phù hợp để xử lý tiếp dữ liệu
  - **PDF**: Phù hợp để in ấn hoặc chia sẻ
  - **CSV** (.csv): Định dạng văn bản đơn giản (tùy phiên bản)

**Bước 4: Chọn vị trí lưu file**
- Nhấn nút "Chọn đường dẫn" hoặc "Browse"
- Chọn thư mục muốn lưu file
- Nhập tên file
- Nhấn **Lưu**

**Bước 5: Xác nhận xuất**
- Kiểm tra lại thông tin
- Nhấn nút **Xuất** hoặc **OK**
- Đợi quá trình xuất hoàn tất

**Bước 6: Kiểm tra file đã xuất**
- Thông báo "Xuất file thành công" sẽ hiển thị
- Mở thư mục đã chọn để kiểm tra file

### Kết quả mong đợi
- File Excel/PDF được tạo tại vị trí đã chọn
- File chứa đầy đủ dữ liệu chấm công theo bộ lọc
- File có thể mở và xem bằng Excel/PDF Reader

### Lưu ý
- Đường dẫn mặc định để xuất file có thể được cấu hình trong tab "Cài đặt"
- Nếu file cùng tên đã tồn tại, hệ thống sẽ hỏi có ghi đè không
- Định dạng CSV và PDF có thể tùy cấu hình bởi IT

### 📸 Hình minh họa

**Tên ảnh đề xuất:** Nút xuất file  
**Mô tả ảnh cần chụp:** Chụp phần filter panel với nút "Xuất file" (icon FileExport) được làm nổi bật  
**Mục đích:** Giúp người dùng tìm nút xuất file

**Tên ảnh đề xuất:** Hộp thoại cấu hình xuất  
**Mô tả ảnh cần chụp:** Chụp dialog chọn định dạng Excel/PDF và nút chọn đường dẫn (nếu có trong UI)  
**Mục đích:** Hướng dẫn cách chọn định dạng và vị trí lưu

---
PAGE BREAK
---

## 8. Xem lịch sử hoạt động

### Mục đích
Xem nhật ký tất cả các thao tác đã thực hiện trong hệ thống.

### Điều kiện tiên quyết
- Đã kết nối thành công với máy chấm công

### Các bước thực hiện

**Bước 1: Mở tab Lịch sử hoạt động**
- Nhấn vào tab **Lịch sử hoạt động** (icon lịch sử) bên trái

**Bước 2: Xem danh sách hoạt động**
- Bảng hiển thị các hoạt động gần nhất, bao gồm:
  - Thời gian thực hiện
  - Loại hoạt động (Kết nối, Đồng bộ, Xuất file, v.v.)
  - Mô tả chi tiết
  - Trạng thái (Thành công / Thất bại)
  - Người thực hiện (nếu có)

**Bước 3: Lọc theo thời gian (nếu có)**
- Tùy phiên bản, có thể có bộ lọc theo ngày
- Chọn khoảng thời gian và nhấn **Lọc**

**Bước 4: Phân trang**
- Sử dụng các nút điều hướng trang nếu có nhiều bản ghi

### Kết quả mong đợi
- Danh sách hoạt động hiển thị đầy đủ
- Có thể theo dõi ai đã làm gì và khi nào

### Mục đích sử dụng lịch sử
- **Kiểm tra thao tác**: Xác nhận các thao tác đã thực hiện
- **Khắc phục lỗi**: Tìm nguyên nhân lỗi khi có sự cố
- **Audit trail**: Theo dõi truy xuất dữ liệu

### Lưu ý
- Lịch sử hoạt động được tự động ghi lại, không cần thao tác thủ công
- Hệ thống tự động xóa lịch sử cũ hơn 90 ngày (tùy cấu hình)
- Tối đa lưu 10,000 bản ghi (tùy cấu hình bởi IT)

---
PAGE BREAK
---

## 9. Cấu hình hệ thống

### Mục đích
Cấu hình đường dẫn lưu file và các thông số hệ thống.

### Điều kiện tiên quyết
- Đã kết nối thành công với máy chấm công
- Có quyền thay đổi cấu hình (tùy phân quyền)

### Các bước thực hiện

**Bước 1: Mở tab Cài đặt**
- Nhấn vào tab **Cài đặt** (icon bánh răng) bên trái
- Tiêu đề "CÀI ĐẶT HỆ THỐNG" sẽ hiển thị

**Bước 2: Cấu hình đường dẫn xuất file điểm danh**
- Trong card "Đường dẫn xuất file điểm danh"
- Nhấn nút **CHỌN** để chọn thư mục
- Hoặc nhập trực tiếp đường dẫn (ví dụ: C:\Data\AttendanceExports)

**Bước 3: Cấu hình đường dẫn file Excel nhân viên**
- Trong card "Đường dẫn file Excel nhân viên"
- Nhấn nút **CHỌN** để chọn file Excel
- Hoặc nhập trực tiếp đường dẫn file

**Bước 4: Cấu hình Table (tùy chọn)**
- Nhập "Tên table điểm danh" (mặc định: AttendanceTable)
- Nhập "Tên table nhân viên" (mặc định: EmployeeTable)
- Tùy chọn này dành cho tích hợp với hệ thống khác, thường không cần thay đổi

**Bước 5: Lưu cấu hình**
- Các thay đổi tự động lưu vào file cấu hình
- Không cần nhấn nút "Lưu" (tùy phiên bản)

### Chức năng Test (Dành cho IT/Admin)

Card "Chức năng Test" có 2 nút:
- **TEST XUẤT ĐIỂM DANH**: Xuất file điểm danh mẫu để kiểm tra
- **TEST XUẤT DANH SÁCH NHÂN VIÊN**: Xuất file nhân viên mẫu để kiểm tra

Những chức năng này chỉ dùng để kiểm tra, không dùng trong vận hành thực tế.

### Kết quả mong đợi
- Đường dẫn được cập nhật
- File xuất sẽ lưu vào thư mục đã cấu hình

### Lưu ý
- Chọn đường dẫn dễ tìm, tránh đường dẫn quá sâu
- Đảm bảo có quyền ghi vào thư mục đã chọn
- Nếu không chắc chắn, hỏi bộ phận IT trước khi thay đổi

### 📸 Hình minh họa

**Tên ảnh đề xuất:** Màn hình cài đặt  
**Mô tả ảnh cần chụp:** Chụp tab "Cài đặt" với 3 cards: Đường dẫn xuất file, Đường dẫn Excel, Cấu hình Table  
**Mục đích:** Giúp người dùng hiểu các tùy chọn cấu hình

---
PAGE BREAK
---

## 10. Các lỗi thường gặp

| Lỗi | Nguyên nhân | Cách xử lý |
|-----|-------------|------------|
| "Không thể kết nối tới thiết bị" | - Địa chỉ IP sai<br/>- Thiết bị không bật<br/>- Không cùng mạng LAN | - Kiểm tra lại IP<br/>- Kiểm tra thiết bị đã bật<br/>- Liên hệ IT kiểm tra mạng |
| "Timeout khi kết nối" | - Mạng chậm<br/>- Thiết bị bận | - Thử lại sau vài giây<br/>- Khởi động lại thiết bị |
| "Không load được dữ liệu nhân viên" | - Thiết bị chưa có dữ liệu<br/>- Lỗi kết nối | - Kiểm tra thiết bị có nhân viên đăng ký<br/>- Ngắt kết nối và kết nối lại |
| "Xuất file thất bại" | - Không có quyền ghi file<br/>- Đường dẫn không tồn tại<br/>- File đang mở | - Chọn đường dẫn khác<br/>- Đóng file Excel/PDF nếu đang mở<br/>- Liên hệ IT |
| "Ứng dụng không khởi động được" | - Thiếu .NET Runtime<br/>- Lỗi cấu hình | - Cài đặt lại ứng dụng<br/>- Liên hệ IT |
| Dữ liệu chấm công không chính xác | - Sai bộ lọc thời gian<br/>- Dữ liệu thiết bị chưa đồng bộ | - Kiểm tra lại bộ lọc<br/>- Làm mới dữ liệu<br/>- Đồng bộ lại từ thiết bị |
| Màn hình bị đơ khi load dữ liệu lớn | - Quá nhiều bản ghi | - Chọn khoảng thời gian ngắn hơn<br/>- Đợi quá trình load hoàn tất |

### Khi không tự xử lý được
Nếu gặp lỗi không có trong bảng trên hoặc không tự xử lý được:
1. Chụp màn hình thông báo lỗi
2. Ghi lại các bước đã làm trước khi gặp lỗi
3. Liên hệ bộ phận IT với thông tin trên

---
PAGE BREAK
---

## 11. Câu hỏi thường gặp

**1. Tôi có thể sử dụng ứng dụng mà không kết nối thiết bị không?**
- Không. Ứng dụng cần kết nối thiết bị để lấy dữ liệu.
- Chế độ thử nghiệm (nếu có) chỉ dùng để test, không dùng thực tế.

**2. Tôi có thể thêm/sửa/xóa nhân viên trong ứng dụng không?**
- Hiện tại ứng dụng chỉ hỗ trợ xem thông tin nhân viên.
- Để thêm/sửa/xóa, vui lòng sử dụng phần mềm quản lý máy chấm công hoặc liên hệ IT.

**3. Dữ liệu chấm công lưu ở đâu?**
- Dữ liệu được lấy trực tiếp từ máy chấm công.
- Khi xuất file, dữ liệu được lưu vào thư mục bạn chọn.
- Có thể có database nội bộ (tùy cấu hình bởi IT).

**4. Tôi có thể xuất dữ liệu của tất cả nhân viên cùng lúc không?**
- Có. Khi lọc dữ liệu, không giới hạn theo nhân viên cụ thể, tất cả dữ liệu sẽ được xuất.

**5. Tại sao tôi không thấy tab SharePoint?**
- Tích hợp SharePoint tùy cấu hình.
- Nếu không thấy, nghĩa là chức năng này chưa được bật.
- Liên hệ IT nếu cần sử dụng SharePoint.

**6. File Excel xuất ra có công thức không?**
- Tuỳ cấu hình.
- Thường file xuất chỉ chứa dữ liệu tĩnh, không có công thức.

**7. Tôi có thể gửi báo cáo qua email tự động không?**
- Tùy phiên bản. Hiện tại có thể chưa hỗ trợ.
- Bạn có thể xuất file rồi gửi thủ công qua email.

**8. Làm sao biết thiết bị đang kết nối hay không?**
- Xem trạng thái trên màn hình:
  - Chấm xanh + "Đã kết nối" = Đang kết nối
  - Chấm xám + "Chưa kết nối" = Chưa kết nối
- Nút "NGẮT KẾT NỐI" chỉ hiện khi đã kết nối.

**9. Có giới hạn số lượng bản ghi xuất không?**
- Không giới hạn về mặt lý thuyết.
- Nếu dữ liệu quá lớn (hàng chục nghìn bản ghi), quá trình xuất có thể chậm.
- Khuyến nghị xuất theo từng tháng.

**10. Ứng dụng có hỗ trợ nhiều ngôn ngữ không?**
- Hiện tại giao diện chủ yếu bằng tiếng Việt.
- Một số thuật ngữ kỹ thuật giữ nguyên tiếng Anh (IP, Port, DN, v.v.).

---
PAGE BREAK
---

## 12. Liên hệ hỗ trợ

### Khi nào cần liên hệ hỗ trợ?
- Gặp lỗi không tự xử lý được
- Cần hướng dẫn sử dụng chức năng nâng cao
- Cần cấu hình thiết bị mới
- Cần cài đặt/gỡ/nâng cấp ứng dụng
- Cần phân quyền hoặc thay đổi cấu hình hệ thống

### Thông tin cần chuẩn bị khi liên hệ
1. **Mô tả vấn đề**: Nêu rõ lỗi gì, khi nào xảy ra
2. **Các bước tái hiện**: Bạn đã làm gì trước khi gặp lỗi
3. **Ảnh chụp màn hình**: Chụp thông báo lỗi hoặc màn hình có vấn đề
4. **Thông tin thiết bị**: Địa chỉ IP, model máy chấm công (nếu có liên quan)
5. **Phiên bản ứng dụng**: Xem trong tab "About" (nếu có) hoặc hỏi IT

### Kênh hỗ trợ
- **Bộ phận IT nội bộ**: Liên hệ theo quy trình công ty
- **Email hỗ trợ**: (Tùy cấu hình bởi IT - thêm email nếu có)
- **Hotline**: (Tùy cấu hình bởi IT - thêm số điện thoại nếu có)

### Thời gian hỗ trợ
- Giờ hành chính: 8:00 - 17:00 (Thứ 2 - Thứ 6)
- Ngoài giờ: Chỉ hỗ trợ sự cố khẩn cấp (tùy chính sách công ty)

### Lưu ý
- Không tự ý sửa file cấu hình hệ thống
- Không chia sẻ thông tin kết nối thiết bị cho người ngoài
- Chỉ sử dụng ứng dụng cho mục đích công việc

---
PAGE BREAK
---

## Phụ lục A - Kiểm tra kết nối mạng (Dành cho IT/Quản trị)

### Mục đích
Hướng dẫn kiểm tra kết nối mạng với máy chấm công khi gặp lỗi.

### Các bước thực hiện

**Bước 1: Kiểm tra Ping**
1. Mở Command Prompt (nhấn Windows + R, gõ `cmd`, Enter)
2. Gõ lệnh: `ping 192.168.10.249` (thay bằng IP thiết bị)
3. Nhấn Enter
4. Kết quả:
   - Nếu thấy "Reply from...": Kết nối OK
   - Nếu thấy "Request timed out": Không kết nối được

**Bước 2: Kiểm tra cùng mạng**
1. Gõ lệnh: `ipconfig`
2. Xem phần "IPv4 Address" của máy tính
3. So sánh với IP thiết bị:
   - Ví dụ: Máy tính 192.168.10.100, Thiết bị 192.168.10.249 → OK
   - Ví dụ: Máy tính 192.168.1.100, Thiết bị 192.168.10.249 → Khác mạng

**Bước 3: Kiểm tra Firewall**
1. Vào Windows Defender Firewall
2. Cho phép cổng 5500 và 4370
3. Hoặc tạm tắt Firewall để test

**Bước 4: Kiểm tra cáp mạng và Switch**
- Kiểm tra cáp mạng máy tính và thiết bị đã cắm chắc
- Kiểm tra đèn LED trên cổng mạng
- Kiểm tra Switch có hoạt động

### Khi nào cần gọi IT chuyên sâu
- Ping không thông nhưng thiết bị đã bật
- Cần cấu hình VLAN hoặc mạng nâng cao
- Cần cấu hình IP tĩnh cho thiết bị

---
PAGE BREAK
---

## Mục lục hình ảnh

| STT | Tên ảnh | Mô tả ngắn |
|-----|--------|-----------|
| 1 | Giao diện màn hình kết nối | Màn hình "Kết nối thiết bị TCP/IP" với các trường nhập liệu |
| 2 | Giao diện màn hình chính | Màn hình chính với tiêu đề và các tab điều hướng |
| 3 | Nhập thông tin kết nối | Các trường IP, Port, DN đã điền thông tin |
| 4 | Trạng thái kết nối thành công | Chấm xanh và text "Đã kết nối" |
| 5 | Màn hình thông tin thiết bị | Tab "Thông tin thiết bị" với 10 thẻ thông tin |
| 6 | Danh sách nhân viên | Danh sách bên trái và thông tin chi tiết bên phải |
| 7 | Thông tin chi tiết nhân viên | Section "THÔNG TIN CHI TIẾT" với các trường thông tin |
| 8 | Bộ lọc chấm công | Filter Panel với 3 radio button và các trường ngày |
| 9 | Bảng dữ liệu chấm công | DataGrid với các cột dữ liệu attendance |
| 10 | Nút xuất file | Icon FileExport trong filter panel |
| 11 | Hộp thoại cấu hình xuất | Dialog chọn định dạng và đường dẫn (nếu có) |
| 12 | Màn hình cài đặt | Tab "Cài đặt" với các cards cấu hình |
