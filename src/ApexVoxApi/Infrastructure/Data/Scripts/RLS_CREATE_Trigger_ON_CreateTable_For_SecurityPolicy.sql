CREATE TRIGGER trig_apply_policy ON DATABASE 
AFTER CREATE_TABLE
AS
	DECLARE @schema sysname
	DECLARE @tableName sysname
	DECLARE @data xml
	-- Set the following bit to 1 to force new tables to include the target column and be included in teh policy
	DECLARE @forcePolicy bit = 1
	-- target column name for the filtering predicate
	DECLARE @targetColumnName sysname = 'tenantId';
	SET @data = EVENTDATA()
	SET @schema = @data.value('(/EVENT_INSTANCE/SchemaName)[1]', 'nvarchar(256)')
	SET @tableName = @data.value('(/EVENT_INSTANCE/ObjectName)[1]', 'nvarchar(256)')
	
	IF @tableName = '__EFMigrationsHistory' 
	BEGIN
		RETURN
	END

	IF (@tableName = 'Tenants' or @tableName = 'Users')
	BEGIN
		RETURN
	END

	BEGIN TRY
		EXEC [dbo].[sp_add_table_to_policy] 'rls', 'secpol_TenantId', 'rls', 'fn_predicate_TenantId', @schema, @tableName, @targetColumnName, @forcePolicy;
	END TRY
	BEGIN CATCH
		declare @err int = error_number()
		declare @msg nvarchar(256) = error_message()
		raiserror( N'Table "%s" cannot be added to policy, it requires to have a column named "%s" in order to apply the filter. Inner error Number: %s',
			12, 1, @tableName, @targetColumnName, @msg )
	END CATCH
go