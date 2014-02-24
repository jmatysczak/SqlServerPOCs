using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using Microsoft.SqlServer.Server;

namespace CLRFunctions {
  public class PriorityQueuePager {
    [SqlFunction(DataAccess = DataAccessKind.Read, FillRowMethodName = "FillRow")]
    public static IEnumerable PageQuery(string query, int rowsPerPage, int page, bool asc) {
      const int COLUMN_INDEX_ID = 0,
                COLUMN_INDEX_SORT = 1;
      int maxRows = rowsPerPage * page,
          upperBound = maxRows - 1,
          totalRowCount = 0;
      IComparer<string> comparer = StringComparer.InvariantCulture;
      if(!asc) comparer = new DescendingComparer(comparer);
      SortedList<string, int> queue = new SortedList<string, int>(maxRows + 1, comparer);

      using(SqlConnection connection = new SqlConnection("context connection=true")) {
        connection.Open();

        using(SqlCommand command = new SqlCommand(query, connection)) {
          using(SqlDataReader reader = command.ExecuteReader()) {
            while(totalRowCount < maxRows && reader.Read()) {
              totalRowCount++;
              queue[reader.GetString(COLUMN_INDEX_SORT)] = reader.GetInt32(COLUMN_INDEX_ID);
            }

            IList<string> keys = queue.Keys;
            string maxSortData = keys[upperBound];
            while(reader.Read()) {
              totalRowCount++;
              string sortData = reader.GetString(COLUMN_INDEX_SORT);
              if(comparer.Compare(sortData, maxSortData) < 0) {
                queue[sortData] = reader.GetInt32(COLUMN_INDEX_ID);
                queue.RemoveAt(maxRows);
                maxSortData = keys[upperBound];
              }
            }
          }
        }
      }

      IList<int> values = queue.Values;
      List<KeyValuePair<int, int>> results = new List<KeyValuePair<int, int>>(rowsPerPage);
      for(int i = maxRows - rowsPerPage; i < maxRows; i++) {
        results.Add(new KeyValuePair<int, int>(values[i], totalRowCount));
      }

      return results;
    }

    public static void FillRow(object row, out SqlInt32 id, out SqlInt32 count) {
      KeyValuePair<int, int> pair = (KeyValuePair<int, int>)row;
      id = pair.Key;
      count = pair.Value;
    }

    class DescendingComparer : IComparer<string> {
      private IComparer<string> comparer;

      public DescendingComparer(IComparer<string> comparer) {
        this.comparer = comparer;
      }

      public int Compare(string x, string y) {
        return this.comparer.Compare(x, y) * -1;
      }
    }
  }
}