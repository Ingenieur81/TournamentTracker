CREATE PROCEDURE dbo.spTournament_Insert
	@TournamentName nvarchar(200),
	@EntryFee money,
	@Id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO dbo.TOURNAMENTS ( TournamentName, EntryFee, Active)
	VALUES ( @TournamentName, @EntryFee, 1)
	select @Id = SCOPE_IDENTITY();
END
GO
