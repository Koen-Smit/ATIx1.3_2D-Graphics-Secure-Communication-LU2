-- CREATE TABLES: Users, Environment2D, Object2D (based on erd from powerpoint)
-- CREATE TABLE Users (
--     Username VARCHAR(255) PRIMARY KEY,
--     Password VARCHAR(255) NOT NULL
-- );

CREATE TABLE Environment2D (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    MaxHeight FLOAT,
    MaxLength FLOAT,
    -- User_Username VARCHAR(255),
    -- FOREIGN KEY (User_Username) REFERENCES Users(Username) ON DELETE CASCADE
);

CREATE TABLE Object2D (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PrefabId VARCHAR(255),
    PositionX FLOAT,
    PositionY FLOAT,
    ScaleX FLOAT,
    ScaleY FLOAT,
    RotationZ FLOAT,
    SortingLayer INT,
    Environment2D_Id INT,
    FOREIGN KEY (Environment2D_Id) REFERENCES Environment2D(Id) ON DELETE CASCADE
);


-- Needs to be same name as SchemaName in TableBase.SchemaName!!!

-- ADD THE CREATE SCHEMA AUTH ALSO!!!! >
-- CREATE SCHEMA auth;
GO

CREATE TABLE [auth].[AspNetRoles] (
    [Id]               NVARCHAR (450) NOT NULL,
    [Name]             NVARCHAR (256) NULL,
    [NormalizedName]   NVARCHAR (256) NULL,
    [ConcurrencyStamp] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetRoles] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [RoleNameIndex]
    ON [auth].[AspNetRoles]([NormalizedName] ASC) WHERE ([NormalizedName] IS NOT NULL);

GO

CREATE TABLE [auth].[AspNetRoleClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [RoleId]     NVARCHAR (450) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [auth].[AspNetRoles] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetRoleClaims_RoleId]
    ON [auth].[AspNetRoleClaims]([RoleId] ASC);

GO


CREATE TABLE [auth].[AspNetUsers] (
    [Id]                   NVARCHAR (450)     NOT NULL,
    [UserName]             NVARCHAR (256)     NULL,
    [NormalizedUserName]   NVARCHAR (256)     NULL,
    [Email]                NVARCHAR (256)     NULL,
    [NormalizedEmail]      NVARCHAR (256)     NULL,
    [EmailConfirmed]       BIT                NOT NULL,
    [PasswordHash]         NVARCHAR (MAX)     NULL,
    [SecurityStamp]        NVARCHAR (MAX)     NULL,
    [ConcurrencyStamp]     NVARCHAR (MAX)     NULL,
    [PhoneNumber]          NVARCHAR (MAX)     NULL,
    [PhoneNumberConfirmed] BIT                NOT NULL,
    [TwoFactorEnabled]     BIT                NOT NULL,
    [LockoutEnd]           DATETIMEOFFSET (7) NULL,
    [LockoutEnabled]       BIT                NOT NULL,
    [AccessFailedCount]    INT                NOT NULL,
    CONSTRAINT [PK_AspNetUsers] PRIMARY KEY CLUSTERED ([Id] ASC)
);


GO
CREATE UNIQUE NONCLUSTERED INDEX [UserNameIndex]
    ON [auth].[AspNetUsers]([NormalizedUserName] ASC) WHERE ([NormalizedUserName] IS NOT NULL);


GO
CREATE NONCLUSTERED INDEX [EmailIndex]
    ON [auth].[AspNetUsers]([NormalizedEmail] ASC);



CREATE TABLE [auth].[AspNetUserClaims] (
    [Id]         INT            IDENTITY (1, 1) NOT NULL,
    [UserId]     NVARCHAR (450) NOT NULL,
    [ClaimType]  NVARCHAR (MAX) NULL,
    [ClaimValue] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY CLUSTERED ([Id] ASC),
    CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserClaims_UserId]
    ON [auth].[AspNetUserClaims]([UserId] ASC);

GO

CREATE TABLE [auth].[AspNetUserLogins] (
    [LoginProvider]       NVARCHAR (128) NOT NULL,
    [ProviderKey]         NVARCHAR (128) NOT NULL,
    [ProviderDisplayName] NVARCHAR (MAX) NULL,
    [UserId]              NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY CLUSTERED ([LoginProvider] ASC, [ProviderKey] ASC),
    CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserLogins_UserId]
    ON [auth].[AspNetUserLogins]([UserId] ASC);


GO

CREATE TABLE [auth].[AspNetUserRoles] (
    [UserId] NVARCHAR (450) NOT NULL,
    [RoleId] NVARCHAR (450) NOT NULL,
    CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY CLUSTERED ([UserId] ASC, [RoleId] ASC),
    CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [auth].[AspNetRoles] ([Id]) ON DELETE CASCADE,
    CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
);


GO
CREATE NONCLUSTERED INDEX [IX_AspNetUserRoles_RoleId]
    ON [auth].[AspNetUserRoles]([RoleId] ASC);

