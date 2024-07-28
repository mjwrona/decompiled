// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.QueryItemComponent7
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.DataAccess;
using System;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  internal class QueryItemComponent7 : QueryItemComponent6
  {
    protected override SqlParameter BindProjectId(Guid projectId) => this.BindInt("@dataspaceId", this.GetDataspaceId(projectId));

    protected override WorkItemTrackingObjectBinder<QueryItemEntry> GetDrilldownQueryItemEntryBinder() => (WorkItemTrackingObjectBinder<QueryItemEntry>) new QueryItemComponent.DrilldownQueryDataBinder4((QueryItemComponent) this);

    protected override WorkItemTrackingObjectBinder<QueryItemEntry> GetFullTreeQueryItemEntryBinder() => (WorkItemTrackingObjectBinder<QueryItemEntry>) new QueryItemComponent.FullTreeQueryDataBinder2((QueryItemComponent) this);
  }
}
