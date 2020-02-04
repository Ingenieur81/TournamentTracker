CREATE PROCEDURE dbo.spTournaments_Complete
	@Id int
AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    UPDATE dbo.TOURNAMENTS
	set Active = 0
	where Id = @Id
END
GO
