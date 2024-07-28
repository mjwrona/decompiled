// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.PermissionTable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP
{
  public class PermissionTable : WorkItemTrackingTableValueParameter<DalAccessControlEntry>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[5]
    {
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData(nameof (SecurityToken), SqlDbType.NVarChar, -1L),
      new SqlMetaData(nameof (IndexableToken), SqlDbType.NVarChar, 350L),
      new SqlMetaData("AllowPermission", SqlDbType.Int),
      new SqlMetaData("DenyPermission", SqlDbType.Int)
    };

    private string SecurityToken { get; set; }

    private string IndexableToken { get; set; }

    private char Separator { get; set; }

    public PermissionTable(
      string token,
      char separator,
      IEnumerable<DalAccessControlEntry> permissions)
      : base(permissions, "typ_PermissionTable", PermissionTable.s_metadata)
    {
      this.Separator = separator;
      if (string.IsNullOrEmpty(token))
        return;
      this.SecurityToken = PermissionTable.AddSeparator(this.Separator, token);
      this.IndexableToken = PermissionTable.GetIndexableTokenFromToken(this.SecurityToken, this.Separator);
    }

    public PermissionTable(char separator, IEnumerable<DalAccessControlEntry> permissions)
      : this((string) null, separator, permissions)
    {
    }

    protected PermissionTable(
      IEnumerable<DalAccessControlEntry> rows,
      string typeName,
      SqlMetaData[] metadata,
      bool omitNullEntries = false)
      : base(rows, typeName, metadata, omitNullEntries)
    {
    }

    public override void SetRecord(DalAccessControlEntry ace, SqlDataRecord record)
    {
      string token = this.SecurityToken;
      string str = this.IndexableToken;
      if (token == null)
      {
        token = PermissionTable.AddSeparator(this.Separator, ace.Token);
        str = PermissionTable.GetIndexableTokenFromToken(token, this.Separator);
      }
      record.SetGuid(0, ace.TeamFoundationId);
      record.SetString(1, token);
      record.SetString(2, str);
      record.SetInt32(3, ace.Allow);
      record.SetInt32(4, ace.Deny);
    }

    internal static string AddSeparator(char separator, string token)
    {
      if (separator != char.MinValue && (token.Length == 0 || (int) token[token.Length - 1] != (int) separator))
        token += (string) (object) separator;
      return token;
    }

    internal static string GetIndexableTokenFromToken(string token, char separator)
    {
      string indexableTokenFromToken = token;
      if (token.Length > 350)
        indexableTokenFromToken = PermissionTable.AddSeparator(separator, token.Substring(0, 350));
      return indexableTokenFromToken;
    }
  }
}
