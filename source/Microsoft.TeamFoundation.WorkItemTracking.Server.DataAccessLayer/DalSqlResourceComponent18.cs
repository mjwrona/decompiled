// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DalSqlResourceComponent18
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 815CF582-E66F-43C7-9B50-57E1C71BBC84
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccessLayer.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  internal class DalSqlResourceComponent18 : DalSqlResourceComponent17
  {
    protected override void PrepareGetWorkItemLinkChanges(
      Guid? projectId,
      long rowVersion,
      bool bypassPermissions,
      int batchSize,
      DateTime? createdDateWatermark,
      DateTime? removedDateWatermark)
    {
      base.PrepareGetWorkItemLinkChanges(projectId, rowVersion, bypassPermissions, batchSize, createdDateWatermark, removedDateWatermark);
      this.BindNullableDateTime("@createdDateWatermark", createdDateWatermark);
      this.BindNullableDateTime("@removedDateWatermark", removedDateWatermark);
    }

    public override Tuple<DateTime, DateTime> GetWorkItemLinkDateForTimestamp(long timestamp)
    {
      this.PrepareStoredProcedure("prc_GetWorkItemLinkDateForTimestamp");
      this.BindLong("@rowversion", timestamp);
      ResultCollection resultCollection = new ResultCollection(this.ExecuteReader(), this.ProcedureName, this.RequestContext);
      resultCollection.AddBinder<Tuple<DateTime, DateTime>>((ObjectBinder<Tuple<DateTime, DateTime>>) new DateTimeTupleBinder());
      return resultCollection.GetCurrent<Tuple<DateTime, DateTime>>().Items.First<Tuple<DateTime, DateTime>>();
    }
  }
}
