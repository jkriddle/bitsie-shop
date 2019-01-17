ALTER TABLE USERS ADD DateRegistered DATETIME DEFAULT NULL;
GO
UPDATE USERS SET DateRegistered = GETUTCDATE();
GO
ALTER TABLE USERS ALTER COLUMN DateRegistered DATETIME NOT NULL;
GO

/****** Object:  StoredProcedure [dbo].[GetPayoutBalances]    Script Date: 09/25/2014 13:49:11 ******/
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
ALTER PROCEDURE [dbo].[GetPayoutBalances]
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
		AND (@startDate IS NULL OR o.OrderDate >= @startDate)
		AND (@endDate IS NULL OR o.OrderDate < @endDate)
	GROUP BY u.UserId, u.FirstName, u.LastName, u.Email, c.Phone, c.Name, s.PaymentMethod, s.PaymentAddress
		
END