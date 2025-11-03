/* IX_Client_ClientPrefix (include เฉพาะคอลัมน์ที่มีจริง) */
IF COL_LENGTH('dbo.Client','ClientPrefix') IS NOT NULL
BEGIN
    DECLARE @sql1 NVARCHAR(MAX) = N'CREATE NONCLUSTERED INDEX IX_Client_ClientPrefix ON dbo.Client ([ClientPrefix])';
    DECLARE @inc1 NVARCHAR(MAX) = N'';
    IF COL_LENGTH('dbo.Client','ClientId')     IS NOT NULL SET @inc1 += N', [ClientId]';
    IF COL_LENGTH('dbo.Client','Description')  IS NOT NULL SET @inc1 += N', [Description]';
    IF COL_LENGTH('dbo.Client','ClientTaxNo')  IS NOT NULL SET @inc1 += N', [ClientTaxNo]';
    IF COL_LENGTH('dbo.Client','ClientStatus') IS NOT NULL SET @inc1 += N', [ClientStatus]';
    IF LEN(@inc1) > 0 SET @sql1 += N' INCLUDE (' + STUFF(@inc1,1,2,'') + N')';
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Client') AND name = 'IX_Client_ClientPrefix')
        DROP INDEX IX_Client_ClientPrefix ON dbo.Client;
    EXEC(@sql1);
END
GO

/* IX_Client_ClientTaxNo */
IF COL_LENGTH('dbo.Client','ClientTaxNo') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Client') AND name = 'IX_Client_ClientTaxNo')
        DROP INDEX IX_Client_ClientTaxNo ON dbo.Client;
    EXEC(N'CREATE NONCLUSTERED INDEX IX_Client_ClientTaxNo ON dbo.Client ([ClientTaxNo]);');
END
GO

/* IX_Client_Description (ถ้าต้องการ) */
IF COL_LENGTH('dbo.Client','Description') IS NOT NULL
BEGIN
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Client') AND name = 'IX_Client_Description')
        DROP INDEX IX_Client_Description ON dbo.Client;
    EXEC(N'CREATE NONCLUSTERED INDEX IX_Client_Description ON dbo.Client ([Description]);');
END
GO

/* IX_Address_Client_Type_Start */
IF COL_LENGTH('dbo.Address','ClientId') IS NOT NULL
AND COL_LENGTH('dbo.Address','AddressType') IS NOT NULL
AND COL_LENGTH('dbo.Address','StartDate') IS NOT NULL
BEGIN
    DECLARE @sqlA NVARCHAR(MAX) = N'CREATE NONCLUSTERED INDEX IX_Address_Client_Type_Start ON dbo.Address ([ClientId], [AddressType], [StartDate])';
    IF COL_LENGTH('dbo.Address','EndDate') IS NOT NULL
        SET @sqlA += N' INCLUDE ([EndDate])';
    IF EXISTS (SELECT 1 FROM sys.indexes WHERE object_id = OBJECT_ID('dbo.Address') AND name = 'IX_Address_Client_Type_Start')
        DROP INDEX IX_Address_Client_Type_Start ON dbo.Address;
    EXEC(@sqlA);
END
GO
