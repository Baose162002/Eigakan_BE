IF OBJECT_ID(N'[__EFMigrationsHistory]') IS NULL
BEGIN
    CREATE TABLE [__EFMigrationsHistory] (
        [MigrationId] nvarchar(150) NOT NULL,
        [ProductVersion] nvarchar(32) NOT NULL,
        CONSTRAINT [PK___EFMigrationsHistory] PRIMARY KEY ([MigrationId])
    );
END;
GO

BEGIN TRANSACTION;
IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [AdPackages] (
        [Id] nvarchar(450) NOT NULL,
        [PackageName] nvarchar(max) NULL,
        [PackPrice] decimal(18,2) NOT NULL,
        [Duration] int NOT NULL,
        [UpdateAt] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_AdPackages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [AdSlots] (
        [Id] nvarchar(450) NOT NULL,
        [SlotLocation] nvarchar(max) NOT NULL,
        [SlotPrice] decimal(18,2) NOT NULL,
        [CreateAt] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_AdSlots] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [AdSlotTimeRanges] (
        [Id] nvarchar(450) NOT NULL,
        [StartTime] datetime2 NOT NULL,
        [EndTime] datetime2 NOT NULL,
        [SlotTimeRangePrice] decimal(18,2) NOT NULL,
        [CreateAt] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        CONSTRAINT [PK_AdSlotTimeRanges] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [Genres] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_Genres] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [Persons] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [Job] nvarchar(max) NULL,
        [Picture] nvarchar(max) NULL,
        [Gender] bit NULL,
        [Birthday] nvarchar(max) NULL,
        CONSTRAINT [PK_Persons] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [Roles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        CONSTRAINT [PK_Roles] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [SubscriptionPackages] (
        [Id] nvarchar(450) NOT NULL,
        [PackageName] nvarchar(max) NULL,
        [Price] decimal(18,2) NULL,
        [Duration] int NULL,
        [UpdateAt] datetime2 NULL,
        [Status] nvarchar(max) NULL,
        CONSTRAINT [PK_SubscriptionPackages] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [UserRegister] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [Reason] nvarchar(max) NULL,
        [ReasonForRejection] nvarchar(max) NULL,
        [FileUrl] nvarchar(max) NULL,
        [CreateDate] datetime2 NOT NULL,
        [Status] nvarchar(max) NULL,
        CONSTRAINT [PK_UserRegister] PRIMARY KEY ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [AdSlotTimes] (
        [Id] nvarchar(450) NOT NULL,
        [SlotTimePrice] decimal(18,2) NOT NULL,
        [CreateAt] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [AdSlotTimeRangeID] nvarchar(450) NOT NULL,
        [AdSlotID] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AdSlotTimes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdSlotTimes_AdSlotTimeRanges_AdSlotTimeRangeID] FOREIGN KEY ([AdSlotTimeRangeID]) REFERENCES [AdSlotTimeRanges] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AdSlotTimes_AdSlots_AdSlotID] FOREIGN KEY ([AdSlotID]) REFERENCES [AdSlots] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [Users] (
        [Id] nvarchar(450) NOT NULL,
        [FullName] nvarchar(max) NULL,
        [Gender] bit NULL,
        [Birthday] nvarchar(max) NULL,
        [Picture] nvarchar(max) NULL,
        [Email] nvarchar(max) NULL,
        [CreateDate] datetime2 NULL,
        [Status] nvarchar(max) NULL,
        [RoleId] nvarchar(450) NULL,
        [UserRegisterId] nvarchar(450) NULL,
        [PasswordHash] varbinary(max) NOT NULL,
        [PasswordSalt] varbinary(max) NOT NULL,
        [VerificationToken] nvarchar(max) NULL,
        [VerifiedAt] datetime2 NULL,
        [PasswordResetToken] nvarchar(max) NULL,
        [ResetTokenExpirex] datetime2 NULL,
        [RefreshToken] nvarchar(max) NOT NULL,
        [TokenCreated] datetime2 NULL,
        [TokenExpires] datetime2 NULL,
        CONSTRAINT [PK_Users] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Users_Roles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [Roles] ([Id]),
        CONSTRAINT [FK_Users_UserRegister_UserRegisterId] FOREIGN KEY ([UserRegisterId]) REFERENCES [UserRegister] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [AdPurchases] (
        [Id] nvarchar(450) NOT NULL,
        [PurchaseDate] datetime2 NOT NULL,
        [TotalPrice] decimal(18,2) NOT NULL,
        [UserID] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AdPurchases] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdPurchases_Users_UserID] FOREIGN KEY ([UserID]) REFERENCES [Users] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [Movies] (
        [Id] nvarchar(450) NOT NULL,
        [Title] nvarchar(max) NULL,
        [OriginName] nvarchar(max) NULL,
        [Description] nvarchar(max) NULL,
        [ViewCount] int NULL,
        [ReleaseYear] nvarchar(max) NULL,
        [Duration] int NULL,
        [Director] nvarchar(max) NULL,
        [Script] nvarchar(max) NULL,
        [Nation] nvarchar(max) NULL,
        [Rating] float NOT NULL,
        [SubmissionDate] datetime2 NULL,
        [ReasonForRejection] nvarchar(max) NULL,
        [Status] nvarchar(max) NULL,
        [UserId] nvarchar(450) NULL,
        CONSTRAINT [PK_Movies] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Movies_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [SubscriptionPurchases] (
        [Id] nvarchar(450) NOT NULL,
        [PurchaseDate] datetime2 NULL,
        [ExpiredDate] datetime2 NULL,
        [TotalPrice] decimal(18,2) NULL,
        [Status] nvarchar(max) NULL,
        [SubscriptionId] nvarchar(450) NULL,
        [UserId] nvarchar(450) NULL,
        CONSTRAINT [PK_SubscriptionPurchases] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_SubscriptionPurchases_SubscriptionPackages_SubscriptionId] FOREIGN KEY ([SubscriptionId]) REFERENCES [SubscriptionPackages] ([Id]),
        CONSTRAINT [FK_SubscriptionPurchases_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [AdPurchaseSlots] (
        [Id] nvarchar(450) NOT NULL,
        [PurchaseSlotPrice] decimal(18,2) NOT NULL,
        [ApprovalDate] datetime2 NULL,
        [ExpiredDate] datetime2 NULL,
        [Content] nvarchar(max) NOT NULL,
        [Image] nvarchar(max) NOT NULL,
        [Video] nvarchar(max) NOT NULL,
        [UrlLink] nvarchar(max) NOT NULL,
        [ReasonForRejection] nvarchar(max) NOT NULL,
        [CreateAt] datetime2 NOT NULL,
        [Status] nvarchar(max) NOT NULL,
        [AdSlotTimeID] nvarchar(450) NOT NULL,
        [AdPurchaseID] nvarchar(450) NOT NULL,
        [AdPackageID] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AdPurchaseSlots] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AdPurchaseSlots_AdPackages_AdPackageID] FOREIGN KEY ([AdPackageID]) REFERENCES [AdPackages] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AdPurchaseSlots_AdPurchases_AdPurchaseID] FOREIGN KEY ([AdPurchaseID]) REFERENCES [AdPurchases] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AdPurchaseSlots_AdSlotTimes_AdSlotTimeID] FOREIGN KEY ([AdSlotTimeID]) REFERENCES [AdSlotTimes] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [Comments] (
        [Id] nvarchar(450) NOT NULL,
        [Content] nvarchar(max) NULL,
        [CreateBy] nvarchar(max) NULL,
        [CreateDate] datetime2 NULL,
        [MovieId] nvarchar(450) NULL,
        CONSTRAINT [PK_Comments] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Comments_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [Contracts] (
        [Id] nvarchar(450) NOT NULL,
        [FileUrl] nvarchar(max) NULL,
        [ContractDate] datetime2 NULL,
        [StartDate] datetime2 NULL,
        [EndDate] datetime2 NULL,
        [Duration] int NULL,
        [Price] decimal(18,2) NULL,
        [Terms] nvarchar(max) NULL,
        [PublisherName] nvarchar(max) NULL,
        [DistributorName] nvarchar(max) NULL,
        [CreateDate] datetime2 NULL,
        [UpdateDate] datetime2 NULL,
        [Status] nvarchar(max) NULL,
        [UserId] nvarchar(450) NULL,
        [MovieId] nvarchar(450) NULL,
        CONSTRAINT [PK_Contracts] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Contracts_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Contracts_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [Media] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [Type] nvarchar(max) NULL,
        [CreateDate] datetime2 NULL,
        [MovieId] nvarchar(450) NULL,
        CONSTRAINT [PK_Media] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Media_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [MovieGenres] (
        [MovieId] nvarchar(450) NOT NULL,
        [GenreId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_MovieGenres] PRIMARY KEY ([MovieId], [GenreId]),
        CONSTRAINT [FK_MovieGenres_Genres_GenreId] FOREIGN KEY ([GenreId]) REFERENCES [Genres] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MovieGenres_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [MovieHistory] (
        [Id] nvarchar(450) NOT NULL,
        [CreateDate] datetime2 NOT NULL,
        [UserId] nvarchar(450) NULL,
        [MovieId] nvarchar(450) NULL,
        CONSTRAINT [PK_MovieHistory] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_MovieHistory_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id]),
        CONSTRAINT [FK_MovieHistory_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [MoviePersons] (
        [MovieId] nvarchar(450) NOT NULL,
        [PersonId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_MoviePersons] PRIMARY KEY ([MovieId], [PersonId]),
        CONSTRAINT [FK_MoviePersons_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_MoviePersons_Persons_PersonId] FOREIGN KEY ([PersonId]) REFERENCES [Persons] ([Id]) ON DELETE CASCADE
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE TABLE [News] (
        [Id] nvarchar(450) NOT NULL,
        [Title] nvarchar(max) NULL,
        [Content] nvarchar(max) NULL,
        [Picture] nvarchar(max) NULL,
        [Url] nvarchar(max) NULL,
        [CreateDate] datetime2 NULL,
        [Status] nvarchar(max) NULL,
        [UserId] nvarchar(450) NULL,
        [MovieId] nvarchar(450) NULL,
        CONSTRAINT [PK_News] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_News_Movies_MovieId] FOREIGN KEY ([MovieId]) REFERENCES [Movies] ([Id]),
        CONSTRAINT [FK_News_Users_UserId] FOREIGN KEY ([UserId]) REFERENCES [Users] ([Id])
    );
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_AdPurchases_UserID] ON [AdPurchases] ([UserID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_AdPurchaseSlots_AdPackageID] ON [AdPurchaseSlots] ([AdPackageID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_AdPurchaseSlots_AdPurchaseID] ON [AdPurchaseSlots] ([AdPurchaseID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_AdPurchaseSlots_AdSlotTimeID] ON [AdPurchaseSlots] ([AdSlotTimeID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_AdSlotTimes_AdSlotID] ON [AdSlotTimes] ([AdSlotID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_AdSlotTimes_AdSlotTimeRangeID] ON [AdSlotTimes] ([AdSlotTimeRangeID]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_Comments_MovieId] ON [Comments] ([MovieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Contracts_MovieId] ON [Contracts] ([MovieId]) WHERE [MovieId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_Contracts_UserId] ON [Contracts] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_Media_MovieId] ON [Media] ([MovieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_MovieGenres_GenreId] ON [MovieGenres] ([GenreId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_MovieHistory_MovieId] ON [MovieHistory] ([MovieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_MovieHistory_UserId] ON [MovieHistory] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_MoviePersons_PersonId] ON [MoviePersons] ([PersonId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_Movies_UserId] ON [Movies] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_News_MovieId] ON [News] ([MovieId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_News_UserId] ON [News] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_SubscriptionPurchases_SubscriptionId] ON [SubscriptionPurchases] ([SubscriptionId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_SubscriptionPurchases_UserId] ON [SubscriptionPurchases] ([UserId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    CREATE INDEX [IX_Users_RoleId] ON [Users] ([RoleId]);
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    EXEC(N'CREATE UNIQUE INDEX [IX_Users_UserRegisterId] ON [Users] ([UserRegisterId]) WHERE [UserRegisterId] IS NOT NULL');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216044339_newdbs'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250216044339_newdbs', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216054249_s'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250216054249_s', N'9.0.0');
END;

IF NOT EXISTS (
    SELECT * FROM [__EFMigrationsHistory]
    WHERE [MigrationId] = N'20250216054623_InitialCreate'
)
BEGIN
    INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
    VALUES (N'20250216054623_InitialCreate', N'9.0.0');
END;

COMMIT;
GO

