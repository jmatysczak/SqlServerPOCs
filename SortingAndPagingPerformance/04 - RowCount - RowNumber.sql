CHECKPOINT; DBCC DROPCLEANBUFFERS; DBCC FREEPROCCACHE;
SET STATISTICS IO ON
SET STATISTICS TIME ON

SELECT   *,
         irn + rn - 1 TotalRows
FROM     (
            SELECT   ROW_NUMBER() OVER (ORDER BY Title ASC) irn,
                     ROW_NUMBER() OVER (ORDER BY Title DESC) rn,
                     *
            FROM     Item
         ) dv
WHERE    rn BETWEEN 81 AND 100
ORDER BY Title DESC