
/****** Object:  StoredProcedure [dbo].[GetNextDerivation]    Script Date: 12/18/2014 9:06:47 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Lock wallet and determine next derivation
-- =============================================
ALTER PROCEDURE [dbo].[GetNextDerivation]
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
		SET @lastDerivation = -1;
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

