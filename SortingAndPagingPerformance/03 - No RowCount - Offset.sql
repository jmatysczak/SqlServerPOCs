CHECKPOINT; DBCC DROPCLEANBUFFERS; DBCC FREEPROCCACHE;
SET STATISTICS IO ON
SET STATISTICS TIME ON

SELECT   *
FROM     Item
ORDER BY Title DESC
OFFSET 80 ROWS FETCH FIRST 20 ROWS ONLY