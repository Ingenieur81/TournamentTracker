USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spMatchupEntries_GetByMatchup]    Script Date: 29-Jan-20 13:23:05 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Change CREATE to ALTER after the procedure is created for the first time
ALTER PROCEDURE [dbo].[spMatchupEntries_GetByMatchup]
	-- Add the parameters for the stored procedure here
	@MatchupId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- I.e. we do not need a count of the number of returned
	-- rows, we can count the resulting rows ourselves
	SET NOCOUNT ON;

    SELECT *
		FROM dbo.MATCHUPENTRIES
		WHERE MatchupId = @MatchupId;
END
