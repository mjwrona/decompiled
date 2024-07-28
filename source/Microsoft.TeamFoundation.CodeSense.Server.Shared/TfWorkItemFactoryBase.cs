// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.TfWorkItemFactoryBase
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.TeamFoundation.CodeSense.Platform.Abstraction;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public abstract class TfWorkItemFactoryBase
  {
    protected readonly IVssRequestContext requestContext;
    protected readonly ITreeDictionary treeSnapshot;
    protected readonly WorkItemTrackingTreeService treeDictionary;
    protected IEnumerable<int> requiredFields;
    private ITeamFoundationWorkItemService m_workItemService;

    protected TfWorkItemFactoryBase(IVssRequestContext requestContext, int[] requiredFields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      this.requestContext = requestContext;
      this.requiredFields = (IEnumerable<int>) requiredFields;
      this.treeDictionary = requestContext.GetService<WorkItemTrackingTreeService>();
      this.treeSnapshot = this.treeDictionary.GetSnapshot(requestContext);
      this.m_workItemService = requestContext.GetService<ITeamFoundationWorkItemService>();
    }

    protected IEnumerable<WorkItemFieldData> PageWorkItems(
      IEnumerable<int> workItemIds,
      DateTime? asOf)
    {
      using (new CodeSenseTraceWatch(this.requestContext, 1025820, TraceLayer.ExternalWorkitems, "Paging work items: {0}", new object[1]
      {
        (object) string.Join<int>(", ", workItemIds)
      }))
        return this.m_workItemService.GetWorkItemFieldValues(this.requestContext, workItemIds, this.requiredFields, 0);
    }

    protected int GetLatestAreaId(int workItemId, int areaId)
    {
      if (!this.treeSnapshot.LegacyTryGetTreeNode(areaId, out TreeNode _))
      {
        string format = string.Format("Work item {0} has deleted areaId ({1}).  Getting current areaId.", (object) workItemId, (object) areaId);
        using (new CodeSenseTraceWatch(this.requestContext, 1025815, TraceLayer.ExternalWorkitems, format, Array.Empty<object>()))
          areaId = this.m_workItemService.GetWorkItemFieldValues(this.requestContext, (IEnumerable<int>) new int[1]
          {
            workItemId
          }, (IEnumerable<int>) new int[1]{ -2 }, 0).First<WorkItemFieldData>().AreaId;
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.CodeSense.Server.Shared.PerformanceCounters.DeletedAreaIds").Increment();
      }
      return areaId;
    }
  }
}
