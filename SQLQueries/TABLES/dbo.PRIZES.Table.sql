USE [Tournaments]
GO
/****** Object:  Table [dbo].[PRIZES]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PRIZES](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[PlaceNumber] [int] NOT NULL,
	[PlaceName] [nvarchar](50) NOT NULL,
	[PrizeAmount] [money] NOT NULL,
	[PrizePercentage] [float] NOT NULL,
 CONSTRAINT [PK_PRIZES] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
ALTER TABLE [dbo].[PRIZES] ADD  CONSTRAINT [DF_PRIZES_PrizeAmount]  DEFAULT ((0)) FOR [PrizeAmount]
GO
ALTER TABLE [dbo].[PRIZES] ADD  CONSTRAINT [DF_PRIZES_PrizePercentage]  DEFAULT ((0)) FOR [PrizePercentage]
GO
