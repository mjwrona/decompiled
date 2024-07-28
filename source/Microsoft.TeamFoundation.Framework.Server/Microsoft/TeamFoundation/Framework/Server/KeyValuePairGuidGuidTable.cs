// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.KeyValuePairGuidGuidTable
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
  public sealed class KeyValuePairGuidGuidTable : 
    TeamFoundationTableValueParameter<KeyValuePair<Guid, Guid>>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
    {
      new SqlMetaData("Key", SqlDbType.UniqueIdentifier),
      new SqlMetaData("Value", SqlDbType.UniqueIdentifier)
    };

    public KeyValuePairGuidGuidTable(IEnumerable<KeyValuePair<Guid, Guid>> rows)
      : base(rows, "typ_KeyValuePairGuidGuidTable", KeyValuePairGuidGuidTable.s_metadata)
    {
    }

    public override void SetRecord(KeyValuePair<Guid, Guid> row, SqlDataRecord record)
    {
      record.SetGuid(0, row.Key);
      record.SetGuid(1, row.Value);
    }
  }
}
