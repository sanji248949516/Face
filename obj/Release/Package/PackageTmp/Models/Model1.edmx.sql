
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 06/25/2016 23:46:55
-- Generated from EDMX file: E:\工作\2016年6月7日百度人脸页面\Face\Face\Models\Model1.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [CtrixDB];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------


-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[BaiduFace]', 'U') IS NOT NULL
    DROP TABLE [dbo].[BaiduFace];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'BaiduFace'
CREATE TABLE [dbo].[BaiduFace] (
    [id] nvarchar(255)  NOT NULL,
    [file] nvarchar(255)  NOT NULL,
    [entityid] nvarchar(255)  NOT NULL,
    [success] bit  NOT NULL,
    [imgsrc] nvarchar(255)  NOT NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [id] in table 'BaiduFace'
ALTER TABLE [dbo].[BaiduFace]
ADD CONSTRAINT [PK_BaiduFace]
    PRIMARY KEY CLUSTERED ([id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------