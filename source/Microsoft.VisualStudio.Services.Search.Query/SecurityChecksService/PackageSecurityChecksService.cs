// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService.PackageSecurityChecksService
// Assembly: Microsoft.VisualStudio.Services.Search.Query, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 71E00698-03D3-4C67-B313-A550333DA80C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Query.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Query.SecurityChecksCache;
using Microsoft.VisualStudio.Services.Search.Server.DataAccess.DataAccessLayer;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Implementations;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using System.Threading;

namespace Microsoft.VisualStudio.Services.Search.Query.SecurityChecksService
{
  public class PackageSecurityChecksService : 
    IPackageSecurityChecksService,
    IVssFrameworkService,
    IDisposable
  {
    internal Dictionary<byte[], List<PackageContainer>> PackageContainerSecuritySets;
    private readonly Guid m_feedSecurityNamespaceGuid = new Guid("9FED0191-DCA2-4112-86B7-A6A48D1B204C");
    private readonly Guid m_searchSecurityNamespaceGuid = new Guid("ca535e7e-67ce-457f-93fe-6e53aa4e4160");
    internal PackageSecurityChecksCache PackageSecurityChecksCache;
    private readonly ReaderWriterLockSlim m_packagecontainerSecuritySetsLock;
    private TeamFoundationTask m_securitySetsRebuildTask;
    private CountdownEvent m_securitySetsRebuildTaskEvent;
    private int m_totalPackageContainers;
    private IVssSecurityNamespace m_packageSecurityNamespace;
    private readonly object m_packageSecurityNamespaceLock;
    private IVssSecurityNamespace m_searchSecurityNamespace;
    private readonly object m_searchSecurityNamespaceLock;
    private readonly IDataAccessFactory m_dataAccessFactoryInstance;
    private int m_numberOfPackageContainersToCheckInASecuritySet;
    private bool m_disposedValue;

    public PackageSecurityChecksService()
      : this(DataAccessFactory.GetInstance())
    {
    }

    internal PackageSecurityChecksService(IDataAccessFactory dataAccessFactory)
    {
      this.m_packagecontainerSecuritySetsLock = new ReaderWriterLockSlim();
      this.PackageContainerSecuritySets = new Dictionary<byte[], List<PackageContainer>>((IEqualityComparer<byte[]>) new ByteArrayComparer());
      this.m_packageSecurityNamespaceLock = new object();
      this.m_searchSecurityNamespaceLock = new object();
      this.m_dataAccessFactoryInstance = dataAccessFactory;
    }

    internal PackageSecurityChecksService(
      PackageSecurityChecksCache securityChecksCache,
      IDataAccessFactory dataAccessFactory)
      : this(dataAccessFactory)
    {
      this.PackageSecurityChecksCache = securityChecksCache;
      this.m_securitySetsRebuildTaskEvent = new CountdownEvent(1);
    }

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_numberOfPackageContainersToCheckInASecuritySet = systemRequestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection) ? systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/NumberOfRepositoriesToCheckInASecuritySet") : throw new UnsupportedHostTypeException(systemRequestContext.ServiceHost.HostType);
      TimeSpan timeSpan = TimeSpan.FromSeconds((double) systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/SecuritySetsRebuildIntervalInSec"));
      if (this.m_securitySetsRebuildTaskEvent == null)
        this.m_securitySetsRebuildTaskEvent = new CountdownEvent(1);
      this.m_securitySetsRebuildTask = new TeamFoundationTask(new TeamFoundationTaskCallback(this.BuildSecuritySetsCallback), (object) null, (int) timeSpan.TotalMilliseconds);
      IVssRequestContext vssRequestContext = systemRequestContext;
      IVssRequestContext context = systemRequestContext.To(TeamFoundationHostType.Deployment);
      Stopwatch stopwatch = Stopwatch.StartNew();
      this.LoadPackageContainerSecurityNamespace(vssRequestContext);
      this.LoadSearchSecurityNamespace(vssRequestContext);
      stopwatch.Stop();
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("SecurityNamespaceLoadTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
      if (this.PackageSecurityChecksCache == null)
        this.PackageSecurityChecksCache = new PackageSecurityChecksCache(systemRequestContext.GetConfigValue<int>("/Service/ALMSearch/Settings/PackageSecurityChecksCacheMaxSize"), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/PackageSecurityChecksCacheExpirationInMin")), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/PackageSecurityChecksCacheCleanupTaskIntervalInMin")), TimeSpan.FromMinutes(systemRequestContext.GetConfigValue<double>("/Service/ALMSearch/Settings/PackageSecurityChecksCacheInactivityExpirationInMin")));
      this.BuildPackageContainerSecuritySets(vssRequestContext, false);
      if (this.m_securitySetsRebuildTask != null)
        context.GetService<ITeamFoundationTaskService>().AddTask(vssRequestContext, this.m_securitySetsRebuildTask);
      this.PackageSecurityChecksCache.Initialize(systemRequestContext);
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
        this.PackageSecurityChecksCache.TearDown(systemRequestContext);
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    private bool IsCollectionAdmin(IVssRequestContext requestContext)
    {
      IdentityDescriptor userContext = requestContext.UserContext;
      return requestContext.GetService<IdentityService>().IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, userContext);
    }

    public IEnumerable<PackageContainer> GetUserAccessiblePackageContainers(
      IVssRequestContext requestContext,
      out bool allFeedsAreAccessible)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1080048, "Query Pipeline", "SecurityChecks", nameof (GetUserAccessiblePackageContainers));
      int numPackageContainersWithMismatchedAccess = 0;
      int numExceptions = 0;
      Stopwatch timer = new Stopwatch();
      timer.Start();
      HashSet<PackageContainer> accessiblePackageContainers = new HashSet<PackageContainer>((IEqualityComparer<PackageContainer>) new PackageContainerIdComparator());
      bool flag = requestContext.IsFeatureEnabled("Search.Server.PackageSecurityChecksCache");
      try
      {
        PackageSecuritySet packageSecuritySet;
        if (flag && this.PackageSecurityChecksCache.TryGetUserData(requestContext, out packageSecuritySet))
        {
          allFeedsAreAccessible = packageSecuritySet.AllPackageContainersAreAccessible;
          if (!allFeedsAreAccessible)
            accessiblePackageContainers.AddRange<PackageContainer, HashSet<PackageContainer>>((IEnumerable<PackageContainer>) packageSecuritySet.AccessiblePackageContainers);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpi("PackageSecurityCacheHit", "Query Pipeline", 1.0);
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1080058, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("User ID = {0} .allPackageContainersAreAccessible = {1} .userAccessiblePackageContainers = {2}.", (object) requestContext.GetUserId(), (object) packageSecuritySet.AllPackageContainersAreAccessible, (object) string.Join<Guid>(", ", (IEnumerable<Guid>) accessiblePackageContainers.Select<PackageContainer, Guid>((Func<PackageContainer, Guid>) (i => i.ContainerId)).ToArray<Guid>())))));
          this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
        }
        else
        {
          bool isCollectionAdmin = this.IsCollectionAdmin(requestContext);
          if (!isCollectionAdmin)
          {
            this.m_packagecontainerSecuritySetsLock.EnterReadLock();
            try
            {
              if (accessiblePackageContainers.Count == this.m_totalPackageContainers)
              {
                if (this.m_totalPackageContainers == 0)
                {
                  allFeedsAreAccessible = false;
                  this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
                }
                else
                  allFeedsAreAccessible = true;
              }
              else
              {
                using (Dictionary<byte[], List<PackageContainer>>.KeyCollection.Enumerator enumerator = this.PackageContainerSecuritySets.Keys.GetEnumerator())
                {
label_29:
                  while (enumerator.MoveNext())
                  {
                    byte[] current = enumerator.Current;
                    List<PackageContainer> containerSecuritySet = this.PackageContainerSecuritySets[current];
                    int count = containerSecuritySet.Count;
                    try
                    {
                      PackageContainer packagecontainer1 = containerSecuritySet.First<PackageContainer>();
                      if (this.PackageContainerHasReadPackagePermission(requestContext, packagecontainer1))
                      {
                        accessiblePackageContainers.UnionWith((IEnumerable<PackageContainer>) containerSecuritySet);
                      }
                      else
                      {
                        if (requestContext.IsTracing(1080059, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks"))
                        {
                          string str = BitConverter.ToString(current);
                          foreach (PackageContainer packagecontainer2 in containerSecuritySet)
                            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080059, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("HasReadPermission: {0}, PackageContainerHash: {1}, PackageContainerToken: {2}, NumberOfPackageContainersInHash: {3}", (object) this.PackageContainerHasReadPackagePermission(requestContext, packagecontainer2), (object) str, (object) packagecontainer2.Token, (object) containerSecuritySet.Count)));
                        }
                        Random random = new Random();
                        int num = 1;
                        while (true)
                        {
                          if (num < this.m_numberOfPackageContainersToCheckInASecuritySet)
                          {
                            if (num < count)
                            {
                              int index = count > this.m_numberOfPackageContainersToCheckInASecuritySet ? random.Next(1, count) : num;
                              PackageContainer packagecontainer3 = containerSecuritySet[index];
                              if (!this.PackageContainerHasReadPackagePermission(requestContext, packagecontainer3))
                                ++num;
                              else
                                break;
                            }
                            else
                              goto label_29;
                          }
                          else
                            goto label_29;
                        }
                        numPackageContainersWithMismatchedAccess++;
                        accessiblePackageContainers.UnionWith((IEnumerable<PackageContainer>) containerSecuritySet);
                      }
                    }
                    catch (Exception ex)
                    {
                      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1081340, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("Ignoring exception during packagecontainer access checks: {0}", (object) ex)));
                      numExceptions++;
                    }
                  }
                }
                allFeedsAreAccessible = accessiblePackageContainers.Count == this.m_totalPackageContainers;
                if (accessiblePackageContainers.Count == 0)
                  this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
              }
            }
            finally
            {
              this.m_packagecontainerSecuritySetsLock.ExitReadLock();
            }
          }
          else
          {
            allFeedsAreAccessible = true;
            this.PopulateUserSecurityChecksDataInRequestContext(requestContext);
          }
          if (flag)
            this.PackageSecurityChecksCache.UpdateCacheWithUserInfo(requestContext, new PackageSecuritySet(allFeedsAreAccessible, (IEnumerable<PackageContainer>) accessiblePackageContainers));
          Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1080056, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("NumberOfUserAccessiblePackageContainers: {0}, UserAccessiblePackageContainerTime: {1}, NumberOfPackageContainerSecuritySets: {2}, isUserCollectionAdmin: {3}", (object) accessiblePackageContainers.Count, (object) timer.ElapsedMilliseconds, (object) this.PackageContainerSecuritySets.Count, (object) isCollectionAdmin))));
        }
        timer.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerboseConditionally(1080057, "Query Pipeline", "SecurityChecks", (Func<string>) (() => FormattableString.Invariant(FormattableStringFactory.Create("PackageContainerSecuritySetsWithMismatchedAccess: {0}, NumberOfExceptions: {1}", (object) numPackageContainersWithMismatchedAccess, (object) numExceptions))));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("GetUserAccessiblePackageContainersTime", "Query Pipeline", (double) timer.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfUserAccessiblePackageContainers", "Query Pipeline", (double) accessiblePackageContainers.Count);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfFailuresDuringAccessChecks", "Query Pipeline", (double) (numPackageContainersWithMismatchedAccess + numExceptions));
        if (requestContext.IsTracing(1080056, TraceLevel.Verbose, "Query Pipeline", "SecurityChecks"))
        {
          foreach (PackageContainer packageContainer in accessiblePackageContainers)
            Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceVerbose(1080056, "Query Pipeline", "SecurityChecks", FormattableString.Invariant(FormattableStringFactory.Create("UserAccessiblePackageContainer: ID: {0}", (object) packageContainer.ContainerId)));
        }
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1080049, "Query Pipeline", "SecurityChecks", nameof (GetUserAccessiblePackageContainers));
      }
      return (IEnumerable<PackageContainer>) accessiblePackageContainers;
    }

    private bool PackageContainerHasReadPackagePermission(
      IVssRequestContext userRequestContext,
      PackageContainer packagecontainer)
    {
      Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceEnter(1081345, "Query Pipeline", "SecurityChecks", nameof (PackageContainerHasReadPackagePermission));
      try
      {
        string token = this.m_packageSecurityNamespace.NamespaceExtension.HandleIncomingToken(userRequestContext, this.m_packageSecurityNamespace, packagecontainer.Token);
        int num = this.m_packageSecurityNamespace.HasPermission(userRequestContext, token, 32, false) ? 1 : 0;
        if (num != 0)
          this.SetSecurityData(userRequestContext, CommonConstants.FeedSecurityNamespaceGuid, 32, token);
        return num != 0;
      }
      finally
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081346, "Query Pipeline", "SecurityChecks", nameof (PackageContainerHasReadPackagePermission));
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
        this.m_securitySetsRebuildTaskEvent.AddCount();
        bool forceUpdateSecuritySets = collectionRequestContext.IsFeatureEnabled("Search.Server.PackageSecurityChecksCache");
        this.BuildPackageContainerSecuritySets(collectionRequestContext, forceUpdateSecuritySets);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081358, "Query Pipeline", "SecurityChecks", ex);
      }
      finally
      {
        this.m_securitySetsRebuildTaskEvent.Signal();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceLeave(1081344, "Query Pipeline", "SecurityChecks", nameof (BuildSecuritySetsCallback));
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.ClearLogicalContext();
      }
    }

    public void PopulateUserSecurityChecksDataInRequestContext(IVssRequestContext requestContext)
    {
      string token = FormattableString.Invariant(FormattableStringFactory.Create("{0}/{1}", (object) "", (object) requestContext.DataspaceIdentifier));
      int requiredPermissions = CommonConstants.SearchReadPermissionBitForMembers;
      if (this.IsUserAnonymous(requestContext))
        requiredPermissions = CommonConstants.SearchReadPermissionBitForPublicUsers;
      this.SetSecurityData(requestContext, CommonConstants.SearchSecurityNamespaceGuid, requiredPermissions, token);
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

    public bool IsUserAnonymous(IVssRequestContext requestContext)
    {
      if (!requestContext.Items.ContainsKey("isUserAnonymousKey"))
        this.ValidateAndSetUserPermissionsForSearchService(requestContext);
      return (bool) requestContext.Items["isUserAnonymousKey"];
    }

    private static byte[] GenerateUniqueHash()
    {
      byte[] data = new byte[RepositoryConstants.SecurityHashLength];
      using (RNGCryptoServiceProvider cryptoServiceProvider = new RNGCryptoServiceProvider())
        cryptoServiceProvider.GetBytes(data);
      return data;
    }

    private static bool IsInvalidSecurityHash(byte[] hash) => hash == null || hash.Length != RepositoryConstants.SecurityHashLength || ((IEnumerable<byte>) hash).All<byte>((Func<byte, bool>) (b => b == (byte) 0));

    internal void BuildPackageContainerSecuritySets(
      IVssRequestContext collectionRequestContext,
      bool forceUpdateSecuritySets)
    {
      try
      {
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<PackageContainer> packageContainers = this.m_dataAccessFactoryInstance.GetPackageContainerDataAccess().GetPackageContainers(collectionRequestContext, -1);
        Dictionary<byte[], List<PackageContainer>> packagecontainerSecuritySetsNew = new Dictionary<byte[], List<PackageContainer>>((IEqualityComparer<byte[]>) new ByteArrayComparer());
        foreach (PackageContainer packageContainer in packageContainers)
        {
          byte[] numArray = packageContainer.SecurityHashCode;
          if (PackageSecurityChecksService.IsInvalidSecurityHash(numArray))
          {
            if (numArray != null && numArray.Length != RepositoryConstants.SecurityHashLength && numArray.Length > 1)
              Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceWarning(1081396, "Query Pipeline", "Query", FormattableString.Invariant(FormattableStringFactory.Create("packagecontainerSecuritySets: Found an invalid hash: Id: '{0}', hashValue: '{1}'", (object) packageContainer.ContainerId, (object) BitConverter.ToString(numArray))));
            numArray = PackageSecurityChecksService.GenerateUniqueHash();
          }
          List<PackageContainer> packageContainerList;
          if (!packagecontainerSecuritySetsNew.TryGetValue(numArray, out packageContainerList))
          {
            packageContainerList = new List<PackageContainer>()
            {
              packageContainer
            };
            packagecontainerSecuritySetsNew.Add(numArray, packageContainerList);
          }
          else
            packageContainerList.Add(packageContainer);
        }
        bool flag = !forceUpdateSecuritySets || this.ClearCacheIfSecuritySetDiffer(packagecontainerSecuritySetsNew);
        this.m_packagecontainerSecuritySetsLock.EnterWriteLock();
        try
        {
          if (this.PackageContainerSecuritySets == null | flag)
            this.PackageContainerSecuritySets = packagecontainerSecuritySetsNew;
          this.m_totalPackageContainers = packageContainers.Count;
        }
        finally
        {
          this.m_packagecontainerSecuritySetsLock.ExitWriteLock();
        }
        stopwatch.Stop();
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("PackageSecuritySetCreationTime", "Query Pipeline", (double) stopwatch.ElapsedMilliseconds);
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.PublishKpiAndCi("NumberOfAreaSecuritySets", "Query Pipeline", (double) packagecontainerSecuritySetsNew.Count);
      }
      catch (Exception ex)
      {
        Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry.Tracer.TraceException(1081340, "Query Pipeline", "SecurityChecks", ex);
      }
    }

    internal bool ClearCacheIfSecuritySetDiffer(
      Dictionary<byte[], List<PackageContainer>> packagecontainerSecuritySetsNew)
    {
      if (this.ArePackageContainerSecuritySetsEqual(this.PackageContainerSecuritySets, packagecontainerSecuritySetsNew))
        return false;
      this.PackageSecurityChecksCache.ClearCache();
      return true;
    }

    private bool ArePackageContainerSecuritySetsEqual(
      Dictionary<byte[], List<PackageContainer>> packagecontainerSecuritySets1,
      Dictionary<byte[], List<PackageContainer>> packagecontainerSecuritySets2)
    {
      if (packagecontainerSecuritySets1.Count != packagecontainerSecuritySets2.Count)
        return false;
      foreach (KeyValuePair<byte[], List<PackageContainer>> keyValuePair in packagecontainerSecuritySets1)
      {
        List<PackageContainer> packageContainers2;
        if (!packagecontainerSecuritySets2.TryGetValue(keyValuePair.Key, out packageContainers2) || !this.ArePackageContainerSetEqual(keyValuePair.Value, packageContainers2))
          return false;
      }
      return true;
    }

    private bool ArePackageContainerSetEqual(
      List<PackageContainer> packageContainers1,
      List<PackageContainer> packageContainers2)
    {
      if (packageContainers1.Count != packageContainers2.Count)
        return false;
      HashSet<PackageContainer> collection = new HashSet<PackageContainer>((IEqualityComparer<PackageContainer>) new PackageContainerIdComparator());
      collection.AddRange<PackageContainer, HashSet<PackageContainer>>((IEnumerable<PackageContainer>) packageContainers2);
      foreach (PackageContainer packageContainer in packageContainers1)
      {
        if (!collection.Contains(packageContainer))
          return false;
      }
      return true;
    }

    private void LoadPackageContainerSecurityNamespace(IVssRequestContext collectionRequestContext)
    {
      if (this.m_packageSecurityNamespace != null)
        return;
      lock (this.m_packageSecurityNamespaceLock)
      {
        if (this.m_packageSecurityNamespace != null)
          return;
        SecurityChecksUtils.LoadRemoteSecurityNamespace(collectionRequestContext, this.m_feedSecurityNamespaceGuid);
        this.m_packageSecurityNamespace = SecurityChecksUtils.GetSecurityNamespace(collectionRequestContext, this.m_feedSecurityNamespaceGuid);
      }
    }

    protected virtual void Dispose(bool disposing)
    {
      if (this.m_disposedValue || !disposing)
        return;
      if (this.m_packagecontainerSecuritySetsLock != null)
        this.m_packagecontainerSecuritySetsLock.Dispose();
      if (this.m_securitySetsRebuildTaskEvent != null)
      {
        this.m_securitySetsRebuildTaskEvent.Dispose();
        this.m_securitySetsRebuildTaskEvent = (CountdownEvent) null;
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
