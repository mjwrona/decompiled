IF NOT EXISTS(SELECT * FROM sys.types WHERE name = 'typ_SqlBatch2' AND schema_id = SCHEMA_ID('dbo'))
CREATE TYPE typ_SqlBatch2 AS TABLE
(
    BatchIndex  INT             NOT NULL,
    Batch       VARCHAR(MAX)    NULL,
    BatchN      NVARCHAR(MAX)   NULL
)

IF EXISTS(SELECT * FROM sys.types WHERE name = 'typ_SqlBatch' AND schema_id = SCHEMA_ID('dbo'))
DROP TYPE dbo.typ_SqlBatch