USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spTournaments_GetAll]    Script Date: 03-Feb-20 15:32:38 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Change CREATE to ALTER after the procedure is created for the first time
CREATE PROCEDURE [dbo].[spTournaments_GetById]
	-- Add the parameters for the stored procedure here
	@TournamentId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- I.e. we do not need a count of the number of returned
	-- rows, we can count the resulting rows ourselves
	SET NOCOUNT ON;

    SELECT *
		FROM dbo.TOURNAMENTS
		WHERE Id = @TournamentId;
END