// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent14
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent14 : DalSqlResourceComponent13
  {
    public override void GetWorkItemIds(
      long rowVersion,
      bool fDestroyed,
      int batchSize,
      long offset,
      out ResultCollection rc)
    {
      try
      {
        if (fDestroyed)
        {
          offset = 0L;
          this.PrepareStoredProcedure("GetDestroyedWorkItemIds");
        }
        else
        {
          rowVersion = Math.Max(rowVersion - offset, 0L);
          this.PrepareStoredProcedure("GetChangedWorkItemIds");
        }
        this.BindLong("@rowVersion", rowVersion);
        this.BindInt("@batchSize", batchSize);
        IDataReader reader = this.ExecuteReader();
        rc = new ResultCollection(reader, this.ProcedureName, this.RequestContext);
        rc.AddBinder<WorkItemId>((ObjectBinder<WorkItemId>) new WorkItemIdBinder(offset));
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
        throw;
      }
    }
  }
}
