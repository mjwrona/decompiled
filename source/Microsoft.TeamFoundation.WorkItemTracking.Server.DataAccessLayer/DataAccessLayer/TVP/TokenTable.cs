// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP.TokenTable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.TVP
{
  public class TokenTable : WorkItemTrackingTableValueParameter<string>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[3]
    {
      new SqlMetaData("Token", SqlDbType.NVarChar, -1L),
      new SqlMetaData(nameof (Recurse), SqlDbType.Bit),
      new SqlMetaData("TeamFoundationId", SqlDbType.UniqueIdentifier)
    };

    private bool Recurse { get; set; }

    private char Separator { get; set; }

    public TokenTable(IEnumerable<string> tokens, bool recurse, char separator)
      : base(tokens, "typ_TokenTable", TokenTable.s_metadata)
    {
      this.Recurse = recurse;
      this.Separator = separator;
    }

    protected TokenTable(
      IEnumerable<string> rows,
      string typeName,
      SqlMetaData[] metadata,
      bool omitNullEntries = false)
      : base(rows, typeName, metadata, omitNullEntries)
    {
    }

    public override void SetRecord(string token, SqlDataRecord record)
    {
      record.SetString(0, PermissionTable.AddSeparator(this.Separator, token));
      record.SetBoolean(1, this.Recurse);
    }
  }
}
