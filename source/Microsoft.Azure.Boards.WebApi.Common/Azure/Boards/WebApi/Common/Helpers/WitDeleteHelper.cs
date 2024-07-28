// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Helpers.WitDeleteHelper
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.Azure.Boards.WebApi.Common.Converters;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.Boards.WebApi.Common.Helpers
{
  public static class WitDeleteHelper
  {
    public static readonly string[] WorkItemDeleteDefaultFields = new string[6]
    {
      "System.Id",
      "System.Title",
      "System.WorkItemType",
      "System.TeamProject",
      "System.ChangedBy",
      "System.ChangedDate"
    };

    public static WorkItemDelete GetWorkItemDeleteInternalResponse(
      IVssRequestContext requestContext,
      ITeamFoundationWorkItemService workItemService,
      WorkItemUpdateResult result,
      WorkItemRetrievalMode workItemRetrievalMode,
      bool returnProjectScopedUrl = true)
    {
      WorkItemDelete internalResponse = (WorkItemDelete) null;
      if (result != null)
      {
        if (result.Exception == null)
        {
          Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem = workItemService.GetWorkItems(requestContext, (IEnumerable<int>) new int[1]
          {
            result.Id
          }, workItemRetrievalMode: workItemRetrievalMode).FirstOrDefault<Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem>();
          internalResponse = WorkItemDeleteFactory.Create(requestContext.WitContext(), (WorkItemRevision) workItem, returnProjectScopedUrl);
        }
        else
          internalResponse = WorkItemDeleteFactory.Create(result.Id, result.Exception);
      }
      return internalResponse;
    }

    public static WorkItemDeleteBatch GetWorkItemDeleteInternalResponse(
      IEnumerable<WorkItemUpdateResult> result)
    {
      return new WorkItemDeleteBatch()
      {
        Results = result.Select<WorkItemUpdateResult, WorkItemDelete>((Func<WorkItemUpdateResult, WorkItemDelete>) (updateResult => WorkItemDeleteFactory.Create(updateResult.Id, updateResult.Exception)))
      };
    }

    public static WorkItemDelete GetWorkItemDeleteResponse(
      IVssRequestContext requestContext,
      ITeamFoundationWorkItemService workItemService,
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItem workItem,
      WorkItemUpdateResult result,
      WorkItemRetrievalMode workItemRetrievalMode,
      bool returnProjectScopedUrl = true)
    {
      WorkItemDelete itemDeleteResponse = (WorkItemDelete) null;
      if (result != null)
        itemDeleteResponse = result.Exception != null ? WorkItemDeleteFactory.Create(result.Id, result.Exception) : WorkItemDeleteFactory.Create(requestContext.WitContext(), (WorkItemRevision) workItem, returnProjectScopedUrl);
      return itemDeleteResponse;
    }

    public static IEnumerable<WorkItemDeleteReference> GetWorkItemDeleteReferencesInternalResponse(
      IVssRequestContext requestContext,
      ITeamFoundationWorkItemService workItemService,
      IEnumerable<WorkItemUpdateResult> results,
      WorkItemRetrievalMode workItemRetrievalMode,
      bool returnProjectScopedUrl = true)
    {
      IEnumerable<int> ints = results.Where<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (x => x.Exception == null)).Select<WorkItemUpdateResult, int>((Func<WorkItemUpdateResult, int>) (x => x.Id)).Distinct<int>();
      ITeamFoundationWorkItemService foundationWorkItemService = workItemService;
      IVssRequestContext requestContext1 = requestContext;
      IEnumerable<int> workItemIds = ints;
      string[] deleteDefaultFields = WitDeleteHelper.WorkItemDeleteDefaultFields;
      WorkItemRetrievalMode itemRetrievalMode = workItemRetrievalMode;
      DateTime? asOf = new DateTime?();
      int workItemRetrievalMode1 = (int) itemRetrievalMode;
      IEnumerable<WorkItemFieldData> workItemFieldValues = foundationWorkItemService.GetWorkItemFieldValues(requestContext1, workItemIds, (IEnumerable<string>) deleteDefaultFields, asOf: asOf, workItemRetrievalMode: (WorkItemRetrievalMode) workItemRetrievalMode1);
      List<WorkItemDeleteReference> internalResponse = new List<WorkItemDeleteReference>();
      foreach (WorkItemFieldData workItem in workItemFieldValues)
        internalResponse.Add(WorkItemDeleteReferenceFactory.Create(requestContext.WitContext(), workItem, returnProjectScopedUrl));
      foreach (WorkItemUpdateResult itemUpdateResult in results.Where<WorkItemUpdateResult>((Func<WorkItemUpdateResult, bool>) (x => x.Exception != null)))
        internalResponse.Add((WorkItemDeleteReference) WorkItemDeleteFactory.Create(itemUpdateResult.Id, itemUpdateResult.Exception));
      return (IEnumerable<WorkItemDeleteReference>) internalResponse;
    }

    public static void CheckIfNonTestWorkItem(IVssRequestContext requestContext, Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models.WorkItem workItem)
    {
      if (workItem == null)
        return;
      string workItemType = workItem.Fields["System.WorkItemType"].ToString();
      string projectName = workItem.Fields["System.TeamProject"].ToString();
      Guid projectId = requestContext.GetService<IProjectService>().GetProjectId(requestContext, projectName);
      if (CommonWITUtils.IsTestWorkItem(requestContext, projectId, workItemType))
        throw new InvalidDeleteWorkItemCallException(ResourceStrings.TestWorkItemsDeletionError());
    }
  }
}
