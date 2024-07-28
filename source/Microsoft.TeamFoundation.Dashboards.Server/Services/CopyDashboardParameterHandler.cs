// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.Services.CopyDashboardParameterHandler
// Assembly: Microsoft.TeamFoundation.Dashboards.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CC7F149F-95E9-4579-9C7B-BAEEA5A10ECA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Dashboards.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;

namespace Microsoft.TeamFoundation.Dashboards.Services
{
  public class CopyDashboardParameterHandler
  {
    public IDictionary<Guid, QueryHierarchyItem> m_oldToNewQueryMap;
    public WebApiTeam m_defaultOrDestinationTeam;
    public Guid? m_QueryFolderPathID;
    public string m_QueryFolderPath;
    public bool m_CopyQueriesFlag;
    private string m_FolderName;
    private QueryHierarchyItem m_QueryFolder;
    private const string c_QueryPathSeparator = "/";
    private const string c_QuerySepartor = " - ";
    private const int c_PrefixCharsForDashboardIdGuid = 5;
    private static readonly Regex c_QueryNameRegex = new Regex(" - ([a-f0-9]{" + 5.ToString() + "})", RegexOptions.Compiled);

    public CopyDashboardParameterHandler() => this.m_oldToNewQueryMap = (IDictionary<Guid, QueryHierarchyItem>) new Dictionary<Guid, QueryHierarchyItem>();

    public CopyDashboardParameterHandler(
      string dashboardName,
      Guid dashboardId,
      WebApiTeam defaultOrDestinationTeam,
      Guid? queryFolderPath = null,
      bool copyQueriesFlag = false)
    {
      this.m_FolderName = this.GetQueryFolderName(dashboardName, dashboardId);
      this.m_oldToNewQueryMap = (IDictionary<Guid, QueryHierarchyItem>) new Dictionary<Guid, QueryHierarchyItem>();
      this.m_CopyQueriesFlag = copyQueriesFlag;
      this.m_QueryFolderPathID = queryFolderPath;
      this.m_defaultOrDestinationTeam = defaultOrDestinationTeam;
    }

    public QueryHierarchyItem GetOrCreateQuery(
      IVssRequestContext requestContext,
      IDashboardConsumer sourceDashboardConsumer,
      IDashboardConsumer targetDashboardConsumer,
      Guid queryId)
    {
      WorkItemTrackingHttpClient client = requestContext.GetClient<WorkItemTrackingHttpClient>();
      if (this.m_CopyQueriesFlag)
      {
        if (this.m_QueryFolderPathID.HasValue)
        {
          try
          {
            WorkItemTrackingHttpClient trackingHttpClient = client;
            string project = targetDashboardConsumer.GetDataspaceId().ToString();
            string query = this.m_QueryFolderPathID.ToString();
            CancellationToken cancellationToken1 = requestContext.CancellationToken;
            QueryExpand? expand = new QueryExpand?(QueryExpand.Wiql);
            int? depth = new int?();
            bool? includeDeleted = new bool?();
            bool? useIsoDateFormat = new bool?();
            CancellationToken cancellationToken2 = cancellationToken1;
            this.m_QueryFolderPath = trackingHttpClient.GetQueryAsync(project, query, expand, depth, includeDeleted, useIsoDateFormat, (object) null, cancellationToken2).SyncResult<QueryHierarchyItem>().Path;
          }
          catch (Exception ex)
          {
            throw new Exception("The selected Folder does not exist, or you do not have permission to read it.");
          }
        }
      }
      else
        this.m_QueryFolderPath = string.Empty;
      if (this.m_oldToNewQueryMap.ContainsKey(queryId))
        return this.m_oldToNewQueryMap[queryId];
      if (this.m_QueryFolder == null || this.m_QueryFolder == null)
        this.m_QueryFolder = this.CreateFolder(client, targetDashboardConsumer, this.m_FolderName);
      WorkItemTrackingHttpClient trackingHttpClient1 = client;
      string project1 = sourceDashboardConsumer.GetDataspaceId().ToString();
      string query1 = queryId.ToString();
      CancellationToken cancellationToken3 = requestContext.CancellationToken;
      QueryExpand? expand1 = new QueryExpand?(QueryExpand.Wiql);
      int? depth1 = new int?();
      bool? includeDeleted1 = new bool?();
      bool? useIsoDateFormat1 = new bool?();
      CancellationToken cancellationToken4 = cancellationToken3;
      QueryHierarchyItem query2 = trackingHttpClient1.GetQueryAsync(project1, query1, expand1, depth1, includeDeleted1, useIsoDateFormat1, (object) null, cancellationToken4).SyncResult<QueryHierarchyItem>();
      QueryHierarchyItem query3 = client.CreateQueryAsync(this.PrepareNewQueryItem(query2, this.m_QueryFolder.Path, sourceDashboardConsumer, targetDashboardConsumer), targetDashboardConsumer.GetDataspaceId().ToString(), this.m_QueryFolder.Path).SyncResult<QueryHierarchyItem>();
      this.m_oldToNewQueryMap[queryId] = query3;
      return query3;
    }

    private QueryHierarchyItem CreateFolder(
      WorkItemTrackingHttpClient client,
      IDashboardConsumer targetDashboardConsumer,
      string queryFolder)
    {
      QueryHierarchyItem postedQuery = new QueryHierarchyItem()
      {
        IsFolder = new bool?(true),
        IsDeleted = false,
        Name = queryFolder,
        Path = (string.IsNullOrEmpty(this.m_QueryFolderPath) ? Microsoft.TeamFoundation.Dashboards.DashboardResources.SharedQueriesString() : this.m_QueryFolderPath) + "/" + queryFolder
      };
      return client.CreateQueryAsync(postedQuery, targetDashboardConsumer.GetDataspaceId().ToString(), string.IsNullOrEmpty(this.m_QueryFolderPath) ? Microsoft.TeamFoundation.Dashboards.DashboardResources.SharedQueriesString() : this.m_QueryFolderPath).SyncResult<QueryHierarchyItem>();
    }

    private QueryHierarchyItem PrepareNewQueryItem(
      QueryHierarchyItem query,
      string queryFolder,
      IDashboardConsumer sourceDashboardConsumer,
      IDashboardConsumer targetDashboardConsumer)
    {
      return new QueryHierarchyItem()
      {
        IsFolder = new bool?(false),
        Path = queryFolder,
        Name = this.GetQueryName(query.Name),
        IsPublic = new bool?(true),
        Id = new Guid(),
        Wiql = targetDashboardConsumer.GetScope() == DashboardScope.Project ? query.Wiql : query.Wiql.Replace(sourceDashboardConsumer.GetGroupId().ToString().ToLower(), targetDashboardConsumer.GetGroupId().ToString().ToLower())
      };
    }

    private string GetQueryFolderName(string dashboardName, Guid dashboardGuid) => dashboardName + " - " + dashboardGuid.ToString("N").Substring(0, 5);

    private string GetQueryName(string queryName)
    {
      if (!queryName.IsNullOrEmpty<char>() && queryName.Length > " - ".Length + 5 && CopyDashboardParameterHandler.c_QueryNameRegex.IsMatch(queryName.Substring(queryName.Length - " - ".Length - 5, " - ".Length + 5)))
        queryName = queryName.Substring(0, queryName.Length - " - ".Length - 5);
      return queryName + " - " + Guid.NewGuid().ToString("N").Substring(0, 5);
    }
  }
}
