﻿CREATE TABLE Migrations
(
    Id       BIGINT AUTO_INCREMENT PRIMARY KEY,
    Title    VARCHAR(100) NOT NULL,
    Timespan DATETIME DEFAULT CURRENT_TIMESTAMP
);
