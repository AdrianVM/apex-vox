CREATE PROCEDURE dbo.sp_add_table_to_policy(
	@rlsSchema sysname, @rlsPolicy sysname, 
	@rlsPredicateSchema sysname, @rlsPredicateName sysname,
	@targetScehma sysname, @targetTable sysname, @targetColName sysname,
	@forcePolicy bit = 0 )
AS
BEGIN
	IF( @forcePolicy = 0 )
	BEGIN
		IF( NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = object_id(quotename(@targetScehma) + N'.' + quotename(@targetTable)) AND name = @targetColName))
		BEGIN
			print 'Skipping Policy creation since the table does not include the target column'
			return;
		END
	END

	DECLARE @filterPredicate nvarchar(max);
	SET @filterPredicate = N'ADD FILTER PREDICATE ' + quotename(@rlsPredicateSchema) + N'.'+ quotename(@rlsPredicateName) + N'(' + quotename(@targetColName) + N')
	ON ' + quotename(@targetScehma) + N'.' + quotename(@targetTable);

	DECLARE @blockPredicate nvarchar(max);
	SET @blockPredicate = N'ADD BLOCK PREDICATE ' + quotename(@rlsPredicateSchema) + N'.'+ quotename(@rlsPredicateName) + N'(' + quotename(@targetColName) + N')
	ON ' + quotename(@targetScehma) + N'.' + quotename(@targetTable);

	DECLARE @cmd nvarchar(max);
	SET @cmd = N'ALTER SECURITY POLICY ' + quotename(@rlsSchema) + N'.' + quotename(@rlsPolicy) + 
	@filterPredicate + N',' +
	@blockPredicate + N';';

	EXECUTE( @cmd )
END
go