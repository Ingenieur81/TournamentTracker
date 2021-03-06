USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spPeople_Insert]    Script Date: 31-Jan-20 14:25:27 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[spPeople_Insert]
	@FirstName nvarchar(100),
	@LastName nvarchar(100),
	@EmailAddress nvarchar(200),
	@CellphoneNumber varchar(20),
	@id int = 0 output --id is not required as input but will be outputted
AS
BEGIN
	SET NOCOUNT ON;
	--Insert new record
    INSERT INTO dbo.PEOPLE(FirstName, LastName, EmailAddress, CellphoneNumber)
	-- with these values
	VALUES (@FirstName, @LastName, @EmailAddress, @CellphoneNumber);

	SELECT @id = SCOPE_IDENTITY();
END
GO
