using System;
using System.Collections.Generic;

namespace BHK.Retrieval.Attendance.WPF.Models.Data
{
    /// <summary>
    /// Enum cho giới tính
    /// </summary>
    public enum Sex
    {
        Male = 0,
        Female = 1
    }

    /// <summary>
    /// Enum cho loại đăng ký
    /// </summary>
    public enum EnrollType
    {
        FP0 = 0,    // Vân tay 0
        FP1 = 1,    // Vân tay 1
        FP2 = 2,    // Vân tay 2
        FP3 = 3,    // Vân tay 3
        FP4 = 4,    // Vân tay 4
        FP5 = 5,    // Vân tay 5
        FP6 = 6,    // Vân tay 6
        FP7 = 7,    // Vân tay 7
        FP8 = 8,    // Vân tay 8
        FP9 = 9,    // Vân tay 9
        PWD = 10,   // Mật khẩu
        Card = 11   // Thẻ từ
    }

    /// <summary>
    /// Model cho thông tin User từ thiết bị
    /// </summary>
    public class DeviceUser
    {
        public ulong DIN { get; set; }
        public string? UserName { get; set; }
        public string? IDNumber { get; set; }
        public string? DeptId { get; set; }
        public int Privilege { get; set; }
        public bool Enable { get; set; }
        public Sex Sex { get; set; }
        public DateTime Birthday { get; set; }
        public string? Comment { get; set; }
        public int AccessControl { get; set; }
        public bool ValidityPeriod { get; set; }
        public DateTime ValidDate { get; set; }
        public DateTime InvalidDate { get; set; }
        public int UserGroup { get; set; }
        public int AccessTimeZone { get; set; }
        public int AttType { get; set; }
        public List<DeviceEnroll>? Enrolls { get; set; }
    }

    /// <summary>
    /// Model cho thông tin Enrollment từ thiết bị
    /// </summary>
    public class DeviceEnroll
    {
        public EnrollType EnrollType { get; set; }
        public byte[]? Data { get; set; }
        public int Quality { get; set; }
    }

    /// <summary>
    /// Model cho bản ghi chấm công từ thiết bị
    /// </summary>
    public class AttendanceRecord
    {
        public ulong DIN { get; set; }
        public DateTime Time { get; set; }
        public int Status { get; set; }
        public int Verified { get; set; }
        public int WorkCode { get; set; }
        public int ReaderID { get; set; }
    }
}