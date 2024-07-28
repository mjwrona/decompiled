// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ChangeEntryTable
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.SqlServer.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public static class ChangeEntryTable
  {
    private static readonly SqlMetaData[] typ_ChangeEntryTable5 = new SqlMetaData[9]
    {
      new SqlMetaData("BasePath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("BaseContentHash", SqlDbType.VarBinary, 20L),
      new SqlMetaData("BaseFlags", SqlDbType.TinyInt),
      new SqlMetaData("ModifiedPath", SqlDbType.NVarChar, -1L),
      new SqlMetaData("ModifiedContentHash", SqlDbType.VarBinary, 20L),
      new SqlMetaData("ModifiedFlags", SqlDbType.TinyInt),
      new SqlMetaData("ChangeType", SqlDbType.Int),
      new SqlMetaData("ChangeTrackingId", SqlDbType.Int),
      new SqlMetaData("ExtendedChangeType", SqlDbType.NVarChar, 30L)
    };

    public static SqlParameter Bind(
      this IEnumerable<ChangeEntry> entries,
      TeamFoundationSqlResourceComponent component,
      string parameterName)
    {
      entries = entries ?? Enumerable.Empty<ChangeEntry>();
      return component.BindTable(parameterName, "CodeReview.typ_ChangeEntryTable5", ChangeEntryTable.BindChangeEntryRows5(entries));
    }

    private static IEnumerable<SqlDataRecord> BindChangeEntryRows5(IEnumerable<ChangeEntry> entries)
    {
      foreach (ChangeEntry entry in entries)
      {
        SqlDataRecord sqlDataRecord = new SqlDataRecord(ChangeEntryTable.typ_ChangeEntryTable5);
        if (entry.Base != null && entry.Base.Path != null)
        {
          byte[] sha1HashBytes = ReviewFileContentExtensions.ToSha1HashBytes(entry.Base.SHA1Hash);
          sqlDataRecord.SetString(0, entry.Base.Path);
          sqlDataRecord.SetBytes(1, 0L, sha1HashBytes, 0, sha1HashBytes.Length);
          sqlDataRecord.SetByte(2, entry.Base.Flags);
        }
        if (entry.Modified != null && entry.Modified.Path != null)
        {
          byte[] sha1HashBytes = ReviewFileContentExtensions.ToSha1HashBytes(entry.Modified.SHA1Hash);
          sqlDataRecord.SetString(3, entry.Modified.Path);
          sqlDataRecord.SetBytes(4, 0L, sha1HashBytes, 0, sha1HashBytes.Length);
          sqlDataRecord.SetByte(5, entry.Modified.Flags);
        }
        sqlDataRecord.SetInt32(6, (int) entry.Type);
        sqlDataRecord.SetInt32(7, entry.ChangeTrackingId);
        if (entry.ExtendedChangeType != null)
          sqlDataRecord.SetString(8, entry.ExtendedChangeType);
        yield return sqlDataRecord;
      }
    }
  }
}
