ALTER TABLE Users ALTER COLUMN MerchantId nvarchar(50);
GO

ALTER TABLE Settings ADD BitpayApiKey nvarchar(MAX) DEFAULT NULL;
ALTER TABLE Settings ADD CoinbaseApiKey nvarchar(MAX) DEFAULT NULL;
ALTER TABLE Settings ADD CoinbaseApiSecret nvarchar(MAX) DEFAULT NULL;
GO
UPDATE Settings SET BitpayApiKey=ApiKey;
GO
ALTER TABLE Settings DROP COLUMN ApiKey;
ALTER TABLE Settings DROP COLUMN ApiSecret;
GO