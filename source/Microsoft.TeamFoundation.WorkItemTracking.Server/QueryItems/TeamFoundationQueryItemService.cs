// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems.TeamFoundationQueryItemService
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.QueryItems
{
  public class TeamFoundationQueryItemService : ITeamFoundationQueryItemService, IVssFrameworkService
  {
    private ConcurrentDictionary<Guid, QueryItemSnapshot> m_queryCacheDictionary = new ConcurrentDictionary<Guid, QueryItemSnapshot>();
    private const string c_contributorGroupName = "Contributors";
    private const string c_readersGroupName = "Readers";
    private const string c_buildAdministratorsGroupName = "Build Administrators";
    private const string c_buildServiceIdentityRole = "Build";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public QueryItem GetQuery(
      IVssRequestContext requestContext,
      Guid projectId,
      string queryReference,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false)
    {
      Guid result;
      return Guid.TryParse(queryReference, out result) ? this.GetQueryById(requestContext, result, expandDepth, includeWiql, includeDeleted, includeExecutionInfo, new Guid?()) : this.GetQueryByPath(requestContext, projectId, queryReference, expandDepth, includeWiql, includeDeleted, includeExecutionInfo);
    }

    public IEnumerable<QueryItem> GetQueriesById(
      IVssRequestContext requestContext,
      IEnumerable<Guid> ids,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false,
      Guid? filterUnderProjectId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) ids, nameof (ids));
      return (IEnumerable<QueryItem>) requestContext.TraceBlock<List<QueryItem>>(902710, 902719, 902718, "Services", "QueryService", nameof (GetQueriesById), (Func<List<QueryItem>>) (() =>
      {
        IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
        IEnumerable<QueryItemEntry> itemEntriesByIds;
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          itemEntriesByIds = component.GetQueryItemEntriesByIds(ids, expandDepth, includeWiql, includeDeleted, configurationInfo.MaxQueryItemResultSizeforPartialHierarchy, includeExecutionInfo, filterUnderProjectId);
        IEnumerable<QueryItem> queries = QueryItem.Create(itemEntriesByIds);
        List<QueryItem> list = requestContext.GetService<ITeamFoundationQueryItemPermissionService>().FilterByReadPermission(requestContext, queries).ToList<QueryItem>();
        WiqlIdToNameTransformer.Transform(requestContext, (IEnumerable<QueryItem>) list);
        return list;
      }));
    }

    public virtual QueryItem GetQueryById(
      IVssRequestContext requestContext,
      Guid id,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false,
      Guid? filterUnderProjectId = null)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<QueryItem>(902710, 902719, 902718, "Services", "QueryService", nameof (GetQueryById), (Func<QueryItem>) (() =>
      {
        IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
        QueryItemEntry queryItemEntryById;
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          queryItemEntryById = component.GetQueryItemEntryById(id, expandDepth, includeWiql, includeDeleted, configurationInfo.MaxQueryItemResultSizeforPartialHierarchy, includeExecutionInfo, filterUnderProjectId);
        if (queryItemEntryById == null)
          this.ThrowQueryItemNotFoundExceptionForQueryId(id, filterUnderProjectId);
        QueryItem query = QueryItem.Create(queryItemEntryById);
        QueryItem queryById = requestContext.GetService<ITeamFoundationQueryItemPermissionService>().FilterByReadPermission(requestContext, query);
        if (queryById == null)
          this.ThrowQueryItemNotFoundExceptionForQueryId(id, filterUnderProjectId);
        WiqlIdToNameTransformer.Transform(requestContext, (IEnumerable<QueryItem>) new QueryItem[1]
        {
          queryById
        });
        return queryById;
      }));
    }

    public virtual QueryItem GetQueryByPath(
      IVssRequestContext requestContext,
      Guid projectId,
      string path,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrEmpty(path, nameof (path));
      return requestContext.TraceBlock<QueryItem>(902720, 902729, 902728, "Services", "QueryService", nameof (GetQueryByPath), (Func<QueryItem>) (() =>
      {
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        QueryItemEntry queryItemByPath;
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          queryItemByPath = component.GetQueryItemByPath(path, projectId, id, expandDepth, includeWiql, includeDeleted, includeExecutionInfo);
        QueryItem myQueriesFolder;
        if (queryItemByPath == null)
        {
          if (TFStringComparer.WorkItemQueryName.Compare(path, this.GetMyQueriesString(requestContext)) == 0)
          {
            myQueriesFolder = (QueryItem) this.CreateMyQueriesFolder(requestContext, projectId);
          }
          else
          {
            if (!TFStringComparer.WorkItemQueryName.Equals(path, this.GetSharedQueriesString(requestContext)))
              throw new QueryItemNotFoundException(path);
            myQueriesFolder = QueryItem.Create(this.CreatePublicQueryItemsHierarchy(requestContext, projectId));
          }
        }
        else
          myQueriesFolder = QueryItem.Create(queryItemByPath);
        QueryItem queryByPath = requestContext.GetService<ITeamFoundationQueryItemPermissionService>().FilterByReadPermission(requestContext, myQueriesFolder);
        if (queryByPath == null)
          throw new QueryItemNotFoundException(path);
        WiqlIdToNameTransformer.Transform(requestContext, (IEnumerable<QueryItem>) new QueryItem[1]
        {
          queryByPath
        });
        return queryByPath;
      }));
    }

    public virtual IEnumerable<QueryItem> GetRootQueries(
      IVssRequestContext requestContext,
      Guid projectId,
      int? expandDepth,
      bool includeWiql,
      bool includeDeleted = false,
      bool includeExecutionInfo = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IEnumerable<QueryItem>) requestContext.TraceBlock<List<QueryItem>>(902730, 902739, 902738, "Services", "QueryService", nameof (GetRootQueries), (Func<List<QueryItem>>) (() =>
      {
        requestContext.WitContext();
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
        IEnumerable<QueryItemEntry> queryItemEntries;
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          queryItemEntries = component.GetRootQueryItemEntries(projectId, id, expandDepth, includeWiql, includeDeleted, configurationInfo.MaxQueryItemResultSizeforPartialHierarchy, includeExecutionInfo);
        if (queryItemEntries == null || !queryItemEntries.Any<QueryItemEntry>())
          queryItemEntries = Enumerable.Repeat<QueryItemEntry>(this.CreatePublicQueryItemsHierarchy(requestContext, projectId), 1);
        IEnumerable<QueryItem> queryItems = QueryItem.Create(queryItemEntries);
        if (!queryItems.Any<QueryItem>((Func<QueryItem, bool>) (queryItem => queryItem.ParentId == Guid.Empty && !queryItem.IsPublic)))
        {
          QueryFolder myQueriesFolder = this.CreateMyQueriesFolder(requestContext, projectId);
          queryItems = queryItems.Concat<QueryItem>((IEnumerable<QueryItem>) Enumerable.Repeat<QueryFolder>(myQueriesFolder, 1));
        }
        List<QueryItem> list = requestContext.GetService<ITeamFoundationQueryItemPermissionService>().FilterByReadPermission(requestContext, queryItems).ToList<QueryItem>();
        WiqlIdToNameTransformer.Transform(requestContext, (IEnumerable<QueryItem>) list);
        return list;
      }));
    }

    public virtual IEnumerable<QueryItem> SearchQueries(
      IVssRequestContext requestContext,
      Guid projectId,
      bool includeWiql,
      string searchText,
      int maxCount,
      bool includeDeleted = false,
      bool includeExecutionInfo = false)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<string>(searchText, nameof (searchText));
      return (IEnumerable<QueryItem>) requestContext.TraceBlock<List<QueryItem>>(902730, 902739, 902738, "Services", "QueryService", nameof (SearchQueries), (Func<List<QueryItem>>) (() =>
      {
        requestContext.WitContext();
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        IEnumerable<QueryItemEntry> queryEntries;
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          queryEntries = (IEnumerable<QueryItemEntry>) component.SearchQueries(projectId, id, maxCount + 1, includeWiql, searchText, includeDeleted, includeExecutionInfo);
        IEnumerable<QueryItem> queries = QueryItem.Create(queryEntries);
        List<QueryItem> list = requestContext.GetService<ITeamFoundationQueryItemPermissionService>().FilterByReadPermission(requestContext, queries).ToList<QueryItem>();
        WiqlIdToNameTransformer.Transform(requestContext, (IEnumerable<QueryItem>) list);
        return list;
      }));
    }

    public virtual QueryItem UndeleteQueryItem(
      IVssRequestContext requestContext,
      Guid queryId,
      bool undeleteDescendants)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<QueryItem>(902740, 902749, 902748, "Services", "QueryService", nameof (UndeleteQueryItem), (Func<QueryItem>) (() =>
      {
        requestContext.WitContext();
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        QueryItem queryById = this.GetQueryById(requestContext, queryId, new int?(0), false, true, false, new Guid?());
        requestContext.GetService<ITeamFoundationQueryItemPermissionService>().CheckQueryPermission(requestContext, queryById, 4, new bool?(true));
        IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
        QueryItemEntry queryEntry;
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          queryEntry = component.UndeleteQueryItem(queryId, undeleteDescendants, id, configurationInfo.MaxQueryItemChildrenUnderParent);
        QueryItem queryItem = QueryItem.Create(queryEntry);
        WiqlIdToNameTransformer.Transform(requestContext, queryItem);
        return queryItem;
      }));
    }

    public virtual QueryFolder[] GetQueryHierarchy(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
      QueryFolder[] queryHierarchy = new QueryFolder[2];
      int forEntireHierarchy = configurationInfo.MaxQueryItemResultSizeForEntireHierarchy;
      int grossMaxResultCount = forEntireHierarchy;
      int totalQueryItems;
      queryHierarchy[0] = !WorkItemTrackingFeatureFlags.IsUsingQueryItemServiceNewAPI(requestContext) ? this.GetPublicQueriesFolderFromComponent(requestContext, projectId, forEntireHierarchy, out totalQueryItems) : this.GetPublicQueriesFolderFromCache(requestContext, projectId, forEntireHierarchy, out totalQueryItems);
      int maxResultCount = forEntireHierarchy - totalQueryItems;
      queryHierarchy[1] = this.GetPrivateQueriesFolder(requestContext, projectId, maxResultCount, grossMaxResultCount);
      return queryHierarchy;
    }

    public virtual IEnumerable<QueryItem> GetDeletedQueryItems(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxCount)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return (IEnumerable<QueryItem>) requestContext.TraceBlock<List<QueryItem>>(902792, 902793, 902794, "Services", "QueryService", nameof (GetDeletedQueryItems), (Func<List<QueryItem>>) (() =>
      {
        if (!requestContext.IsFeatureEnabled("WebAccess.WorkItemTracking.GetDeletedQueries"))
          return new List<QueryItem>();
        IEnumerable<QueryItemEntry> deletedQueryItems;
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          deletedQueryItems = (IEnumerable<QueryItemEntry>) component.GetDeletedQueryItems(projectId, maxCount);
        IEnumerable<QueryItem> queries = QueryItem.Create(deletedQueryItems);
        List<QueryItem> list = requestContext.GetService<ITeamFoundationQueryItemPermissionService>().FilterByReadPermission(requestContext, queries).ToList<QueryItem>();
        WiqlIdToNameTransformer.Transform(requestContext, (IEnumerable<QueryItem>) list);
        return list;
      }));
    }

    internal virtual QueryFolder GetPublicQueriesFolderFromComponent(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxResultCount,
      out int totalQueryItems)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      int tempTotalQueryItems = 0;
      QueryFolder folderFromComponent = requestContext.TraceBlock<QueryFolder>(902773, 902775, 902774, "Services", "QueryService", nameof (GetPublicQueriesFolderFromComponent), (Func<QueryFolder>) (() =>
      {
        long newwatermark = 0;
        return this.FilterAndReturnQueryItems(requestContext, projectId, QueryItem.Create(this.EnsurePublicQueryItemsHierarchy(requestContext, projectId, maxResultCount, out newwatermark, out tempTotalQueryItems)) as QueryFolder);
      }));
      totalQueryItems = tempTotalQueryItems;
      return folderFromComponent;
    }

    internal virtual QueryFolder GetPublicQueriesFolderFromCache(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxResultCount,
      out int totalQueryItems)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      int tempTotalQueryItems = 0;
      QueryFolder queriesFolderFromCache = requestContext.TraceBlock<QueryFolder>(902750, 902752, 902751, "Services", "QueryService", nameof (GetPublicQueriesFolderFromCache), (Func<QueryFolder>) (() =>
      {
        requestContext.WitContext();
        long queryItemsTimestamp = this.GetPublicQueryItemsTimestamp(requestContext, projectId);
        long currentWaterMark = 0;
        QueryItemSnapshot queryItemSnapshot = (QueryItemSnapshot) null;
        if (this.m_queryCacheDictionary.TryGetValue(projectId, out queryItemSnapshot))
        {
          currentWaterMark = queryItemSnapshot.Watermark;
          if (currentWaterMark >= queryItemsTimestamp)
          {
            requestContext.Trace(902757, TraceLevel.Info, "Services", "QueryService", projectId.ToString());
            maxResultCount = queryItemSnapshot.TotalQueryItemCount;
            return this.FilterAndReturnQueryItems(requestContext, projectId);
          }
        }
        requestContext.Trace(902758, TraceLevel.Info, "Services", "QueryService", projectId.ToString());
        if (queryItemSnapshot == null)
          this.InitializeSnapshot(requestContext, projectId, maxResultCount, out tempTotalQueryItems);
        else
          this.MergeUpdatesInSnapshot(requestContext, projectId, currentWaterMark, maxResultCount, out tempTotalQueryItems);
        return this.FilterAndReturnQueryItems(requestContext, projectId);
      }));
      totalQueryItems = tempTotalQueryItems;
      return queriesFolderFromCache;
    }

    protected virtual QueryFolder FilterAndReturnQueryItems(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      QueryItemSnapshot queryItemSnapshot = (QueryItemSnapshot) null;
      this.m_queryCacheDictionary.TryGetValue(projectId, out queryItemSnapshot);
      QueryFolder rootFolder = queryItemSnapshot.Root.Clone() as QueryFolder;
      return this.FilterAndReturnQueryItems(requestContext, projectId, rootFolder);
    }

    protected virtual QueryFolder FilterAndReturnQueryItems(
      IVssRequestContext requestContext,
      Guid projectId,
      QueryFolder rootFolder)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<QueryFolder>(rootFolder, nameof (rootFolder));
      QueryItem queryItem = requestContext.GetService<ITeamFoundationQueryItemPermissionService>().FilterByReadPermission(requestContext, (QueryItem) rootFolder);
      if (queryItem != null)
        WiqlIdToNameTransformer.Transform(requestContext, queryItem);
      return queryItem as QueryFolder;
    }

    private QueryItemEntry EnsurePublicQueryItemsHierarchy(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxResultCount,
      out long newwatermark,
      out int queryEntryCount)
    {
      QueryItemEntry queryItemsHierarchy;
      using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
        queryItemsHierarchy = component.GetPublicQueryItemsHierarchy(projectId, maxResultCount, out newwatermark, out queryEntryCount);
      if (queryItemsHierarchy == null)
      {
        using (PerformanceTimer.StartMeasure(requestContext, "TeamFoundationQueryItemService.CreatePublicQueryItemsHierarchy"))
          queryItemsHierarchy = this.CreatePublicQueryItemsHierarchy(requestContext, projectId);
      }
      return queryItemsHierarchy;
    }

    private QueryItemEntry CreatePublicQueryItemsHierarchy(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      QueryItemEntry sharedQuery = (QueryItemEntry) null;
      string sharedQueriesString = this.GetSharedQueriesString(requestContext);
      Guid id = requestContext.WitContext().RequestIdentity.Id;
      using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
        sharedQuery = component.CreateRootQueryFolder(projectId, id, sharedQueriesString, true);
      if (sharedQuery == null)
        throw new QueryItemNotFoundException(Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.SharedQueries());
      using (PerformanceTimer.StartMeasure(requestContext, "TeamFoundationQueryItemService.ProvisionSharedQueriesPermission"))
        this.ProvisionSharedQueriesPermission(requestContext, projectId, sharedQuery);
      return sharedQuery;
    }

    private void ProvisionSharedQueriesPermission(
      IVssRequestContext requestContext,
      Guid projectId,
      QueryItemEntry sharedQuery)
    {
      IdentityService service = requestContext.GetService<IdentityService>();
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, WitConstants.SecurityConstants.QueryItemSecurityNamespaceGuid);
      List<AccessControlEntry> source1 = new List<AccessControlEntry>();
      string token = string.Format("$/{0}/{1}", (object) projectId, (object) sharedQuery.Id);
      CustomerIntelligenceData properties = new CustomerIntelligenceData();
      Microsoft.VisualStudio.Services.Identity.Identity frameworkIdentity = IdentityHelper.GetFrameworkIdentity(requestContext, FrameworkIdentityType.ServiceIdentity, "Build", requestContext.ServiceHost.InstanceId.ToString("D"));
      if (frameworkIdentity != null)
        source1.Add(new AccessControlEntry(frameworkIdentity.Descriptor, WitConstants.SecurityConstants.QueryItemPermissions.Read, WitConstants.SecurityConstants.QueryItemPermissions.None));
      else
        properties.Add("Build", false);
      IdentityDescriptor descriptor1 = service.ReadIdentities(requestContext, IdentitySearchFilter.AccountName, this.GetProjectGroupSearchFactor(projectId, "Contributors"), QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>()?.Descriptor;
      if (descriptor1 != (IdentityDescriptor) null)
        source1.Add(new AccessControlEntry(descriptor1, WitConstants.SecurityConstants.QueryItemPermissions.Read, WitConstants.SecurityConstants.QueryItemPermissions.None));
      else
        properties.Add("Contributors", false);
      IdentityDescriptor descriptor2 = service.ReadIdentities(requestContext, IdentitySearchFilter.AccountName, this.GetProjectGroupSearchFactor(projectId, "Readers"), QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>()?.Descriptor;
      if (descriptor2 != (IdentityDescriptor) null)
        source1.Add(new AccessControlEntry(descriptor2, WitConstants.SecurityConstants.QueryItemPermissions.Read, WitConstants.SecurityConstants.QueryItemPermissions.None));
      else
        properties.Add("Contributors", false);
      IdentityDescriptor descriptor3 = service.ReadIdentities(requestContext, IdentitySearchFilter.AccountName, this.GetProjectGroupSearchFactor(projectId, "Build Administrators"), QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>()?.Descriptor;
      if (descriptor3 != (IdentityDescriptor) null)
        source1.Add(new AccessControlEntry(descriptor3, WitConstants.SecurityConstants.QueryItemPermissions.Read, WitConstants.SecurityConstants.QueryItemPermissions.None));
      else
        properties.Add("Contributors", false);
      IdentityDescriptor descriptor4 = service.ReadIdentities(requestContext, IdentitySearchFilter.AdministratorsGroup, ProjectInfo.GetProjectUri(projectId), QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>()?.Descriptor;
      if (descriptor4 != (IdentityDescriptor) null)
        source1.Add(new AccessControlEntry(descriptor4, WitConstants.SecurityConstants.QueryItemPermissions.Read | WitConstants.SecurityConstants.QueryItemPermissions.Contribute | WitConstants.SecurityConstants.QueryItemPermissions.Delete | WitConstants.SecurityConstants.QueryItemPermissions.ManagePermissions | WitConstants.SecurityConstants.QueryItemPermissions.FullControl, WitConstants.SecurityConstants.QueryItemPermissions.None));
      else
        properties.Add("Project Administrators", false);
      IdentityDescriptor administratorsGroup = GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup;
      source1.Add(new AccessControlEntry(administratorsGroup, WitConstants.SecurityConstants.QueryItemPermissions.Read | WitConstants.SecurityConstants.QueryItemPermissions.Contribute | WitConstants.SecurityConstants.QueryItemPermissions.Delete | WitConstants.SecurityConstants.QueryItemPermissions.ManagePermissions | WitConstants.SecurityConstants.QueryItemPermissions.FullControl, WitConstants.SecurityConstants.QueryItemPermissions.None));
      IEnumerable<IAccessControlEntry> source2 = securityNamespace.SetAccessControlEntries(requestContext, token, (IEnumerable<IAccessControlEntry>) source1, false, false);
      if (source2.Count<IAccessControlEntry>() != source1.Count)
      {
        properties.Add("ExpectedACEs", string.Join(",", source1.Select<AccessControlEntry, string>((Func<AccessControlEntry, string>) (ace => string.Format("{0}-{1}-{2}", (object) ace.Descriptor, (object) ace.Allow, (object) ace.Deny)))));
        properties.Add("ActualACEs", string.Join(",", source2.Select<IAccessControlEntry, string>((Func<IAccessControlEntry, string>) (entry => string.Format("{0}-{1}-{2}", (object) entry.Descriptor, (object) entry.Allow, (object) entry.Deny)))));
      }
      if (!properties.GetData().Any<KeyValuePair<string, object>>())
        return;
      requestContext.GetService<CustomerIntelligenceService>().Publish(requestContext, "Services", nameof (ProvisionSharedQueriesPermission), properties);
    }

    private string GetProjectGroupSearchFactor(Guid projectId, string groupName) => LinkingUtilities.EncodeUri(new ArtifactId("Classification", "TeamProject", projectId.ToString())) + "\\" + groupName;

    protected virtual void InitializeSnapshot(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxResultCount,
      out int totalQueryCount)
    {
      requestContext.Trace(902753, TraceLevel.Info, "Services", "QueryService", projectId.ToString());
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      int queryEntryCount = 0;
      long newwatermark;
      QueryItemEntry rootData = this.EnsurePublicQueryItemsHierarchy(requestContext, projectId, maxResultCount, out newwatermark, out queryEntryCount);
      QueryItemSnapshot queryItemSnapshot = this.CreateQueryItemSnapshot();
      using (new TraceWatch(requestContext, 902763, TraceLevel.Info, TimeSpan.MinValue, "Services", "QueryService", projectId.ToString(), Array.Empty<object>()))
        queryItemSnapshot.Initialize(rootData, newwatermark);
      totalQueryCount = queryItemSnapshot.TotalQueryItemCount;
      this.ReAssignQueryItemCache(requestContext, projectId, queryItemSnapshot);
      stopwatch.Stop();
      requestContext.Trace(902754, TraceLevel.Info, "Services", "QueryService", projectId.ToString() + "_" + stopwatch.ElapsedMilliseconds.ToString());
    }

    protected virtual void MergeUpdatesInSnapshot(
      IVssRequestContext requestContext,
      Guid projectId,
      long currentWaterMark,
      int maxResultCount,
      out int totalQueryCount)
    {
      requestContext.Trace(902755, TraceLevel.Info, "Services", "QueryService", projectId.ToString());
      Stopwatch stopwatch = new Stopwatch();
      stopwatch.Start();
      long newTimestamp;
      IEnumerable<QueryItemEntry> publicQueryItems;
      using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
        publicQueryItems = component.GetChangedPublicQueryItems(projectId, new long?(currentWaterMark), maxResultCount, out newTimestamp);
      QueryItemSnapshot queryItemSnapshot;
      this.m_queryCacheDictionary.TryGetValue(projectId, out queryItemSnapshot);
      QueryItemSnapshot newSnapshot;
      using (new TraceWatch(requestContext, 902764, TraceLevel.Info, TimeSpan.MinValue, "Services", "QueryService", projectId.ToString(), Array.Empty<object>()))
        newSnapshot = queryItemSnapshot.Clone() as QueryItemSnapshot;
      using (new TraceWatch(requestContext, 902765, TraceLevel.Info, TimeSpan.MinValue, "Services", "QueryService", "Count of QueryItems {0}", new object[1]
      {
        (object) publicQueryItems.Count<QueryItemEntry>()
      }))
        newSnapshot.Merge(publicQueryItems, newTimestamp);
      totalQueryCount = newSnapshot.TotalQueryItemCount;
      this.ReAssignQueryItemCache(requestContext, projectId, newSnapshot);
      stopwatch.Stop();
      requestContext.Trace(902756, TraceLevel.Info, "Services", "QueryService", projectId.ToString() + "_" + stopwatch.ElapsedMilliseconds.ToString());
    }

    internal virtual void ReAssignQueryItemCache(
      IVssRequestContext requestContext,
      Guid projectId,
      QueryItemSnapshot newSnapshot)
    {
      this.m_queryCacheDictionary.AddOrUpdate(projectId, newSnapshot, (Func<Guid, QueryItemSnapshot, QueryItemSnapshot>) ((k, v) => v == null || v.Watermark < newSnapshot.Watermark ? newSnapshot : v));
    }

    public void StripOutCurrentIterationTeamParameter(
      IVssRequestContext requestContext,
      QueryItem queryItem)
    {
      this.StripOutCurrentIterationTeamParameter(requestContext, (IEnumerable<QueryItem>) new QueryItem[1]
      {
        queryItem
      });
    }

    public void StripOutCurrentIterationTeamParameter(
      IVssRequestContext requestContext,
      IEnumerable<QueryItem> queryItems)
    {
      WalkHierarchy(new Action<Query>(RemoveParameter));

      void WalkHierarchy(Action<Query> visit)
      {
        Queue<QueryItem> source = new Queue<QueryItem>(queryItems);
        while (source.Any<QueryItem>())
        {
          QueryItem queryItem = source.Dequeue();
          if (queryItem is Query query && query.Wiql != null)
            visit(query);
          else if (queryItem is QueryFolder queryFolder && queryFolder.Children != null)
          {
            foreach (QueryItem child in (IEnumerable<QueryItem>) queryFolder.Children)
              source.Enqueue(child);
          }
        }
      }

      static void RemoveParameter(Query query)
      {
        try
        {
          NodeSelect syntax = Parser.ParseSyntax(query.Wiql);
          syntax.Walk((Action<Microsoft.TeamFoundation.WorkItemTracking.Client.Wiql.Node>) (n =>
          {
            if (!(n is NodeVariable nodeVariable2) || !string.Equals(nodeVariable2.Value, "currentIteration", StringComparison.OrdinalIgnoreCase))
              return;
            nodeVariable2.Parameters.Arguments.Clear();
          }));
          query.Wiql = syntax.ToString();
        }
        catch (SyntaxException ex)
        {
        }
      }
    }

    internal virtual IEnumerable<QueryItem> GetChangedPublicQueryItems(
      IVssRequestContext requestContext,
      Guid projectId,
      long? timestamp,
      int maxResultCount,
      out long newTimestamp)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ICollection<QueryItemEntry> list;
      using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
        list = (ICollection<QueryItemEntry>) component.GetChangedPublicQueryItems(projectId, timestamp, maxResultCount, out newTimestamp).ToList<QueryItemEntry>();
      IEnumerable<QueryItem> source = list.Select<QueryItemEntry, QueryItem>((Func<QueryItemEntry, QueryItem>) (qie => QueryItem.Create(qie)));
      ITeamFoundationQueryItemPermissionService queryPermissionService = requestContext.GetService<ITeamFoundationQueryItemPermissionService>();
      Func<QueryItem, bool> predicate = (Func<QueryItem, bool>) (qie => queryPermissionService.HasQueryPermission(requestContext, qie, 1));
      return source.Where<QueryItem>(predicate);
    }

    internal virtual long GetPublicQueryItemsTimestamp(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
        return component.GetPublicQueryItemsTimestamp(projectId);
    }

    internal virtual QueryItemSnapshot CreateQueryItemSnapshot() => new QueryItemSnapshot();

    internal virtual QueryFolder GetPrivateQueriesFolder(
      IVssRequestContext requestContext,
      Guid projectId,
      int maxResultCount,
      int grossMaxResultCount)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      return requestContext.TraceBlock<QueryFolder>(902760, 902762, 902761, "Services", "QueryService", nameof (GetPrivateQueriesFolder), (Func<QueryFolder>) (() =>
      {
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        QueryItemEntry queryItemsHierarchy;
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          queryItemsHierarchy = component.GetPrivateQueryItemsHierarchy(projectId, id, maxResultCount, grossMaxResultCount);
        QueryFolder myQueriesFolder;
        if (queryItemsHierarchy == null)
        {
          myQueriesFolder = this.CreateMyQueriesFolder(requestContext, projectId);
        }
        else
        {
          myQueriesFolder = QueryItem.Create(queryItemsHierarchy) as QueryFolder;
          WiqlIdToNameTransformer.Transform(requestContext, (QueryItem) myQueriesFolder);
        }
        return myQueriesFolder;
      }));
    }

    protected virtual QueryFolder CreateMyQueriesFolder(
      IVssRequestContext requestContext,
      Guid projectId)
    {
      return requestContext.TraceBlock<QueryFolder>(902770, 902771, 902772, "Services", "QueryService", nameof (CreateMyQueriesFolder), (Func<QueryFolder>) (() =>
      {
        string myQueriesString = this.GetMyQueriesString(requestContext);
        Guid id = requestContext.WitContext().RequestIdentity.Id;
        QueryItemEntry rootQueryFolder;
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          rootQueryFolder = component.CreateRootQueryFolder(projectId, id, myQueriesString, false);
        QueryItem queryItem = QueryItem.Create(rootQueryFolder);
        WiqlIdToNameTransformer.Transform(requestContext, queryItem);
        return queryItem as QueryFolder;
      }));
    }

    public string GetMyQueriesString(IVssRequestContext requestContext) => Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.Manager.GetString("MyQueries", requestContext.ServiceHost.GetCulture(requestContext));

    public string GetSharedQueriesString(IVssRequestContext requestContext) => Microsoft.TeamFoundation.WorkItemTracking.Server.ServerResources.Manager.GetString("SharedQueries", requestContext.ServiceHost.GetCulture(requestContext));

    public IEnumerable<QueryItem> GetQueriesExceedingMaxWiqlLength(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      IEnumerable<QueryItemEntry> queryEntries;
      return requestContext.TraceBlock<IEnumerable<QueryItem>>(902786, 902787, 902788, "Services", "QueryService", nameof (GetQueriesExceedingMaxWiqlLength), (Func<IEnumerable<QueryItem>>) (() =>
      {
        IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          queryEntries = component.GetQueriesExceedingMaxWiqlLength(configurationInfo.MaxWiqlTextLengthForDataImport);
        IEnumerable<QueryItem> queryItems = QueryItem.Create(queryEntries);
        WiqlIdToNameTransformer.Transform(requestContext, queryItems);
        return queryItems;
      }));
    }

    public void DeleteQueriesExceedingMaxWiqlLength(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      requestContext.TraceBlock(902789, 902790, 902791, "Services", "QueryService", nameof (DeleteQueriesExceedingMaxWiqlLength), (Action) (() =>
      {
        IWorkItemTrackingConfigurationInfo configurationInfo = requestContext.GetService<WorkItemTrackingConfigurationSettingService>().GetConfigurationInfo(requestContext);
        using (QueryItemComponent component = requestContext.CreateComponent<QueryItemComponent>())
          component.DeleteQueriesExceedingMaxWiqlLength(configurationInfo.MaxWiqlTextLengthForDataImport);
      }));
    }

    private void ThrowQueryItemNotFoundExceptionForQueryId(Guid queryId, Guid? projectId)
    {
      Guid? nullable = projectId.HasValue ? projectId : throw new QueryItemNotFoundException(queryId);
      Guid empty = Guid.Empty;
      if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
        throw new QueryItemNotFoundException(queryId, projectId.Value);
    }
  }
}
