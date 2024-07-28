// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ContentHashTable
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal static class ContentHashTable
  {
    private static readonly SqlMetaData[] typ_ContentHashTable = new SqlMetaData[1]
    {
      new SqlMetaData("Val", SqlDbType.VarBinary, 20L)
    };

    internal static SqlParameter Bind(
      this IEnumerable<byte[]> entries,
      CodeReviewComponent component,
      string parameterName)
    {
      entries = entries ?? Enumerable.Empty<byte[]>();
      return component.BindTable(parameterName, "CodeReview.typ_ContentHashTable", ContentHashTable.BindContentHashRows(entries));
    }

    private static IEnumerable<SqlDataRecord> BindContentHashRows(IEnumerable<byte[]> entries)
    {
      foreach (byte[] entry in entries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ContentHashTable.typ_ContentHashTable);
        sqlDataRecord.SetBytes(0, 0L, entry, 0, entry.Length);
        yield return sqlDataRecord;
      }
    }
  }
}
