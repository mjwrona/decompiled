// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Devops.Work.RemoteServices.WorkItemFrameworkService
// Assembly: Microsoft.Azure.Devops.Work.RemoteServices, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C97796CA-4166-42B2-B96F-9A166B07FF72
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Devops.Work.RemoteServices.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace Microsoft.Azure.Devops.Work.RemoteServices
{
  public class WorkItemFrameworkService : IWorkItemRemotableService, IVssFrameworkService
  {
    void IVssFrameworkService.ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    void IVssFrameworkService.ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public WorkItem GetWorkItemTemplate(
      IVssRequestContext requestContext,
      Guid projectId,
      string type,
      WorkItemExpand expand = WorkItemExpand.None)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      return requestContext.TraceBlock<WorkItem>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (GetWorkItemTemplate), (Func<WorkItem>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        Guid project = projectId;
        string type1 = type;
        WorkItemExpand? nullable = new WorkItemExpand?(expand);
        DateTime? asOf = new DateTime?();
        WorkItemExpand? expand1 = nullable;
        CancellationToken cancellationToken = new CancellationToken();
        return client.GetWorkItemTemplateAsync(project, type1, asOf: asOf, expand: expand1, cancellationToken: cancellationToken).Result;
      }));
    }

    public WorkItem GetWorkItemTemplate(
      IVssRequestContext requestContext,
      string projectName,
      string type,
      WorkItemExpand expand = WorkItemExpand.None)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      return requestContext.TraceBlock<WorkItem>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (GetWorkItemTemplate), (Func<WorkItem>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        string project = projectName;
        string type1 = type;
        WorkItemExpand? nullable = new WorkItemExpand?(expand);
        DateTime? asOf = new DateTime?();
        WorkItemExpand? expand1 = nullable;
        CancellationToken cancellationToken = new CancellationToken();
        return client.GetWorkItemTemplateAsync(project, type1, asOf: asOf, expand: expand1, cancellationToken: cancellationToken).Result;
      }));
    }

    public IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemIds, nameof (workItemIds));
      return requestContext.TraceBlock<IEnumerable<WorkItem>>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (GetWorkItems), (Func<IEnumerable<WorkItem>>) (() => this.GetAllPageWorkItems(requestContext, workItemIds, fields, asOf, new WorkItemExpand?(expand), errorPolicy)));
    }

    public IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      Guid projectId,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) workItemIds, nameof (workItemIds));
      return requestContext.TraceBlock<IEnumerable<WorkItem>>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (GetWorkItems), (Func<IEnumerable<WorkItem>>) (() => this.GetAllPageWorkItems(requestContext, workItemIds, fields, asOf, new Guid?(projectId), expand, errorPolicy)));
    }

    public IEnumerable<WorkItem> GetWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      string projectName,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      ArgumentUtility.CheckForNull<IEnumerable<int>>(workItemIds, nameof (workItemIds));
      return requestContext.TraceBlock<IEnumerable<WorkItem>>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (GetWorkItems), (Func<IEnumerable<WorkItem>>) (() => this.GetAllPageWorkItems(requestContext, workItemIds, fields, asOf, projectName, expand, errorPolicy)));
    }

    public WorkItem GetWorkItem(
      IVssRequestContext requestContext,
      int id,
      IEnumerable<string> fields = null,
      DateTime? asOf = null,
      WorkItemExpand expand = WorkItemExpand.None)
    {
      return requestContext.TraceBlock<WorkItem>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (GetWorkItem), (Func<WorkItem>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemAsync(id, fields, asOf, new WorkItemExpand?(expand)).Result));
    }

    public WorkItem UpdateWorkItem(
      IVssRequestContext requestContext,
      int workItemId,
      JsonPatchDocument workItemUpdate,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForNull<JsonPatchDocument>(workItemUpdate, nameof (workItemUpdate));
      return requestContext.TraceBlock<WorkItem>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (UpdateWorkItem), (Func<WorkItem>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().UpdateWorkItemAsync(workItemUpdate, workItemId, new bool?(validateOnly), new bool?(bypassRules), new bool?(suppressNotifications), new WorkItemExpand?(), (object) null, new CancellationToken()).Result));
    }

    public IEnumerable<WitBatchResponse> CreateWorkItems(
      IVssRequestContext requestContext,
      string projectName,
      IList<CreateWitRequest> createWitRequests,
      bool bypassRules,
      bool suppressNotifications)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) createWitRequests, nameof (createWitRequests));
      return (IEnumerable<WitBatchResponse>) requestContext.TraceBlock<List<WitBatchResponse>>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (CreateWorkItems), (Func<List<WitBatchResponse>>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        List<WitBatchRequest> requests = new List<WitBatchRequest>();
        foreach (CreateWitRequest createWitRequest in (IEnumerable<CreateWitRequest>) createWitRequests)
          requests.Add(client.CreateWorkItemBatchRequest(projectName, createWitRequest.TypeName, createWitRequest.document, bypassRules, suppressNotifications));
        return client.ExecuteBatchRequest((IEnumerable<WitBatchRequest>) requests).Result;
      }));
    }

    public IEnumerable<WitBatchResponse> UpdateWorkItems(
      IVssRequestContext requestContext,
      IList<UpdateWitRequest> updateWitRequests,
      bool bypassRules,
      bool suppressNotifications)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) updateWitRequests, nameof (updateWitRequests));
      return (IEnumerable<WitBatchResponse>) requestContext.TraceBlock<List<WitBatchResponse>>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), "CreateWorkItems", (Func<List<WitBatchResponse>>) (() =>
      {
        WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
        List<WitBatchRequest> requests = new List<WitBatchRequest>();
        foreach (UpdateWitRequest updateWitRequest in (IEnumerable<UpdateWitRequest>) updateWitRequests)
          requests.Add(client.CreateWorkItemBatchRequest(updateWitRequest.Id, updateWitRequest.document, bypassRules, suppressNotifications));
        return client.ExecuteBatchRequest((IEnumerable<WitBatchRequest>) requests).Result;
      }));
    }

    public WorkItem CreateWorkItem(
      IVssRequestContext requestContext,
      Guid projectId,
      string workItemType,
      JsonPatchDocument workItemUpdate,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckStringForNullOrEmpty(workItemType, nameof (workItemType));
      ArgumentUtility.CheckForNull<JsonPatchDocument>(workItemUpdate, nameof (workItemUpdate));
      return requestContext.TraceBlock<WorkItem>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (CreateWorkItem), (Func<WorkItem>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().CreateWorkItemAsync(workItemUpdate, projectId, workItemType, new bool?(validateOnly), new bool?(bypassRules), new bool?(suppressNotifications), new WorkItemExpand?(), (object) null, new CancellationToken()).Result));
    }

    public WorkItem CreateWorkItem(
      IVssRequestContext requestContext,
      string projectName,
      string workItemType,
      JsonPatchDocument workItemUpdate,
      bool validateOnly = false,
      bool bypassRules = false,
      bool suppressNotifications = false)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(projectName, nameof (projectName));
      ArgumentUtility.CheckStringForNullOrEmpty(workItemType, nameof (workItemType));
      ArgumentUtility.CheckForNull<JsonPatchDocument>(workItemUpdate, nameof (workItemUpdate));
      return requestContext.TraceBlock<WorkItem>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (CreateWorkItem), (Func<WorkItem>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().CreateWorkItemAsync(workItemUpdate, projectName, workItemType, new bool?(validateOnly), new bool?(bypassRules), new bool?(suppressNotifications), new WorkItemExpand?(), (object) null, new CancellationToken()).Result));
    }

    public IEnumerable<WorkItemDelete> DeleteWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds)
    {
      throw new NotImplementedException();
    }

    public WorkItemDelete DeleteWorkItem(IVssRequestContext requestContext, int id, bool destroy = false) => requestContext.TraceBlock<WorkItemDelete>(919600, 919601, 919602, "FrameworkServices", nameof (WorkItemFrameworkService), nameof (DeleteWorkItem), (Func<WorkItemDelete>) (() => requestContext.GetClient<WorkItemTrackingHttpClient>().DeleteWorkItemAsync(id, new bool?(destroy)).Result));

    private IEnumerable<WorkItem> GetAllPageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields,
      DateTime? asOf,
      Guid? projectId,
      WorkItemExpand expand = WorkItemExpand.None,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      int count = 200;
      IList<WorkItem> collection = (IList<WorkItem>) new List<WorkItem>();
      int num = 1;
      IEnumerable<int> ints = (IEnumerable<int>) new List<int>();
      WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
      for (IEnumerable<int> source = workItemIds != null ? workItemIds.Skip<int>((num - 1) * count).Take<int>(count) : (IEnumerable<int>) null; source.Count<int>() > 0; source = workItemIds != null ? workItemIds.Skip<int>((num - 1) * count).Take<int>(count) : (IEnumerable<int>) null)
      {
        collection.AddRange<WorkItem, IList<WorkItem>>((IEnumerable<WorkItem>) client.GetWorkItemsAsync(projectId.Value, workItemIds, fields, asOf, new WorkItemExpand?(expand), new WorkItemErrorPolicy?(errorPolicy)).Result);
        ++num;
      }
      return (IEnumerable<WorkItem>) collection;
    }

    private IEnumerable<WorkItem> GetAllPageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields,
      DateTime? asOf,
      string projectName = null,
      WorkItemExpand expand = WorkItemExpand.None,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      int count = 200;
      IList<WorkItem> collection = (IList<WorkItem>) new List<WorkItem>();
      int num = 1;
      IEnumerable<int> ints = (IEnumerable<int>) new List<int>();
      WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
      for (IEnumerable<int> source = workItemIds != null ? workItemIds.Skip<int>((num - 1) * count).Take<int>(count) : (IEnumerable<int>) null; source.Count<int>() > 0; source = workItemIds != null ? workItemIds.Skip<int>((num - 1) * count).Take<int>(count) : (IEnumerable<int>) null)
      {
        collection.AddRange<WorkItem, IList<WorkItem>>((IEnumerable<WorkItem>) client.GetWorkItemsAsync(projectName, workItemIds, fields, asOf, new WorkItemExpand?(expand), new WorkItemErrorPolicy?(errorPolicy)).Result);
        ++num;
      }
      return (IEnumerable<WorkItem>) collection;
    }

    private IEnumerable<WorkItem> GetAllPageWorkItems(
      IVssRequestContext requestContext,
      IEnumerable<int> workItemIds,
      IEnumerable<string> fields,
      DateTime? asOf,
      WorkItemExpand? expand = WorkItemExpand.None,
      WorkItemErrorPolicy errorPolicy = WorkItemErrorPolicy.Fail)
    {
      int count = 200;
      IList<WorkItem> collection = (IList<WorkItem>) new List<WorkItem>();
      int num = 1;
      IEnumerable<int> ints = (IEnumerable<int>) new List<int>();
      WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
      for (IEnumerable<int> source = workItemIds != null ? workItemIds.Skip<int>((num - 1) * count).Take<int>(count) : (IEnumerable<int>) null; source.Count<int>() > 0; source = workItemIds != null ? workItemIds.Skip<int>((num - 1) * count).Take<int>(count) : (IEnumerable<int>) null)
      {
        collection.AddRange<WorkItem, IList<WorkItem>>((IEnumerable<WorkItem>) client.GetWorkItemsAsync(workItemIds, fields, asOf, expand, new WorkItemErrorPolicy?(errorPolicy)).Result);
        ++num;
      }
      return (IEnumerable<WorkItem>) collection;
    }
  }
}
