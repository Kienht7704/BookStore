-- ===================================================
-- BookStoreDB - Database Creation Script
-- Version: 2.0 - Added ImageUrl column
-- ===================================================

-- Switch to master to drop existing database
USE master
GO

<<<<<<< HEAD
-- Drop database if exists
DROP DATABASE IF EXISTS BookStoreDB;
GO

-- Create new database
CREATE DATABASE BookStoreDB
GO

USE BookStoreDB
GO

-- ===================================================
-- CREATE TABLES
-- ===================================================

-- =====================
-- ROLE
-- =====================
CREATE TABLE Role (
    RoleId int PRIMARY KEY,
    RoleName nvarchar(50) NOT NULL
)
GO

-- =====================
-- USER
-- =====================
CREATE TABLE [User] (
    MemberID int PRIMARY KEY,
    FullName nvarchar(100) NOT NULL,
    EmailAddress nvarchar(150),
    [Password] nvarchar(50) NOT NULL,
    RoleId int NOT NULL
)
GO

ALTER TABLE [User]
ADD CONSTRAINT FK_User_Role
FOREIGN KEY (RoleId) REFERENCES Role(RoleId)
GO

-- =====================
-- CATEGORY
-- =====================
CREATE TABLE Category (
    CategoryId int PRIMARY KEY,
    BookGenreType nvarchar(50) NOT NULL,
    [Description] nvarchar(500) NOT NULL
)
GO

-- =====================
-- BOOK
-- =====================
CREATE TABLE Book (
    BookId int PRIMARY KEY,
    BookName nvarchar(100) NOT NULL,
    [Description] nvarchar(1000) NOT NULL,
    PublicationDate datetime NOT NULL,
    Quantity int NOT NULL,
    Price float NOT NULL,
    Author nvarchar(50) NOT NULL,
    CategoryId int NOT NULL,
    ImageUrl nvarchar(500)
)
GO

ALTER TABLE Book
ADD CONSTRAINT FK_Book_Category
FOREIGN KEY (CategoryId) REFERENCES Category(CategoryId)
GO

-- =====================
-- INSERT ROLE
-- =====================
INSERT Role VALUES
(1, N'Admin'),
(2, N'User'),
(3, N'Staff')
GO

-- =====================
-- INSERT USER
-- =====================
INSERT [User] VALUES
(1, N'Admin', N'admin@1', N'@1', 1),
(2, N'User', N'user@2', N'@2', 3)
GO

-- =====================
-- INSERT CATEGORY
-- =====================
INSERT Category VALUES
(1, N'Fiction', N'Creative imaginary stories'),
(2, N'Science Fiction', N'Futuristic science-based stories'),
(3, N'Historical Fiction', N'Stories based on historical settings'),
(4, N'Finance', N'Books about finance and investment'),
(5, N'Self Help', N'Personal development books')
GO

-- =====================
-- INSERT BOOK
-- =====================
INSERT Book VALUES
(1, N'The Handbook Of International Trade And Finance', N'International trade guide', '2005-01-01', 10, 45.99, N'Anders Grath', 4, NULL),
(2, N'Snow Crash', N'Cyberpunk sci-fi novel', '2001-01-01', 20, 12.99, N'Neal Stephenson', 2, NULL),
(3, N'Contact', N'Cosmic science fiction', '2019-02-26', 15, 12.99, N'Carl Sagan', 2, NULL),
(4, N'The Time Machine', N'Time travel classic', '2021-06-29', 11, 6.99, N'H.G. Wells', 2, NULL),
(5, N'Rosewater', N'Alien biodome story', '2018-09-18', 27, 10.49, N'Tade Thompson', 2, NULL)
GO
-- ===================================================
-- VERIFY DATA
-- ===================================================
SELECT 'Users Created:' AS Info, COUNT(*) AS Count FROM [User]
SELECT 'Categories Created:' AS Info, COUNT(*) AS Count FROM Category
SELECT 'Books Created:' AS Info, COUNT(*) AS Count FROM Book
GO

Select * from [user]
select * from Book
select * from Category
select * from Role