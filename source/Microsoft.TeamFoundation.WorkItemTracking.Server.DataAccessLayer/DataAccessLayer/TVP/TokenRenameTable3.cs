// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.TokenRenameTable3
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP
{
  public class TokenRenameTable3 : WorkItemTrackingTableValueParameter<TokenRename>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[6]
    {
      new SqlMetaData("DataspaceId", SqlDbType.Int),
      new SqlMetaData("NewDataspaceId", SqlDbType.Int),
      new SqlMetaData("OldToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("NewToken", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Copy", SqlDbType.Bit),
      new SqlMetaData("Recurse", SqlDbType.Bit)
    };
    private readonly System.Func<string, int> m_dataspaceMapper;

    public TokenRenameTable3(System.Func<string, int> dataspaceMapper, IEnumerable<TokenRename> renames)
      : base(renames, "typ_TokenRenameTable3", TokenRenameTable3.s_metadata)
    {
      ArgumentUtility.CheckForNull<System.Func<string, int>>(dataspaceMapper, nameof (dataspaceMapper));
      this.m_dataspaceMapper = dataspaceMapper;
    }

    public override void SetRecord(TokenRename rename, SqlDataRecord record)
    {
      record.SetInt32(0, this.m_dataspaceMapper(rename.OldToken));
      record.SetInt32(1, this.m_dataspaceMapper(rename.NewToken));
      record.SetString(2, rename.OldToken);
      record.SetString(3, rename.NewToken);
      record.SetBoolean(4, rename.Copy);
      record.SetBoolean(5, rename.Recurse);
    }
  }
}
