DECLARE @DbName VARCHAR(50) 
DECLARE @BackUpPath VARCHAR(256) 
DECLARE @FileName VARCHAR(256) 
DECLARE @FileDate VARCHAR(20) 

--Specify your backup path here - make sure the path exists or it will fail  
--** This is the path on the target server**
--Use MakeDir in your build for create the folder if it does not exist 

SET @BackUpPath = 'E:\Backups\'
SELECT @FileDate = CONVERT(VARCHAR(20),GETDATE(),112) + '_' + REPLACE(CONVERT(VARCHAR(20),GETDATE(),108),':','') --Time stamping the files

DECLARE db_cursor CURSOR FOR

SELECT name 
FROM master.dbo.sysdatabases 
WHERE name IN ('Shop') --Specify your DB Name(s) here Can do ('DB1','DB2')

--Alternate Configuration: If you want to backup all databases except ??
--WHERE name NOT IN ('master','model','msdb','tempdb')  
  

OPEN db_cursor

FETCH NEXT FROM db_cursor INTO @DbName

WHILE @@FETCH_STATUS = 0
BEGIN

SET @FileName = @BackUpPath + @DbName + '_' + @FileDate + '.BAK' 
BACKUP DATABASE @DbName TO DISK = @FileName 
FETCH NEXT FROM db_cursor INTO @DbName
END

CLOSE db_cursor

DEALLOCATE db_cursor
