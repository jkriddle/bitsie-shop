ALTER TABLE OfflineAddresses ADD EncryptedPrivateKey nvarchar(MAX) DEFAULT NULL;

ALTER TABLE Settings ADD ApiKey nvarchar(MAX) DEFAULT NULL;
