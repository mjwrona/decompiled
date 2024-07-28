// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService.WorkItemSecurityChecksService
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Common.Entities.EntityProperties;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Server.WorkItemFieldCache;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using Microsoft.VisualStudio.Services.Search.Shared.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem;
using Microsoft.VisualStudio.Services.Search.WebServer;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService
{
  public class WorkItemSecurityChecksService : 
    IWorkItemSecurityChecksService,
    ISecurityChecksService,
    IVssFrameworkService,
    IDisposable
  {
    internal Dictionary<byte[], List<ClassificationNode>> AreaSecuritySets;
    private readonly Guid m_searchSecurityNamespaceGuid = new Guid("ca535e7e-67ce-457f-93fe-6e53aa4e4160");
    internal Dictionary<ProjectAdminIdentity, List<ClassificationNode>> ProjectToAreaSecuritySets;
    internal WorkItemSecurityChecksCache WorkItemSecurityChecksCache;
    private readonly ReaderWriterLockSlim m_areaSecuritySetsLock;
    private TeamFoundationTask m_securitySetsRebuildTask;
    private CountdownEvent m_securitySetsRebuildTaskEvent;
    private int m_totalAreas;
    private IVssSecurityNamespace m_workItemSecurityNamespace;
    private readonly object m_workItemSecurityNamespaceLock;
    private IVssSecurityNamespace m_searchSecurityNamespace;
    private readonly object m_searchSecurityNamespaceLock;
    private readonly IDataAccessFactory m_dataAccessFactoryInstance;
    private int m_numberOfAreasToCheckInASecuritySet;
    private bool m_disposedValue;

    public WorkItemSecurityChecksService()
      : this(DataAccessFactory.GetInstance())
    {
    }

    internal WorkItemSecurityChecksService(IDataAccessFactory dataAccessFactory)
    {
      this.m_areaSecuritySetsLock = new ReaderWriterLockSlim();
      this.AreaSecuritySets = new Dictionary<byte[], List<ClassificationNode>>((IEqualityComparer<byte[]>) new ByteArrayComparer());
      this.ProjectToAreaSecuritySets = new Dictionary<ProjectAdminIdentity, List<ClassificationNode>>();
      this.m_workItemSecurityNamespaceLock = new object();
      this.m_searchSecurityNamespaceLock = new object();
      this.m_dataAccessFactoryInstance = dataAccessFactory;
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_numberOfAreasToCheckInASecuritySet = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/NumberOfRepositoriesToCheckInASecuritySet") : throw new UnsupportedHostTypeException(systemRequestContext.ServiceHost.HostType);
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SecuritySetsRebuildIntervalInSec"));
      this.m_securitySetsRebuildTaskEvent = new CountdownEvent(1);
      this.m_securitySetsRebuildTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.BuildSecuritySetsCallback), (object) null, (int) timeSpan.TotalMilliseconds);
      IVssRequestContext vssRequestContext = systemRequestContext;
      IVssRequestContext context = systemRequestContext.To(TeamFoundationHostType.Deployment);
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.LoadCssNodeSecurityNamespace(vssRequestContext);
      this.LoadSearchSecurityNamespace(vssRequestContext);
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("SecurityNamespaceLoadTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      if (this.WorkItemSecurityChecksCache == null)
        this.WorkItemSecurityChecksCache = new WorkItemSecurityChecksCache(systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/WorkItemSecurityChecksCacheMaxSize"), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/WorkItemSecurityChecksCacheExpirationInMin")), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/WorkItemSecurityChecksCacheCleanupTaskIntervalInMin")), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/WorkItemSecurityChecksCacheInactivityExpirationInMin")));
      bool isCacheFeatureEnabled = false;
      this.BuildAreaSecuritySets(vssRequestContext, isCacheFeatureEnabled, true);
      if (this.m_securitySetsRebuildTask != null)
        context.GetService<ITeamFoundationTaskService>().AddTask(vssRequestContext, this.m_securitySetsRebuildTask);
      this.WorkItemSecurityChecksCache.Initialize(systemRequestContext);
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(systemRequestContext));
      try
      {
        IVssRequestContext context = systemRequestContext.To(TeamFoundationHostType.Deployment);
        IVssRequestContext requestContext = systemRequestContext;
        if (this.m_securitySetsRebuildTask != null)
          context.GetService<ITeamFoundationTaskService>().RemoveTask(requestContext, this.m_securitySetsRebuildTask);
        this.m_securitySetsRebuildTaskEvent.Signal();
        this.m_securitySetsRebuildTaskEvent.Wait();
        this.m_securitySetsRebuildTaskEvent.Dispose();
        this.WorkItemSecurityChecksCache.TearDown(systemRequestContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    public IEnumerable<ClassificationNode> GetUserAccessibleAreas(
      IVssRequestContext requestContext,
      out bool allAreasAreAccessible)
    {
      return this.GetUserAccessibleAreas(requestContext, (string) null, out allAreasAreAccessible);
    }

    private bool IsCollectionAdmin(IVssRequestContext requestContext)
    {
      IdentityDescriptor userContext = requestContext.UserContext;
      return requestContext.GetService<IdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, userContext);
    }

    private IList<ClassificationNode> GetAreasWhereUserIsProjectAdmin(
      IVssRequestContext userRequestContext)
    {
      IdentityService service = userRequestContext.GetService<IdentityService>();
      List<ClassificationNode> userIsProjectAdmin = new List<ClassificationNode>();
      foreach (KeyValuePair<ProjectAdminIdentity, List<ClassificationNode>> toAreaSecuritySet in this.ProjectToAreaSecuritySets)
      {
        IdentityDescriptor administrators = toAreaSecuritySet.Key.Administrators;
        List<ClassificationNode> collection = toAreaSecuritySet.Value;
        if (service.IsMember(userRequestContext, administrators, userRequestContext.UserContext))
          userIsProjectAdmin.AddRange((IEnumerable<ClassificationNode>) collection);
      }
      return (IList<ClassificationNode>) userIsProjectAdmin;
    }

    private IEnumerable<ClassificationNode> GetUserAccessibleAreas(
      IVssRequestContext requestContext,
      string projectIdentifier,
      out bool allAreasAreAccessible)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081360, "Query Pipeline", "SecurityChecks", nameof (GetUserAccessibleAreas));
      int numAreasWithMismatchedAccess = 0;
      int numExceptions = 0;
      int numAreasWhereUserIsAdmin = 0;
      Stopwatch timer = new Stopwatch();
      timer.Start();
      HashSet<ClassificationNode> accessibleAreas = new HashSet<ClassificationNode>((IEqualityComparer<ClassificationNode>) new ClassificationNodeIdComparator());
      bool flag = requestContext.IsFeatureEnabled("Search.Server.WorkItemSecurityChecksCache");
      try
      {
        WorkItemSecuritySet witSecuritySet;
        if (flag && this.WorkItemSecurityChecksCache.TryGetUserData(requestContext, out witSecuritySet))
        {
          allAreasAreAccessible = witSecuritySet.AllAreasAreAccessible;
          if (!allAreasAreAccessible)
            accessibleAreas.AddRange<ClassificationNode, HashSet<ClassificationNode>>((IEnumerable<ClassificationNode>) witSecuritySet.AccessibleAreas);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("WitSecurityCacheHit", "Query Pipeline", 1.0);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081365, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("User ID = {0} .allAreasAreAccessible = {1} .userAccessibleAreas = {2}.", (object) requestContext.GetUserId(), (object) witSecuritySet.AllAreasAreAccessible, (object) string.Join<int>(", ", (IEnumerable<int>) accessibleAreas.Select<ClassificationNode, int>((Func<ClassificationNode, int>) (i => i.Id)).ToArray<int>())))));
          this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
        }
        else
        {
          bool isCollectionAdmin = this.IsCollectionAdmin(requestContext);
          if (!isCollectionAdmin)
          {
            this.m_areaSecuritySetsLock.EnterReadLock();
            try
            {
              IList<ClassificationNode> userIsProjectAdmin = this.GetAreasWhereUserIsProjectAdmin(requestContext);
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetAreasWhereUserIsAdminTime", "Query Pipeline", (double) timer.ElapsedMilliseconds);
              numAreasWhereUserIsAdmin = userIsProjectAdmin.Count;
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetAreasWhereUserIsAdmin", "Query Pipeline", (double) userIsProjectAdmin.Count);
              accessibleAreas.UnionWith((IEnumerable<ClassificationNode>) userIsProjectAdmin);
              if (userIsProjectAdmin.Count > 0)
                this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
              if (accessibleAreas.Count == this.m_totalAreas)
              {
                allAreasAreAccessible = true;
                if (this.m_totalAreas == 0)
                  this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
              }
              else
              {
                using (Dictionary<byte[], List<ClassificationNode>>.KeyCollection.Enumerator enumerator = this.AreaSecuritySets.Keys.GetEnumerator())
                {
label_30:
                  while (enumerator.MoveNext())
                  {
                    byte[] current = enumerator.Current;
                    List<ClassificationNode> areaList = this.AreaSecuritySets[current];
                    int count = areaList.Count;
                    try
                    {
                      ClassificationNode area1 = areaList.First<ClassificationNode>();
                      if (this.AreaHasReadWorkItemPermission(requestContext, area1))
                      {
                        accessibleAreas.UnionWith(WorkItemSecurityChecksService.FilterAreasByProject((IEnumerable<ClassificationNode>) areaList, projectIdentifier));
                      }
                      else
                      {
                        if (requestContext.IsTracing(1081364, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks"))
                        {
                          string areaHash = BitConverter.ToString(current);
                          foreach (ClassificationNode classificationNode in areaList)
                          {
                            ClassificationNode area = classificationNode;
                            bool hasPermission = this.AreaHasReadWorkItemPermission(requestContext, area);
                            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081364, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("HasReadPermission: {0}, AreaHash: {1}, AreaToken: {2}, NumberOfAreasInHash: {3}", (object) hasPermission, (object) areaHash, (object) area.Token, (object) areaList.Count))));
                          }
                        }
                        Random random = new Random();
                        int num = 1;
                        while (true)
                        {
                          if (num < this.m_numberOfAreasToCheckInASecuritySet)
                          {
                            if (num < count)
                            {
                              int index = count > this.m_numberOfAreasToCheckInASecuritySet ? random.Next(1, count) : num;
                              ClassificationNode area2 = areaList[index];
                              if (!this.AreaHasReadWorkItemPermission(requestContext, area2))
                                ++num;
                              else
                                break;
                            }
                            else
                              goto label_30;
                          }
                          else
                            goto label_30;
                        }
                        numAreasWithMismatchedAccess++;
                        accessibleAreas.UnionWith(WorkItemSecurityChecksService.FilterAreasByProject((IEnumerable<ClassificationNode>) areaList, projectIdentifier));
                      }
                    }
                    catch (Exception ex)
                    {
                      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1081340, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("Ignoring exception during area access checks: {0}", (object) ex)));
                    }
                  }
                }
                allAreasAreAccessible = accessibleAreas.Count == this.m_totalAreas;
                if (accessibleAreas.Count == 0)
                  this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
              }
            }
            finally
            {
              this.m_areaSecuritySetsLock.ExitReadLock();
            }
          }
          else
          {
            allAreasAreAccessible = true;
            this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
          }
          if (flag)
            this.WorkItemSecurityChecksCache.UpdateCacheWithUserInfo(requestContext, new WorkItemSecuritySet(allAreasAreAccessible, (IEnumerable<ClassificationNode>) accessibleAreas));
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceInfoConditionally(1081362, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("NumberOfUserAccessibleAreas: {0}, UserAccessibleAreaTime: {1}, NumberOfAreaSecuritySets: {2}, NumAreasWhereUserIsAdmin: {3}, isUserCollectionAdmin: {4}", (object) accessibleAreas.Count, (object) timer.ElapsedMilliseconds, (object) this.AreaSecuritySets.Count, (object) numAreasWhereUserIsAdmin, (object) isCollectionAdmin))));
        }
        timer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081363, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("AreaSecuritySetsWithMismatchedAccess: {0}, NumberOfExceptions: {1}", (object) numAreasWithMismatchedAccess, (object) numExceptions))));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetUserAccessibleAreasTime", "Query Pipeline", (double) timer.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfUserAccessibleAreas", "Query Pipeline", (double) accessibleAreas.Count);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfFailuresDuringAccessChecks", "Query Pipeline", (double) (numAreasWithMismatchedAccess + numExceptions));
        if (requestContext.IsTracing(1081362, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks"))
        {
          foreach (ClassificationNode classificationNode in accessibleAreas)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1081362, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("UserAccessibleArea: ID: {0}, ProjectID: {1}", (object) classificationNode.Id, (object) classificationNode.ProjectId)));
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081361, "Query Pipeline", "SecurityChecks", nameof (GetUserAccessibleAreas));
      }
      return (IEnumerable<ClassificationNode>) accessibleAreas;
    }

    public void PopulateUserSecurityChecksDataInRequestContext(IVssRequestContext requestContext)
    {
      int requiredPermissions = CommonConstants.SearchReadPermissionBitForMembers;
      string token = FormattableString.Invariant(FormattableStringFactory.Create("{0}/{1}", (object) "", (object) requestContext.DataspaceIdentifier));
      if (this.IsUserAnonymous(requestContext))
        requiredPermissions = CommonConstants.SearchReadPermissionBitForPublicUsers;
      this.SetSecurityData(requestContext, CommonConstants.SearchSecurityNamespaceGuid, requiredPermissions, token);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private static IEnumerable<ClassificationNode> FilterAreasByProject(
      IEnumerable<ClassificationNode> areaList,
      string projectIdentifier)
    {
      if (projectIdentifier == null)
        return areaList;
      Guid projectGuid;
      bool isProjectGuid = Guid.TryParse(projectIdentifier, out projectGuid);
      return areaList.Where<ClassificationNode>((Func<ClassificationNode, bool>) (r =>
      {
        if (isProjectGuid && r.ProjectId == projectGuid)
          return true;
        if (isProjectGuid)
          return false;
        return r.Path.StartsWith(FormattableString.Invariant(FormattableStringFactory.Create("{0}\\\\", (object) projectIdentifier)), StringComparison.Ordinal);
      })).Select<ClassificationNode, ClassificationNode>((Func<ClassificationNode, ClassificationNode>) (r => r));
    }

    public void ScrubEmailsFromIdentityFieldsForAnonymousPublicUsers(
      IVssRequestContext requestContext,
      WorkItemSearchResponse response)
    {
      if (requestContext.IsFeatureEnabled("Search.Server.WorkItem.QueryIdentityFields") || !this.IsUserAnonymous(requestContext))
        return;
      HashSet<string> identityFieldReferenceNames = requestContext.GetService<IWorkItemFieldCacheService>().GetIdentityFieldsList(requestContext);
      this.RemoveIdentityFileldsFromFilters(response);
      IEnumerable<WorkItemResult> resultsList = response.Results.Values;
      Parallel.For(0, resultsList.Count<WorkItemResult>(), (Action<int>) (i =>
      {
        WorkItemResult workItemResult = resultsList.ElementAt<WorkItemResult>(i);
        foreach (WorkItemHit hit in workItemResult.Hits)
        {
          if (identityFieldReferenceNames.Contains<string>(hit.FieldReferenceName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          {
            List<string> stringList = new List<string>();
            foreach (string highlight in hit.Highlights)
              stringList.Add(WorkItemSecurityChecksService.RemoveEmailFromIdentityString(highlight));
            hit.Highlights = (IEnumerable<string>) stringList;
          }
        }
        foreach (WorkItemField field in workItemResult.Fields)
        {
          string referenceName = field.ReferenceName;
          if (identityFieldReferenceNames.Contains<string>(field.ReferenceName, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
            field.Value = WorkItemSecurityChecksService.RemoveEmailFromIdentityString(field.Value);
        }
      }));
    }

    internal void RemoveIdentityFileldsFromFilters(WorkItemSearchResponse response)
    {
      if (response.FilterCategories == null)
        return;
      IEnumerable<FilterCategory> filterCategories = response.FilterCategories.Where<FilterCategory>((Func<FilterCategory, bool>) (f => f.Name != Microsoft.VisualStudio.Services.Search.WebApi.Legacy.Contracts.WorkItem.Constants.FilterCategories.WorkItemAssignedTo));
      response.FilterCategories = filterCategories;
    }

    private static string RemoveEmailFromIdentityString(string highlight)
    {
      highlight = highlight.Trim();
      int length1 = highlight.Length;
      if (length1 == 0)
        return highlight;
      int num = highlight[length1 - 1] == '>' ? 1 : 0;
      if (num == 0)
        return highlight;
      int length2 = length1 - 1;
      while (length2 > 0)
      {
        if (highlight[length2 - 1] == '<')
          --num;
        else if (highlight[length2 - 1] == '>')
          ++num;
        --length2;
        if (num == 0)
          break;
      }
      return length2 <= 0 ? string.Empty : highlight.Substring(0, length2).Trim();
    }

    private bool AreaHasReadWorkItemPermission(
      IVssRequestContext userRequestContext,
      ClassificationNode area)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081345, "Query Pipeline", "SecurityChecks", nameof (AreaHasReadWorkItemPermission));
      try
      {
        string token1 = this.m_workItemSecurityNamespace.NamespaceExtension.HandleIncomingToken(userRequestContext, this.m_workItemSecurityNamespace, area.Token);
        bool hasPermission = this.m_workItemSecurityNamespace.HasPermission(userRequestContext, token1, AuthorizationCssNodePermissions.WorkItemRead, false);
        if (!hasPermission && !userRequestContext.IsFeatureEnabled("Search.Server.WorkItem.RedoSecurityCheckForBadTokensDisabled") && token1.Contains("::"))
        {
          CustomerIntelligenceData properties = new CustomerIntelligenceData();
          properties.Add("Token with double separator", token1);
          string token2 = token1.Replace("::", ":");
          hasPermission = this.m_workItemSecurityNamespace.HasPermission(userRequestContext, token2, AuthorizationCssNodePermissions.WorkItemRead, false);
          if (hasPermission)
          {
            properties.Add("HasPermissionOnNewToken", "true");
            token1 = token2;
          }
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishCi(nameof (WorkItemSecurityChecksService), nameof (AreaHasReadWorkItemPermission), properties);
        }
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081364, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("HasReadPermission: {0}, AreaToken: {1}, AreaProjectId: {2}", (object) hasPermission, (object) area.Token, (object) area.ProjectId))));
        if (hasPermission)
          this.SetSecurityData(userRequestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid, AuthorizationCssNodePermissions.WorkItemRead, token1);
        return hasPermission;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081346, "Query Pipeline", "SecurityChecks", nameof (AreaHasReadWorkItemPermission));
      }
    }

    public void ValidateAndSetUserPermissionsForSearchService(IVssRequestContext requestContext)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081399, "Query Pipeline", "SecurityChecks", nameof (ValidateAndSetUserPermissionsForSearchService));
      try
      {
        string token = FormattableString.Invariant(FormattableStringFactory.Create("{0}/{1}", (object) "", (object) requestContext.DataspaceIdentifier));
        if (this.m_searchSecurityNamespace.HasPermission(requestContext, token, CommonConstants.SearchReadPermissionBitForMembers))
          requestContext.Items["isUserAnonymousKey"] = (object) false;
        else if (this.m_searchSecurityNamespace.HasPermission(requestContext, token, CommonConstants.SearchReadPermissionBitForPublicUsers))
          requestContext.Items["isUserAnonymousKey"] = (object) true;
        else
          throw new InvalidAccessException(FormattableString.Invariant(FormattableStringFactory.Create("Expecting user to be authenticated or anonymous. User id is {0}", (object) requestContext.GetUserId())));
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081400, "Query Pipeline", "SecurityChecks", nameof (ValidateAndSetUserPermissionsForSearchService));
      }
    }

    private void SetSecurityData(
      IVssRequestContext userRequestContext,
      Guid namespaceId,
      int requiredPermissions,
      string token)
    {
      userRequestContext.Items["searchServiceSecurityTokenKey"] = (object) token;
      userRequestContext.Items["searchServiceSecurityPermissionKey"] = (object) requiredPermissions;
      userRequestContext.Items["searchServiceSecurityNamespaceGuidKey"] = (object) namespaceId;
    }

    internal void BuildSecuritySetsCallback(
      IVssRequestContext collectionRequestContext,
      object taskArgs)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.SetLogicalContext((IDiagnosticContext) new TfsDiagnosticContext(collectionRequestContext));
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081343, "Query Pipeline", "SecurityChecks", nameof (BuildSecuritySetsCallback));
      try
      {
        try
        {
          this.m_securitySetsRebuildTaskEvent.AddCount();
          bool isCacheFeatureEnabled = collectionRequestContext.IsFeatureEnabled("Search.Server.WorkItemSecurityChecksCache");
          this.BuildAreaSecuritySets(collectionRequestContext, isCacheFeatureEnabled, false);
        }
        finally
        {
          this.m_securitySetsRebuildTaskEvent.Signal();
        }
      }
      catch (ObjectDisposedException ex)
      {
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081358, "Query Pipeline", "SecurityChecks", ex);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081344, "Query Pipeline", "SecurityChecks", nameof (BuildSecuritySetsCallback));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private static byte[] GenerateUniqueHash()
    {
      byte[] data = new byte[RepositoryConstants.SecurityHashLength];
      using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
        cryptoServiceProvider.GetBytes(data);
      return data;
    }

    private static bool IsInvalidSecurityHash(byte[] hash) => hash == null || hash.Length != RepositoryConstants.SecurityHashLength || ((IEnumerable<byte>) hash).All<byte>((Func<byte, bool>) (b => b == (byte) 0));

    internal void BuildAreaSecuritySets(
      IVssRequestContext collectionRequestContext,
      bool isCacheFeatureEnabled,
      bool isCalledInUserContext)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<ClassificationNode> classificationNodes = this.m_dataAccessFactoryInstance.GetClassificationNodeDataAccess().GetClassificationNodes(collectionRequestContext, ClassificationNodeType.Area, -1);
        Dictionary<byte[], List<ClassificationNode>> areaSecuritySetsNew = new Dictionary<byte[], List<ClassificationNode>>((IEqualityComparer<byte[]>) new ByteArrayComparer());
        foreach (ClassificationNode classificationNode in classificationNodes)
        {
          byte[] numArray = classificationNode.SecurityHashcode;
          if (WorkItemSecurityChecksService.IsInvalidSecurityHash(numArray))
          {
            if (numArray != null && numArray.Length != RepositoryConstants.SecurityHashLength && numArray.Length > 1)
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1081396, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("areaSecuritySets: Found an invalid hash: Id: '{0}', ProjectId: '{1}', hashValue: '{2}'", (object) classificationNode.Id, (object) classificationNode.ProjectId, (object) BitConverter.ToString(numArray))));
            numArray = WorkItemSecurityChecksService.GenerateUniqueHash();
          }
          List<ClassificationNode> classificationNodeList;
          if (!areaSecuritySetsNew.TryGetValue(numArray, out classificationNodeList))
          {
            classificationNodeList = new List<ClassificationNode>()
            {
              classificationNode
            };
            areaSecuritySetsNew.Add(numArray, classificationNodeList);
          }
          else
            classificationNodeList.Add(classificationNode);
        }
        if (collectionRequestContext.IsTracing(1081364, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks"))
        {
          foreach (KeyValuePair<byte[], List<ClassificationNode>> keyValuePair in areaSecuritySetsNew)
          {
            KeyValuePair<byte[], List<ClassificationNode>> kvp = keyValuePair;
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1081364, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("List of areas in set {0} ", (object) BitConverter.ToString(kvp.Key))) + string.Join(",", kvp.Value.Select<ClassificationNode, string>((Func<ClassificationNode, string>) (s => s.Identifier.ToString())))));
          }
        }
        Dictionary<ProjectAdminIdentity, List<ClassificationNode>> areaSecuritySets = this.ComputeProjectToAreaSecuritySets(collectionRequestContext, classificationNodes, isCalledInUserContext);
        bool flag = !isCacheFeatureEnabled || this.ClearCacheIfSecuritySetDiffer(areaSecuritySetsNew);
        this.m_areaSecuritySetsLock.EnterWriteLock();
        try
        {
          if (this.AreaSecuritySets == null | flag)
            this.AreaSecuritySets = areaSecuritySetsNew;
          this.ProjectToAreaSecuritySets = areaSecuritySets ?? new Dictionary<ProjectAdminIdentity, List<ClassificationNode>>();
          this.m_totalAreas = classificationNodes.Count;
        }
        finally
        {
          this.m_areaSecuritySetsLock.ExitWriteLock();
        }
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("AreaSecuritySetCreationTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfAreaSecuritySets", "Query Pipeline", (double) areaSecuritySetsNew.Count);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081340, "Query Pipeline", "SecurityChecks", ex);
      }
    }

    internal bool ClearCacheIfSecuritySetDiffer(
      Dictionary<byte[], List<ClassificationNode>> areaSecuritySetsNew)
    {
      if (this.AreAreaSecuritySetsEqual(this.AreaSecuritySets, areaSecuritySetsNew))
        return false;
      this.WorkItemSecurityChecksCache.ClearCache();
      return true;
    }

    private bool AreAreaSecuritySetsEqual(
      Dictionary<byte[], List<ClassificationNode>> areaSecuritySets1,
      Dictionary<byte[], List<ClassificationNode>> areaSecuritySets2)
    {
      if (areaSecuritySets1.Count != areaSecuritySets2.Count)
        return false;
      foreach (KeyValuePair<byte[], List<ClassificationNode>> keyValuePair in areaSecuritySets1)
      {
        List<ClassificationNode> nodes2;
        if (!areaSecuritySets2.TryGetValue(keyValuePair.Key, out nodes2) || !this.AreClassicationNodeSetEqual(keyValuePair.Value, nodes2))
          return false;
      }
      return true;
    }

    private bool AreClassicationNodeSetEqual(
      List<ClassificationNode> nodes1,
      List<ClassificationNode> nodes2)
    {
      if (nodes1.Count != nodes2.Count)
        return false;
      HashSet<ClassificationNode> collection = new HashSet<ClassificationNode>((IEqualityComparer<ClassificationNode>) new ClassificationNodeIdComparator());
      collection.AddRange<ClassificationNode, HashSet<ClassificationNode>>((IEnumerable<ClassificationNode>) nodes2);
      foreach (ClassificationNode classificationNode in nodes1)
      {
        if (!collection.Contains(classificationNode))
          return false;
      }
      return true;
    }

    private Dictionary<ProjectAdminIdentity, List<ClassificationNode>> ComputeProjectToAreaSecuritySets(
      IVssRequestContext collectionRequestContext,
      List<ClassificationNode> areaNodes,
      bool isCalledInUserContext)
    {
      IIndexingUnitDataAccess indexingUnitDataAccess = this.m_dataAccessFactoryInstance.GetIndexingUnitDataAccess();
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnits = indexingUnitDataAccess.GetIndexingUnits(collectionRequestContext, "Project", (IEntityType) WorkItemEntityType.GetInstance(), -1);
      FriendlyDictionary<Guid, IdentityDescriptor> friendlyDictionary = new FriendlyDictionary<Guid, IdentityDescriptor>(indexingUnits.Count);
      List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit> indexingUnit1 = new List<Microsoft.VisualStudio.Services.Search.Common.IndexingUnit>();
      foreach (Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit2 in indexingUnits)
      {
        Guid tfsEntityId = indexingUnit2.TFSEntityId;
        ProjectWorkItemIndexingProperties properties = (ProjectWorkItemIndexingProperties) indexingUnit2.Properties;
        IdentityDescriptor projectAdministrators = properties.ProjectAdministrators;
        if (projectAdministrators != (IdentityDescriptor) null)
        {
          friendlyDictionary[tfsEntityId] = projectAdministrators;
        }
        else
        {
          IdentityService service = collectionRequestContext.GetService<IdentityService>();
          try
          {
            properties.ProjectAdministrators = WorkItemSecurityChecksService.GetScope(collectionRequestContext, service, tfsEntityId).Administrators;
            friendlyDictionary[tfsEntityId] = properties.ProjectAdministrators;
            indexingUnit1.Add(indexingUnit2);
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("ProjectAdminUpdateCacheCount", "Query Pipeline", 1.0);
          }
          catch (GroupScopeDoesNotExistException ex)
          {
          }
        }
      }
      if (!isCalledInUserContext && indexingUnit1.Count > 0)
        indexingUnitDataAccess.UpdateIndexingUnits(collectionRequestContext, indexingUnit1);
      Dictionary<ProjectAdminIdentity, List<ClassificationNode>> areaSecuritySets = new Dictionary<ProjectAdminIdentity, List<ClassificationNode>>();
      foreach (IGrouping<Guid, ClassificationNode> collection in areaNodes.GroupBy<ClassificationNode, Guid, ClassificationNode>((Func<ClassificationNode, Guid>) (n => n.ProjectId), (Func<ClassificationNode, ClassificationNode>) (n => n)))
      {
        Guid key1 = collection.Key;
        IdentityDescriptor administrators;
        if (friendlyDictionary.TryGetValue(key1, out administrators))
        {
          ProjectAdminIdentity key2 = new ProjectAdminIdentity(key1, administrators);
          areaSecuritySets[key2] = new List<ClassificationNode>((IEnumerable<ClassificationNode>) collection);
        }
      }
      return areaSecuritySets;
    }

    private void LoadCssNodeSecurityNamespace(IVssRequestContext collectionRequestContext)
    {
      if (this.m_workItemSecurityNamespace != null)
        return;
      lock (this.m_workItemSecurityNamespaceLock)
      {
        if (this.m_workItemSecurityNamespace != null)
          return;
        SecurityChecksUtils.LoadRemoteSecurityNamespace(collectionRequestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
        this.m_workItemSecurityNamespace = SecurityChecksUtils.GetSecurityNamespace(collectionRequestContext, AuthorizationSecurityConstants.CommonStructureNodeSecurityGuid);
      }
    }

    private void LoadSearchSecurityNamespace(IVssRequestContext requestContext)
    {
      if (this.m_searchSecurityNamespace != null)
        return;
      lock (this.m_searchSecurityNamespaceLock)
      {
        if (this.m_searchSecurityNamespace != null)
          return;
        this.m_searchSecurityNamespace = SecurityChecksUtils.GetSecurityNamespace(requestContext, this.m_searchSecurityNamespaceGuid);
      }
    }

    private static IdentityScope GetScope(
      IVssRequestContext collectionRequestContext,
      IdentityService service,
      Guid projectId)
    {
      Stopwatch stopwatch = Stopwatch.StartNew();
      IdentityScope scope = service.GetScope(collectionRequestContext, projectId);
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("GetScopeTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      return scope;
    }

    public bool IsUserAnonymous(IVssRequestContext requestContext)
    {
      if (!requestContext.Items.ContainsKey("isUserAnonymousKey"))
        this.ValidateAndSetUserPermissionsForSearchService(requestContext);
      return (bool) requestContext.Items["isUserAnonymousKey"];
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue)
        return;
      if (disposing)
      {
        if (this.m_areaSecuritySetsLock != null)
          this.m_areaSecuritySetsLock.Dispose();
        if (this.m_securitySetsRebuildTaskEvent != null)
        {
          this.m_securitySetsRebuildTaskEvent.Dispose();
          this.m_securitySetsRebuildTaskEvent = (CountdownEvent) null;
        }
      }
      this.m_disposedValue = true;
    }

    public void Dispose()
    {
      this.Dispose(true);
      GC.SuppressFinalize((object) this);
    }
  }
}
