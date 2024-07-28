// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KeyValuePairStringInt32Table
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.SqlServer.Server;
using System;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.Framework.Server
{
  [Obsolete]
  public sealed class KeyValuePairStringInt32Table : 
    TeamFoundationTableValueParameter<KeyValuePair<string, int>>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.NVarChar, -1L),
      new SqlMetaData("Value", SqlDbType.Int)
    };

    public KeyValuePairStringInt32Table(IEnumerable<KeyValuePair<string, int>> rows)
      : base(rows, "typ_KeyValuePairStringInt32Table", KeyValuePairStringInt32Table.s_metadata)
    {
    }

    public override void SetRecord(KeyValuePair<string, int> row, SqlDataRecord record)
    {
      record.SetString(0, row.Key);
      record.SetInt32(1, row.Value);
    }
  }
}
