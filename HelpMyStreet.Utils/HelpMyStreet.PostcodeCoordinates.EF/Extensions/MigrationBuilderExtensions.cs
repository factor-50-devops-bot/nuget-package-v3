using Microsoft.EntityFrameworkCore.Migrations;

namespace HelpMyStreet.PostcodeCoordinates.EF.Extensions
{
		public static class MigrationBuilderExtensions
    {
        public static void CreatePostcodeLoadProc(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
CREATE PROCEDURE [Staging].[LoadPostcodesFromStagingTableAndSwitch]
AS
BEGIN
	SET XACT_ABORT ON
	SET	NOCOUNT ON

	BEGIN TRY
		
		DECLARE @dateNow DATETIME2(0) = GetUTCDate()
		
		TRUNCATE TABLE [Staging].[Postcode_Switch]

		-- make copy of table (the postcode table is not updated by the application so we won't lose any data)
		SET IDENTITY_INSERT [Staging].[Postcode_Switch] ON
		
		INSERT INTO [Staging].[Postcode_Switch] (
			[Id],
			[Postcode],
			[LastUpdated],
			[Latitude],
			[Longitude],
			[IsActive]
			)
		SELECT [Id],
			[Postcode],
			[LastUpdated],
			[Latitude],
			[Longitude],
			[IsActive]
		FROM [Address].[Postcode] WITH (NOLOCK)

		SET IDENTITY_INSERT [Staging].[Postcode_Switch] OFF

		-- update postcodes
		MERGE [Staging].[Postcode_Switch] sw
		USING [Staging].[Postcode_Staging] st
			ON (sw.[Postcode] = st.[Postcode])
		WHEN MATCHED
			AND (
				sw.[Latitude] != st.[Latitude]
				OR sw.[Longitude] != st.[Longitude]
				OR sw.[IsActive] != st.[IsActive]
				)
			THEN
				UPDATE
				SET sw.[Latitude] = st.[Latitude],
					sw.[Longitude] = st.[Longitude],
					sw.[LastUpdated] = @dateNow,
					sw.[IsActive] = st.[IsActive]
		WHEN NOT MATCHED BY TARGET
			THEN
				INSERT (
					[Postcode],
					[LastUpdated],
					[Latitude],
					[Longitude],
					[IsActive]
					)
				VALUES (
					st.[Postcode],
					@dateNow,
					st.[Latitude],
					st.[Longitude],
					st.[IsActive]
					);

		BEGIN TRANSACTION T1

			-- swap updated Postode_Switch table with Postcode table
		
			ALTER TABLE [Address].[Postcode] SWITCH TO [Staging].[Postcode_Old];
			ALTER TABLE [Staging].[Postcode_Switch] SWITCH TO [Address].[Postcode];
			ALTER TABLE [Staging].[Postcode_Old] SWITCH TO [Staging].[Postcode_Switch];

		COMMIT
	END TRY

	BEGIN CATCH
		IF @@trancount > 0
			ROLLBACK ;
			THROW;
	END CATCH
END
  
  ");

        }

        public static void DropPostcodeLoadProcIfItExists(this MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
IF EXISTS ( SELECT * 
            FROM   sysobjects 
            WHERE  id = object_id(N'[Staging].[LoadPostcodesFromStagingTableAndSwitch]') 
                   and OBJECTPROPERTY(id, N'IsProcedure') = 1 )
BEGIN
	DROP PROCEDURE [Staging].[LoadPostcodesFromStagingTableAndSwitch]
END
  ");
        }
    }
}

