-- Migration: 001_InitialCreate
-- Description: Tạo bảng ActivityHistory và các indices

-- Tạo bảng chính
CREATE TABLE IF NOT EXISTS ActivityHistory (
    Id INTEGER PRIMARY KEY AUTOINCREMENT,
    Timestamp TEXT NOT NULL,
    DeviceIp TEXT NOT NULL,
    DeviceName TEXT,
    ActivityType INTEGER NOT NULL,
    Action TEXT NOT NULL,
    Status INTEGER NOT NULL,
    Details TEXT,
    UserCount INTEGER,
    RecordCount INTEGER,
    Duration TEXT,
    ErrorMessage TEXT,
    CreatedAt TEXT NOT NULL DEFAULT (datetime('now'))
);

-- Index cho các truy vấn phổ biến
CREATE INDEX IF NOT EXISTS idx_ActivityHistory_Timestamp 
    ON ActivityHistory(Timestamp DESC);

CREATE INDEX IF NOT EXISTS idx_ActivityHistory_DeviceIp 
    ON ActivityHistory(DeviceIp);

CREATE INDEX IF NOT EXISTS idx_ActivityHistory_ActivityType 
    ON ActivityHistory(ActivityType);

CREATE INDEX IF NOT EXISTS idx_ActivityHistory_Status 
    ON ActivityHistory(Status);

CREATE INDEX IF NOT EXISTS idx_ActivityHistory_CreatedAt 
    ON ActivityHistory(CreatedAt DESC);

-- Composite index cho filter phổ biến (device + time)
CREATE INDEX IF NOT EXISTS idx_ActivityHistory_DeviceIp_Timestamp 
    ON ActivityHistory(DeviceIp, Timestamp DESC);
