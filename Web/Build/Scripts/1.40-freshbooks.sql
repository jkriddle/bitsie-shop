ALTER TABLE Settings ADD FreshbooksApiUrl nvarchar(MAX) DEFAULT NULL;
ALTER TABLE Settings ADD FreshbooksAuthToken nvarchar(MAX) DEFAULT NULL;
ALTER TABLE Orders ADD FreshbooksInvoiceId BIGINT DEFAULT NULL;
ALTER TABLE Orders ADD FreshbooksPaymentId BIGINT DEFAULT NULL;