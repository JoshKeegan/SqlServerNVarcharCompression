/*
	SqlServerNVarcharCompression install script
	Authors:
		Josh Keegan 18/04/2017
*/

/* Settings */
DECLARE @installDir nvarchar(1024) = N'C:\SqlAssemblies\SqlServerNVarcharCompression';

USE master;

IF OBJECT_ID('master.dbo.CompressNVarChar') IS NOT NULL
	DROP FUNCTION CompressNVarChar;

IF OBJECT_ID('master.dbo.DecompressNVarChar') IS NOT NULL
	DROP FUNCTION DecompressNVarChar;

IF EXISTS (SELECT * FROM sys.assemblies WHERE name = 'SqlServerNVarcharCompression')
	DROP ASSEMBLY SqlServerNVarcharCompression;

CREATE ASSEMBLY SqlServerNVarcharCompression
FROM @installDir + '\SqlServerNVarcharCompression.dll';
GO

CREATE FUNCTION CompressNVarChar
(
	@sqlString nvarchar(max)
)
RETURNS varbinary(max)
AS 
EXTERNAL NAME SqlServerNVarcharCompression.[SqlServerNVarcharCompression.UserDefinedFunctions].CompressNVarChar;
GO

CREATE FUNCTION DecompressNVarChar
(
	@sqlBytes varbinary(max)
)
RETURNS nvarchar(max)
AS 
EXTERNAL NAME SqlServerNVarcharCompression.[SqlServerNVarcharCompression.UserDefinedFunctions].DecompressNVarChar;
GO