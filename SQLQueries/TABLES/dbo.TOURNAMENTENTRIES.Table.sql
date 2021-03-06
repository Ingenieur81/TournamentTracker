USE [Tournaments]
GO
/****** Object:  Table [dbo].[TOURNAMENTENTRIES]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TOURNAMENTENTRIES](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TournamentId] [int] NOT NULL,
	[TeamId] [int] NOT NULL,
 CONSTRAINT [PK_TOURNAMENTENTRIES] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[TOURNAMENTENTRIES]  WITH CHECK ADD  CONSTRAINT [FK_TOURNAMENTENTRIES_TEAMS] FOREIGN KEY([TeamId])
REFERENCES [dbo].[TEAMS] ([Id])
GO
ALTER TABLE [dbo].[TOURNAMENTENTRIES] CHECK CONSTRAINT [FK_TOURNAMENTENTRIES_TEAMS]
GO
ALTER TABLE [dbo].[TOURNAMENTENTRIES]  WITH CHECK ADD  CONSTRAINT [FK_TOURNAMENTENTRIES_TOURNAMENTS] FOREIGN KEY([TournamentId])
REFERENCES [dbo].[TOURNAMENTS] ([Id])
GO
ALTER TABLE [dbo].[TOURNAMENTENTRIES] CHECK CONSTRAINT [FK_TOURNAMENTENTRIES_TOURNAMENTS]
GO
