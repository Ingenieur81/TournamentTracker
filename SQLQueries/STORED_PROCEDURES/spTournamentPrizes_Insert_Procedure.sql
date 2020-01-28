CREATE PROCEDURE dbo.spTournamentPrizes_Insert
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