CREATE TABLE [auth].[AspNetUserTokens] (
    [UserId]        NVARCHAR (450) NOT NULL,
    [LoginProvider] NVARCHAR (128) NOT NULL,
    [Name]          NVARCHAR (128) NOT NULL,
    [Value]         NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY CLUSTERED ([UserId] ASC, [LoginProvider] ASC, [Name] ASC),
    CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [auth].[AspNetUsers] ([Id]) ON DELETE CASCADE
);


insert auth.AspNetUsers (
		Id, 
		UserName, 
		NormalizedUserName, 
		Email, 
		NormalizedEmail, 
		EmailConfirmed, 
		PasswordHash, 
		SecurityStamp, 
		ConcurrencyStamp, 
		PhoneNumberConfirmed, 
		TwoFactorEnabled, 
		LockoutEnabled, 
		AccessFailedCount
		) 
	values (
		'45967735-0d27-4c36-9818-5b00b77ce5a9', 
		'dat@dat.dat', 
		'DAT@DAT.DAT', 
		'dat@dat.dat', 
		'DAT@DAT.DAT', 
		0, 
		'hashed_password', 
		null, -- <-- securitystamp, setting this to null messes up session resets. ok for now 
		null, -- <-- concurrencystamp, setting this to null will mess with concurrent change detection. ok for now
		0, 
		0, 
		1, 
		0
	)
	insert auth.AspNetUserClaims (UserId, ClaimType, ClaimValue) values ('45967735-0d27-4c36-9818-5b00b77ce5a9', 'some_claim', '8')



    -- add users:
    -- Voeg 3 rollen toe: Admin, Manager, User
INSERT INTO [auth].[AspNetRoles] (Id, Name, NormalizedName, ConcurrencyStamp)
VALUES 
    (NEWID(), 'Admin', 'ADMIN', NEWID()),
    (NEWID(), 'Manager', 'MANAGER', NEWID()),
    (NEWID(), 'User', 'USER', NEWID());

-- Verkrijg de aangemaakte RoleIds
DECLARE @AdminRoleId NVARCHAR(450) = (SELECT Id FROM [auth].[AspNetRoles] WHERE NormalizedName = 'ADMIN');
DECLARE @ManagerRoleId NVARCHAR(450) = (SELECT Id FROM [auth].[AspNetRoles] WHERE NormalizedName = 'MANAGER');
DECLARE @UserRoleId NVARCHAR(450) = (SELECT Id FROM [auth].[AspNetRoles] WHERE NormalizedName = 'USER');

-- Voeg 3 testgebruikers toe
INSERT INTO [auth].[AspNetUsers] (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
VALUES 
    (NEWID(), 'admin@example.com', 'ADMIN@EXAMPLE.COM', 'admin@example.com', 'ADMIN@EXAMPLE.COM', 1, 'hashed_password', NEWID(), NEWID(), 0, 0, 1, 0),
    (NEWID(), 'manager@example.com', 'MANAGER@EXAMPLE.COM', 'manager@example.com', 'MANAGER@EXAMPLE.COM', 1, 'hashed_password', NEWID(), NEWID(), 0, 0, 1, 0),
    (NEWID(), 'user@example.com', 'USER@EXAMPLE.COM', 'user@example.com', 'USER@EXAMPLE.COM', 1, 'hashed_password', NEWID(), NEWID(), 0, 0, 1, 0);

-- Verkrijg de UserIds van de aangemaakte gebruikers
DECLARE @AdminUserId NVARCHAR(450) = (SELECT Id FROM [auth].[AspNetUsers] WHERE NormalizedUserName = 'ADMIN@EXAMPLE.COM');
DECLARE @ManagerUserId NVARCHAR(450) = (SELECT Id FROM [auth].[AspNetUsers] WHERE NormalizedUserName = 'MANAGER@EXAMPLE.COM');
DECLARE @UserUserId NVARCHAR(450) = (SELECT Id FROM [auth].[AspNetUsers] WHERE NormalizedUserName = 'USER@EXAMPLE.COM');

-- Koppel gebruikers aan rollen
INSERT INTO [auth].[AspNetUserRoles] (UserId, RoleId)
VALUES 
    (@AdminUserId, @AdminRoleId),
    (@ManagerUserId, @ManagerRoleId),
    (@UserUserId, @UserRoleId);

-- Voeg claims toe aan gebruikers
INSERT INTO [auth].[AspNetUserClaims] (UserId, ClaimType, ClaimValue)
VALUES 
    -- Admin krijgt volledige toegang
    (@AdminUserId, 'entity:read', 'true'),
    (@AdminUserId, 'entity:write', 'true'),
    (@AdminUserId, 'entity:delete', 'true'),

    -- Manager mag lezen en schrijven, maar niet verwijderen
    (@ManagerUserId, 'entity:read', 'true'),
    (@ManagerUserId, 'entity:write', 'true'),

    -- Gewone gebruiker mag alleen lezen
    (@UserUserId, 'entity:read', 'true');

SELECT 'Testgebruikers en rollen succesvol aangemaakt' AS Resultaat;
