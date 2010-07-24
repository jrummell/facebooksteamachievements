IF NOT EXISTS (SELECT * FROM master.dbo.syslogins WHERE loginname = N'steam')
CREATE LOGIN [steam] WITH PASSWORD = 'p@ssw0rd'
GO
CREATE USER [steam] FOR LOGIN [steam] WITH DEFAULT_SCHEMA=[dbo]
GO
GRANT CONNECT TO [steam]
