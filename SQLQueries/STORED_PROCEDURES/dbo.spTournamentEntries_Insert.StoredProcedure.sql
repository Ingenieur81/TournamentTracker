USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spTournamentEntries_Insert]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTournamentEntries_Insert]
	@TournamentId int,
	@TeamId int,
	@Id int = 0 output
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

	INSERT INTO dbo.TOURNAMENTENTRIES ( TournamentId, TeamId)
	VALUES ( @TournamentId, @TeamId )
	
	SELECT @Id = SCOPE_IDENTITY();
END
GO
