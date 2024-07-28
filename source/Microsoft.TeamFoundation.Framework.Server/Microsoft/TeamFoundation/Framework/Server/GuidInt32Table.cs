// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GuidInt32Table
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
  public class GuidInt32Table : TeamFoundationTableValueParameter<Tuple<Guid, int>>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[2]
    {
      new SqlMetaData("a", SqlDbType.UniqueIdentifier),
      new SqlMetaData("b", SqlDbType.Int)
    };

    public GuidInt32Table(IEnumerable<Tuple<Guid, int>> values)
      : base(values, "typ_GuidInt32Table", GuidInt32Table.s_metadata)
    {
    }

    public override void SetRecord(Tuple<Guid, int> value, SqlDataRecord record)
    {
      record.SetGuid(0, value.Item1);
      record.SetInt32(1, value.Item2);
    }
  }
}
