// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Configuration.BulkCopyComponent
// Assembly: Microsoft.VisualStudio.Services.Configuration, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3AB461A1-8255-4EAB-B12B-E1D379571DC1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Configuration.dll

using System.Data.Common;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.Configuration
{
  public class BulkCopyComponent : DatabaseTransferComponent
  {
    public void BulkCopy(Table table, SqlDataReader reader)
    {
      this.ExecuteNonQuery("SELECT 1 WHERE 1 = 2");
      using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(this.Connection, SqlBulkCopyOptions.KeepIdentity | SqlBulkCopyOptions.TableLock | SqlBulkCopyOptions.KeepNulls, (SqlTransaction) null))
      {
        sqlBulkCopy.BatchSize = 20000;
        sqlBulkCopy.BulkCopyTimeout = 0;
        sqlBulkCopy.DestinationTableName = string.IsNullOrEmpty(table.Schema) ? table.Name : table.Schema + "." + table.Name;
        foreach (Column column in table.Columns)
        {
          if (!column.IsComputed)
            sqlBulkCopy.ColumnMappings.Add(column.Name, column.Name);
        }
        sqlBulkCopy.NotifyAfter = 10000;
        Stopwatch stopwatch = Stopwatch.StartNew();
        sqlBulkCopy.SqlRowsCopied += (SqlRowsCopiedEventHandler) ((sender, e) => this.Logger.Info("Table: {0}. {1} rows copied. Rows per second: {2:0}.", (object) table.Name, (object) e.RowsCopied, (object) ((double) e.RowsCopied * 1000.0 / (double) stopwatch.ElapsedMilliseconds)));
        sqlBulkCopy.WriteToServer((DbDataReader) reader);
      }
    }
  }
}
