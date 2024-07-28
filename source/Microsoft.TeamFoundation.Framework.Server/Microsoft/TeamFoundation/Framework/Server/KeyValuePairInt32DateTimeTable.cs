// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KeyValuePairInt32DateTimeTable
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
  public sealed class KeyValuePairInt32DateTimeTable : 
    TeamFoundationTableValueParameter<KeyValuePair<int, DateTime>>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.Int),
      new SqlMetaData("Value", SqlDbType.DateTime)
    };

    public KeyValuePairInt32DateTimeTable(IEnumerable<KeyValuePair<int, DateTime>> rows)
      : base(rows, "typ_KeyValuePairInt32DateTimeTable", KeyValuePairInt32DateTimeTable.s_metadata)
    {
    }

    public override void SetRecord(KeyValuePair<int, DateTime> row, SqlDataRecord record)
    {
      record.SetInt32(0, row.Key);
      record.SetDateTime(1, row.Value);
    }
  }
}
