IF OBJECT_ID('dbo.sp_Client_UsedBy','P') IS NOT NULL
    DROP PROCEDURE dbo.sp_Client_UsedBy;
GO
CREATE PROCEDURE dbo.sp_Client_UsedBy
    @ClientId BIGINT
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @Tables TABLE (TableName SYSNAME);
    INSERT INTO @Tables(TableName)
    VALUES ('TransactionMaster'), ('TransactionKeyMaster'),
           ('Product'), ('Campaign'), ('Material'),
           ('PlanDetail'), ('Address'), ('IDMaster');

    DECLARE @CandidateCols TABLE (ColName SYSNAME);
    INSERT INTO @CandidateCols(ColName)
    VALUES ('ClientId'), ('ClientID'), ('ClientCode'), ('Client_No'), ('Client');

    CREATE TABLE #Result (
        TableName SYSNAME NOT NULL,
        RefCount  INT     NOT NULL
    );

    DECLARE @t SYSNAME, @col SYSNAME, @sql NVARCHAR(MAX), @cnt INT;

    DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
        SELECT TableName FROM @Tables;
    OPEN cur;
    FETCH NEXT FROM cur INTO @t;
    WHILE @@FETCH_STATUS = 0
    BEGIN
        SET @col = NULL;
        SELECT TOP (1) @col = c.ColName
        FROM @CandidateCols c
        WHERE EXISTS (
            SELECT 1
            FROM sys.columns sc
            WHERE sc.object_id = OBJECT_ID(QUOTENAME('dbo') + '.' + QUOTENAME(@t))
              AND sc.name = c.ColName
        );

        SET @cnt = 0;
        IF @col IS NOT NULL
        BEGIN
            SET @sql = N'SELECT @o = COUNT(1) FROM dbo.' + QUOTENAME(@t) +
                       N' WITH (NOLOCK) WHERE TRY_CONVERT(BIGINT,' + QUOTENAME(@col) + N') = @ClientId;';
            EXEC sp_executesql @sql, N'@ClientId BIGINT, @o INT OUTPUT', @ClientId=@ClientId, @o=@cnt OUTPUT;
        END

        IF @cnt > 0
            INSERT INTO #Result(TableName, RefCount) VALUES (@t, @cnt);

        FETCH NEXT FROM cur INTO @t;
    END
    CLOSE cur; DEALLOCATE cur;

    SELECT TableName, RefCount
    FROM #Result
    ORDER BY TableName;
END
GO
