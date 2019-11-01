/*Create row level security schema*/
CREATE SCHEMA [rls]
go

/*Encode the access logic in a predicate function*/
CREATE FUNCTION rls.fn_predicate_TenantId(@TenantId bigint)
    RETURNS TABLE
    WITH SCHEMABINDING
AS
    RETURN SELECT 1 AS fn_accessResult 
		where convert(bigint,convert(varbinary(4),CONTEXT_INFO())) = @TenantId;
GO

/*Bind the function to our tables with a security policy*/
CREATE SECURITY POLICY rls.secpol_TenantId
GO