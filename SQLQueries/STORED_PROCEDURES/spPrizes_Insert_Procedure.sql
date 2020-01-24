USE [Tournaments]
GO
/****** Object:  StoredProcedure [dbo].[spPrizes_Insert]    Script Date: 24-Jan-20 16:05:51 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
ALTER PROCEDURE [dbo].[spPrizes_Insert]
	@PlaceNumber int,
	@PlaceName nvarchar(50),
	@PrizeAmount money,
	@PrizePercentage float,
	@id int = 0 output --id is not required as input but will be outputted
AS
BEGIN
	SET NOCOUNT ON;
	--Insert new record
    INSERT INTO dbo.PRIZES (PlaceNumber, PlaceName, PrizeAmount, PrizePercentage)
	-- with these values
	VALUES (@PlaceNumber, @PlaceName, @PrizeAmount, @PrizePercentage);

	SELECT @id = SCOPE_IDENTITY();
END
