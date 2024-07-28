SELECT  recovery_model 
FROM    sys.databases 
WHERE   database_id = db_id()