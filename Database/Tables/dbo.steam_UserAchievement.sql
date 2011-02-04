CREATE TABLE [dbo].[steam_UserAchievement]
(
[Id] [int] NOT NULL IDENTITY(1, 1),
[FacebookUserId] [bigint] NOT NULL CONSTRAINT [DF_steam_UserAchievement_FacebookUserId] DEFAULT ((0)),
[AchievementId] [int] NOT NULL,
[Date] [datetime] NOT NULL,
[Published] [bit] NOT NULL,
[Hidden] [bit] NOT NULL CONSTRAINT [DF_steam_UserAchievement_Hidden] DEFAULT ((0))
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[steam_UserAchievement] ADD CONSTRAINT [PK_steam_UserAchievement] PRIMARY KEY CLUSTERED  ([Id]) ON [PRIMARY]
GO
ALTER TABLE [dbo].[steam_UserAchievement] ADD CONSTRAINT [FK_steam_UserAchievement_steam_Achievement] FOREIGN KEY ([AchievementId]) REFERENCES [dbo].[steam_Achievement] ([Id])
GO
ALTER TABLE [dbo].[steam_UserAchievement] ADD CONSTRAINT [FK_steam_UserAchievement_steam_Achievement1] FOREIGN KEY ([FacebookUserId]) REFERENCES [dbo].[steam_User] ([FacebookUserId])
GO
