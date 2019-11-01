CREATE PROCEDURE dbo.sp_drop_table_from_policy(
	@rlsSchema sysname, @rlsPolicy sysname, 
	@targetScehma sysname, @targetTable sysname)
AS
BEGIN
	DECLARE @filterPredicate nvarchar(max);
	SET @filterPredicate = N'DROP FILTER PREDICATE ON ' + quotename(@targetScehma) + N'.' + quotename(@targetTable);

	DECLARE @blockPredicate nvarchar(max);
	SET @blockPredicate = N'DROP BLOCK PREDICATE ON ' + quotename(@targetScehma) + N'.' + quotename(@targetTable);

	DECLARE @cmd nvarchar(max);
	SET @cmd = N'ALTER SECURITY POLICY ' + quotename(@rlsSchema) + N'.' + quotename(@rlsPolicy) + 
	@filterPredicate + N',' +
	@blockPredicate + N';';

	EXECUTE( @cmd )
END
GO