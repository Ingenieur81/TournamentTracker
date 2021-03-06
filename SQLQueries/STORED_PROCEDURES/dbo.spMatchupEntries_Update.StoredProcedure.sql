USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spMatchupEntries_Update]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMatchupEntries_Update]
	@Id int,
	@TeamCompetingId int = null,
	@Score float = null
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE dbo.MATCHUPENTRIES
	SET TeamCompetingId = @TeamCompetingId, Score = @Score
	WHERE Id = @Id; -- Update needs a where option, otherwise all record are updated
END
GO
