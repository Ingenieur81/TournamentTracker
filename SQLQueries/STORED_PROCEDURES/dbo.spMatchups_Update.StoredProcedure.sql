USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spMatchups_Update]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spMatchups_Update]
	@Id int,
	@WinnerId int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE dbo.MATCHUPS
	SET WinnerId = @WinnerId
	WHERE Id = @Id; -- Update needs a where option, otherwise all record are updated
END
GO
