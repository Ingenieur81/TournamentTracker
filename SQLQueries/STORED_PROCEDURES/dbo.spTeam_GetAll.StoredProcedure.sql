USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spTeam_GetAll]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spTeam_GetAll]
AS
BEGIN
	SET NOCOUNT ON;

	SELECT *
	FROM dbo.TEAMS;
END
GO
