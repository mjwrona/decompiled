// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.PermissionTable2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP
{
  public class PermissionTable2 : PermissionTable
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[6]
    {
      new SqlMetaData(nameof (DataspaceId), SqlDbType.Int),
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier),
      new SqlMetaData(nameof (SecurityToken), SqlDbType.NVarChar, -1L),
      new SqlMetaData(nameof (IndexableToken), SqlDbType.NVarChar, 350L),
      new SqlMetaData("AllowPermission", SqlDbType.Int),
      new SqlMetaData("DenyPermission", SqlDbType.Int)
    };
    private System.Func<string, int> m_dataspaceMapper;

    private int DataspaceId { get; set; }

    private string SecurityToken { get; set; }

    private string IndexableToken { get; set; }

    private char Separator { get; set; }

    public PermissionTable2(
      System.Func<string, int> dataspaceMapper,
      string token,
      char separator,
      IEnumerable<DalAccessControlEntry> permissions)
      : base(permissions, "typ_PermissionTable2", PermissionTable2.s_metadata)
    {
      ArgumentUtility.CheckForNull<System.Func<string, int>>(dataspaceMapper, nameof (dataspaceMapper));
      this.m_dataspaceMapper = dataspaceMapper;
      this.Separator = separator;
      if (string.IsNullOrEmpty(token))
        return;
      int num = 0;
      if (this.m_dataspaceMapper != null)
        num = this.m_dataspaceMapper(token);
      this.DataspaceId = num;
      this.SecurityToken = PermissionTable.AddSeparator(this.Separator, token);
      this.IndexableToken = PermissionTable.GetIndexableTokenFromToken(this.SecurityToken, this.Separator);
    }

    public PermissionTable2(
      System.Func<string, int> dataspaceMapper,
      char separator,
      IEnumerable<DalAccessControlEntry> permissions)
      : this(dataspaceMapper, (string) null, separator, permissions)
    {
    }

    public override void SetRecord(DalAccessControlEntry ace, SqlDataRecord record)
    {
      int num = this.DataspaceId;
      string token1 = this.SecurityToken;
      string str = this.IndexableToken;
      if (token1 == null)
      {
        string token2 = ace.Token;
        if (this.m_dataspaceMapper != null)
          num = this.m_dataspaceMapper(token2);
        token1 = PermissionTable.AddSeparator(this.Separator, token2);
        str = PermissionTable.GetIndexableTokenFromToken(token1, this.Separator);
      }
      record.SetInt32(0, num);
      record.SetGuid(1, ace.TeamFoundationId);
      record.SetString(2, token1);
      record.SetString(3, str);
      record.SetInt32(4, ace.Allow);
      record.SetInt32(5, ace.Deny);
    }
  }
}
