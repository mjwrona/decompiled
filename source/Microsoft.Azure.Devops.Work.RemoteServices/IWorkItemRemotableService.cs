// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.IWorkItemRemotableService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  [DefaultServiceImplementation("Microsoft.Azure.Devops.Work.PlatformServices.PlatformWorkItemService, Microsoft.Azure.Devops.Work.PlatformServices")]
  public interface IWorkItemRemotableService : IVssFrameworkService
  {
    WorkItem GetWorkItemTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      string type,
      WorkItemExpand expand = WorkItemExpand.None);

    WorkItem GetWorkItemTemplate(
      IVssRequestContext requestContext,
      string projectName,
      string type,
      WorkItemExpand expand = WorkItemExpand.None);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      string projectName,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      Guid projectId,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail);

    IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail);

    WorkItem GetWorkItem(
      IVssRequestContext requestContext,
      int id,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None);

    WorkItem UpdateWorkItem(
      IVssRequestContext requestContext,
      int workItemId,
      JsonPatchDocument workItemPatchDocument,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false);

    IEnumerable<WitBatchResponse> CreateWorkItems(
      IVssRequestContext requestContext,
      string projectName,
      IList<CreateWitRequest> createWitRequests,
      bool bypassRules,
      bool suppressNotifications);

    IEnumerable<WitBatchResponse> UpdateWorkItems(
      IVssRequestContext requestContext,
      IList<UpdateWitRequest> updateWitRequests,
      bool bypassRules,
      bool suppressNotifications);

    WorkItem CreateWorkItem(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemtype,
      JsonPatchDocument workItemUpdate,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false);

    WorkItem CreateWorkItem(
      IVssRequestContext requestContext,
      string projectName,
      string workItemtype,
      JsonPatchDocument workItemUpdate,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false);

    IEnumerable<WorkItemDelete> DeleteWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds);

    WorkItemDelete DeleteWorkItem(IVssRequestContext requestContext, int id, bool destroy = false);
  }
}
