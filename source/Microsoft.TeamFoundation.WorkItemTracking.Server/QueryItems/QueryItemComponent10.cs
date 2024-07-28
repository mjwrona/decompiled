// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemComponent10
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  internal class QueryItemComponent10 : QueryItemComponent9
  {
    public override IEnumerable<QueryItemEntry> GetQueriesExceedingMaxWiqlLength(int maxWiqlLength)
    {
      this.PrepareStoredProcedure("prc_GetQueriesExceedingMaxWiqlLength");
      this.BindInt("@maxWiqlLength", maxWiqlLength);
      try
      {
        return (IEnumerable<QueryItemEntry>) this.ExecuteUnknown<IEnumerable<QueryItemEntry>>((System.Func<IDataReader, IEnumerable<QueryItemEntry>>) (reader => this.GetQueryItemDataBinder().BindAll(reader))).ToList<QueryItemEntry>();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }

    public override void DeleteQueriesExceedingMaxWiqlLength(int maxWiqlLength)
    {
      this.PrepareStoredProcedure("prc_DeleteQueriesExceedingMaxWiqlLength");
      this.BindInt("@maxWiqlLength", maxWiqlLength);
      try
      {
        this.ExecuteNonQuery();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }
  }
}
