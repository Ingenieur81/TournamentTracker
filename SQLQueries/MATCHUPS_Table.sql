-- Creates the MATCHUPS table

USE [Tournaments]
GO

SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[MATCHUPS](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[WinnerId] [int] NOT NULL,
	[MatchupRound] [int] NOT NULL,
CONSTRAINT [PK_MATCHUPS] PRIMARY KEY CLUSTERED
(
	[Id] ASC
)WITH(PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO

ALTER TABLE [dbo].[MATCHUPS]  WITH CHECK ADD  CONSTRAINT [FK_MATCHUPS_TEAMS] FOREIGN KEY([WinnerId])
REFERENCES [dbo].[TEAMS] ([Id])
GO

ALTER TABLE [dbo].[MATCHUPS] CHECK CONSTRAINT [FK_MATCHUPS_TEAMS]
GO