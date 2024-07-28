// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem.WorkItemFieldCacheRefreshTask
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Search.Indexer;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.WorkItem
{
  internal class WorkItemFieldCacheRefreshTask : IIndexingPatchTask
  {
    public string Name { get; } = nameof (WorkItemFieldCacheRefreshTask);

    public void Patch(
      IndexingExecutionContext indexingExecutionContext,
      StringBuilder resultMessageBuilder)
    {
      indexingExecutionContext.RequestContext.GetService<ITeamFoundationSqlNotificationService>().SendNotification(indexingExecutionContext.RequestContext, Microsoft.VisualStudio.Services.Search.Common.SqlNotificationEventClasses.WITFieldDefinitionUpdated, Guid.NewGuid().ToString());
      indexingExecutionContext.Log.Append("Triggered SQL notification for updating WorkItemFieldCacheService.");
    }
  }
}
