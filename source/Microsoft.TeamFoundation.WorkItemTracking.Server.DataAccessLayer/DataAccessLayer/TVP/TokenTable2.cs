// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.TokenTable2
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP
{
  public class TokenTable2 : TokenTable
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[4]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("Token", SqlDbType.NVarChar, -1L),
      new SqlMetaData(nameof (Recurse), SqlDbType.Bit),
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier)
    };
    private System.Func<string, int> m_dataspaceMapper;

    private bool Recurse { get; set; }

    private char Separator { get; set; }

    public TokenTable2(
      System.Func<string, int> dataspaceMapper,
      IEnumerable<string> tokens,
      bool recurse,
      char separator)
      : base(tokens, "typ_TokenTable2", TokenTable2.s_metadata)
    {
      ArgumentUtility.CheckForNull<System.Func<string, int>>(dataspaceMapper, nameof (dataspaceMapper));
      this.m_dataspaceMapper = dataspaceMapper;
      this.Recurse = recurse;
      this.Separator = separator;
    }

    public override void SetRecord(string token, SqlDataRecord record)
    {
      int num = 0;
      if (this.m_dataspaceMapper != null)
        num = this.m_dataspaceMapper(token);
      record.SetInt32(0, num);
      record.SetString(1, PermissionTable.AddSeparator(this.Separator, token));
      record.SetBoolean(2, this.Recurse);
    }
  }
}
