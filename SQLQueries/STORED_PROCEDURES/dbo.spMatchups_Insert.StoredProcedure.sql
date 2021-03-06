USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spMatchups_Insert]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMatchups_Insert]
	@TournamentId int,
	@MatchupRound int,
	@Id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	--Insert new record
    INSERT INTO dbo.MATCHUPS (TournamentId, MatchupRound)
	-- with these values
	VALUES (@TournamentId, @MatchupRound);

	SELECT @Id = SCOPE_IDENTITY();
END
GO
