USE RoboBrailleJobDB;
GO

IF OBJECT_ID ('dbo.RoboBrailleCleanUp','P') IS NOT NULL
DROP PROCEDURE dbo.RoboBrailleCleanUp;
GO

CREATE PROCEDURE dbo.RoboBrailleCleanUp AS
DELETE FROM dbo.Jobs 
WHERE (SELECT DATEADD(MONTH,1,Jobs.FinishTime)) < CURRENT_TIMESTAMP;
GO

EXEC dbo.RoboBrailleCleanUp;
GO