

/****************************************** INVOICES ******************************/
CREATE TABLE [dbo].[Invoices](
	[InvoiceID] [int] IDENTITY(1,1) NOT NULL,
	[USDAmount] [decimal](19, 5) NOT NULL,
	[InvoiceGuid] [nvarchar](max) NOT NULL,
	[SendDate] [datetime] NULL,
	[DueDate] [datetime] NOT NULL,
	[InvoiceDescription] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[InvoiceNumber] [nvarchar](max) NULL,
	[CustomerID] [int] NULL,
	[MerchantID] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[InvoiceID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK48A70817745D6B0] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Users] ([UserID])
GO

ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK48A70817745D6B0]
GO

ALTER TABLE [dbo].[Invoices]  WITH CHECK ADD  CONSTRAINT [FK48A70817CB6D9350] FOREIGN KEY([MerchantID])
REFERENCES [dbo].[Users] ([UserID])
GO

ALTER TABLE [dbo].[Invoices] CHECK CONSTRAINT [FK48A70817CB6D9350]
GO


/****************************************** INVOICE ITEMS ******************************/

CREATE TABLE [dbo].[InvoiceItems](
	[InvoiceItemId] [int] IDENTITY(1,1) NOT NULL,
	[UsdAmount] [decimal](19, 5) NOT NULL,
	[Quantity] [int] NOT NULL,
	[Description] [nvarchar](max) NOT NULL,
	[Position] [int] NOT NULL,
	[InvoiceID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[InvoiceItemId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[InvoiceItems]  WITH CHECK ADD  CONSTRAINT [FK748A9DF82E823120] FOREIGN KEY([InvoiceID])
REFERENCES [dbo].[Invoices] ([InvoiceID])
GO

ALTER TABLE [dbo].[InvoiceItems] CHECK CONSTRAINT [FK748A9DF82E823120]
GO

/****************************************** OFFLINE ADDRESSES ******************************/
ALTER TABLE [dbo].[OfflineAddresses] ADD [Status] int DEFAULT NULL;
GO
UPDATE OfflineAddresses SET Status=1;
GO
ALTER TABLE [dbo].[OfflineAddresses] ALTER COLUMN [Status] int NOT NULL;
GO


/****************************************** PRODDUCTS ******************************/
CREATE TABLE [dbo].[Products](
	[ProductID] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](max) NOT NULL,
	[Description] [nvarchar](max) NULL,
	[Price] [decimal](19, 5) NOT NULL,
	[RedirectUrl] [nvarchar](max) NULL,
	[Status] [int] NOT NULL,
	[UserID] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ProductID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Products]  WITH CHECK ADD  CONSTRAINT [FK4A7FD86A2B89ADAE] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO

ALTER TABLE [dbo].[Products] CHECK CONSTRAINT [FK4A7FD86A2B89ADAE]
GO


/****************************************** ORDER ******************************/
ALTER TABLE [dbo].[Orders] ADD [InvoiceId] int DEFAULT NULL;
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK318A099B2E823120] FOREIGN KEY([InvoiceId])
REFERENCES [dbo].[Invoices] ([InvoiceID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK318A099B2E823120]
GO

ALTER TABLE [dbo].[Orders] ADD [ProductID] int DEFAULT NULL;
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK318A099BA1C9DCB3] FOREIGN KEY([ProductID])
REFERENCES [dbo].[Products] ([ProductID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK318A099BA1C9DCB3]
GO

/****************************************** PERMISSIONS ******************************/
INSERT INTO [dbo].[Permissions]
           ([Name]
           ,[Description])
     VALUES
           ('edit-products', 'Edit products')
GO

/****************************************** SETTINGS ******************************/
ALTER TABLE [dbo].[Settings] ADD [CoinbaseApiKey] nvarchar(MAX) DEFAULT NULL;
GO

ALTER TABLE [dbo].[Settings] ADD [CoinbaseApiSecret] nvarchar(MAX) DEFAULT NULL;
GO

ALTER TABLE [dbo].[Settings] ADD [GoCoinAccessToken] nvarchar(MAX) DEFAULT NULL;
GO

ALTER TABLE [dbo].[Settings] ADD [HtmlTemplate] nvarchar(MAX) DEFAULT NULL;
GO

ALTER TABLE [dbo].[Settings] ADD [BitpayApiKey] nvarchar(MAX) DEFAULT NULL;
GO

UPDATE [dbo].[Settings] SET BitpayApiKey = ApiKey;
GO

ALTER TABLE [dbo].[Settings] DROP CONSTRAINT DF__Settings__ApiKey__4C6B5938;
GO

ALTER TABLE [dbo].[Settings] DROP COLUMN ApiKey;
GO
EXEC sp_RENAME 'Settings.HostedCheckoutLogo' , 'LogoUrl', 'COLUMN'
GO
EXEC sp_RENAME 'Settings.HostedCheckoutTitle' , 'StoreTitle', 'COLUMN'
GO
EXEC sp_RENAME 'Settings.HostedCheckoutBackground' , 'BackgroundColor', 'COLUMN'
GO

/****************************************** USER PERMISSIONS ******************************/
INSERT INTO UserPermissions (UserID, PermissionID) VALUES (3, 3);
GO

/****************************************** USERS ******************************/
ALTER TABLE [dbo].[Users] ADD [MerchantUserID] int DEFAULT NULL;
GO

ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK2C1C7FE513A0C804] FOREIGN KEY([MerchantUserID])
REFERENCES [dbo].[Users] ([UserID])
GO

ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK2C1C7FE513A0C804]
GO

ALTER TABLE [dbo].[Users] ADD [DateRegistered] DATETIME NULL;
GO

UPDATE [dbo].[Users] SET DateRegistered = GETUTCDATE()
GO

ALTER TABLE [dbo].[Users] ALTER COLUMN [DateRegistered] DATETIME NOT NULL;
GO

/* Remove unique email requirement (needed for Customers feature) */
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK2C1C7FE5E7B25992]
GO

ALTER TABLE [dbo].[Users] DROP CONSTRAINT [UQ__Users__A9D10534282DF8C2]
GO

ALTER TABLE Users ALTER COLUMN MerchantId nvarchar(45) NULL;

/****************************************** COMPANIES ******************************/
ALTER TABLE [dbo].[Companies] ALTER COLUMN [Name] nvarchar(MAX)  NULL;
GO