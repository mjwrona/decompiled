// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ContentMetadataTable
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
  internal static class ContentMetadataTable
  {
    private static readonly SqlMetaData[] typ_ContentMetadataTable2 = new SqlMetaData[4]
    {
      new SqlMetaData("ContentHash", SqlDbType.VarBinary, 20L),
      new SqlMetaData("FileServiceFileId", SqlDbType.Int),
      new SqlMetaData("NeedsCleanup", SqlDbType.Bit),
      new SqlMetaData("Flags", SqlDbType.TinyInt)
    };

    internal static SqlParameter Bind(
      this IEnumerable<ContentAccessMetadata> entries,
      CodeReviewComponent component,
      string parameterName)
    {
      entries = entries ?? Enumerable.Empty<ContentAccessMetadata>();
      return component.BindTable(parameterName, "CodeReview.typ_ContentMetadataTable2", ContentMetadataTable.BindChangeEntryRows(entries));
    }

    private static IEnumerable<SqlDataRecord> BindChangeEntryRows(
      IEnumerable<ContentAccessMetadata> entries)
    {
      foreach (ContentAccessMetadata entry in entries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ContentMetadataTable.typ_ContentMetadataTable2);
        byte[] contentHash = entry.ContentHash;
        sqlDataRecord.SetBytes(0, 0L, contentHash, 0, contentHash.Length);
        sqlDataRecord.SetInt32(1, entry.FileId);
        sqlDataRecord.SetByte(3, entry.Flags);
        yield return sqlDataRecord;
      }
    }
  }
}
