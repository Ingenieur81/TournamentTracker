CREATE PROCEDURE dbo.spMatchupsEntries_Insert
	@MatchupId int,
	@ParentMatchupId int,
	@TeamCompetingId int,
	@Id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Insert new record
    INSERT INTO dbo.MATCHUPENTRIES(MatchupId, ParentMatchupId, TeamCompetingId)
	-- with these values
	VALUES (@MatchupId, @ParentMatchupId, @TeamCompetingId);

	SELECT @Id = SCOPE_IDENTITY();
END
GO
