CREATE TABLE [dbo].[steam_UserAchievement]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[SteamUserId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[AchievementId] [int] NOT NULL,
[Date] [datetime] NOT NULL,
[Published] [bit] NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[steam_UserAchievement] ADD CONSTRAINT [PK_steam_UserAchievement] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_steam_UserAchievement] ON [dbo].[steam_UserAchievement] ([SteamUserId], [AchievementId]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[steam_UserAchievement] ADD CONSTRAINT [FK_steam_UserAchievement_steam_Achievement1] FOREIGN KEY ([AchievementId]) REFERENCES [dbo].[steam_Achievement] ([Id])
GO
ALTER TABLE [dbo].[steam_UserAchievement] ADD CONSTRAINT [FK_steam_UserAchievement_steam_User] FOREIGN KEY ([SteamUserId]) REFERENCES [dbo].[steam_User] ([SteamUserId])
GO
