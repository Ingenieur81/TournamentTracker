CREATE PROCEDURE dbo.spTeams_Insert
	@TeamName nvarchar(50),
	@Id int = 0 output
AS
BEGIN
	SET NOCOUNT ON;

    INSERT INTO dbo.TEAMS (TeamName)
	values (@TeamName)

	select @Id = SCOPE_IDENTITY();

END
GO
