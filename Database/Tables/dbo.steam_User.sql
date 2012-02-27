CREATE TABLE [dbo].[steam_User]
(
[FacebookUserId] [bigint] NOT NULL,
[SteamUserId] [varchar] (50) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL,
[AccessToken] [varchar] (250) COLLATE SQL_Latin1_General_CP1_CI_AS NOT NULL CONSTRAINT [DF_steam_User_AccessToken] DEFAULT (N''),
[AutoUpdate] [bit] NOT NULL CONSTRAINT [DF_steam_User_AutoUpdate] DEFAULT ((0)),
[PublishDescription] [bit] NOT NULL CONSTRAINT [DF_steam_User_PublishDescription] DEFAULT ((1)),
[Language] [varchar] (20) COLLATE SQL_Latin1_General_CP1_CI_AS NULL
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[steam_User] ADD CONSTRAINT [PK_steam_User] PRIMARY KEY CLUSTERED  ([FacebookUserId]) ON [PRIMARY]
GO
