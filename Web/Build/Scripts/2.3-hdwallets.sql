

/****** Object:  Table [dbo].[Wallets]    Script Date: 11/21/2014 12:32:01 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Wallets](
	[WalletID] [int] IDENTITY(1,1) NOT NULL,
	[PublicMasterKey] [nvarchar](max) NOT NULL,
	[EncryptedPrivateMasterKey] [nvarchar](max) NULL,
	[UserID] [int] NOT NULL,
	[LastDerivation] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[WalletID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Wallets]  WITH CHECK ADD  CONSTRAINT [FK74C0E4DD2B89ADAE] FOREIGN KEY([UserID])
REFERENCES [dbo].[Users] ([UserID])
GO

ALTER TABLE [dbo].[Wallets] CHECK CONSTRAINT [FK74C0E4DD2B89ADAE]
GO

/****** Object:  Table [dbo].[WalletAddresses]    Script Date: 11/21/2014 12:32:10 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[WalletAddresses](
	[WalletAddressId] [int] IDENTITY(1,1) NOT NULL,
	[Address] [nvarchar](max) NULL,
	[DateExpires] [datetime] NULL,
	[IsUsed] [bit] NULL,
	[Derivation] [int] NULL,
	[WalletId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[WalletAddressId] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[WalletAddresses]  WITH CHECK ADD  CONSTRAINT [FKA52C53B36B29EBEA] FOREIGN KEY([WalletId])
REFERENCES [dbo].[Wallets] ([WalletID])
GO

ALTER TABLE [dbo].[WalletAddresses] CHECK CONSTRAINT [FKA52C53B36B29EBEA]
GO


ALTER TABLE [dbo].[Users]  ADD [WalletID] INT NULL;
GO

ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK_Users_Wallets] FOREIGN KEY([WalletId])
REFERENCES [dbo].[Wallets] ([WalletID])
GO

ALTER TABLE [dbo].[Users]  CHECK CONSTRAINT [FK_Users_Wallets] 
GO


IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetNextDerivation]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetNextDerivation]
GO

-- =============================================
-- Lock wallet and determine next derivation
-- =============================================
CREATE PROCEDURE GetNextDerivation
(
	@walletId int
)
AS
BEGIN

SET XACT_ABORT ON;

BEGIN TRY
	BEGIN TRAN;

	DECLARE @lastDerivation int;
	DECLARE @derivation int;
	SET @derivation = NULL;
	
	-- Get wallet
	SELECT @lastDerivation=LastDerivation FROM Wallets WITH (ROWLOCK, XLOCK) WHERE WalletID=@walletId;
	
	IF @lastDerivation IS NULL
	BEGIN
		SET @lastDerivation = 0;
	END
	
	-- Check for any previously generated addresses that were not used
	SELECT @derivation=Derivation FROM WalletAddresses
		WHERE WalletID=@walletId 
		AND IsUsed = 0 
		AND DateExpires <= GETUTCDATE()
		
	IF @derivation IS NULL
	BEGIN 
		SET @derivation = @lastDerivation + 1;
		UPDATE Wallets SET LastDerivation=@derivation WHERE WalletID=@walletId;
	END
	
	SELECT @derivation
	COMMIT TRAN;
END TRY
BEGIN CATCH  
    IF (XACT_STATE()) = -1  
    BEGIN  
        ROLLBACK TRAN;  
    END;  
      
    IF (XACT_STATE()) = 1  
    BEGIN  
        COMMIT TRAN;     
    END;  
END CATCH;  
END

