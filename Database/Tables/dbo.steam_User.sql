CREATE TABLE [dbo].[steam_User]
(
[FacebookUserId] [bigint] NOT NULL,
[SteamUserId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[steam_User] ADD CONSTRAINT [PK_steam_User] PRIMARY KEY CLUSTERED  ([FacebookUserId]) ON [PRIMARY]
GO
CREATE UNIQUE NONCLUSTERED INDEX [IX_steam_User] ON [dbo].[steam_User] ([SteamUserId]) ON [PRIMARY]
GO
