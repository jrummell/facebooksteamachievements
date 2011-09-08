CREATE TABLE [dbo].[steam_Achievement]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[ApiName] [nvarchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_steam_Achievement_ApiName] DEFAULT (N''),
[Name] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[GameId] [int] NOT NULL,
[Description] [nvarchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[ImageUrl] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
CREATE UNIQUE NONCLUSTERED INDEX [IX_steam_Achievement] ON [dbo].[steam_Achievement] ([GameId], [ApiName]) ON [PRIMARY]





GO
ALTER TABLE [dbo].[steam_Achievement] ADD CONSTRAINT [PK_steam_Achievement] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
