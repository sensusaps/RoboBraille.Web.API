ECHO OFF

sqlcmd -S .\SQLEXPRESS -d RoboBrailleJobDB -b -Q "EXEC dbo.RoboBrailleCleanUp"



