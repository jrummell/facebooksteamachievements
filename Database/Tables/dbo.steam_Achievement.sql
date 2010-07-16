CREATE TABLE [dbo].[steam_Achievement]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[Name] [varchar] (100) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[GameId] [int] NOT NULL,
[Description] [varchar] (500) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[ImageUrl] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[steam_Achievement] ADD CONSTRAINT [PK_steam_Achievement] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_steam_Achievement] ON [dbo].[steam_Achievement] ([Name], [GameId], [Description]) ON [PRIMARY]
GO
