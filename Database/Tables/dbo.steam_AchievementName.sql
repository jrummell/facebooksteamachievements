CREATE TABLE [dbo].[steam_AchievementName]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[AchievementId] [int] NOT NULL,
[Language] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Name] [nvarchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[Description] [nvarchar] (1000) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[steam_AchievementName] ADD CONSTRAINT [PK_steam_AchievementName] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_steam_AchievementName] ON [dbo].[steam_AchievementName] ([AchievementId], [Language]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[steam_AchievementName] ADD CONSTRAINT [FK_steam_AchievementName_steam_Achievement] FOREIGN KEY ([AchievementId]) REFERENCES [dbo].[steam_Achievement] ([Id])
GO
