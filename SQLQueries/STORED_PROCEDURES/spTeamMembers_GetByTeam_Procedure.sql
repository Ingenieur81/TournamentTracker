USE Tournaments
GO
/****** Object:  StoredProcedure [dbo].[spTeamMembers_GetByTeam]    Script Date: 24-Jan-20 08:31:58 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Change CREATE to ALTER after the procedure is created for the first time
ALTER PROCEDURE dbo.spTeamMembers_GetByTeam
	-- Add the parameters for the stored procedure here
	@TeamId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	-- I.e. we do not need a count of the number of returned
	-- rows, we can count the resulting rows ourselves
	SET NOCOUNT ON;

    SELECT p.*
		FROM dbo.PEOPLE p
		inner join dbo.TEAMMEMBERS t on p.Id = t.PersonId
		WHERE t.TeamId = @TeamId;
END