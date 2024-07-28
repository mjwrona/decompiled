// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.WorkItemServiceHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi.Types;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Work.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Patch;
using Microsoft.VisualStudio.Services.WebApi.Patch.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class WorkItemServiceHelper : IWorkItemServiceHelper
  {
    private IVssRequestContext m_RequestContext;

    public WorkItemServiceHelper(IVssRequestContext requestContext) => this.m_RequestContext = requestContext;

    public IList<ProjectWorkItemStateColors> GetWorkItemStateColors(string[] projectNames)
    {
      try
      {
        return (IList<ProjectWorkItemStateColors>) this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemStateColorsAsync(projectNames).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (IList<ProjectWorkItemStateColors>) null;
    }

    public IList<WorkItem> GetWorkItems(
      IList<int> ids,
      IList<string> fields,
      WorkItemErrorPolicy errorPolicy)
    {
      try
      {
        if (ids.Where<int>((Func<int, bool>) (id => id > 0)).ToList<int>().Count > 0)
        {
          WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
          IList<int> ids1 = ids;
          IList<string> fields1 = fields;
          WorkItemErrorPolicy? nullable = new WorkItemErrorPolicy?(errorPolicy);
          DateTime? asOf = new DateTime?();
          WorkItemExpand? expand = new WorkItemExpand?();
          WorkItemErrorPolicy? errorPolicy1 = nullable;
          CancellationToken cancellationToken = new CancellationToken();
          return (IList<WorkItem>) this.getValidWorkItems(client.GetWorkItemsAsync((IEnumerable<int>) ids1, (IEnumerable<string>) fields1, asOf, expand, errorPolicy1, cancellationToken: cancellationToken).Result);
        }
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (IList<WorkItem>) new List<WorkItem>();
    }

    public IList<WorkItem> GetWorkItems(
      Guid projectId,
      IList<int> ids,
      IList<string> fields,
      WorkItemExpand expand,
      WorkItemErrorPolicy errorPolicy)
    {
      try
      {
        List<int> list = ids.Where<int>((Func<int, bool>) (id => id > 0)).ToList<int>();
        if (list.Count > 0)
        {
          WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
          Guid project = projectId;
          List<int> ids1 = list;
          IList<string> fields1 = fields;
          WorkItemErrorPolicy? nullable = new WorkItemErrorPolicy?(errorPolicy);
          DateTime? asOf = new DateTime?();
          WorkItemExpand? expand1 = new WorkItemExpand?();
          WorkItemErrorPolicy? errorPolicy1 = nullable;
          CancellationToken cancellationToken = new CancellationToken();
          return (IList<WorkItem>) this.getValidWorkItems(client.GetWorkItemsAsync(project, (IEnumerable<int>) ids1, (IEnumerable<string>) fields1, asOf, expand1, errorPolicy1, cancellationToken: cancellationToken).Result);
        }
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (IList<WorkItem>) new List<WorkItem>();
    }

    public IList<WorkItem> GetWorkItems(
      IList<int> ids,
      WorkItemExpand expand,
      WorkItemErrorPolicy errorPolicy)
    {
      try
      {
        List<int> list = ids.Where<int>((Func<int, bool>) (id => id > 0)).ToList<int>();
        if (list.Count > 0)
        {
          WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
          List<int> ids1 = list;
          WorkItemExpand? nullable1 = new WorkItemExpand?(expand);
          WorkItemErrorPolicy? nullable2 = new WorkItemErrorPolicy?(errorPolicy);
          DateTime? asOf = new DateTime?();
          WorkItemExpand? expand1 = nullable1;
          WorkItemErrorPolicy? errorPolicy1 = nullable2;
          CancellationToken cancellationToken = new CancellationToken();
          return (IList<WorkItem>) this.getValidWorkItems(client.GetWorkItemsAsync((IEnumerable<int>) ids1, asOf: asOf, expand: expand1, errorPolicy: errorPolicy1, cancellationToken: cancellationToken).Result);
        }
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (IList<WorkItem>) new List<WorkItem>();
    }

    public WorkItemQueryResult QueryByWiql(Guid projectId, string wiqlQuery, int? top = null)
    {
      try
      {
        WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
        Wiql wiql = new Wiql();
        wiql.Query = wiqlQuery;
        Guid project = projectId;
        int? nullable = top;
        bool? timePrecision = new bool?();
        int? top1 = nullable;
        CancellationToken cancellationToken = new CancellationToken();
        return client.QueryByWiqlAsync(wiql, project, timePrecision, top1, cancellationToken: cancellationToken).Result;
      }
      catch (Exception ex)
      {
        this.HandleQueryByWiqlError(wiqlQuery, ex);
      }
      return (WorkItemQueryResult) null;
    }

    public WorkItemQueryResult QueryByWiql(string wiqlQuery)
    {
      try
      {
        WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
        Wiql wiql = new Wiql();
        wiql.Query = wiqlQuery;
        bool? timePrecision = new bool?();
        int? top = new int?();
        CancellationToken cancellationToken = new CancellationToken();
        return client.QueryByWiqlAsync(wiql, timePrecision, top, cancellationToken: cancellationToken).Result;
      }
      catch (Exception ex)
      {
        this.HandleQueryByWiqlError(wiqlQuery, ex);
      }
      return (WorkItemQueryResult) null;
    }

    public void LinkArtifactToWorkItems(
      string artifactUri,
      Dictionary<string, object> attributes,
      IList<WorkItem> workItems)
    {
      try
      {
        WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
        List<WitBatchRequest> requests = new List<WitBatchRequest>();
        foreach (WorkItem workItem in (IEnumerable<WorkItem>) workItems)
        {
          JsonPatchDocument document = new JsonPatchDocument();
          JsonPatchDocument jsonPatchDocument = document;
          JsonPatchOperation jsonPatchOperation = new JsonPatchOperation();
          jsonPatchOperation.Path = WorkItemLinkPath.Relation;
          jsonPatchOperation.Operation = Operation.Add;
          WorkItemRelation workItemRelation = new WorkItemRelation();
          workItemRelation.Rel = WorkItemLinkTypes.ArtifactLink;
          workItemRelation.Url = artifactUri;
          workItemRelation.Attributes = (IDictionary<string, object>) attributes;
          jsonPatchOperation.Value = (object) workItemRelation;
          jsonPatchDocument.Add(jsonPatchOperation);
          if (workItem != null && workItem.Id.HasValue)
            requests.Add(client.CreateWorkItemBatchRequest(workItem.Id.Value, document, false, false));
        }
        List<WitBatchResponse> result = client.ExecuteBatchRequest((IEnumerable<WitBatchRequest>) requests).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
    }

    public void LinkArtifactsToWorkItem(
      List<string> artifactUris,
      Dictionary<string, object> attributes,
      int workItemId)
    {
      try
      {
        if (artifactUris == null || !artifactUris.Any<string>())
          return;
        WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
        List<WitBatchRequest> requests = new List<WitBatchRequest>();
        JsonPatchDocument document = new JsonPatchDocument();
        foreach (string artifactUri in artifactUris)
        {
          JsonPatchDocument jsonPatchDocument = document;
          JsonPatchOperation jsonPatchOperation = new JsonPatchOperation();
          jsonPatchOperation.Path = WorkItemLinkPath.Relation;
          jsonPatchOperation.Operation = Operation.Add;
          WorkItemRelation workItemRelation = new WorkItemRelation();
          workItemRelation.Rel = WorkItemLinkTypes.ArtifactLink;
          workItemRelation.Url = artifactUri;
          workItemRelation.Attributes = (IDictionary<string, object>) attributes;
          jsonPatchOperation.Value = (object) workItemRelation;
          jsonPatchDocument.Add(jsonPatchOperation);
        }
        requests.Add(client.CreateWorkItemBatchRequest(workItemId, document, false, false));
        List<WitBatchResponse> result = client.ExecuteBatchRequest((IEnumerable<WitBatchRequest>) requests).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
    }

    public void UnLinkArtifactsFromWorkItem(List<int> deletedIndices, int workItemId)
    {
      try
      {
        if (deletedIndices == null || !deletedIndices.Any<int>())
          return;
        WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
        List<WitBatchRequest> requests = new List<WitBatchRequest>();
        JsonPatchDocument document = new JsonPatchDocument();
        foreach (int deletedIndex in deletedIndices)
          document.Add(new JsonPatchOperation()
          {
            Path = string.Format(WorkItemLinkPath.RelationStringFormat, (object) deletedIndex),
            Operation = Operation.Remove
          });
        requests.Add(client.CreateWorkItemBatchRequest(workItemId, document, false, false));
        List<WitBatchResponse> result = client.ExecuteBatchRequest((IEnumerable<WitBatchRequest>) requests).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
    }

    public ArtifactUriQueryResult GetLinkedWorkItemIds(IList<string> artifactUris)
    {
      try
      {
        WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
        ArtifactUriQuery artifactUriQuery = new ArtifactUriQuery();
        artifactUriQuery.ArtifactUris = (IEnumerable<string>) artifactUris;
        CancellationToken cancellationToken = new CancellationToken();
        return client.QueryWorkItemsForArtifactUrisAsync(artifactUriQuery, cancellationToken: cancellationToken).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (ArtifactUriQueryResult) null;
    }

    public ArtifactUriQueryResult GetLinkedWorkItemIds(Guid projectId, IList<string> artifactUris)
    {
      try
      {
        WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>();
        ArtifactUriQuery artifactUriQuery = new ArtifactUriQuery();
        artifactUriQuery.ArtifactUris = (IEnumerable<string>) artifactUris;
        Guid project = projectId;
        CancellationToken cancellationToken = new CancellationToken();
        return client.QueryWorkItemsForArtifactUrisAsync(artifactUriQuery, project, (object) null, cancellationToken).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (ArtifactUriQueryResult) null;
    }

    public ArtifactUriQueryResult GetLinkedWorkItemIdsReadReplica(
      Guid projectId,
      IList<string> artifactUris)
    {
      try
      {
        WorkItemTrackingHttpClient client = this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>(this.m_RequestContext.GetHttpClientOptionsForEventualReadConsistencyLevel("TestManagement.Server.QueryWorkItemsForArtifactUris.SqlReadReplica"));
        ArtifactUriQuery artifactUriQuery = new ArtifactUriQuery();
        artifactUriQuery.ArtifactUris = (IEnumerable<string>) artifactUris;
        Guid project = projectId;
        CancellationToken cancellationToken = new CancellationToken();
        return client.QueryWorkItemsForArtifactUrisAsync(artifactUriQuery, project, (object) null, cancellationToken).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (ArtifactUriQueryResult) null;
    }

    public WorkItemTypeCategory GetWorkItemTypeCategory(Guid projectId, string category)
    {
      try
      {
        return this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemTypeCategoryAsync(projectId, category).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (WorkItemTypeCategory) null;
    }

    public TeamFieldValues GetTeamFieldValues(TeamContext teamContext)
    {
      try
      {
        return this.m_RequestContext.GetClient<WorkHttpClient>().GetTeamFieldValuesAsync(teamContext).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (TeamFieldValues) null;
    }

    public WorkItemField GetField(Guid projectId, string fieldRefName)
    {
      try
      {
        return (WorkItemField) this.m_RequestContext.GetClient<WorkItemTrackingHttpClient>().GetWorkItemFieldAsync(projectId, fieldRefName).Result;
      }
      catch (Exception ex)
      {
        this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      }
      return (WorkItemField) null;
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference GetWorkItemShallowReference(
      int workItemId,
      string title)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.ShallowReference()
      {
        Id = workItemId.ToString(),
        Name = title,
        Url = UrlBuildHelper.GetResourceUrl(this.m_RequestContext, ServiceInstanceTypes.TFS, "wit", WitConstants.WorkItemTrackingLocationIds.WorkItems, (object) new
        {
          id = workItemId
        })
      };
    }

    public Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference GetWorkItemRepresentation(
      int workItemId)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.WorkItemReference()
      {
        Id = workItemId.ToString(),
        Url = UrlBuildHelper.GetResourceUrl(this.m_RequestContext, ServiceInstanceTypes.TFS, "wit", WitConstants.WorkItemTrackingLocationIds.WorkItems, (object) new
        {
          id = workItemId
        })
      };
    }

    private List<WorkItem> getValidWorkItems(List<WorkItem> workItems)
    {
      bool hasInvalidWorkItems = false;
      if (workItems == null)
        return new List<WorkItem>();
      List<WorkItem> list = workItems.Where<WorkItem>((Func<WorkItem, bool>) (item =>
      {
        if (item != null || hasInvalidWorkItems)
          return true;
        hasInvalidWorkItems = true;
        return false;
      })).ToList<WorkItem>();
      if (!hasInvalidWorkItems)
        return list;
      this.m_RequestContext.Trace(1015095, TraceLevel.Warning, "TestResultsInsights", "RestLayer", "Invalid workitems are ignored");
      return list;
    }

    private void HandleQueryByWiqlError(string wiqlQuery, Exception ex)
    {
      this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", ex.ToString());
      if (wiqlQuery.Length > 2000)
        wiqlQuery = wiqlQuery.Substring(0, 2000);
      this.m_RequestContext.Trace(1015656, TraceLevel.Error, "TestManagement", "BusinessLayer", "WiqlQuery: " + wiqlQuery);
    }
  }
}
