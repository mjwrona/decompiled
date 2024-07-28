// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestResultsWorkItemHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TestResultsWorkItemHelper : IWorkItemHelper
  {
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> GetWorkItemReference(
      IVssRequestContext context,
      List<int> workItemIds,
      bool skipPermissionCheck)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItemReference), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", string.Format("TestResultsWorkItemHelper.GetWorkItemReference"));
          List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> workItemReferenceList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>();
          IEnumerable<WorkItem> workItems;
          try
          {
            WorkItemTrackingHttpClient client = context.GetClient<WorkItemTrackingHttpClient>();
            List<string> stringList = new List<string>()
            {
              "System.Id",
              "System.Title",
              "System.WorkItemType"
            };
            List<int> ids = workItemIds;
            List<string> fields = stringList;
            DateTime? asOf = new DateTime?();
            WorkItemExpand? expand = new WorkItemExpand?();
            WorkItemErrorPolicy? errorPolicy = new WorkItemErrorPolicy?();
            CancellationToken cancellationToken = new CancellationToken();
            workItems = (IEnumerable<WorkItem>) client.GetWorkItemsAsync((IEnumerable<int>) ids, (IEnumerable<string>) fields, asOf, expand, errorPolicy, cancellationToken: cancellationToken).Result;
          }
          catch (Exception ex)
          {
            context.Trace(1015070, TraceLevel.Warning, "TestResultsInsights", "BusinessLayer", ex.ToString());
            workItems = (IEnumerable<WorkItem>) null;
          }
          return this.GetWorkItemReference(workItemIds, workItems);
        }
        finally
        {
          context.TraceLeave("BusinessLayer", string.Format("TestResultsWorkItemHelper.GetWorkItemReference"));
        }
      }
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> GetWorkItemReference(
      IVssRequestContext context,
      Guid projectId,
      List<int> workItemIds,
      bool skipPermissionCheck)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItemReference), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", string.Format("Project {0}: TestResultsWorkItemHelper.GetWorkItemReference", (object) projectId));
          IEnumerable<WorkItem> workItems;
          try
          {
            WorkItemTrackingHttpClient client = context.GetClient<WorkItemTrackingHttpClient>();
            List<string> stringList = new List<string>()
            {
              "System.Id",
              "System.Title",
              "System.WorkItemType"
            };
            Guid project = projectId;
            List<int> ids = workItemIds;
            List<string> fields = stringList;
            DateTime? asOf = new DateTime?();
            WorkItemExpand? expand = new WorkItemExpand?();
            WorkItemErrorPolicy? errorPolicy = new WorkItemErrorPolicy?();
            CancellationToken cancellationToken = new CancellationToken();
            workItems = (IEnumerable<WorkItem>) client.GetWorkItemsAsync(project, (IEnumerable<int>) ids, (IEnumerable<string>) fields, asOf, expand, errorPolicy, cancellationToken: cancellationToken).Result;
          }
          catch (Exception ex)
          {
            context.Trace(1015070, TraceLevel.Warning, "TestResultsInsights", "BusinessLayer", ex.ToString());
            workItems = (IEnumerable<WorkItem>) null;
          }
          return this.GetWorkItemReference(workItemIds, workItems);
        }
        finally
        {
          context.TraceLeave("BusinessLayer", string.Format("TestResultsWorkItemHelper.GetWorkItemReference"));
        }
      }
    }

    public IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext context,
      List<int> workItemIds,
      bool includeResourceLinks = false,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItems), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", string.Format("TestResultsWorkItemHelper.GetWorkItems"));
          WorkItemExpand workItemExpand = includeResourceLinks ? WorkItemExpand.Relations : WorkItemExpand.None;
          WorkItemTrackingHttpClient client = context.GetClient<WorkItemTrackingHttpClient>();
          List<int> ids = workItemIds;
          WorkItemExpand? nullable1 = new WorkItemExpand?(workItemExpand);
          WorkItemErrorPolicy? nullable2 = new WorkItemErrorPolicy?(errorPolicy);
          DateTime? asOf = new DateTime?();
          WorkItemExpand? expand = nullable1;
          WorkItemErrorPolicy? errorPolicy1 = nullable2;
          CancellationToken cancellationToken = new CancellationToken();
          return (IEnumerable<WorkItem>) client.GetWorkItemsAsync((IEnumerable<int>) ids, asOf: asOf, expand: expand, errorPolicy: errorPolicy1, cancellationToken: cancellationToken).Result;
        }
        finally
        {
          context.TraceLeave("BusinessLayer", string.Format("TestResultsWorkItemHelper.GetWorkItems"));
        }
      }
    }

    public IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext context,
      Guid projectId,
      List<int> workItemIds,
      bool includeResourceLinks = false,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (GetWorkItems), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", string.Format("Project {0}: TestResultsWorkItemHelper.GetWorkItems", (object) projectId));
          WorkItemExpand workItemExpand = includeResourceLinks ? WorkItemExpand.Relations : WorkItemExpand.None;
          WorkItemTrackingHttpClient client = context.GetClient<WorkItemTrackingHttpClient>();
          Guid project = projectId;
          List<int> ids = workItemIds;
          WorkItemExpand? nullable1 = new WorkItemExpand?(workItemExpand);
          WorkItemErrorPolicy? nullable2 = new WorkItemErrorPolicy?(errorPolicy);
          DateTime? asOf = new DateTime?();
          WorkItemExpand? expand = nullable1;
          WorkItemErrorPolicy? errorPolicy1 = nullable2;
          CancellationToken cancellationToken = new CancellationToken();
          return (IEnumerable<WorkItem>) client.GetWorkItemsAsync(project, (IEnumerable<int>) ids, asOf: asOf, expand: expand, errorPolicy: errorPolicy1, cancellationToken: cancellationToken).Result;
        }
        finally
        {
          context.TraceLeave("BusinessLayer", string.Format("TestResultsWorkItemHelper.GetWorkItems"));
        }
      }
    }

    public bool BelongsToRequirementCategory(
      IVssRequestContext context,
      GuidAndString projectId,
      int workItemId,
      bool skipPermissionCheck)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (BelongsToRequirementCategory), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", string.Format("TestResultsWorkItemHelper.BelongsToRequirementCategory"));
          WorkItem workItem = (WorkItem) null;
          try
          {
            WorkItemTrackingHttpClient client = context.GetClient<WorkItemTrackingHttpClient>();
            List<string> stringList = new List<string>()
            {
              "System.WorkItemType"
            };
            int id = workItemId;
            List<string> fields = stringList;
            DateTime? asOf = new DateTime?();
            WorkItemExpand? expand = new WorkItemExpand?();
            CancellationToken cancellationToken = new CancellationToken();
            workItem = client.GetWorkItemAsync(id, (IEnumerable<string>) fields, asOf, expand, cancellationToken: cancellationToken).Result;
          }
          catch (Exception ex)
          {
            context.Trace(1015070, TraceLevel.Warning, "TestResultsInsights", "BusinessLayer", ex.ToString());
          }
          return workItem != null && workItem.Fields["System.WorkItemType"] is string field && this.BelongsToCategory(context, projectId, WitCategoryRefName.Requirement, field);
        }
        finally
        {
          context.TraceLeave("BusinessLayer", string.Format("TestResultsWorkItemHelper.BelongsToRequirementCategory"));
        }
      }
    }

    public bool BelongsToCategory(
      IVssRequestContext context,
      GuidAndString projectId,
      string categoryName,
      string workItemTypeName)
    {
      using (PerfManager.Measure(context, "CrossService", TraceUtils.GetActionName(nameof (BelongsToCategory), "WorkItem")))
      {
        try
        {
          context.TraceEnter("BusinessLayer", string.Format("TestResultsWorkItemHelper.BelongsToCategory"));
          WorkItemTypeCategory result = context.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypeCategoryAsync(projectId.GuidId, categoryName).Result;
          return result != null && result.WorkItemTypes != null && result.WorkItemTypes.Any<WorkItemTypeReference>() && result.WorkItemTypes.Where<WorkItemTypeReference>((Func<WorkItemTypeReference, bool>) (wi => string.Equals(wi.Name, workItemTypeName, StringComparison.OrdinalIgnoreCase))).Any<WorkItemTypeReference>();
        }
        finally
        {
          context.TraceLeave("BusinessLayer", string.Format("TestResultsWorkItemHelper.BelongsToCategory"));
        }
      }
    }

    private List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> GetWorkItemReference(
      List<int> workItemIds,
      IEnumerable<WorkItem> workItems)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference> workItemReference = new List<Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference>();
      if (workItems != null && workItems.Any<WorkItem>())
      {
        foreach (int workItemId in workItemIds)
        {
          int id = workItemId;
          WorkItem workItem = workItems.Where<WorkItem>((Func<WorkItem, bool>) (wi =>
          {
            int num = id;
            int? id1 = wi.Id;
            int valueOrDefault = id1.GetValueOrDefault();
            return num == valueOrDefault & id1.HasValue;
          })).FirstOrDefault<WorkItem>();
          if (workItem != null)
            workItemReference.Add(new Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference()
            {
              Id = workItem.Id.ToString(),
              Name = workItem.Fields[WorkItemFieldRefNames.Title] as string,
              Type = workItem.Fields[WorkItemFieldRefNames.WorkItemType] as string
            });
        }
      }
      return workItemReference;
    }
  }
}
