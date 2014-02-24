IF EXISTS (SELECT NULL FROM sysobjects WHERE name = 'PageQuery') DROP FUNCTION PageQuery;
IF EXISTS (SELECT NULL FROM sys.assemblies WHERE name = 'CLRFunctions') DROP ASSEMBLY CLRFunctions;
GO


CREATE ASSEMBLY CLRFunctions FROM
   'C:\MyData\PD\GitHub\SqlServerPOCs\SortingAndPagingPerformance\CLRFunctions\bin\Release\CLRFunctions.dll'
   WITH PERMISSION_SET = SAFE;
GO

CREATE FUNCTION PageQuery(
   @query         NVARCHAR(MAX),
   @rowsPerPage   INT,
   @page          INT,
   @asc           BIT
) RETURNS TABLE (
   Id    INT,
   count INT
) AS EXTERNAL NAME [CLRFunctions].[CLRFunctions.PriorityQueuePager].[PageQuery];
GO

sp_configure 'clr enabled', 1
GO

RECONFIGURE
GO


CHECKPOINT; DBCC DROPCLEANBUFFERS; DBCC FREEPROCCACHE;
SET STATISTICS IO ON
SET STATISTICS TIME ON

SELECT   Item.*,
         page_of_data.count
FROM     PageQuery('SELECT Id, Title FROM Item', 20, 5, 0) page_of_data
         INNER JOIN Item ON
            Item.Id = page_of_data.Id
ORDER BY Title DESC
