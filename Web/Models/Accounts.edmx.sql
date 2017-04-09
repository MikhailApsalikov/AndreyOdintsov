
-- --------------------------------------------------
-- Entity Designer DDL Script for SQL Server 2005, 2008, 2012 and Azure
-- --------------------------------------------------
-- Date Created: 02/19/2017 23:20:29
-- Generated from EDMX file: E:\OneDrive\AndreyOdintsovProjects\Accounts\Web\Models\Accounts.edmx
-- --------------------------------------------------

SET QUOTED_IDENTIFIER OFF;
GO
USE [Accounts];
GO
IF SCHEMA_ID(N'dbo') IS NULL EXECUTE(N'CREATE SCHEMA [dbo]');
GO

-- --------------------------------------------------
-- Dropping existing FOREIGN KEY constraints
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[FK_AccountManager]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Accounts] DROP CONSTRAINT [FK_AccountManager];
GO
IF OBJECT_ID(N'[dbo].[FK_EvaluationEvaluationValue]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[EvaluationValues] DROP CONSTRAINT [FK_EvaluationEvaluationValue];
GO
IF OBJECT_ID(N'[dbo].[FK_ExamineeEvaluation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Evaluations] DROP CONSTRAINT [FK_ExamineeEvaluation];
GO
IF OBJECT_ID(N'[dbo].[FK_ExaminerEvaluation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Evaluations] DROP CONSTRAINT [FK_ExaminerEvaluation];
GO
IF OBJECT_ID(N'[dbo].[FK_ManagerEvaluation]', 'F') IS NOT NULL
    ALTER TABLE [dbo].[Evaluations] DROP CONSTRAINT [FK_ManagerEvaluation];
GO

-- --------------------------------------------------
-- Dropping existing tables
-- --------------------------------------------------

IF OBJECT_ID(N'[dbo].[Accounts]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Accounts];
GO
IF OBJECT_ID(N'[dbo].[Evaluations]', 'U') IS NOT NULL
    DROP TABLE [dbo].[Evaluations];
GO
IF OBJECT_ID(N'[dbo].[EvaluationValues]', 'U') IS NOT NULL
    DROP TABLE [dbo].[EvaluationValues];
GO

-- --------------------------------------------------
-- Creating all tables
-- --------------------------------------------------

-- Creating table 'Accounts'
CREATE TABLE [dbo].[Accounts] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Code] nvarchar(max)  NULL,
    [Login] nvarchar(max)  NULL,
    [Password] nvarchar(max)  NULL,
    [Active] bit  NOT NULL,
    [Salt] nvarchar(max)  NULL,
    [FullName] nvarchar(max)  NULL,
    [Sex] nvarchar(max)  NULL,
    [Region] nvarchar(max)  NULL,
    [MicroRegion] nvarchar(max)  NULL,
    [Department] nvarchar(max)  NULL,
    [Position] nvarchar(max)  NULL,
    [Role] int  NULL,
    [Guid] nvarchar(max)  NULL,
    [ManagerId] int  NULL,
    [ManagerFullName] nvarchar(max)  NULL,
    [LastEvaluationPercent] float  NULL
);
GO

-- Creating table 'Evaluations'
CREATE TABLE [dbo].[Evaluations] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Passed] datetime  NOT NULL,
    [Reviewed] datetime  NULL,
    [ExamineeId] int  NOT NULL,
    [ExaminerId] int  NULL,
    [ReviewedResult] float  NULL,
    [IndicatorsCount] int  NULL,
    [ManagerId] int  NULL,
    [ManagerResult] float  NULL,
    [ManagerReviewed] datetime  NULL
);
GO

-- Creating table 'EvaluationValues'
CREATE TABLE [dbo].[EvaluationValues] (
    [Id] int IDENTITY(1,1) NOT NULL,
    [Competency] int  NULL,
    [Indicator] int  NULL,
    [Value] float  NULL,
    [ReviewValue] float  NULL,
    [EvaluationId] int  NOT NULL,
    [ManagerValue] float  NULL
);
GO

-- --------------------------------------------------
-- Creating all PRIMARY KEY constraints
-- --------------------------------------------------

-- Creating primary key on [Id] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [PK_Accounts]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'Evaluations'
ALTER TABLE [dbo].[Evaluations]
ADD CONSTRAINT [PK_Evaluations]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- Creating primary key on [Id] in table 'EvaluationValues'
ALTER TABLE [dbo].[EvaluationValues]
ADD CONSTRAINT [PK_EvaluationValues]
    PRIMARY KEY CLUSTERED ([Id] ASC);
GO

-- --------------------------------------------------
-- Creating all FOREIGN KEY constraints
-- --------------------------------------------------

-- Creating foreign key on [ExamineeId] in table 'Evaluations'
ALTER TABLE [dbo].[Evaluations]
ADD CONSTRAINT [FK_ExamineeEvaluation]
    FOREIGN KEY ([ExamineeId])
    REFERENCES [dbo].[Accounts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ExamineeEvaluation'
CREATE INDEX [IX_FK_ExamineeEvaluation]
ON [dbo].[Evaluations]
    ([ExamineeId]);
GO

-- Creating foreign key on [ExaminerId] in table 'Evaluations'
ALTER TABLE [dbo].[Evaluations]
ADD CONSTRAINT [FK_ExaminerEvaluation]
    FOREIGN KEY ([ExaminerId])
    REFERENCES [dbo].[Accounts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ExaminerEvaluation'
CREATE INDEX [IX_FK_ExaminerEvaluation]
ON [dbo].[Evaluations]
    ([ExaminerId]);
GO

-- Creating foreign key on [EvaluationId] in table 'EvaluationValues'
ALTER TABLE [dbo].[EvaluationValues]
ADD CONSTRAINT [FK_EvaluationEvaluationValue]
    FOREIGN KEY ([EvaluationId])
    REFERENCES [dbo].[Evaluations]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_EvaluationEvaluationValue'
CREATE INDEX [IX_FK_EvaluationEvaluationValue]
ON [dbo].[EvaluationValues]
    ([EvaluationId]);
GO

-- Creating foreign key on [ManagerId] in table 'Accounts'
ALTER TABLE [dbo].[Accounts]
ADD CONSTRAINT [FK_AccountManager]
    FOREIGN KEY ([ManagerId])
    REFERENCES [dbo].[Accounts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_AccountManager'
CREATE INDEX [IX_FK_AccountManager]
ON [dbo].[Accounts]
    ([ManagerId]);
GO

-- Creating foreign key on [ManagerId] in table 'Evaluations'
ALTER TABLE [dbo].[Evaluations]
ADD CONSTRAINT [FK_ManagerEvaluation]
    FOREIGN KEY ([ManagerId])
    REFERENCES [dbo].[Accounts]
        ([Id])
    ON DELETE NO ACTION ON UPDATE NO ACTION;
GO

-- Creating non-clustered index for FOREIGN KEY 'FK_ManagerEvaluation'
CREATE INDEX [IX_FK_ManagerEvaluation]
ON [dbo].[Evaluations]
    ([ManagerId]);
GO

-- --------------------------------------------------
-- Script has ended
-- --------------------------------------------------