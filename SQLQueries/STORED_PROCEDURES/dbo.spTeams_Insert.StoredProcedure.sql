USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spTeams_Insert]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTeams_Insert]
	@TeamName nvarchar(100),
	@Id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO dbo.TEAMS (TeamName)
	values (@TeamName)

	select @Id = SCOPE_IDENTITY();

END
GO
