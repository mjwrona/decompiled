// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.GuidTable
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
  public class GuidTable : TeamFoundationTableValueParameter<Guid>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[1]
    {
      new SqlMetaData("Id", SqlDbType.UniqueIdentifier)
    };

    public GuidTable(IEnumerable<Guid> guids)
      : base(guids, "typ_GuidTable", GuidTable.s_metadata)
    {
    }

    public override void SetRecord(Guid guid, SqlDataRecord record) => record.SetGuid(0, guid);
  }
}
