IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[GetPayoutBalances]') AND type in (N'P', N'PC'))
DROP PROCEDURE [dbo].[GetPayoutBalances]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
-- ===========================================================
-- Author:		Joshua Riddle
-- Create date: 7/25/2014
-- Description:	Retrieve balances that are pending payout 
-- for each customer
-- ===========================================================
CREATE PROCEDURE GetPayoutBalances
	@paymentMethod INT,
	@startDate DATETIME,
	@endDate DATETIME
AS
BEGIN
	SET NOCOUNT ON;

	SELECT u.UserId, c.Name AS CompanyName, u.FirstName, u.LastName, u.Email, 
	c.Phone,  SUM(o.Total) AS PayoutBalance, s.PaymentAddress,
	s.PaymentMethod FROM Users u
	LEFT JOIN Orders o ON u.UserID=o.UserID
	INNER JOIN Companies c ON u.CompanyID=c.CompanyID
	INNER JOIN Settings s On u.SettingsID = s.SettingsID
	WHERE (o.Status=7) 
		AND (@paymentMethod IS NULL OR s.PaymentMethod=@paymentMethod)
		AND (@startDate IS NULL OR o.OrderDate >= @startDate)
		AND (@endDate IS NULL OR o.OrderDate < @endDate)
	GROUP BY u.UserId, u.FirstName, u.LastName, u.Email, c.Phone, c.Name, s.PaymentMethod, s.PaymentAddress
		
END
GO
