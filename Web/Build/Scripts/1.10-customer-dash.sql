IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetDashboardReport]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetDashboardReport]
GO


SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- =============================================
-- Author:		Joshua Riddle
-- Create date: 07/30/2014
-- Description:	Report used for customer dash
-- =============================================
CREATE PROCEDURE GetDashboardReport
	-- Add the parameters for the stored procedure here
	@userID INT
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	DECLARE @revenueTodayUsd decimal(9, 2);
	DECLARE @revenueTodayBtc decimal(18, 9);
	DECLARE @revenueThisMonthUsd decimal(9, 2);
	DECLARE @revenueThisMonthBtc decimal(18, 9);
	DECLARE @revenueAllTimeUsd decimal(9, 2);
	DECLARE @revenueAllTimeBtc decimal(18, 9);
	DECLARE @revenuePendingPayoutUsd decimal(9, 2);
	DECLARE @revenuePendingPayoutBtc decimal(18, 9);
	
	SELECT @revenueTodayUsd=SUM(Total), @revenueTodayBtc=SUM(BtcTotal) FROM Orders 
			WHERE UserId=@userId
			AND (DATEDIFF(DAY, GETUTCDATE(), OrderDate) = 0)
			AND (Status=3 OR Status=4 Or Status=7);
			
	SELECT @revenueThisMonthUsd=SUM(Total), @revenueThisMonthBtc=SUM(BtcTotal) FROM Orders 
			WHERE UserID=@userId
			AND (DATEDIFF(MONTH, GETUTCDATE(), OrderDate) = 0)
			AND (Status=3 OR Status=4 Or Status=7);
			
	SELECT @revenueAllTimeUsd=SUM(Total), @revenueAllTimeBtc=SUM(BtcTotal) FROM Orders 
			WHERE UserID=@userId 
			AND (Status=3 OR Status=4 Or Status=7);
			
	SELECT @revenuePendingPayoutUsd=SUM(Total), @revenuePendingPayoutBtc=SUM(BtcTotal) FROM Orders 
			WHERE UserID=@userId
			AND (Status=7);
			
	SELECT  @revenueTodayUsd AS RevenueTodayUsd,
			@revenueTodayBtc AS RevenueTodayBtc,
			@revenueThisMonthUsd AS RevenueThisMonthUsd,
			@revenueThisMonthBtc AS RevenueThisMonthBtc,
			@revenueAllTimeUsd AS RevenueAllTimeUsd,
			@revenueAllTimeBtc AS RevenueAllTimeBtc,
			@revenuePendingPayoutUsd AS RevenuePendingPayoutUsd,
			@revenuePendingPayoutBtc AS RevenuePendingPayoutBtc;
		
END
GO
