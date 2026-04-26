/*
    SQL Script to initialize MediaServiceAgency (MSA) Database
    Target: SQL Server
*/

CREATE DATABASE MSADb;
GO

USE MSADb;
GO

CREATE TABLE AppPartners (
    AppCode NVARCHAR(50) PRIMARY KEY, -- Unique code for the application (e.g., 'POS', 'CRM')
    ApiKey NVARCHAR(100) NOT NULL,    -- Secret key for authentication
    AppName NVARCHAR(200) NOT NULL,   -- Human readable name
    IsActive BIT DEFAULT 1,           -- Status of the application
    CreatedDate DATETIME DEFAULT GETDATE()
);
GO

-- Optional: Create index for faster ApiKey lookup if table grows
CREATE INDEX IX_AppPartners_ApiKey ON AppPartners(ApiKey);
GO

-- Sample Data
-- INSERT INTO AppPartners (AppCode, ApiKey, AppName, IsActive, CreatedDate)
-- VALUES ('MSA_SYSTEM', '5D8B6C7A8E4F4D3C9B0A1B2C3D4E5F6G', 'MSA System Management', 1, GETDATE());
