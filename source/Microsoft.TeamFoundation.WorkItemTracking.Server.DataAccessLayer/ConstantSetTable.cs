// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.ConstantSetTable
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.SqlServer.Server;
using System.Collections.Generic;
using System.Data;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class ConstantSetTable : TempIdReferencingTable<ServerConstantSet>
  {
    private static readonly SqlMetaData[] s_metadata = new SqlMetaData[3]
    {
      new SqlMetaData("SetId", SqlDbType.Int),
      new SqlMetaData("ParentID", SqlDbType.Int),
      new SqlMetaData("ConstID", SqlDbType.Int)
    };

    public ConstantSetTable(IEnumerable<ServerConstantSet> entries)
      : base(entries, "typ_WitConstantSetTable", ConstantSetTable.s_metadata)
    {
    }

    public override void SetRecord(ServerConstantSet entry, SqlDataRecord record)
    {
      record.SetInt32(0, entry.SetId);
      record.SetInt32(1, this.TempIdOrRealId(entry.TempParentId, entry.ParentId));
      record.SetInt32(2, this.TempIdOrRealId(entry.TempConstId, entry.ConstId));
    }
  }
}
