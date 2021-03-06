USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spTournamentPrizes_Insert]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTournamentPrizes_Insert]
	@TournamentId int,
	@PrizeId int,
	@Id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO dbo.TOURNAMENTPRIZES( TournamentId, PrizeId)
	VALUES ( @TournamentId, @PrizeId )
	
	SELECT @Id = SCOPE_IDENTITY();
END
GO
