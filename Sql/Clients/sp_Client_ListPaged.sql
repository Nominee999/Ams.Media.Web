IF OBJECT_ID('dbo.sp_Client_ListPaged','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Client_ListPaged;
GO
CREATE PROCEDURE dbo.sp_Client_ListPaged
    @q         NVARCHAR(100) = NULL,
    @page      INT           = 1,
    @pageSize  INT           = 50,
    @show      CHAR(1)       = 'C'
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @sql NVARCHAR(MAX) =
    N'SELECT {cols}
      FROM dbo.Client WITH (NOLOCK)
      WHERE 1=1 {whereQ} {whereShow}
      ORDER BY {sort}
      OFFSET (@off) ROWS FETCH NEXT (@ps) ROWS ONLY;

      SELECT COUNT(1) AS Total
      FROM dbo.Client WITH (NOLOCK)
      WHERE 1=1 {whereQ} {whereShow};';

    DECLARE @cols NVARCHAR(MAX) =
    STUFF((
        SELECT N',' + QUOTENAME(name)
        FROM sys.columns
        WHERE object_id = OBJECT_ID('dbo.Client')
          AND name IN ('ClientId','Description','ClientPrefix','ClientTaxNo','ClientStatus')
        FOR XML PATH(''), TYPE
    ).value('.','nvarchar(max)'),1,1,'');

    IF (@cols = N'') SET @cols = N'*';

    DECLARE @whereQ NVARCHAR(MAX) = N'';
    IF (NULLIF(LTRIM(RTRIM(@q)),N'') IS NOT NULL)
    BEGIN
        SET @whereQ = N' AND (1=0';
        IF COL_LENGTH('dbo.Client','ClientId')     IS NOT NULL SET @whereQ += N' OR CAST([ClientId] AS NVARCHAR(50)) LIKE ''%'' + @q + ''%''';
        IF COL_LENGTH('dbo.Client','Description')  IS NOT NULL SET @whereQ += N' OR [Description] LIKE ''%'' + @q + ''%''';
        IF COL_LENGTH('dbo.Client','ClientPrefix') IS NOT NULL SET @whereQ += N' OR [ClientPrefix] LIKE ''%'' + @q + ''%''';
        IF COL_LENGTH('dbo.Client','ClientTaxNo')  IS NOT NULL SET @whereQ += N' OR [ClientTaxNo] LIKE ''%'' + @q + ''%''';
        SET @whereQ += N')';
    END

    DECLARE @whereShow NVARCHAR(MAX) = N'';
    IF (@show = 'C')
    BEGIN
        IF COL_LENGTH('dbo.Client','ClientStatus') IS NOT NULL
            SET @whereShow = N' AND ISNULL([ClientStatus],1) = 1';
        ELSE IF COL_LENGTH('dbo.Client','Description') IS NOT NULL
            SET @whereShow = N' AND RIGHT(RTRIM([Description]),8) <> ''NOT USE''';
    END

    DECLARE @sort NVARCHAR(100) = CASE 
        WHEN COL_LENGTH('dbo.Client','Description') IS NOT NULL THEN N'[Description]'
        WHEN COL_LENGTH('dbo.Client','ClientId')     IS NOT NULL THEN N'[ClientId]'
        ELSE N'(SELECT 1)'
    END;

    SET @sql = REPLACE(@sql, N'{cols}',     @cols);
    SET @sql = REPLACE(@sql, N'{whereQ}',   @whereQ);
    SET @sql = REPLACE(@sql, N'{whereShow}',@whereShow);
    SET @sql = REPLACE(@sql, N'{sort}',     @sort);

    DECLARE @off INT = (@page-1)*@pageSize;

    EXEC sp_executesql
        @sql,
        N'@q nvarchar(100), @off int, @ps int',
        @q=@q, @off=@off, @ps=@pageSize;
END
GO
