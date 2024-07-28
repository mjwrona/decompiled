DECLARE @invalidCharacters NVARCHAR(MAX) = N''
;WITH 
    N2(n)       AS (SELECT 1 UNION SELECT 0),
    N4(n)       AS (SELECT 1 FROM N2 n1    CROSS JOIN N2    n2),
    N16(n)      AS (SELECT 1 FROM N4 n1    CROSS JOIN N4    n2),
    N256(n)     AS (SELECT 1 FROM N16 n1   CROSS JOIN N16   n2),
    N65536(n)   AS (SELECT 1 FROM N256 n1  CROSS JOIN N256  n2)
SELECT  @invalidCharacters = @invalidCharacters + NCHAR(n)
FROM    (SELECT ROW_NUMBER() OVER (ORDER BY n) FROM N65536) D(n)
WHERE   n < 65536
        AND REPLACE(NCHAR(n), N' ', N'\') = N''
ORDER BY n

SELECT @invalidCharacters InvalidCharacters

