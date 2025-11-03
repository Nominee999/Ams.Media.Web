IF OBJECT_ID('dbo.sp_Client_SoftDelete','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Client_SoftDelete;
GO
CREATE PROCEDURE dbo.sp_Client_SoftDelete
    @ClientId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @tbl SYSNAME = 'Client';
    DECLARE @NameCol SYSNAME = NULL;
    DECLARE @IdCol   SYSNAME = NULL;
    DECLARE @StatCol SYSNAME = NULL;

    SELECT TOP (1) @IdCol = name
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.' + @tbl)
      AND name IN ('ClientId','ClientID','ClientCode','Client_No','Client');

    SELECT TOP (1) @NameCol = name
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.' + @tbl)
      AND name IN ('Description','ClientName','ClientDesc','Name','clieName');

    SELECT TOP (1) @StatCol = name
    FROM sys.columns
    WHERE object_id = OBJECT_ID('dbo.' + @tbl)
      AND name IN ('ClientStatus','Status','InUse','Active','IsActive');

    IF @IdCol IS NULL
    BEGIN
        RAISERROR('Cannot find Client Id column in table [Client].',16,1);
        RETURN;
    END

    DECLARE @sql NVARCHAR(MAX) = N'UPDATE dbo.' + QUOTENAME(@tbl) + N' SET ';

    IF @StatCol IS NOT NULL
        SET @sql += QUOTENAME(@StatCol) + N' = 0, ';

    IF @NameCol IS NOT NULL
        SET @sql += QUOTENAME(@NameCol) + N' = CASE WHEN RIGHT(RTRIM(' + QUOTENAME(@NameCol) +
                 N'), 8) = ''NOT USE'' THEN ' + QUOTENAME(@NameCol) +
                 N' ELSE RTRIM(' + QUOTENAME(@NameCol) + N') + '' - NOT USE'' END, ';

    IF RIGHT(@sql,2) = N', ' SET @sql = LEFT(@sql, LEN(@sql)-2);
    SET @sql += N' WHERE TRY_CONVERT(BIGINT,' + QUOTENAME(@IdCol) + N') = @ClientId;';

    EXEC sp_executesql @sql, N'@ClientId BIGINT', @ClientId=@ClientId;
END
GO
