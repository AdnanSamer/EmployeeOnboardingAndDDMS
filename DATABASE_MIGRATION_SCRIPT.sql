-- Database Migration Script
-- Add missing columns to Documents table
-- Run this script on your SQL Server database

USE [Employee Onboarding&DDMS];
GO

-- Check if columns already exist before adding them
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Documents]') AND name = 'AnnotationsJson')
BEGIN
    ALTER TABLE [dbo].[Documents]
    ADD [AnnotationsJson] NVARCHAR(MAX) NULL;
    PRINT 'Added column: AnnotationsJson';
END
ELSE
BEGIN
    PRINT 'Column AnnotationsJson already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Documents]') AND name = 'IsArchived')
BEGIN
    ALTER TABLE [dbo].[Documents]
    ADD [IsArchived] BIT NOT NULL DEFAULT 0;
    PRINT 'Added column: IsArchived';
END
ELSE
BEGIN
    PRINT 'Column IsArchived already exists';
END
GO

IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[Documents]') AND name = 'ReviewComments')
BEGIN
    ALTER TABLE [dbo].[Documents]
    ADD [ReviewComments] NVARCHAR(2000) NULL;
    PRINT 'Added column: ReviewComments';
END
ELSE
BEGIN
    PRINT 'Column ReviewComments already exists';
END
GO

-- Update existing records to have default values for IsArchived
UPDATE [dbo].[Documents]
SET [IsArchived] = 0
WHERE [IsArchived] IS NULL;
GO

PRINT 'Migration completed successfully!';
GO

