USE [RoboBrailleJobDB]
GO

INSERT INTO [dbo].[ServiceUsers]
           ([UserId]
           ,[UserName]
           ,[ApiKey]
           ,[FromDate]
           ,[ToDate]
           ,[EmailAddress])
     VALUES
           ('d2b97532-e8c5-e411-8270-f0def103cfd0'
           ,'test'
           ,convert(VARBINARY(max), '7b76ae41-def3-e411-8030-0c8bfd2336cd')
           ,Cast('12/01/2017' as datetime) 
           ,Cast('12/01/2018' as datetime)
           ,'test@test.eu')
GO
