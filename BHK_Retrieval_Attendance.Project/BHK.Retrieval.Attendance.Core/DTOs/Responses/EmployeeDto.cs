using System;
using System.Collections.Generic;

namespace BHK.Retrieval.Attendance.Core.DTOs.Responses
{
    /// <summary>
    /// DTO cho thông tin nhân viên từ thiết bị
    /// Clean Architecture - DTO không phụ thuộc vào Riss.Devices
    /// </summary>
    public class EmployeeDto
    {
        /// <summary>
        /// Device ID Number - ID duy nhất trong thiết bị
        /// </summary>
        public ulong DIN { get; set; }

        /// <summary>
        /// Tên nhân viên
        /// </summary>
        public string UserName { get; set; } = string.Empty;

        /// <summary>
        /// Số CMND/CCCD
        /// </summary>
        public string IDNumber { get; set; } = string.Empty;

        /// <summary>
        /// Mã phòng ban
        /// </summary>
        public string DeptId { get; set; } = string.Empty;

        /// <summary>
        /// Quyền hạn (1=User, 2=Enroller, 4=QueryLog, 8=Manager, 16=Guest)
        /// </summary>
        public int Privilege { get; set; }

        /// <summary>
        /// Trạng thái kích hoạt
        /// </summary>
        public bool Enable { get; set; }

        /// <summary>
        /// Giới tính (0=Male, 1=Female)
        /// </summary>
        public int Sex { get; set; }

        /// <summary>
        /// Ngày sinh
        /// </summary>
        public DateTime Birthday { get; set; }

        /// <summary>
        /// Ghi chú
        /// </summary>
        public string Comment { get; set; } = string.Empty;

        /// <summary>
        /// Kiểm soát truy cập (0=Off, 1=Lock1, 2=Lock2, 3=Both)
        /// </summary>
        public int AccessControl { get; set; }

        /// <summary>
        /// Có thời gian hiệu lực không
        /// </summary>
        public bool ValidityPeriod { get; set; }

        /// <summary>
        /// Ngày bắt đầu hiệu lực
        /// </summary>
        public DateTime ValidDate { get; set; }

        /// <summary>
        /// Ngày hết hiệu lực
        /// </summary>
        public DateTime InvalidDate { get; set; }

        /// <summary>
        /// Nhóm người dùng
        /// </summary>
        public int UserGroup { get; set; }

        /// <summary>
        /// Vùng thời gian truy cập
        /// </summary>
        public int AccessTimeZone { get; set; }

        /// <summary>
        /// Loại chấm công
        /// </summary>
        public int AttType { get; set; }

        /// <summary>
        /// Danh sách thông tin đăng ký (vân tay, mật khẩu, thẻ)
        /// </summary>
        public List<EnrollmentDto> Enrollments { get; set; } = new();
    }

    /// <summary>
    /// DTO cho thông tin đăng ký (vân tay, mật khẩu, thẻ từ)
    /// </summary>
    public class EnrollmentDto
    {
        /// <summary>
        /// Loại đăng ký
        /// FP0-FP9 (0-9): Vân tay
        /// PWD (10): Mật khẩu  
        /// Card (11): Thẻ từ
        /// </summary>
        public int EnrollType { get; set; }

        /// <summary>
        /// Dữ liệu đăng ký (encoded)
        /// </summary>
        public string Data { get; set; } = string.Empty;

        /// <summary>
        /// Độ dài dữ liệu
        /// </summary>
        public int DataLength { get; set; }
    }

    /// <summary>
    /// DTO cho bản ghi chấm công
    /// </summary>
    public class AttendanceRecordDto
    {
        /// <summary>
        /// Device Number - Số hiệu thiết bị
        /// </summary>
        public ulong DN { get; set; }

        /// <summary>
        /// Device ID Number
        /// </summary>
        public ulong DIN { get; set; }

        /// <summary>
        /// Thời gian chấm công
        /// </summary>
        public DateTime Time { get; set; }

        /// <summary>
        /// Trạng thái chấm công (0=CheckIn, 1=CheckOut, 2=Break, etc.)
        /// </summary>
        public int State { get; set; }

        /// <summary>
        /// Phương thức xác thực (0=PW, 1=FP, 2=Card, 3=Face, 4=Iris)
        /// </summary>
        public int VerifyMode { get; set; }

        /// <summary>
        /// Hành động (0=In, 1=Out, etc.)
        /// </summary>
        public int Action { get; set; }

        /// <summary>
        /// Ghi chú (nếu có)
        /// </summary>
        public string Remark { get; set; } = string.Empty;

        /// <summary>
        /// ID bản ghi
        /// </summary>
        public ulong RecordId { get; set; }
    }
}
