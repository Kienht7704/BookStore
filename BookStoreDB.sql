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
=======
USE BookStoreDB
GO

DROP DATABASE IF EXISTS BookStoreDB;
>>>>>>> 6a50b5217d402ba7418bb5d9450f39e799016e68

CREATE TABLE [User](
	MemberID int PRIMARY KEY,
	FullName nvarchar(100) NOT NULL,
	EmailAddress nvarchar(150), 
	[Password] nvarchar(50) NOT NULL,
	[Role] int NOT NULL
)
GO

CREATE TABLE Category(
	CategoryId int PRIMARY KEY,
	BookGenreType nvarchar(50) NOT NULL,
	[Description] nvarchar(500) NOT NULL
)
GO

CREATE TABLE Book(
	BookId int PRIMARY KEY,
	BookName nvarchar(100) NOT NULL,
	[Description] nvarchar(1000) NOT NULL,
	PublicationDate datetime NOT NULL,
	Quantity int NOT NULL,
	Price float NOT NULL,
	Author nvarchar(50) NOT NULL,
	CategoryId int NOT NULL,
	ImageUrl nvarchar(500) NULL
)
GO

ALTER TABLE Book WITH CHECK ADD CONSTRAINT FK_Book_Category FOREIGN KEY(CategoryId)
REFERENCES Category (CategoryId)
GO

-- ===================================================
-- INSERT DATA TO USER TABLE
-- Role 1 = Admin, Role 2 = User, Role 3 = Member (no access)
-- ===================================================

INSERT [User] VALUES (1, N'Admin', N'admin@1', N'@1', 1) 
GO

INSERT [User] VALUES (2, N'User', N'user@2', N'@2', 2) 
GO

-- ===================================================
-- INSERT DATA TO CATEGORY TABLE
-- ===================================================

INSERT Category (CategoryId, BookGenreType, Description) VALUES (1, N'Fiction', N'Fiction is any creative work, chiefly any narrative work, portraying individuals, events, or places that are imaginary, or in ways that are imaginary.')
GO

INSERT Category (CategoryId, BookGenreType, Description) VALUES (2, N'Science Fiction', N'Science fiction is a genre of speculative fiction, which typically deals with imaginative and futuristic concepts such as advanced science and technology, space exploration, time travel, parallel universes, and extraterrestrial life.')
GO

INSERT Category (CategoryId, BookGenreType, Description) VALUES (3, N'Historical Fiction', N'Historical fiction is a literary genre in which the plot takes place in a setting related to the past events, but is fictional.')
GO

INSERT Category (CategoryId, BookGenreType, Description) VALUES (4, N'Finance', N'Finance is a field that deals with the study of investments. It includes the dynamics of assets and liabilities over time under conditions of different degrees of uncertainty and risk.')
GO

INSERT Category (CategoryId, BookGenreType, Description) VALUES (5, N'Self Help', N'The one that is written with the intention to instruct its readers on solving personal problems')
GO

-- ===================================================
-- INSERT DATA TO BOOK TABLE
-- ===================================================

INSERT Book (BookId, BookName, Description, PublicationDate, Quantity, Price, Author, CategoryId) VALUES (1, N'The Handbook Of International Trade And Finance', N'An international trade transaction guide.', CAST(N'2005-01-01T00:00:00.000' AS DateTime), 10, 45.99, N'Anders Grath', 4)
GO

INSERT Book (BookId, BookName, Description, PublicationDate, Quantity, Price, Author, CategoryId) VALUES (2, N'Snow Crash', N'Hiro lives in a Los Angeles where franchises line the freeway.', CAST(N'2001-01-01T00:00:00.000' AS DateTime), 20, 12.99, N'Neal Stephenson', 2)
GO

INSERT Book (BookId, BookName, Description, PublicationDate, Quantity, Price, Author, CategoryId) VALUES (3, N'Contact: A Novel', N'The future is here in an adventure of cosmic dimension.', CAST(N'2019-02-26T00:00:00.000' AS DateTime), 15, 12.99, N'Carl Sagan', 2)
GO

INSERT Book (BookId, BookName, Description, PublicationDate, Quantity, Price, Author, CategoryId) VALUES (4, N'The Time Machine', N'The story follows a Victorian scientist who invented time travel.', CAST(N'2021-06-29T00:00:00.000' AS DateTime), 11, 6.99, N'H.G. Wells', 2)
GO

INSERT Book (BookId, BookName, Description, PublicationDate, Quantity, Price, Author, CategoryId) VALUES (5, N'Rosewater', N'Rosewater is a town on the edge of a mysterious alien biodome.', CAST(N'2018-09-18T00:00:00.000' AS DateTime), 27, 10.49, N'Tade Thompson', 2)
GO

-- ===================================================
-- VERIFY DATA
-- ===================================================
SELECT 'Users Created:' AS Info, COUNT(*) AS Count FROM [User]
SELECT 'Categories Created:' AS Info, COUNT(*) AS Count FROM Category
SELECT 'Books Created:' AS Info, COUNT(*) AS Count FROM Book
GO