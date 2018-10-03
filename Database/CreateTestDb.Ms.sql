IF (NOT EXISTS (
	SELECT [name] 
	FROM [master].[dbo].[sysdatabases] 
	WHERE '[' + [name] + ']' = 'DbAgnostic.Test' OR [name] = 'DbAgnostic.Test')
)
 BEGIN
	CREATE DATABASE [DbAgnostic.Test]
	PRINT 'Created DbAgnostic.Test Database...'
 END
ELSE
 BEGIN
	PRINT 'DbAgnostic.Test Already Exists.'
 END
GO
USE [DbAgnostic.Test]
GO
IF (NOT EXISTS (
	SELECT * FROM INFORMATION_SCHEMA.TABLES 
	WHERE [TABLE_SCHEMA] = 'dbo' AND [TABLE_NAME] = 'User')
)
 BEGIN

	CREATE TABLE [dbo].[User](
		[UserID] [int] IDENTITY(1,1) NOT NULL,
		[FirstName] [varchar](250) NOT NULL,
		[LastName] [varchar](250) NOT NULL,
		[EmailAddress] [varchar](350) NOT NULL,
		[DateCreated] [datetime] NOT NULL,
		[Bio] [varchar](max) NULL,
	CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
	(
		[UserID] ASC
	)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
	) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
	ALTER TABLE [dbo].[User] ADD  CONSTRAINT [DF_User_DateCreated]  DEFAULT (getdate()) FOR [DateCreated]

	PRINT 'Created User Table...'

 END
 GO

--USE [master] 
--DROP DATABASE [DbAgnostic.Test]--REMOVE ME!
--GO