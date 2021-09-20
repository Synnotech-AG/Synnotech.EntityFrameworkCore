CREATE TABLE Contacts (
    Id INT IDENTITY(1, 1) PRIMARY KEY CLUSTERED,
    [Name] NVARCHAR(100) NOT NULL
);

INSERT INTO Contacts ([Name])
VALUES ('John Doe');

INSERT INTO Contacts ([Name])
VALUES ('Margaret Johnson');