/****** Object:  Table [dbo].[Queue]    Script Date: 10/21/2014 18:43:54 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Subscriptions](
	[SubscriptionID] [int] IDENTITY(1,1) NOT NULL,
	[DateExpires] [datetime] NOT NULL,
	[DateRenewed] [datetime] NOT NULL,
	[DateSubscribed] [datetime] NOT NULL,
	[Type] [int] NOT NULL,
	[Status] [int] NOT NULL,
	[Price] [decimal](19, 5) NOT NULL,
	[Term] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SubscriptionID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO

ALTER TABLE [dbo].[Users] ADD  SubscriptionID int NULL;
GO

ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [FK2C1C7FE59A095476] FOREIGN KEY([SubscriptionID])
REFERENCES [dbo].[Subscriptions] ([SubscriptionID])
GO

ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [FK2C1C7FE59A095476]
GO

ALTER TABLE [dbo].[Orders] ADD  CustomerID int NULL;
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK318A099B745D6B0] FOREIGN KEY([CustomerID])
REFERENCES [dbo].[Users] ([UserID])
GO

ALTER TABLE [dbo].[Orders] ADD SubscriptionID int NULL;
GO

ALTER TABLE [dbo].[Orders]  WITH CHECK ADD  CONSTRAINT [FK318A099B9A095476] FOREIGN KEY([SubscriptionID])
REFERENCES [dbo].[Subscriptions] ([SubscriptionID])
GO

ALTER TABLE [dbo].[Orders] CHECK CONSTRAINT [FK318A099B9A095476]
GO
