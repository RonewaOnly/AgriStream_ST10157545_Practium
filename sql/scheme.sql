-- Optional: the MVC app creates these tables automatically on first start (EnsureCreated).
-- Use this script instead if you prefer to create the schema yourself first, then start the app.

CREATE TABLE Roles (
    RoleId   INT IDENTITY(1,1) PRIMARY KEY,
    RoleName NVARCHAR(30) NOT NULL UNIQUE
);

CREATE TABLE Users (
    UserId       INT IDENTITY(1,1) PRIMARY KEY,
    Username     NVARCHAR(60)  NOT NULL UNIQUE,
    PasswordHash NVARCHAR(256) NOT NULL,
    RoleId       INT NOT NULL REFERENCES Roles(RoleId)
);

CREATE TABLE DeviceRegistry (
    DeviceId          NVARCHAR(50) PRIMARY KEY,
    FieldId           NVARCHAR(50) NOT NULL,
    OperationalStatus NVARCHAR(30) NOT NULL DEFAULT 'Idle',
    LastCommand       NVARCHAR(50) NULL,
    LastUpdatedUtc    DATETIME2    NOT NULL DEFAULT SYSUTCDATETIME()
);

INSERT INTO Roles (RoleName) VALUES ('Farmer'), ('Agronomist'), ('Admin');
INSERT INTO DeviceRegistry (DeviceId, FieldId) VALUES
    ('sensor-001', 'field-A'),
    ('sensor-002', 'field-A'),
    ('valve-001',  'field-B');
