-- Creates the TEAMMEMBERS table

USE [Tournaments]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[TEAMMEMBERS](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TeamId] [int] NOT NULL,
	[PersonId] [int] NOT NULL,
CONSTRAINT [PK_TEAMMEMBERS] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[TEAMMEMBERS]  WITH CHECK ADD  CONSTRAINT [FK_TEAMMEMBERS_PEOPLE] FOREIGN KEY([PersonId])
REFERENCES [dbo].[PEOPLE] ([Id])
GO

ALTER TABLE [dbo].[TEAMMEMBERS] CHECK CONSTRAINT [FK_TEAMMEMBERS_PEOPLE]
GO

ALTER TABLE [dbo].[TEAMMEMBERS]  WITH CHECK ADD  CONSTRAINT [FK_TEAMMEMBERS_TEAMS] FOREIGN KEY([TeamId])
REFERENCES [dbo].[TEAMS] ([Id])
GO

ALTER TABLE [dbo].[TEAMMEMBERS] CHECK CONSTRAINT [FK_TEAMMEMBERS_TEAMS]
GO