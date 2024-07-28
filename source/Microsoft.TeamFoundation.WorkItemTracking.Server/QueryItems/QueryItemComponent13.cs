// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemComponent13
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  internal class QueryItemComponent13 : QueryItemComponent12
  {
    protected override WorkItemTrackingObjectBinder<QueryItemEntry> GetDrilldownQueryItemEntryBinder() => (WorkItemTrackingObjectBinder<QueryItemEntry>) new QueryItemComponent.DrilldownQueryDataBinder7((QueryItemComponent) this);

    public override IList<QueryItemEntry> FetchTopQueryItemsWithoutQueryType(int top = 1000)
    {
      this.PrepareStoredProcedure("prc_FetchTopQueryItemsWithoutQueryType");
      this.BindInt("@top", top);
      try
      {
        return (IList<QueryItemEntry>) this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader => new QueryItemComponent.QueryItemWiqlDataBinder().BindAll(reader))).ToList<QueryItemEntry>();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public override void UpdateQueryType(IList<QueryTypeInfo> queryTypeInfos)
    {
      this.PrepareStoredProcedure("prc_PopulateQueryItemsType");
      new QueryItemComponent.QueryTypeTable("@queryTypes", (IEnumerable<QueryTypeInfo>) queryTypeInfos).BindTable(this);
      this.ExecuteNonQuery();
    }
  }
}
