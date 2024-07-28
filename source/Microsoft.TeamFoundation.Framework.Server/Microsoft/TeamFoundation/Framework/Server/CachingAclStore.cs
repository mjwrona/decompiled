// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.CachingAclStore
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class CachingAclStore : 
    IQueryableAclStore,
    ISecurityAclStore,
    IMutableAclStore,
    IInvalidatableAclStore
  {
    protected readonly SecurityNamespaceDescription m_description;
    protected readonly bool m_isHierarchical;
    protected readonly ISecurityNamespaceBackingStore m_backingStore;
    protected readonly ILockName m_rwLockName;
    private readonly InitialLoadGate m_initialLoadGate;
    protected readonly Guid m_serviceHostInstanceId;
    protected readonly string m_securityEvaluatorKey;
    protected readonly IdentityMapper m_identityMapper;
    private readonly ITeamFoundationTaskService m_taskService;
    private readonly SecuritySettingsService m_settingsService;
    private readonly SecurityBackingStoreChangedEventHandler m_onChanged;
    private readonly string m_uniqueId;
    protected long m_completedRefreshId;
    protected long m_hardRefreshId;
    protected long m_softRefreshId;
    private long m_lastQueuedAsyncRefreshId;
    protected TokenStoreSequenceId m_sequenceId;
    private uint m_lastRefreshTickCount;
    private readonly uint m_cacheLifetimeInMilliseconds;
    protected ITokenStore<AccessControlListSlim> m_tokenStore;
    private static readonly VssPerformanceCounter s_refreshesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_TotalSecurityRefreshesPerSec");
    private const int c_maxIdentityBatchSize = 5000;
    private const string c_area = "Security";
    private const string c_layer = "CachingAclStore";

    public CachingAclStore(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description,
      ISecurityNamespaceBackingStore backingStore,
      int cacheLifetimeInMilliseconds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(description, nameof (description));
      ArgumentUtility.CheckForNull<ISecurityNamespaceBackingStore>(backingStore, nameof (backingStore));
      ArgumentUtility.CheckForOutOfRange(cacheLifetimeInMilliseconds, nameof (cacheLifetimeInMilliseconds), 0);
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      this.m_description = description;
      this.m_isHierarchical = this.m_description.NamespaceStructure == SecurityNamespaceStructure.Hierarchical;
      this.m_backingStore = backingStore;
      this.m_rwLockName = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}", (object) this.GetType().FullName, (object) "cachingaclstore"));
      this.m_serviceHostInstanceId = requestContext.ServiceHost.InstanceId;
      this.m_securityEvaluatorKey = RequestContextItemsKeys.SecurityEvaluatorKeyBase + (object) Guid.NewGuid();
      this.m_identityMapper = new IdentityMapper(requestContext.ServiceHost.InstanceId);
      this.m_taskService = context.GetService<ITeamFoundationTaskService>();
      this.m_settingsService = context.GetService<SecuritySettingsService>();
      this.m_initialLoadGate = new InitialLoadGate(this.m_settingsService.Settings.InitialLoadGateSize, this.m_settingsService.Settings.LoadGateWaiterLimit);
      this.m_uniqueId = Guid.NewGuid().ToString();
      this.m_cacheLifetimeInMilliseconds = (uint) cacheLifetimeInMilliseconds;
      this.m_sequenceId = new TokenStoreSequenceId();
      this.m_tokenStore = CachingAclStore.CreateTokenStore(this.m_description);
      this.m_tokenStore.Seal();
      this.m_lastRefreshTickCount = (uint) Environment.TickCount;
      this.m_completedRefreshId = 0L;
      this.m_lastQueuedAsyncRefreshId = 0L;
      this.m_hardRefreshId = 1L;
      this.m_softRefreshId = 1L;
      this.m_onChanged = this.m_backingStore.WeakSubscribeToPushInvalidations(requestContext, new SecurityBackingStoreChangedEventHandler(this.NotifyChanged));
    }

    public Guid NamespaceId => this.m_description.NamespaceId;

    public Guid AclStoreId => this.m_backingStore.AclStoreId;

    internal ISecurityNamespaceBackingStore BackingStore => this.m_backingStore;

    internal int InitialLoadGateMaxCount => this.m_initialLoadGate.MaxCount;

    internal int InitialLoadGateWaiterCount => this.m_initialLoadGate.Waiters;

    internal long LastQueuedAsyncRefreshId => Interlocked.Read(ref this.m_lastQueuedAsyncRefreshId);

    public void QueryEffectivePermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      out int effectiveAllow,
      out int effectiveDeny,
      int bitsToConsider = -1)
    {
      requestContext.TraceEnter(56250, "Security", nameof (CachingAclStore), nameof (QueryEffectivePermissions));
      try
      {
        ArgumentUtility.CheckForNull<EvaluationPrincipal>(evaluationPrincipal, nameof (evaluationPrincipal));
        this.ValidateRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token);
        evaluationPrincipal = this.MapToWellKnownIdentifier(evaluationPrincipal);
        SecurityEvaluator.EvaluationResult evaluationResult = this.GetSecurityEvaluator(requestContext).QueryEffectivePermissions(requestContext, token, evaluationPrincipal, bitsToConsider);
        effectiveAllow = evaluationResult.Allow;
        effectiveDeny = evaluationResult.Deny;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56258, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56259, "Security", nameof (CachingAclStore), nameof (QueryEffectivePermissions));
      }
    }

    public IEnumerable<EnumeratedPermission> QueryChildPermissions(
      IVssRequestContext requestContext,
      string token,
      EvaluationPrincipal evaluationPrincipal,
      int bitsToConsider = -1)
    {
      requestContext.TraceEnter(56210, "Security", nameof (CachingAclStore), nameof (QueryChildPermissions));
      try
      {
        ArgumentUtility.CheckForNull<EvaluationPrincipal>(evaluationPrincipal, nameof (evaluationPrincipal));
        this.ValidateRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token);
        evaluationPrincipal = this.MapToWellKnownIdentifier(evaluationPrincipal);
        return this.GetSecurityEvaluator(requestContext).EnumChildPermissions(requestContext, token, evaluationPrincipal, bitsToConsider);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56212, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56211, "Security", nameof (CachingAclStore), nameof (QueryChildPermissions));
      }
    }

    public IEnumerable<QueriedAccessControlList> QueryAccessControlLists(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse)
    {
      requestContext.TraceEnter(56200, "Security", nameof (CachingAclStore), nameof (QueryAccessControlLists));
      try
      {
        this.ValidateRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token, true);
        evaluationPrincipals = this.MapToWellKnownIdentifiers(evaluationPrincipals);
        List<QueriedAccessControlList> toPrune = this.QueryPermissions(requestContext, token, evaluationPrincipals, includeExtendedInfo, recurse);
        if (WellKnownAclStores.User == this.m_backingStore.AclStoreId)
          this.PrunePermissions(requestContext, evaluationPrincipals, toPrune);
        for (int index = toPrune.Count - 1; index >= 0; --index)
        {
          if (toPrune[index].AccessControlEntries.Count == 0 && (!this.m_isHierarchical || toPrune[index].Inherit))
            toPrune.RemoveAt(index);
        }
        return (IEnumerable<QueriedAccessControlList>) toPrune;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56208, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56209, "Security", nameof (CachingAclStore), nameof (QueryAccessControlLists));
      }
    }

    private List<QueriedAccessControlList> QueryPermissions(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      bool includeExtendedInfo,
      bool recurse)
    {
      requestContext.TraceEnter(56300, "Security", nameof (CachingAclStore), nameof (QueryPermissions));
      try
      {
        SecurityEvaluator securityEvaluator = this.GetSecurityEvaluator(requestContext);
        ITokenStore<AccessControlListSlim> aclStore = securityEvaluator.GetAclStore();
        IEnumerable<AccessControlListSlim> accessControlListSlims;
        if (recurse || token == null)
        {
          accessControlListSlims = aclStore.EnumSubTree(token);
        }
        else
        {
          AccessControlListSlim referencedObject;
          if (aclStore.TryGetValue(token, out referencedObject))
            accessControlListSlims = (IEnumerable<AccessControlListSlim>) new AccessControlListSlim[1]
            {
              referencedObject
            };
          else
            accessControlListSlims = Enumerable.Empty<AccessControlListSlim>();
        }
        if (includeExtendedInfo && token != null)
        {
          AccessControlListSlim accessControlListSlim = accessControlListSlims.FirstOrDefault<AccessControlListSlim>();
          if (accessControlListSlim == null || !StringComparer.OrdinalIgnoreCase.Equals(accessControlListSlim.Token, token))
            accessControlListSlims = ((IEnumerable<AccessControlListSlim>) new AccessControlListSlim[1]
            {
              new AccessControlListSlim(token, this.m_isHierarchical)
            }).Concat<AccessControlListSlim>(accessControlListSlims);
        }
        List<QueriedAccessControlList> accessControlListList = new List<QueriedAccessControlList>();
        foreach (AccessControlListSlim accessControlListSlim in accessControlListSlims)
        {
          IEnumerable<EvaluationPrincipal> evaluationPrincipals1 = evaluationPrincipals;
          if (evaluationPrincipals1 == null & includeExtendedInfo)
          {
            HashSet<IdentityDescriptor> descriptorSet = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
            aclStore.EnumAndEvaluateParents(accessControlListSlim.Token, false, (Func<string, AccessControlListSlim, string, bool, bool>) ((nodeToken, acl, nodeNoChildrenBelow, isExactMatch) =>
            {
              foreach (IdentityDescriptor identityDescriptor in acl.AccessControlEntries.Select<AccessControlEntrySlim, IdentityDescriptor>((Func<AccessControlEntrySlim, IdentityDescriptor>) (s => s.Descriptor)))
                descriptorSet.Add(identityDescriptor);
              return acl.InheritPermissions;
            }));
            evaluationPrincipals1 = descriptorSet.Select<IdentityDescriptor, EvaluationPrincipal>((Func<IdentityDescriptor, EvaluationPrincipal>) (s => new EvaluationPrincipal(s)));
          }
          List<QueriedAccessControlEntry> accessControlEntries = new List<QueriedAccessControlEntry>();
          if (evaluationPrincipals1 == null)
          {
            accessControlEntries.AddRange(accessControlListSlim.AccessControlEntries.Select<AccessControlEntrySlim, QueriedAccessControlEntry>((Func<AccessControlEntrySlim, QueriedAccessControlEntry>) (s => new QueriedAccessControlEntry(s.Descriptor, s.Allow, s.Deny))));
          }
          else
          {
            foreach (EvaluationPrincipal evaluationPrincipal in evaluationPrincipals1)
            {
              AccessControlEntrySlim controlEntrySlim = accessControlListSlim.QueryAccessControlEntry(evaluationPrincipal.PrimaryDescriptor);
              accessControlEntries.Add(new QueriedAccessControlEntry(controlEntrySlim.Descriptor, controlEntrySlim.Allow, controlEntrySlim.Deny));
            }
          }
          if (includeExtendedInfo)
          {
            for (int index = 0; index < accessControlEntries.Count; ++index)
            {
              SecurityEvaluator.EvaluationResult withExtendedInfo = securityEvaluator.ComputeEffectivePermissionsForQueryWithExtendedInfo(requestContext, accessControlListSlim.Token, (EvaluationPrincipal) accessControlEntries[index].IdentityDescriptor);
              accessControlEntries[index] = new QueriedAccessControlEntry(accessControlEntries[index].IdentityDescriptor, accessControlEntries[index].Allow, accessControlEntries[index].Deny, withExtendedInfo.InheritedAllow, withExtendedInfo.InheritedDeny, withExtendedInfo.Allow, withExtendedInfo.Deny);
            }
          }
          accessControlListList.Add(new QueriedAccessControlList(accessControlListSlim.Token, accessControlListSlim.InheritPermissions, (IList<QueriedAccessControlEntry>) accessControlEntries));
        }
        return accessControlListList;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56308, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56309, "Security", nameof (CachingAclStore), nameof (QueryPermissions));
      }
    }

    private void PrunePermissions(
      IVssRequestContext requestContext,
      IEnumerable<EvaluationPrincipal> evaluationPrincipals,
      List<QueriedAccessControlList> toPrune)
    {
      requestContext.TraceEnter(56340, "Security", nameof (CachingAclStore), nameof (PrunePermissions));
      try
      {
        if (requestContext.IsServicingContext || evaluationPrincipals != null && evaluationPrincipals.Any<EvaluationPrincipal>())
          return;
        requestContext = requestContext.Elevate();
        IdentityService service = requestContext.GetService<IdentityService>();
        HashSet<IdentityDescriptor> source1 = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        foreach (QueriedAccessControlList accessControlList in toPrune)
          source1.UnionWith(accessControlList.AccessControlEntries.Select<QueriedAccessControlEntry, IdentityDescriptor>((Func<QueriedAccessControlEntry, IdentityDescriptor>) (ace => ace.IdentityDescriptor)));
        if (!source1.Any<IdentityDescriptor>())
          return;
        HashSet<IdentityDescriptor> source2 = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        int batchSize = Math.Min(source1.Count, 5000);
        foreach (IList<IdentityDescriptor> descriptors in source1.Batch<IdentityDescriptor>(batchSize))
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext, descriptors, QueryMembership.None, (IEnumerable<string>) null);
          for (int index = 0; index < descriptors.Count; ++index)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
            if ((identity == null || !identity.IsActive) && (requestContext.ExecutionEnvironment.IsOnPremisesDeployment || !AadIdentityHelper.IsAadGroupNotDeleted((IReadOnlyVssIdentity) identity)))
              source2.Add(descriptors[index]);
          }
        }
        if (!source2.Any<IdentityDescriptor>())
          return;
        foreach (QueriedAccessControlList accessControlList in toPrune)
        {
          for (int index = accessControlList.AccessControlEntries.Count - 1; index >= 0; --index)
          {
            IdentityDescriptor identityDescriptor = accessControlList.AccessControlEntries[index].IdentityDescriptor;
            if (source2.Contains(identityDescriptor))
              accessControlList.AccessControlEntries.RemoveAt(index);
          }
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56348, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56349, "Security", nameof (CachingAclStore), nameof (PrunePermissions));
      }
    }

    public int QueryAclCount(IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56380, "Security", nameof (CachingAclStore), nameof (QueryAclCount));
      try
      {
        this.ValidateRequestContext(requestContext);
        ITokenStore<AccessControlListSlim> tokenStore;
        this.GetTokenStoreSnapshot(requestContext, this.GetMinimumRefreshId(requestContext), out TokenStoreSequenceId _, out tokenStore);
        return tokenStore.Count;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56388, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56389, "Security", nameof (CachingAclStore), nameof (QueryAclCount));
      }
    }

    public bool RemoveAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IdentityDescriptor> descriptors)
    {
      requestContext.TraceEnter(56140, "Security", nameof (CachingAclStore), nameof (RemoveAccessControlEntries));
      try
      {
        ArgumentUtility.CheckForNull<IEnumerable<IdentityDescriptor>>(descriptors, nameof (descriptors));
        this.ValidateRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token);
        bool flag = false;
        List<IdentityDescriptor> identityDescriptorList = new List<IdentityDescriptor>(this.MapToWellKnownIdentifiers(descriptors));
        TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
        SecurityChangedNotification notificationEvent = new SecurityChangedNotification(this.m_description.NamespaceId, token, identityDescriptorList);
        try
        {
          service.PublishDecisionPoint(requestContext, (object) notificationEvent);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          requestContext.Trace(56145, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "'RemoveAccessControlEntries' action has been handled by a notification subscriber. Info: {0}", (object) ex);
          return false;
        }
        IList<Microsoft.VisualStudio.Services.Identity.Identity> source = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<IdentityDescriptor>) identityDescriptorList, QueryMembership.None, (IEnumerable<string>) null);
        this.RefreshIfNeeded(requestContext);
        TokenStoreSequenceId tokenStoreSequenceId = this.m_backingStore.RemovePermissions(requestContext, token, source.Where<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (s => s != null)).Select<Microsoft.VisualStudio.Services.Identity.Identity, Guid>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (s => s.Id)));
        ITokenStore<AccessControlListSlim> tokenStore;
        TokenStoreSequenceId sequenceId;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          tokenStore = this.m_tokenStore;
          sequenceId = this.m_sequenceId;
        }
        if (sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
        {
          ITokenStore<AccessControlListSlim> newTokenStore = tokenStore;
          AccessControlListSlim referencedObject;
          if (tokenStore.TryGetValue(token, out referencedObject))
          {
            AccessControlListSlim acl = new AccessControlListSlim(referencedObject);
            foreach (IdentityDescriptor descriptor in identityDescriptorList)
              flag |= acl.RemoveAccessControlEntry(descriptor);
            if (flag)
            {
              newTokenStore = tokenStore.Copy(requestContext);
              if (acl.IsEmpty(this.m_isHierarchical))
                newTokenStore.Remove(token, false);
              else
                newTokenStore[token] = acl;
            }
          }
          using (requestContext.AcquireWriterLock(this.m_rwLockName))
          {
            if (this.m_sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
              this.SetTokenStore(requestContext, tokenStoreSequenceId, newTokenStore);
            else
              this.HardInvalidate(requestContext);
          }
        }
        else
        {
          AccessControlListSlim referencedObject;
          if (tokenStore.TryGetValue(token, out referencedObject))
          {
            foreach (AccessControlEntrySlim accessControlEntry in referencedObject.AccessControlEntries)
            {
              if (identityDescriptorList.Contains(accessControlEntry.Descriptor))
              {
                flag = true;
                break;
              }
            }
          }
          using (requestContext.AcquireReaderLock(this.m_rwLockName))
            this.HardInvalidate(requestContext);
        }
        service.SyncPublishNotification(requestContext, (object) notificationEvent);
        return flag;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56148, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56149, "Security", nameof (CachingAclStore), nameof (RemoveAccessControlEntries));
      }
    }

    public bool RemoveAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<string> tokens,
      bool recurse)
    {
      requestContext.TraceEnter(56150, "Security", nameof (CachingAclStore), nameof (RemoveAccessControlLists));
      try
      {
        ArgumentUtility.CheckForNull<IEnumerable<string>>(tokens, nameof (tokens));
        this.ValidateRequestContext(requestContext);
        List<string> tokens1 = new List<string>(tokens.Select<string, string>((Func<string, string>) (s => this.CheckAndCanonicalizeToken(s))));
        bool flag = false;
        TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
        SecurityChangedNotification notificationEvent = new SecurityChangedNotification(this.m_description.NamespaceId, tokens1, recurse);
        try
        {
          service.PublishDecisionPoint(requestContext, (object) notificationEvent);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          requestContext.Trace(56155, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "'RemoveAccessControlLists' action has been handled by a notification subscriber. Info: {0}", (object) ex);
          return false;
        }
        this.RefreshIfNeeded(requestContext);
        TokenStoreSequenceId tokenStoreSequenceId = this.m_backingStore.RemoveAccessControlLists(requestContext, (IEnumerable<string>) tokens1, recurse);
        ITokenStore<AccessControlListSlim> tokenStore;
        TokenStoreSequenceId sequenceId;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          tokenStore = this.m_tokenStore;
          sequenceId = this.m_sequenceId;
        }
        if (sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
        {
          ITokenStore<AccessControlListSlim> newTokenStore = tokenStore.Copy(requestContext);
          foreach (string token in tokens1)
            flag |= newTokenStore.Remove(token, recurse);
          using (requestContext.AcquireWriterLock(this.m_rwLockName))
          {
            if (this.m_sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
              this.SetTokenStore(requestContext, tokenStoreSequenceId, newTokenStore);
            else
              this.HardInvalidate(requestContext);
          }
        }
        else
        {
          foreach (string token in tokens1)
          {
            if (this.m_isHierarchical & recurse)
            {
              if (tokenStore.HasSubItem(token))
              {
                flag = true;
                break;
              }
            }
            else if (tokenStore.TryGetValue(token, out AccessControlListSlim _))
            {
              flag = true;
              break;
            }
          }
          using (requestContext.AcquireReaderLock(this.m_rwLockName))
            this.HardInvalidate(requestContext);
        }
        service.SyncPublishNotification(requestContext, (object) notificationEvent);
        return flag;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56158, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56159, "Security", nameof (CachingAclStore), nameof (RemoveAccessControlLists));
      }
    }

    public IEnumerable<IAccessControlEntry> SetAccessControlEntries(
      IVssRequestContext requestContext,
      string token,
      IEnumerable<IAccessControlEntry> accessControlEntries,
      bool merge,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      requestContext.TraceEnter(56170, "Security", nameof (CachingAclStore), nameof (SetAccessControlEntries));
      try
      {
        ArgumentUtility.CheckForNull<IEnumerable<IAccessControlEntry>>(accessControlEntries, nameof (accessControlEntries));
        this.ValidateRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token);
        List<IAccessControlEntry> accessControlEntryList = new List<IAccessControlEntry>(this.MapToWellKnownIdentifiers(accessControlEntries));
        TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
        try
        {
          service.PublishDecisionPoint(requestContext, (object) new SecurityChangedNotification(this.m_description.NamespaceId, token, accessControlEntryList, merge, throwOnInvalidIdentity, rootNewIdentities));
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          requestContext.Trace(56175, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "'SetAccessControlEntries' action has been handled by a notification subscriber. Info: {0}", (object) ex);
          return (IEnumerable<IAccessControlEntry>) Array.Empty<IAccessControlEntry>();
        }
        if (rootNewIdentities)
        {
          if (!this.RootNewIdentities(requestContext, (IEnumerable<IAccessControlList>) new IAccessControlList[1]
          {
            (IAccessControlList) new AccessControlList(token, true, (IEnumerable<IAccessControlEntry>) accessControlEntryList)
          }, (throwOnInvalidIdentity ? 1 : 0) != 0))
            goto label_7;
        }
        this.RefreshIfNeeded(requestContext);
label_7:
        TokenStoreSequenceId tokenStoreSequenceId = this.m_backingStore.SetPermissions(requestContext, token, (IEnumerable<IAccessControlEntry>) accessControlEntryList, merge, throwOnInvalidIdentity);
        List<AccessControlEntrySlim> aces = new List<AccessControlEntrySlim>(accessControlEntryList.Select<IAccessControlEntry, AccessControlEntrySlim>((Func<IAccessControlEntry, AccessControlEntrySlim>) (s => new AccessControlEntrySlim(s))));
        List<IAccessControlEntry> permissions = new List<IAccessControlEntry>();
        ITokenStore<AccessControlListSlim> tokenStore;
        TokenStoreSequenceId sequenceId;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          tokenStore = this.m_tokenStore;
          sequenceId = this.m_sequenceId;
        }
        if (sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
        {
          ITokenStore<AccessControlListSlim> newTokenStore = tokenStore.Copy(requestContext);
          AccessControlListSlim referencedObject;
          AccessControlListSlim acl = !newTokenStore.TryGetValue(token, out referencedObject) ? new AccessControlListSlim(token, this.m_isHierarchical) : new AccessControlListSlim(referencedObject);
          permissions.AddRange((IEnumerable<IAccessControlEntry>) acl.SetAccessControlEntries((IEnumerable<AccessControlEntrySlim>) aces, merge).Select<AccessControlEntrySlim, AccessControlEntry>((Func<AccessControlEntrySlim, AccessControlEntry>) (s => new AccessControlEntry(s))));
          acl.RemoveZeroEntries();
          if (acl.IsEmpty(this.m_isHierarchical))
            newTokenStore.Remove(token, false);
          else
            newTokenStore[token] = acl;
          using (requestContext.AcquireWriterLock(this.m_rwLockName))
          {
            if (this.m_sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
              this.SetTokenStore(requestContext, tokenStoreSequenceId, newTokenStore);
            else
              this.HardInvalidate(requestContext);
          }
        }
        else
        {
          AccessControlListSlim referencedObject;
          if (tokenStore.TryGetValue(token, out referencedObject))
          {
            referencedObject = (AccessControlListSlim) referencedObject.Clone();
            permissions.AddRange((IEnumerable<IAccessControlEntry>) referencedObject.SetAccessControlEntries((IEnumerable<AccessControlEntrySlim>) aces, merge).Select<AccessControlEntrySlim, AccessControlEntry>((Func<AccessControlEntrySlim, AccessControlEntry>) (s => new AccessControlEntry(s))));
          }
          if (tokenStoreSequenceId != TokenStoreSequenceId.NoWorkPerformed)
          {
            using (requestContext.AcquireReaderLock(this.m_rwLockName))
              this.HardInvalidate(requestContext);
          }
        }
        service.SyncPublishNotification(requestContext, (object) new SecurityChangedNotification(this.m_description.NamespaceId, token, permissions, merge, throwOnInvalidIdentity, rootNewIdentities));
        return (IEnumerable<IAccessControlEntry>) permissions;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56178, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56179, "Security", nameof (CachingAclStore), nameof (SetAccessControlEntries));
      }
    }

    public void SetAccessControlLists(
      IVssRequestContext requestContext,
      IEnumerable<IAccessControlList> accessControlLists,
      bool throwOnInvalidIdentity = true,
      bool rootNewIdentities = true)
    {
      requestContext.TraceEnter(56180, "Security", nameof (CachingAclStore), nameof (SetAccessControlLists));
      try
      {
        ArgumentUtility.CheckForNull<IEnumerable<IAccessControlList>>(accessControlLists, nameof (accessControlLists));
        this.ValidateRequestContext(requestContext);
        foreach (IAccessControlList accessControlList in accessControlLists)
          this.CheckValidToken(accessControlList.Token);
        List<IAccessControlList> accessControlListList = new List<IAccessControlList>(this.MapToWellKnownIdentifiersAndCanonicalize(accessControlLists));
        TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
        SecurityChangedNotification notificationEvent = new SecurityChangedNotification(this.m_description.NamespaceId, accessControlListList, throwOnInvalidIdentity, rootNewIdentities);
        try
        {
          service.PublishDecisionPoint(requestContext, (object) notificationEvent);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          requestContext.Trace(56185, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "'SetAccessControlLists' action has been handled by a notification subscriber. Info: {0}", (object) ex);
          return;
        }
        if (!rootNewIdentities || this.RootNewIdentities(requestContext, (IEnumerable<IAccessControlList>) accessControlListList, throwOnInvalidIdentity))
          this.RefreshIfNeeded(requestContext);
        TokenStoreSequenceId tokenStoreSequenceId = this.m_backingStore.SetAccessControlLists(requestContext, (IEnumerable<IAccessControlList>) accessControlListList, throwOnInvalidIdentity);
        List<AccessControlListSlim> accessControlListSlimList = new List<AccessControlListSlim>(accessControlListList.Select<IAccessControlList, AccessControlListSlim>((Func<IAccessControlList, AccessControlListSlim>) (s => new AccessControlListSlim(s))));
        ITokenStore<AccessControlListSlim> tokenStore;
        TokenStoreSequenceId sequenceId;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          tokenStore = this.m_tokenStore;
          sequenceId = this.m_sequenceId;
        }
        if (sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId) && accessControlListList.Select<IAccessControlList, string>((Func<IAccessControlList, string>) (s => s.Token)).Distinct<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase).Count<string>() == accessControlListList.Count)
        {
          ITokenStore<AccessControlListSlim> newTokenStore = tokenStore.Copy(requestContext);
          foreach (AccessControlListSlim acl in accessControlListSlimList)
          {
            acl.RemoveZeroEntries();
            if (acl.IsEmpty(this.m_isHierarchical))
              newTokenStore.Remove(acl.Token, false);
            else
              newTokenStore[acl.Token] = acl;
          }
          using (requestContext.AcquireWriterLock(this.m_rwLockName))
          {
            if (this.m_sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
              this.SetTokenStore(requestContext, tokenStoreSequenceId, newTokenStore);
            else
              this.HardInvalidate(requestContext);
          }
        }
        else
        {
          using (requestContext.AcquireReaderLock(this.m_rwLockName))
            this.HardInvalidate(requestContext);
        }
        service.SyncPublishNotification(requestContext, (object) notificationEvent);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56188, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56189, "Security", nameof (CachingAclStore), nameof (SetAccessControlLists));
      }
    }

    public void SetInheritFlag(IVssRequestContext requestContext, string token, bool inherit)
    {
      requestContext.TraceEnter(56190, "Security", nameof (CachingAclStore), nameof (SetInheritFlag));
      try
      {
        this.ValidateRequestContext(requestContext);
        token = this.CheckAndCanonicalizeToken(token);
        TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
        SecurityChangedNotification notificationEvent = new SecurityChangedNotification(this.m_description.NamespaceId, token, inherit);
        try
        {
          service.PublishDecisionPoint(requestContext, (object) notificationEvent);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          requestContext.Trace(56195, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "'SetInheritFlag' action has been handled by a notification subscriber. Info: {0}", (object) ex);
          return;
        }
        if (!this.m_isHierarchical)
          throw new InvalidOperationException(TFCommonResources.InvalidOperationOnNonHierarchicalNamespace());
        this.RefreshIfNeeded(requestContext);
        TokenStoreSequenceId tokenStoreSequenceId = this.m_backingStore.SetInheritFlag(requestContext, token, inherit);
        ITokenStore<AccessControlListSlim> tokenStore;
        TokenStoreSequenceId sequenceId;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          tokenStore = this.m_tokenStore;
          sequenceId = this.m_sequenceId;
        }
        if (sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
        {
          ITokenStore<AccessControlListSlim> newTokenStore = tokenStore;
          if (inherit)
          {
            AccessControlListSlim referencedObject;
            if (tokenStore.TryGetValue(token, out referencedObject) && !referencedObject.InheritPermissions)
            {
              newTokenStore = tokenStore.Copy(requestContext);
              if (referencedObject.Count == 0)
              {
                newTokenStore.Remove(token, false);
              }
              else
              {
                referencedObject = new AccessControlListSlim(referencedObject);
                referencedObject.InheritPermissions = true;
                newTokenStore[token] = referencedObject;
              }
            }
          }
          else
          {
            AccessControlListSlim referencedObject;
            if (newTokenStore.TryGetValue(token, out referencedObject))
            {
              referencedObject = new AccessControlListSlim(referencedObject);
              referencedObject.InheritPermissions = false;
            }
            else
              referencedObject = new AccessControlListSlim(token, false);
            newTokenStore = tokenStore.Copy(requestContext);
            newTokenStore[token] = referencedObject;
          }
          using (requestContext.AcquireWriterLock(this.m_rwLockName))
          {
            if (this.m_sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
              this.SetTokenStore(requestContext, tokenStoreSequenceId, newTokenStore);
            else
              this.HardInvalidate(requestContext);
          }
        }
        else
        {
          using (requestContext.AcquireReaderLock(this.m_rwLockName))
            this.HardInvalidate(requestContext);
        }
        service.SyncPublishNotification(requestContext, (object) notificationEvent);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56198, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56199, "Security", nameof (CachingAclStore), nameof (SetInheritFlag));
      }
    }

    public void RenameToken(
      IVssRequestContext requestContext,
      string existingToken,
      string newToken,
      bool copy)
    {
      requestContext.TraceEnter(56230, "Security", nameof (CachingAclStore), nameof (RenameToken));
      try
      {
        existingToken = this.CheckAndCanonicalizeToken(existingToken);
        newToken = this.CheckAndCanonicalizeToken(newToken);
        TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
        SecurityChangedNotification notificationEvent = new SecurityChangedNotification(this.m_description.NamespaceId, existingToken, newToken, copy);
        try
        {
          service.PublishDecisionPoint(requestContext, (object) notificationEvent);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          requestContext.Trace(56235, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "'RenameToken' action has been handled by a notification subscriber. Info: {0}", (object) ex);
          return;
        }
        this.RefreshIfNeeded(requestContext);
        TokenRename[] renames = new TokenRename[1]
        {
          new TokenRename(existingToken, newToken, copy, true)
        };
        TokenStoreSequenceId tokenStoreSequenceId = this.m_backingStore.RenameTokens(requestContext, (IEnumerable<TokenRename>) renames);
        ITokenStore<AccessControlListSlim> tokenStore;
        TokenStoreSequenceId sequenceId;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          tokenStore = this.m_tokenStore;
          sequenceId = this.m_sequenceId;
        }
        if (!(sequenceId == tokenStoreSequenceId))
        {
          if (sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
          {
            ITokenStore<AccessControlListSlim> newTokenStore = tokenStore.Copy(requestContext);
            newTokenStore.Remove(newToken, true);
            Action<AccessControlListSlim> action = (Action<AccessControlListSlim>) (existingAcl =>
            {
              AccessControlListSlim accessControlListSlim = new AccessControlListSlim(existingAcl);
              accessControlListSlim.Token = newToken + accessControlListSlim.Token.Substring(existingToken.Length);
              newTokenStore[accessControlListSlim.Token] = accessControlListSlim;
            });
            foreach (AccessControlListSlim accessControlListSlim in tokenStore.EnumSubTree(existingToken))
              action(accessControlListSlim);
            if (!copy)
              newTokenStore.Remove(existingToken, true);
            using (requestContext.AcquireWriterLock(this.m_rwLockName))
            {
              if (this.m_sequenceId.ImmediatelyPrecedes(tokenStoreSequenceId))
                this.SetTokenStore(requestContext, tokenStoreSequenceId, newTokenStore);
              else
                this.HardInvalidate(requestContext);
            }
          }
          else
          {
            using (requestContext.AcquireReaderLock(this.m_rwLockName))
              this.HardInvalidate(requestContext);
          }
        }
        service.SyncPublishNotification(requestContext, (object) notificationEvent);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56238, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56239, "Security", nameof (CachingAclStore), nameof (RenameToken));
      }
    }

    public void RenameTokens(
      IVssRequestContext requestContext,
      IEnumerable<TokenRename> renameTokens)
    {
      requestContext.TraceEnter(56240, "Security", nameof (CachingAclStore), nameof (RenameTokens));
      try
      {
        foreach (TokenRename renameToken in renameTokens)
        {
          this.CheckValidToken(renameToken.OldToken);
          this.CheckValidToken(renameToken.NewToken);
        }
        TeamFoundationEventService service = requestContext.GetService<TeamFoundationEventService>();
        SecurityChangedNotification notificationEvent = new SecurityChangedNotification(this.m_description.NamespaceId, renameTokens);
        try
        {
          service.PublishDecisionPoint(requestContext, (object) notificationEvent);
        }
        catch (ActionDeniedBySubscriberException ex)
        {
          requestContext.Trace(56245, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "'RenameTokens' action has been handled by a notification subscriber. Info: {0}", (object) ex);
          return;
        }
        this.RefreshIfNeeded(requestContext);
        TokenStoreSequenceId tokenStoreSequenceId = this.m_backingStore.RenameTokens(requestContext, renameTokens);
        TokenStoreSequenceId sequenceId;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
          sequenceId = this.m_sequenceId;
        if (!(sequenceId == tokenStoreSequenceId))
        {
          using (requestContext.AcquireReaderLock(this.m_rwLockName))
            this.HardInvalidate(requestContext);
        }
        service.SyncPublishNotification(requestContext, (object) notificationEvent);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56248, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56249, "Security", nameof (CachingAclStore), nameof (RenameTokens));
      }
    }

    protected bool RootNewIdentities(
      IVssRequestContext requestContext,
      IEnumerable<IAccessControlList> acls,
      bool throwOnInvalidIdentity)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment && requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
        return false;
      HashSet<IdentityDescriptor> identityDescriptorSet = new HashSet<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
      bool flag = false;
      ITokenStore<AccessControlListSlim> tokenStore;
      this.GetTokenStore(requestContext, out TokenStoreSequenceId _, out tokenStore);
      foreach (IAccessControlList acl in acls)
      {
        AccessControlListSlim referencedObject;
        if (!tokenStore.TryGetValue(acl.Token, out referencedObject))
        {
          foreach (IAccessControlEntry accessControlEntry in acl.AccessControlEntries)
          {
            if (accessControlEntry.Allow != 0)
              identityDescriptorSet.Add(accessControlEntry.Descriptor);
          }
        }
        else
        {
          foreach (IAccessControlEntry accessControlEntry in acl.AccessControlEntries)
          {
            if (accessControlEntry.Allow != 0 && referencedObject.QueryAccessControlEntry(accessControlEntry.Descriptor).Allow == 0)
              identityDescriptorSet.Add(accessControlEntry.Descriptor);
          }
        }
      }
      if (identityDescriptorSet.Count > 0)
      {
        IdentityService service = requestContext.GetService<IdentityService>();
        foreach (IdentityDescriptor identityDescriptor in identityDescriptorSet)
        {
          if (!ServicePrincipals.IsServicePrincipal(requestContext, identityDescriptor) && !IdentityDescriptorComparer.Instance.Equals(identityDescriptor, GroupWellKnownIdentityDescriptors.EveryoneGroup))
          {
            if (!service.IsMember(requestContext, GroupWellKnownIdentityDescriptors.EveryoneGroup, identityDescriptor))
            {
              try
              {
                service.AddMemberToGroup(requestContext, GroupWellKnownIdentityDescriptors.SecurityServiceGroup, identityDescriptor);
              }
              catch (Exception ex)
              {
                requestContext.TraceException(56255, "Security", nameof (CachingAclStore), ex);
                if (throwOnInvalidIdentity)
                  throw;
              }
              flag = true;
            }
          }
        }
      }
      return flag;
    }

    public TokenStoreSequenceId GetCurrentSequenceId(IVssRequestContext requestContext)
    {
      TokenStoreSequenceId sequenceId;
      this.GetTokenStore(requestContext, out sequenceId, out ITokenStore<AccessControlListSlim> _);
      return sequenceId;
    }

    internal bool IsRefreshPending(IVssRequestContext requestContext)
    {
      using (requestContext.AcquireReaderLock(this.m_rwLockName))
        return Interlocked.Read(ref this.m_softRefreshId) > this.m_completedRefreshId;
    }

    internal TokenStoreSequenceId ReadCurrentSequenceId(IVssRequestContext requestContext)
    {
      using (requestContext.AcquireReaderLock(this.m_rwLockName))
        return this.m_sequenceId;
    }

    public void NotifyChanged(
      IVssRequestContext requestContext,
      TokenStoreSequenceId newSequenceId,
      bool hardInvalidate = false)
    {
      long requestedRefreshId = 0;
      if (TokenStoreSequenceId.DropCache == newSequenceId)
      {
        hardInvalidate = true;
        using (requestContext.AcquireWriterLock(this.m_rwLockName))
        {
          this.HardInvalidate(requestContext);
          this.SetTokenStore(requestContext, new TokenStoreSequenceId(), CachingAclStore.CreateTokenStore(this.m_description));
        }
      }
      else if (TokenStoreSequenceId.UnconditionalRefresh == newSequenceId)
      {
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          requestedRefreshId = !hardInvalidate ? Interlocked.Increment(ref this.m_softRefreshId) : this.HardInvalidate(requestContext);
          if (this.m_completedRefreshId == 0L)
            requestedRefreshId = 0L;
        }
      }
      else
      {
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          if (this.m_sequenceId.IsSupersededBy(newSequenceId))
          {
            requestedRefreshId = !hardInvalidate ? Interlocked.Increment(ref this.m_softRefreshId) : this.HardInvalidate(requestContext);
            if (this.m_completedRefreshId == 0L)
              requestedRefreshId = 0L;
          }
        }
      }
      if (requestedRefreshId <= 0L)
        return;
      this.EnsureAsyncRefreshQueued(requestContext, requestedRefreshId);
    }

    protected long HardInvalidate(IVssRequestContext requestContext)
    {
      long num = Interlocked.Increment(ref this.m_softRefreshId);
      long comparand;
      do
      {
        comparand = Interlocked.Read(ref this.m_hardRefreshId);
      }
      while (comparand < num && comparand != Interlocked.CompareExchange(ref this.m_hardRefreshId, num, comparand));
      return num;
    }

    public bool PollForRequestLocalInvalidation(IVssRequestContext requestContext)
    {
      long num = 0;
      if (this.GetMinimumRefreshId(requestContext) != -2480L && this.m_settingsService.Settings.AllowPollForRequestLocalInvalidation)
      {
        TokenStoreSequenceId newer = this.m_backingStore.PollForSequenceId(requestContext);
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          if (this.m_sequenceId.IsSupersededBy(newer))
            num = Interlocked.Increment(ref this.m_softRefreshId);
        }
      }
      if (num > 0L)
        requestContext.Items[this.m_uniqueId] = (object) num;
      else if (requestContext.IsUserContext)
        requestContext.Items[this.m_uniqueId] = (object) -2480L;
      return true;
    }

    public long GetMinimumRefreshId(IVssRequestContext requestContext)
    {
      long minimumRefreshId = 0;
      object obj;
      if (requestContext.Items.TryGetValue(this.m_uniqueId, out obj))
        minimumRefreshId = (long) obj;
      return minimumRefreshId;
    }

    private SecurityEvaluator GetSecurityEvaluator(IVssRequestContext requestContext)
    {
      TokenStoreSequenceId sequenceId;
      ITokenStore<AccessControlListSlim> tokenStore;
      this.GetTokenStoreSnapshot(requestContext, this.GetMinimumRefreshId(requestContext), out sequenceId, out tokenStore);
      SecurityEvaluator securityEvaluator;
      if (!requestContext.Items.TryGetValue<SecurityEvaluator>(this.m_securityEvaluatorKey, out securityEvaluator) || securityEvaluator.SequenceId != sequenceId)
      {
        securityEvaluator = new SecurityEvaluator(this.m_description.NamespaceId, this.m_backingStore.AclStoreId, sequenceId, tokenStore, requestContext.GetService<IdentityService>().IdentityServiceInternal(), this.m_settingsService);
        requestContext.Items[this.m_securityEvaluatorKey] = (object) securityEvaluator;
      }
      return securityEvaluator;
    }

    protected void RefreshIfNeeded(IVssRequestContext requestContext) => this.GetTokenStore(requestContext, out TokenStoreSequenceId _, out ITokenStore<AccessControlListSlim> _);

    protected void GetTokenStoreSnapshot(
      IVssRequestContext requestContext,
      long minimumRefreshId,
      out TokenStoreSequenceId sequenceId,
      out ITokenStore<AccessControlListSlim> tokenStore)
    {
      requestContext.TraceEnter(56365, "Security", nameof (CachingAclStore), nameof (GetTokenStoreSnapshot));
      try
      {
        bool flag;
        long requestedRefreshId;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          sequenceId = this.m_sequenceId;
          tokenStore = this.m_tokenStore;
          flag = this.m_completedRefreshId < Math.Max(Interlocked.Read(ref this.m_hardRefreshId), minimumRefreshId) || this.m_backingStore.SupportsPollingInvalidation && this.m_sequenceId.IsSupersededBy(this.m_backingStore.PollForSequenceId(requestContext));
          if (!flag)
          {
            if (this.IsRefreshNeeded(requestContext, out requestedRefreshId))
              goto label_6;
          }
          requestedRefreshId = 0L;
        }
label_6:
        if (flag)
        {
          requestContext.Trace(56367, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "GetTokenStoreSnapshot: Synchronous refresh");
          this.GetTokenStore(requestContext, out sequenceId, out tokenStore);
        }
        else if (requestedRefreshId > 0L)
        {
          requestContext.Trace(56368, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "GetTokenStoreSnapshot: Ensuring refresh is queued for ID {0}", (object) requestedRefreshId);
          this.EnsureAsyncRefreshQueued(requestContext, requestedRefreshId);
        }
        else
          requestContext.Trace(56369, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "GetTokenStoreSnapshot: Up to date");
      }
      finally
      {
        requestContext.TraceLeave(56366, "Security", nameof (CachingAclStore), nameof (GetTokenStoreSnapshot));
      }
    }

    private void EnsureAsyncRefreshQueued(
      IVssRequestContext requestContext,
      long requestedRefreshId)
    {
      requestContext.TraceEnter(56360, "Security", nameof (CachingAclStore), nameof (EnsureAsyncRefreshQueued));
      try
      {
        bool flag = true;
        long comparand;
        do
        {
          comparand = Interlocked.Read(ref this.m_lastQueuedAsyncRefreshId);
          if (requestedRefreshId <= comparand)
          {
            requestContext.Trace(56362, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "EnsureAsyncRefreshQueued will not queue a refresh on the task service; requested refresh ID is {0} but {1} was observed as already queued", (object) requestedRefreshId, (object) comparand);
            flag = false;
            break;
          }
        }
        while (Interlocked.CompareExchange(ref this.m_lastQueuedAsyncRefreshId, requestedRefreshId, comparand) != comparand);
        if (!flag)
          return;
        requestContext.Trace(56363, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "EnsureAsyncRefreshQueued will queue a refresh on the task service; requested refresh ID is {0}, last queued was {1}", (object) requestedRefreshId, (object) comparand);
        this.m_taskService.AddTask(requestContext.ServiceHost.InstanceId, new TeamFoundationTask(new TeamFoundationTaskCallback(this.AsyncRefreshCallback), (object) requestedRefreshId, 0));
      }
      finally
      {
        requestContext.TraceLeave(56361, "Security", nameof (CachingAclStore), nameof (EnsureAsyncRefreshQueued));
      }
    }

    private void AsyncRefreshCallback(IVssRequestContext requestContext, object param)
    {
      long requestedRefreshId = (long) param;
      requestContext.TraceEnter(56370, "Security", nameof (CachingAclStore), nameof (AsyncRefreshCallback));
      try
      {
        bool flag;
        long scalarForDelta;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          flag = this.m_completedRefreshId < requestedRefreshId;
          scalarForDelta = this.m_sequenceId.ToScalarForDelta();
        }
        if (flag)
        {
          requestContext.Trace(56372, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "AsyncRefreshCallback will attempt a refresh for ID {0}", (object) requestedRefreshId);
          this.Refresh(requestContext, scalarForDelta, requestedRefreshId);
        }
        else
          requestContext.Trace(56373, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "AsyncRefreshCallback will not refresh; the observed completed refresh ID was greater or equal to {0}", (object) requestedRefreshId);
      }
      catch (HostShutdownException ex)
      {
        requestContext.Trace(56375, TraceLevel.Info, "Security", nameof (CachingAclStore), "AsyncRefreshCallback caught a HostShutdownException: " + ex.ToReadableStackTrace());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56374, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56371, "Security", nameof (CachingAclStore), nameof (AsyncRefreshCallback));
      }
    }

    protected bool IsRefreshNeeded(IVssRequestContext requestContext) => this.IsRefreshNeeded(requestContext, out long _);

    protected bool IsRefreshNeeded(IVssRequestContext requestContext, out long requestedRefreshId)
    {
      requestedRefreshId = Interlocked.Read(ref this.m_softRefreshId);
      if (requestedRefreshId > this.m_completedRefreshId)
        return true;
      if (((false ? 1 : (this.m_cacheLifetimeInMilliseconds <= 0U ? 0 : ((uint) Environment.TickCount - this.m_lastRefreshTickCount > this.m_cacheLifetimeInMilliseconds ? 1 : 0))) != 0 ? 1 : (!this.m_backingStore.SupportsPollingInvalidation ? 0 : (this.m_sequenceId.IsSupersededBy(this.m_backingStore.PollForSequenceId(requestContext)) ? 1 : 0))) == 0)
        return false;
      requestedRefreshId = this.EnsureRefreshIsOutstanding(requestContext, requestedRefreshId);
      return true;
    }

    private long EnsureRefreshIsOutstanding(
      IVssRequestContext requestContext,
      long requestedRefreshId)
    {
      if (requestedRefreshId == this.m_completedRefreshId)
      {
        ++requestedRefreshId;
        if (this.m_completedRefreshId != Interlocked.CompareExchange(ref this.m_softRefreshId, requestedRefreshId, this.m_completedRefreshId))
          requestedRefreshId = Interlocked.Read(ref this.m_softRefreshId);
      }
      return requestedRefreshId;
    }

    protected void GetTokenStore(
      IVssRequestContext requestContext,
      out TokenStoreSequenceId sequenceId,
      out ITokenStore<AccessControlListSlim> tokenStore)
    {
      requestContext.TraceEnter(56330, "Security", nameof (CachingAclStore), nameof (GetTokenStore));
      try
      {
        long scalarForDelta;
        long requestedRefreshId;
        using (requestContext.AcquireReaderLock(this.m_rwLockName))
        {
          scalarForDelta = this.m_sequenceId.ToScalarForDelta();
          if (!this.IsRefreshNeeded(requestContext, out requestedRefreshId))
          {
            sequenceId = this.m_sequenceId;
            tokenStore = this.m_tokenStore;
            requestContext.Trace(56334, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "GetTokenStore returning from first reader lock. Sequence ID: {0} Requested refresh ID: {1} Completed refresh ID: {2}", (object) sequenceId, (object) requestedRefreshId, (object) this.m_completedRefreshId);
            return;
          }
        }
        this.Refresh(requestContext, scalarForDelta, requestedRefreshId, out sequenceId, out tokenStore);
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56338, "Security", nameof (CachingAclStore), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56339, "Security", nameof (CachingAclStore), nameof (GetTokenStore));
      }
    }

    private void Refresh(
      IVssRequestContext requestContext,
      long oldSequenceId,
      long requestedRefreshId)
    {
      this.Refresh(requestContext, oldSequenceId, requestedRefreshId, out TokenStoreSequenceId _, out ITokenStore<AccessControlListSlim> _);
    }

    private void Refresh(
      IVssRequestContext requestContext,
      long oldSequenceId,
      long requestedRefreshId,
      out TokenStoreSequenceId sequenceId,
      out ITokenStore<AccessControlListSlim> tokenStore)
    {
      requestContext = requestContext.Elevate();
      long nextRequestedRefreshId;
      if (-1L == oldSequenceId)
      {
        bool flag = false;
        try
        {
          flag = this.m_initialLoadGate.Wait(requestContext.CancellationToken);
          if (this.InitialLoadGateWaiterCount != 0 && this.InitialLoadGateWaiterCount % 10 == 0)
            requestContext.TraceAlways(10390001, TraceLevel.Info, "Security", nameof (CachingAclStore), string.Format("Number of waiters outside of the local namespace initial load gate: {0}", (object) this.InitialLoadGateWaiterCount));
          this.RefreshHelper(requestContext, oldSequenceId, requestedRefreshId, out sequenceId, out tokenStore, out nextRequestedRefreshId);
        }
        finally
        {
          if (flag)
            this.m_initialLoadGate.Release();
        }
      }
      else
        this.RefreshHelper(requestContext, oldSequenceId, requestedRefreshId, out sequenceId, out tokenStore, out nextRequestedRefreshId);
      if (nextRequestedRefreshId <= requestedRefreshId)
        return;
      requestContext.Trace(56375, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "Refresh: Ensuring async refresh is queued for ID {0}", (object) nextRequestedRefreshId);
      this.EnsureAsyncRefreshQueued(requestContext, nextRequestedRefreshId);
    }

    private void RefreshHelper(
      IVssRequestContext requestContext,
      long oldSequenceId,
      long requestedRefreshId,
      out TokenStoreSequenceId sequenceId,
      out ITokenStore<AccessControlListSlim> tokenStore,
      out long nextRequestedRefreshId)
    {
      requestContext.TraceEnter(56060, "Security", nameof (CachingAclStore), nameof (RefreshHelper));
      try
      {
        int num = 2;
        IQuerySecurityDataResult securityDataResult;
        ITokenStore<AccessControlListSlim> tokenStore1;
        ITokenStore<AccessControlListSlim> tokenStore2;
        while (true)
        {
          using (requestContext.AcquireReaderLock(this.m_rwLockName))
          {
            if (this.m_completedRefreshId >= requestedRefreshId)
            {
              sequenceId = this.m_sequenceId;
              tokenStore = this.m_tokenStore;
              nextRequestedRefreshId = Interlocked.Read(ref this.m_softRefreshId);
              requestContext.Trace(56334, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "Refresh returning from first reader lock. Sequence ID: {0} Requested refresh ID: {1} Completed refresh ID: {2}", (object) sequenceId, (object) requestedRefreshId, (object) this.m_completedRefreshId);
              return;
            }
          }
          requestContext.Trace(56331, TraceLevel.Info, "Security", nameof (CachingAclStore), "Refresh going to backing store. Old sequence ID: {0} Requested refresh ID: {1} Retries remaining: {2}", (object) oldSequenceId, (object) requestedRefreshId, (object) num);
          CachingAclStore.s_refreshesPerSec.Increment();
          using (new TraceWatch(requestContext, 56342, TraceLevel.Error, TimeSpan.FromSeconds(60.0), "Security", nameof (CachingAclStore), "Namespace: '{0}' ({1}), Remoted: {2}, AclStoreId: {3}, OldSequenceId: {4}", new object[5]
          {
            (object) this.m_description.DisplayName,
            (object) this.m_description.NamespaceId,
            (object) this.m_description.IsRemoted,
            (object) this.m_backingStore.AclStoreId,
            (object) oldSequenceId
          }))
            securityDataResult = this.m_backingStore.QuerySecurityData(requestContext, oldSequenceId);
          if (requestContext.IsTracing(56337, TraceLevel.Info, "Security", nameof (CachingAclStore)))
            requestContext.Trace(56337, TraceLevel.Info, "Security", nameof (CachingAclStore), "Refresh of namespace {0}, ACL store {1} acquired {2} ACEs and {3} no-inherit tokens from the backing store. Old sequence ID: {4} New sequence ID: {5}", (object) this.m_description.NamespaceId.ToString("D"), (object) this.m_backingStore.AclStoreId.ToString("D"), (object) securityDataResult.AccessControlEntries.Count<BackingStoreAccessControlEntry>(), (object) securityDataResult.NoInheritTokens.Count<string>(), (object) securityDataResult.OldSequenceId, (object) securityDataResult.NewSequenceId);
          tokenStore1 = (ITokenStore<AccessControlListSlim>) null;
          TokenStoreSequenceId sequenceId1;
          using (requestContext.AcquireReaderLock(this.m_rwLockName))
          {
            if (this.m_completedRefreshId >= requestedRefreshId)
            {
              sequenceId = this.m_sequenceId;
              tokenStore = this.m_tokenStore;
              nextRequestedRefreshId = Interlocked.Read(ref this.m_softRefreshId);
              requestContext.Trace(56334, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "Refresh returning from second reader lock. Sequence ID: {0} Requested refresh ID: {1} Completed refresh ID: {2}", (object) sequenceId, (object) requestedRefreshId, (object) this.m_completedRefreshId);
              return;
            }
            tokenStore2 = this.m_tokenStore;
            sequenceId1 = this.m_sequenceId;
          }
          if (sequenceId1.IsSupersededBy(securityDataResult.NewSequenceId))
          {
            requestContext.Trace(56332, TraceLevel.Info, "Security", nameof (CachingAclStore), "Refresh would like to commit data from backing store. Old sequence ID: {0} New sequence ID: {1} Current sequence ID: {2} Requested refresh ID: {3}", (object) securityDataResult.OldSequenceId, (object) securityDataResult.NewSequenceId, (object) sequenceId1, (object) requestedRefreshId);
            if (-1L != securityDataResult.OldSequenceId)
            {
              if (!(sequenceId1 == (TokenStoreSequenceId) securityDataResult.OldSequenceId))
              {
                if (num > 0)
                {
                  --num;
                  requestContext.Trace(56335, TraceLevel.Info, "Security", nameof (CachingAclStore), "Refresh cannot apply this delta; spinning. Old sequence ID: {0} New sequence ID: {1} Current sequence ID: {2} Retries remaining after this spin: {3}", (object) securityDataResult.OldSequenceId, (object) securityDataResult.NewSequenceId, (object) sequenceId1, (object) num);
                  oldSequenceId = sequenceId1.ToScalarForDelta();
                }
                else
                {
                  requestContext.Trace(56336, TraceLevel.Warning, "Security", nameof (CachingAclStore), "Refresh ran out of retries; performing full reload. Old sequence ID: {0} New sequence ID: {1} Current sequence ID: {2}", (object) securityDataResult.OldSequenceId, (object) securityDataResult.NewSequenceId, (object) sequenceId1);
                  oldSequenceId = -1L;
                }
              }
              else
                goto label_22;
            }
            else
              break;
          }
          else
            goto label_27;
        }
        tokenStore1 = CachingAclStore.CreateTokenStore(this.m_description);
        goto label_26;
label_22:
        tokenStore1 = tokenStore2.Copy(requestContext);
label_26:
        this.ApplyDeltaToTokenStore(requestContext, tokenStore1, securityDataResult.AccessControlEntries, securityDataResult.NoInheritTokens);
label_27:
        using (requestContext.AcquireWriterLock(this.m_rwLockName))
        {
          if (this.m_completedRefreshId >= requestedRefreshId)
          {
            sequenceId = this.m_sequenceId;
            tokenStore = this.m_tokenStore;
            nextRequestedRefreshId = this.m_softRefreshId;
            requestContext.Trace(56334, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "Refresh returning from writer lock entry. Sequence ID: {0} Requested refresh ID: {1} Completed refresh ID: {2}", (object) sequenceId, (object) requestedRefreshId, (object) this.m_completedRefreshId);
          }
          else
          {
            if (this.m_sequenceId.IsSupersededBy(securityDataResult.NewSequenceId))
              this.SetTokenStore(requestContext, securityDataResult.NewSequenceId, tokenStore1);
            this.m_completedRefreshId = requestedRefreshId;
            this.m_lastRefreshTickCount = (uint) Environment.TickCount;
            sequenceId = this.m_sequenceId;
            tokenStore = this.m_tokenStore;
            nextRequestedRefreshId = this.m_softRefreshId;
            requestContext.Trace(56334, TraceLevel.Verbose, "Security", nameof (CachingAclStore), "Refresh returning from writer lock exit. Sequence ID: {0} Requested refresh ID: {1} Completed refresh ID: {2}", (object) sequenceId, (object) requestedRefreshId, (object) this.m_completedRefreshId);
          }
        }
      }
      finally
      {
        requestContext.TraceLeave(56061, "Security", nameof (CachingAclStore), nameof (RefreshHelper));
      }
    }

    private void ApplyDeltaToTokenStore(
      IVssRequestContext requestContext,
      ITokenStore<AccessControlListSlim> tokenStore,
      IEnumerable<BackingStoreAccessControlEntry> aces,
      IEnumerable<string> noInheritTokens)
    {
      requestContext.TraceEnter(56064, "Security", nameof (CachingAclStore), nameof (ApplyDeltaToTokenStore));
      try
      {
        HashSet<string> stringSet = (HashSet<string>) null;
        AccessControlListSlim accessControlListSlim = (AccessControlListSlim) null;
        aces = aces.Where<BackingStoreAccessControlEntry>((Func<BackingStoreAccessControlEntry, bool>) (s => this.IsValidToken(s.Token, true)));
        noInheritTokens = noInheritTokens.Where<string>((Func<string, bool>) (s => this.IsValidToken(s)));
        if (tokenStore.Count > 0)
        {
          stringSet = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
          foreach (BackingStoreAccessControlEntry ace in aces)
            stringSet.Add(this.CanonicalizeToken(ace.Token));
        }
        Func<string, AccessControlListSlim> valueFactory = (Func<string, AccessControlListSlim>) (valueToken => new AccessControlListSlim(valueToken, this.m_isHierarchical));
        foreach (BackingStoreAccessControlEntry ace in aces)
        {
          if (!ace.IsDeleted)
          {
            string str = this.CanonicalizeToken(ace.Token);
            if (accessControlListSlim == null || !StringComparer.OrdinalIgnoreCase.Equals(accessControlListSlim.Token, str))
            {
              accessControlListSlim?.SortAndRemoveDuplicates();
              if (stringSet != null && stringSet.Remove(str))
                tokenStore[str] = new AccessControlListSlim(str, this.m_isHierarchical);
              accessControlListSlim = tokenStore.GetOrAdd<string>(str, valueFactory, str);
            }
            accessControlListSlim.UnsafeAddUnsorted(new AccessControlEntrySlim(ace.Subject, ace.Allow & ~ace.Deny, ace.Deny));
          }
        }
        accessControlListSlim?.SortAndRemoveDuplicates();
        if (stringSet != null)
        {
          foreach (string token in stringSet)
            tokenStore.Remove(token, false);
        }
        if (!this.m_isHierarchical)
          return;
        foreach (string noInheritToken in noInheritTokens)
        {
          string token = this.CanonicalizeToken(noInheritToken);
          AccessControlListSlim referencedObject;
          if (!tokenStore.TryGetValue(token, out referencedObject))
            tokenStore[token] = new AccessControlListSlim(token, false);
          else
            referencedObject.InheritPermissions = false;
        }
      }
      finally
      {
        requestContext.TraceLeave(56065, "Security", nameof (CachingAclStore), nameof (ApplyDeltaToTokenStore));
      }
    }

    protected static ITokenStore<AccessControlListSlim> CreateTokenStore(
      SecurityNamespaceDescription description)
    {
      switch (description.NamespaceStructure)
      {
        case SecurityNamespaceStructure.Flat:
          return (ITokenStore<AccessControlListSlim>) new FlatStore<AccessControlListSlim>(StringComparison.OrdinalIgnoreCase);
        case SecurityNamespaceStructure.Hierarchical:
          return (ITokenStore<AccessControlListSlim>) new HierarchicalStore<AccessControlListSlim>(description.SeparatorValue, description.ElementLength, StringComparison.OrdinalIgnoreCase);
        default:
          throw new ArgumentOutOfRangeException("NamespaceStructure");
      }
    }

    protected bool IsValidToken(string token, bool allowNull = false)
    {
      if (token == null)
        return allowNull;
      return this.m_description.ElementLength == -1 || token.Length <= 0 || token.Length % this.m_description.ElementLength == 0;
    }

    protected void CheckValidToken(string token, bool allowNull = false)
    {
      if (token == null)
      {
        if (!allowNull)
          throw new ArgumentNullException(nameof (token));
      }
      else if (this.m_description.ElementLength != -1 && token.Length > 0 && token.Length % this.m_description.ElementLength != 0)
        throw new InvalidSecurityTokenException(FrameworkResources.InvalidSecurityTokenElementLength((object) token, (object) this.m_description.ElementLength));
    }

    protected string CheckAndCanonicalizeToken(string token, bool allowNull = false)
    {
      this.CheckValidToken(token, allowNull);
      return token == null ? (string) null : this.CanonicalizeToken(token);
    }

    protected string CanonicalizeToken(string token) => SecurityServiceHelpers.CanonicalizeToken(this.m_description, token);

    protected void ValidateRequestContext(IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      if (this.m_serviceHostInstanceId != requestContext.ServiceHost.InstanceId)
        throw new InvalidRequestContextHostException(FrameworkResources.SecurityNamespaceRequestContextHostMessage((object) this.m_description.DisplayName, (object) this.m_serviceHostInstanceId, (object) requestContext.ServiceHost.InstanceId));
    }

    protected void SetTokenStore(
      IVssRequestContext requestContext,
      TokenStoreSequenceId newSequenceId,
      ITokenStore<AccessControlListSlim> newTokenStore)
    {
      this.m_sequenceId = newSequenceId;
      this.m_tokenStore = newTokenStore;
      this.m_tokenStore.Seal();
      this.m_lastRefreshTickCount = (uint) Environment.TickCount;
    }

    protected IEnumerable<IdentityDescriptor> MapToWellKnownIdentifiers(
      IEnumerable<IdentityDescriptor> descriptors)
    {
      return descriptors != null ? descriptors.Select<IdentityDescriptor, IdentityDescriptor>((Func<IdentityDescriptor, IdentityDescriptor>) (s => this.m_identityMapper.MapToWellKnownIdentifier(s))) : (IEnumerable<IdentityDescriptor>) null;
    }

    protected IEnumerable<IAccessControlEntry> MapToWellKnownIdentifiers(
      IEnumerable<IAccessControlEntry> aces)
    {
      return aces != null ? aces.Select<IAccessControlEntry, IAccessControlEntry>((Func<IAccessControlEntry, IAccessControlEntry>) (ace =>
      {
        if (ace == null)
          throw new ArgumentNullException(nameof (ace));
        if ((IdentityDescriptor) null == ace.Descriptor)
          throw new ArgumentNullException("Descriptor");
        IdentityDescriptor wellKnownIdentifier = this.m_identityMapper.MapToWellKnownIdentifier(ace.Descriptor);
        if ((object) wellKnownIdentifier != (object) ace.Descriptor)
        {
          ace = ace.Clone();
          ace.Descriptor = wellKnownIdentifier;
        }
        return ace;
      })) : (IEnumerable<IAccessControlEntry>) null;
    }

    protected EvaluationPrincipal MapToWellKnownIdentifier(EvaluationPrincipal evaluationPrincipal) => evaluationPrincipal?.ToWellKnownEvaluationPrincipal(this.m_identityMapper);

    protected IEnumerable<EvaluationPrincipal> MapToWellKnownIdentifiers(
      IEnumerable<EvaluationPrincipal> evaluationPrincipals)
    {
      return evaluationPrincipals != null ? evaluationPrincipals.Select<EvaluationPrincipal, EvaluationPrincipal>((Func<EvaluationPrincipal, EvaluationPrincipal>) (s => this.MapToWellKnownIdentifier(s))) : (IEnumerable<EvaluationPrincipal>) null;
    }

    protected IEnumerable<IAccessControlList> MapToWellKnownIdentifiersAndCanonicalize(
      IEnumerable<IAccessControlList> acls)
    {
      return acls != null ? (IEnumerable<IAccessControlList>) acls.Select<IAccessControlList, AccessControlList>((Func<IAccessControlList, AccessControlList>) (acl =>
      {
        AccessControlList acl1 = new AccessControlList(this.CanonicalizeToken(acl.Token), acl.InheritPermissions);
        acl1.SetAccessControlEntries(this.MapToWellKnownIdentifiers(acl.AccessControlEntries), false);
        return acl1;
      })) : (IEnumerable<IAccessControlList>) null;
    }
  }
}
