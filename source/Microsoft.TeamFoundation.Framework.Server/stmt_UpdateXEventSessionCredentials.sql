-- Parameter: @blobStoragePath NVARCHAR(MAX) - Blob storage container URL
-- Parameter: @sharedAccessSignature NVARCHAR(MAX) - Blob storage SAS key to allow the session to write to the container
SET XACT_ABORT ON

IF (CAST(SERVERPROPERTY('Edition') AS SYSNAME) COLLATE DATABASE_DEFAULT = N'SQL Azure')
BEGIN
    IF EXISTS (SELECT * FROM sys.database_credentials WHERE name = @blobStoragePath)
    BEGIN
        DECLARE @statement     NVARCHAR(MAX) = N'
                ALTER DATABASE SCOPED CREDENTIAL ' + QUOTENAME(@blobStoragePath) + N'
                WITH IDENTITY = ''SHARED ACCESS SIGNATURE'',
                       SECRET = ''' + REPLACE(@sharedAccessSignature,'''', '''''') + N''''

        EXEC sp_executesql @statement
    END
END