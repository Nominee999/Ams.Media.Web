CREATE OR ALTER PROC dbo.sp_Client_ListPaged
    @q         NVARCHAR(100) = NULL,
    @page      INT           = 1,
    @pageSize  INT           = 50,
    @showMode  CHAR(1)       = 'C'   -- 'C' = hide NOT USE, 'A' = all
AS
BEGIN
    SET NOCOUNT ON;

    ;WITH F AS
    (
        SELECT *
        FROM dbo.Client WITH (NOLOCK)
        WHERE 1 = 1
          AND (
                @q IS NULL OR @q = '' OR
                CONVERT(NVARCHAR(50), ClientID) LIKE '%' + @q + '%' OR
                ClieName        LIKE '%' + @q + '%' OR
                ClientPrefix    LIKE '%' + @q + '%' OR
                ClientTaxNo     LIKE '%' + @q + '%'
              )
          AND (
                @showMode <> 'C' OR ClieName NOT LIKE '%NOT USE%'
              )
    )
    SELECT
        (SELECT COUNT(*) FROM F) AS Total,
        (
            SELECT
                ClientID, ClieName, AgencyCom, ClientPrefix,
                ClientTaxNo, CreditTerm, ClientStatus AS ClientStatus
            FROM F
            ORDER BY ClientID
            OFFSET (@page-1) * @pageSize ROWS
            FETCH NEXT  @pageSize ROWS ONLY
            FOR JSON PATH
        ) AS ItemsJson;
END
GO
