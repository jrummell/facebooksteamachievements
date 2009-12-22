﻿#pragma warning disable 1591
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:2.0.50727.4927
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace SteamAchievements.Data
{
	using System.Data.Linq;
	using System.Data.Linq.Mapping;
	using System.Data;
	using System.Collections.Generic;
	using System.Reflection;
	using System.Linq;
	using System.Linq.Expressions;
	using System.ComponentModel;
	using System;
	
	
	[System.Data.Linq.Mapping.DatabaseAttribute(Name="steam")]
	internal partial class SteamDataContext : System.Data.Linq.DataContext
	{
		
		private static System.Data.Linq.Mapping.MappingSource mappingSource = new AttributeMappingSource();
		
    #region Extensibility Method Definitions
    partial void OnCreated();
    partial void InsertAchievement(Achievement instance);
    partial void UpdateAchievement(Achievement instance);
    partial void DeleteAchievement(Achievement instance);
    partial void InsertUserAchievement(UserAchievement instance);
    partial void UpdateUserAchievement(UserAchievement instance);
    partial void DeleteUserAchievement(UserAchievement instance);
    partial void InsertGame(Game instance);
    partial void UpdateGame(Game instance);
    partial void DeleteGame(Game instance);
    partial void InsertUser(User instance);
    partial void UpdateUser(User instance);
    partial void DeleteUser(User instance);
    #endregion
		
		public SteamDataContext() : 
				base(global::System.Configuration.ConfigurationManager.ConnectionStrings["steamConnectionString"].ConnectionString, mappingSource)
		{
			OnCreated();
		}
		
		public SteamDataContext(string connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SteamDataContext(System.Data.IDbConnection connection) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SteamDataContext(string connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public SteamDataContext(System.Data.IDbConnection connection, System.Data.Linq.Mapping.MappingSource mappingSource) : 
				base(connection, mappingSource)
		{
			OnCreated();
		}
		
		public System.Data.Linq.Table<Achievement> Achievements
		{
			get
			{
				return this.GetTable<Achievement>();
			}
		}
		
		public System.Data.Linq.Table<UserAchievement> UserAchievements
		{
			get
			{
				return this.GetTable<UserAchievement>();
			}
		}
		
		public System.Data.Linq.Table<Game> Games
		{
			get
			{
				return this.GetTable<Game>();
			}
		}
		
		public System.Data.Linq.Table<User> Users
		{
			get
			{
				return this.GetTable<User>();
			}
		}
	}
	
	[Table(Name="dbo.steam_Achievement")]
	public partial class Achievement : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Name;
		
		private int _GameId;
		
		private string _Description;
		
		private string _ImageUrl;
		
		private EntitySet<UserAchievement> _steam_UserAchievements;
		
		private EntityRef<Game> _steam_Game;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    partial void OnGameIdChanging(int value);
    partial void OnGameIdChanged();
    partial void OnDescriptionChanging(string value);
    partial void OnDescriptionChanged();
    partial void OnImageUrlChanging(string value);
    partial void OnImageUrlChanged();
    #endregion
		
		public Achievement()
		{
			this._steam_UserAchievements = new EntitySet<UserAchievement>(new Action<UserAchievement>(this.attach_steam_UserAchievements), new Action<UserAchievement>(this.detach_steam_UserAchievements));
			this._steam_Game = default(EntityRef<Game>);
			OnCreated();
		}
		
		[Column(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[Column(Storage="_Name", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[Column(Storage="_GameId", DbType="Int NOT NULL")]
		public int GameId
		{
			get
			{
				return this._GameId;
			}
			set
			{
				if ((this._GameId != value))
				{
					if (this._steam_Game.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnGameIdChanging(value);
					this.SendPropertyChanging();
					this._GameId = value;
					this.SendPropertyChanged("GameId");
					this.OnGameIdChanged();
				}
			}
		}
		
		[Column(Storage="_Description", DbType="VarChar(500) NOT NULL", CanBeNull=false)]
		public string Description
		{
			get
			{
				return this._Description;
			}
			set
			{
				if ((this._Description != value))
				{
					this.OnDescriptionChanging(value);
					this.SendPropertyChanging();
					this._Description = value;
					this.SendPropertyChanged("Description");
					this.OnDescriptionChanged();
				}
			}
		}
		
		[Column(Storage="_ImageUrl", DbType="VarChar(250) NOT NULL", CanBeNull=false)]
		public string ImageUrl
		{
			get
			{
				return this._ImageUrl;
			}
			set
			{
				if ((this._ImageUrl != value))
				{
					this.OnImageUrlChanging(value);
					this.SendPropertyChanging();
					this._ImageUrl = value;
					this.SendPropertyChanged("ImageUrl");
					this.OnImageUrlChanged();
				}
			}
		}
		
		[Association(Name="Achievement_UserAchievement", Storage="_steam_UserAchievements", ThisKey="Id", OtherKey="AchievementId")]
		public EntitySet<UserAchievement> UserAchievements
		{
			get
			{
				return this._steam_UserAchievements;
			}
			set
			{
				this._steam_UserAchievements.Assign(value);
			}
		}
		
		[Association(Name="Game_Achievement", Storage="_steam_Game", ThisKey="GameId", OtherKey="Id", IsForeignKey=true)]
		public Game Game
		{
			get
			{
				return this._steam_Game.Entity;
			}
			set
			{
				Game previousValue = this._steam_Game.Entity;
				if (((previousValue != value) 
							|| (this._steam_Game.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._steam_Game.Entity = null;
						previousValue.Achievements.Remove(this);
					}
					this._steam_Game.Entity = value;
					if ((value != null))
					{
						value.Achievements.Add(this);
						this._GameId = value.Id;
					}
					else
					{
						this._GameId = default(int);
					}
					this.SendPropertyChanged("Game");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_steam_UserAchievements(UserAchievement entity)
		{
			this.SendPropertyChanging();
			entity.Achievement = this;
		}
		
		private void detach_steam_UserAchievements(UserAchievement entity)
		{
			this.SendPropertyChanging();
			entity.Achievement = null;
		}
	}
	
	[Table(Name="dbo.steam_UserAchievement")]
	public partial class UserAchievement : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private string _SteamUserId;
		
		private int _AchievementId;
		
		private System.DateTime _Date;
		
		private EntityRef<Achievement> _steam_Achievement;
		
		private EntityRef<User> _steam_User;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnSteamUserIdChanging(string value);
    partial void OnSteamUserIdChanged();
    partial void OnAchievementIdChanging(int value);
    partial void OnAchievementIdChanged();
    partial void OnDateChanging(System.DateTime value);
    partial void OnDateChanged();
    #endregion
		
		public UserAchievement()
		{
			this._steam_Achievement = default(EntityRef<Achievement>);
			this._steam_User = default(EntityRef<User>);
			OnCreated();
		}
		
		[Column(Storage="_SteamUserId", DbType="VarChar(50) NOT NULL", CanBeNull=false, IsPrimaryKey=true)]
		public string SteamUserId
		{
			get
			{
				return this._SteamUserId;
			}
			set
			{
				if ((this._SteamUserId != value))
				{
					if (this._steam_User.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnSteamUserIdChanging(value);
					this.SendPropertyChanging();
					this._SteamUserId = value;
					this.SendPropertyChanged("SteamUserId");
					this.OnSteamUserIdChanged();
				}
			}
		}
		
		[Column(Storage="_AchievementId", DbType="Int NOT NULL", IsPrimaryKey=true)]
		public int AchievementId
		{
			get
			{
				return this._AchievementId;
			}
			set
			{
				if ((this._AchievementId != value))
				{
					if (this._steam_Achievement.HasLoadedOrAssignedValue)
					{
						throw new System.Data.Linq.ForeignKeyReferenceAlreadyHasValueException();
					}
					this.OnAchievementIdChanging(value);
					this.SendPropertyChanging();
					this._AchievementId = value;
					this.SendPropertyChanged("AchievementId");
					this.OnAchievementIdChanged();
				}
			}
		}
		
		[Column(Storage="_Date", DbType="datetime not null")]
		public System.DateTime Date
		{
			get
			{
				return this._Date;
			}
			set
			{
				if ((this._Date != value))
				{
					this.OnDateChanging(value);
					this.SendPropertyChanging();
					this._Date = value;
					this.SendPropertyChanged("Date");
					this.OnDateChanged();
				}
			}
		}
		
		[Association(Name="Achievement_UserAchievement", Storage="_steam_Achievement", ThisKey="AchievementId", OtherKey="Id", IsForeignKey=true)]
		public Achievement Achievement
		{
			get
			{
				return this._steam_Achievement.Entity;
			}
			set
			{
				Achievement previousValue = this._steam_Achievement.Entity;
				if (((previousValue != value) 
							|| (this._steam_Achievement.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._steam_Achievement.Entity = null;
						previousValue.UserAchievements.Remove(this);
					}
					this._steam_Achievement.Entity = value;
					if ((value != null))
					{
						value.UserAchievements.Add(this);
						this._AchievementId = value.Id;
					}
					else
					{
						this._AchievementId = default(int);
					}
					this.SendPropertyChanged("Achievement");
				}
			}
		}
		
		[Association(Name="User_UserAchievement", Storage="_steam_User", ThisKey="SteamUserId", OtherKey="SteamUserId", IsForeignKey=true)]
		public User User
		{
			get
			{
				return this._steam_User.Entity;
			}
			set
			{
				User previousValue = this._steam_User.Entity;
				if (((previousValue != value) 
							|| (this._steam_User.HasLoadedOrAssignedValue == false)))
				{
					this.SendPropertyChanging();
					if ((previousValue != null))
					{
						this._steam_User.Entity = null;
						previousValue.UserAchievements.Remove(this);
					}
					this._steam_User.Entity = value;
					if ((value != null))
					{
						value.UserAchievements.Add(this);
						this._SteamUserId = value.SteamUserId;
					}
					else
					{
						this._SteamUserId = default(string);
					}
					this.SendPropertyChanged("User");
				}
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
	}
	
	[Table(Name="dbo.steam_Game")]
	public partial class Game : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private int _Id;
		
		private string _Abbreviation;
		
		private string _Name;
		
		private EntitySet<Achievement> _steam_Achievements;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnIdChanging(int value);
    partial void OnIdChanged();
    partial void OnAbbreviationChanging(string value);
    partial void OnAbbreviationChanged();
    partial void OnNameChanging(string value);
    partial void OnNameChanged();
    #endregion
		
		public Game()
		{
			this._steam_Achievements = new EntitySet<Achievement>(new Action<Achievement>(this.attach_steam_Achievements), new Action<Achievement>(this.detach_steam_Achievements));
			OnCreated();
		}
		
		[Column(Storage="_Id", AutoSync=AutoSync.OnInsert, DbType="Int NOT NULL", IsPrimaryKey=true, IsDbGenerated=true)]
		public int Id
		{
			get
			{
				return this._Id;
			}
			set
			{
				if ((this._Id != value))
				{
					this.OnIdChanging(value);
					this.SendPropertyChanging();
					this._Id = value;
					this.SendPropertyChanged("Id");
					this.OnIdChanged();
				}
			}
		}
		
		[Column(Storage="_Abbreviation", DbType="VarChar(10) NOT NULL", CanBeNull=false)]
		public string Abbreviation
		{
			get
			{
				return this._Abbreviation;
			}
			set
			{
				if ((this._Abbreviation != value))
				{
					this.OnAbbreviationChanging(value);
					this.SendPropertyChanging();
					this._Abbreviation = value;
					this.SendPropertyChanged("Abbreviation");
					this.OnAbbreviationChanged();
				}
			}
		}
		
		[Column(Storage="_Name", DbType="VarChar(100) NOT NULL", CanBeNull=false)]
		public string Name
		{
			get
			{
				return this._Name;
			}
			set
			{
				if ((this._Name != value))
				{
					this.OnNameChanging(value);
					this.SendPropertyChanging();
					this._Name = value;
					this.SendPropertyChanged("Name");
					this.OnNameChanged();
				}
			}
		}
		
		[Association(Name="Game_Achievement", Storage="_steam_Achievements", ThisKey="Id", OtherKey="GameId")]
		public EntitySet<Achievement> Achievements
		{
			get
			{
				return this._steam_Achievements;
			}
			set
			{
				this._steam_Achievements.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_steam_Achievements(Achievement entity)
		{
			this.SendPropertyChanging();
			entity.Game = this;
		}
		
		private void detach_steam_Achievements(Achievement entity)
		{
			this.SendPropertyChanging();
			entity.Game = null;
		}
	}
	
	[Table(Name="dbo.steam_User")]
	public partial class User : INotifyPropertyChanging, INotifyPropertyChanged
	{
		
		private static PropertyChangingEventArgs emptyChangingEventArgs = new PropertyChangingEventArgs(String.Empty);
		
		private long _FacebookUserId;
		
		private string _SteamUserId;
		
		private EntitySet<UserAchievement> _steam_UserAchievements;
		
    #region Extensibility Method Definitions
    partial void OnLoaded();
    partial void OnValidate(System.Data.Linq.ChangeAction action);
    partial void OnCreated();
    partial void OnFacebookUserIdChanging(long value);
    partial void OnFacebookUserIdChanged();
    partial void OnSteamUserIdChanging(string value);
    partial void OnSteamUserIdChanged();
    #endregion
		
		public User()
		{
			this._steam_UserAchievements = new EntitySet<UserAchievement>(new Action<UserAchievement>(this.attach_steam_UserAchievements), new Action<UserAchievement>(this.detach_steam_UserAchievements));
			OnCreated();
		}
		
		[Column(Storage="_FacebookUserId", DbType="BigInt NOT NULL", IsPrimaryKey=true)]
		public long FacebookUserId
		{
			get
			{
				return this._FacebookUserId;
			}
			set
			{
				if ((this._FacebookUserId != value))
				{
					this.OnFacebookUserIdChanging(value);
					this.SendPropertyChanging();
					this._FacebookUserId = value;
					this.SendPropertyChanged("FacebookUserId");
					this.OnFacebookUserIdChanged();
				}
			}
		}
		
		[Column(Storage="_SteamUserId", DbType="VarChar(50) NOT NULL", CanBeNull=false)]
		public string SteamUserId
		{
			get
			{
				return this._SteamUserId;
			}
			set
			{
				if ((this._SteamUserId != value))
				{
					this.OnSteamUserIdChanging(value);
					this.SendPropertyChanging();
					this._SteamUserId = value;
					this.SendPropertyChanged("SteamUserId");
					this.OnSteamUserIdChanged();
				}
			}
		}
		
		[Association(Name="User_UserAchievement", Storage="_steam_UserAchievements", ThisKey="SteamUserId", OtherKey="SteamUserId")]
		public EntitySet<UserAchievement> UserAchievements
		{
			get
			{
				return this._steam_UserAchievements;
			}
			set
			{
				this._steam_UserAchievements.Assign(value);
			}
		}
		
		public event PropertyChangingEventHandler PropertyChanging;
		
		public event PropertyChangedEventHandler PropertyChanged;
		
		protected virtual void SendPropertyChanging()
		{
			if ((this.PropertyChanging != null))
			{
				this.PropertyChanging(this, emptyChangingEventArgs);
			}
		}
		
		protected virtual void SendPropertyChanged(String propertyName)
		{
			if ((this.PropertyChanged != null))
			{
				this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
			}
		}
		
		private void attach_steam_UserAchievements(UserAchievement entity)
		{
			this.SendPropertyChanging();
			entity.User = this;
		}
		
		private void detach_steam_UserAchievements(UserAchievement entity)
		{
			this.SendPropertyChanging();
			entity.User = null;
		}
	}
}
#pragma warning restore 1591
