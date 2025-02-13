-- CREATE TABLES: Users, Environment2D, Object2D (based on erd from powerpoint)
-- CREATE TABLE Users (
--     Username VARCHAR(255) PRIMARY KEY,
--     Password VARCHAR(255) NOT NULL
-- );

CREATE TABLE Environment2D (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    Name VARCHAR(255) NOT NULL,
    MaxHeight FLOAT,
    MaxLength FLOAT,
    -- User_Username VARCHAR(255),
    -- FOREIGN KEY (User_Username) REFERENCES Users(Username) ON DELETE CASCADE
);

CREATE TABLE Object2D (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PrefabId VARCHAR(255),
    PositionX FLOAT,
    PositionY FLOAT,
    ScaleX FLOAT,
    ScaleY FLOAT,
    RotationZ FLOAT,
    SortingLayer INT,
    Environment2D_Id INT,
    FOREIGN KEY (Environment2D_Id) REFERENCES Environment2D(Id) ON DELETE CASCADE
);

