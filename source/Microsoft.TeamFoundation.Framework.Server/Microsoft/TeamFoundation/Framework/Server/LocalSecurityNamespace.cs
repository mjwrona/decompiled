// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.LocalSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Messages;
using Microsoft.VisualStudio.Services.Security.Server;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class LocalSecurityNamespace : CompositeSecurityNamespace, ILocalSecurityNamespace
  {
    private readonly ILockName m_aclStoresLock;
    private LocalSecurityNamespace.LocalCompositeAclStores m_aclStores;
    private const string c_area = "Security";
    private const string c_layer = "LocalSecurityNamespace";

    public LocalSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description,
      ISecurityNamespaceExtension extension,
      ISecurityDataspaceMapper dataspaceMapper)
      : base(requestContext, description, extension)
    {
      this.DataspaceMapper = dataspaceMapper;
      this.m_aclStoresLock = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}", (object) this.GetType().FullName, (object) "aclStores"));
    }

    internal void RemoveIdentityACEs(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
    {
      this.GetLocalAclStores(requestContext).MutableStore.RemoveIdentityACEs(requestContext, identities);
    }

    internal void RemoveAllAccessControlLists(IVssRequestContext requestContext) => this.GetLocalAclStores(requestContext).MutableStore.RemoveAllAccessControlLists(requestContext);

    public TokenStoreSequenceId GetCurrentSequenceId(IVssRequestContext requestContext) => this.GetLocalAclStores(requestContext).PrimaryQueryableStore.GetCurrentSequenceId(requestContext);

    internal bool IsRefreshPending(IVssRequestContext requestContext) => this.GetLocalAclStores(requestContext).PrimaryQueryableStore.IsRefreshPending(requestContext);

    internal TokenStoreSequenceId ReadCurrentSequenceId(IVssRequestContext requestContext) => this.GetLocalAclStores(requestContext).PrimaryQueryableStore.ReadCurrentSequenceId(requestContext);

    public override void OnDataChanged(IVssRequestContext requestContext)
    {
      LocalSecurityNamespace.LocalCompositeAclStores localAclStores = this.GetLocalAclStores(requestContext);
      CompositeSecurityNamespace.RelayOnDataChanged(requestContext, (CompositeSecurityNamespace.CompositeAclStores) localAclStores);
      localAclStores.MutableStore.EnqueueCacheInvalidation(requestContext, TokenStoreSequenceId.UnconditionalRefresh);
    }

    public ISecurityDataspaceMapper DataspaceMapper { get; private set; }

    internal IReadOnlyDictionary<Guid, CachingAclStore> GetRemotableAclStores(
      IVssRequestContext requestContext)
    {
      return this.GetLocalAclStores(requestContext).RemotableStores;
    }

    protected LocalSecurityNamespace.LocalCompositeAclStores GetLocalAclStores(
      IVssRequestContext requestContext)
    {
      return (LocalSecurityNamespace.LocalCompositeAclStores) this.GetAclStores(requestContext);
    }

    protected override CompositeSecurityNamespace.CompositeAclStores GetAclStores(
      IVssRequestContext requestContext)
    {
      LocalSecurityNamespace.LocalCompositeAclStores aclStores1 = this.m_aclStores;
      if (aclStores1 != null)
        return (CompositeSecurityNamespace.CompositeAclStores) aclStores1;
      LocalSecurityNamespace.ILocalUserBackingStore backingStore = !requestContext.IsVirtualServiceHost() ? (LocalSecurityNamespace.ILocalUserBackingStore) new LocalSecurityNamespace.LocalUserBackingStore(requestContext, this.m_description, this.DataspaceMapper) : (LocalSecurityNamespace.ILocalUserBackingStore) new LocalSecurityNamespace.VirtualLocalUserBackingStore();
      LocalSecurityNamespace.LocalUserAclStore localUserAclStore = new LocalSecurityNamespace.LocalUserAclStore(requestContext, this.m_description, backingStore, 0);
      CachingAclStore cachingAclStore = new CachingAclStore(requestContext, this.m_description, (ISecurityNamespaceBackingStore) new LocalSecurityNamespace.LocalSystemBackingStore(requestContext, this.m_description), 0);
      LocalSecurityNamespace.LocalCompositeAclStores aclStores2 = new LocalSecurityNamespace.LocalCompositeAclStores((IEnumerable<IQueryableAclStore>) new CachingAclStore[2]
      {
        cachingAclStore,
        (CachingAclStore) localUserAclStore
      }, localUserAclStore, localUserAclStore, (IEnumerable<CachingAclStore>) new CachingAclStore[2]
      {
        cachingAclStore,
        (CachingAclStore) localUserAclStore
      });
      using (requestContext.Lock(this.m_aclStoresLock))
      {
        if (this.m_aclStores == null)
          this.m_aclStores = aclStores2;
        else
          aclStores2 = this.m_aclStores;
      }
      return (CompositeSecurityNamespace.CompositeAclStores) aclStores2;
    }

    protected class LocalCompositeAclStores : CompositeSecurityNamespace.CompositeAclStores
    {
      public readonly IReadOnlyDictionary<Guid, CachingAclStore> RemotableStores;

      public LocalCompositeAclStores(
        IEnumerable<IQueryableAclStore> queryableStores,
        LocalSecurityNamespace.LocalUserAclStore primaryQueryableStore,
        LocalSecurityNamespace.LocalUserAclStore mutableStore,
        IEnumerable<CachingAclStore> remotableStores)
        : base(queryableStores, (IQueryableAclStore) primaryQueryableStore, (IMutableAclStore) mutableStore)
      {
        this.RemotableStores = (IReadOnlyDictionary<Guid, CachingAclStore>) remotableStores.ToDictionary<CachingAclStore, Guid>((Func<CachingAclStore, Guid>) (s => s.AclStoreId));
      }

      public LocalSecurityNamespace.LocalUserAclStore PrimaryQueryableStore => (LocalSecurityNamespace.LocalUserAclStore) this.PrimaryQueryableStore;

      public LocalSecurityNamespace.LocalUserAclStore MutableStore => (LocalSecurityNamespace.LocalUserAclStore) this.MutableStore;
    }

    protected class LocalUserAclStore : CachingAclStore
    {
      public LocalUserAclStore(
        IVssRequestContext requestContext,
        SecurityNamespaceDescription description,
        LocalSecurityNamespace.ILocalUserBackingStore backingStore,
        int cacheLifetimeInMilliseconds)
        : base(requestContext, description, (ISecurityNamespaceBackingStore) backingStore, cacheLifetimeInMilliseconds)
      {
      }

      public void EnqueueCacheInvalidation(
        IVssRequestContext requestContext,
        TokenStoreSequenceId newSequenceId)
      {
        ((LocalSecurityNamespace.ILocalUserBackingStore) this.m_backingStore).EnqueueCacheInvalidation(requestContext, newSequenceId);
      }

      public void RemoveIdentityACEs(
        IVssRequestContext requestContext,
        IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity> identities)
      {
        requestContext.TraceEnter(56270, "Security", nameof (LocalSecurityNamespace), nameof (RemoveIdentityACEs));
        try
        {
          ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) identities, nameof (identities));
          foreach (Microsoft.VisualStudio.Services.Identity.Identity identity in identities)
            ArgumentUtility.CheckForNull<Microsoft.VisualStudio.Services.Identity.Identity>(identity, "identity");
          Dictionary<Guid, IdentityDescriptor> dictionary = identities.ToDictionary<Microsoft.VisualStudio.Services.Identity.Identity, Guid, IdentityDescriptor>((Func<Microsoft.VisualStudio.Services.Identity.Identity, Guid>) (s => s.Id), (Func<Microsoft.VisualStudio.Services.Identity.Identity, IdentityDescriptor>) (s => this.m_identityMapper.MapToWellKnownIdentifier(s.Descriptor)));
          this.RefreshIfNeeded(requestContext);
          List<RemovedAccessControlEntry> removed;
          TokenStoreSequenceId tokenStoreSequenceId = this.LocalBackingStore.RemoveIdentityACEs(requestContext, (IEnumerable<Guid>) dictionary.Keys, out removed);
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
            foreach (RemovedAccessControlEntry accessControlEntry in removed)
            {
              AccessControlListSlim referencedObject;
              if (newTokenStore.TryGetValue(accessControlEntry.Token, out referencedObject))
              {
                referencedObject = new AccessControlListSlim(referencedObject);
                referencedObject.RemoveAccessControlEntry(dictionary[accessControlEntry.TeamFoundationId]);
                if (!referencedObject.AccessControlEntries.Any<AccessControlEntrySlim>() && (referencedObject.InheritPermissions || !this.m_isHierarchical))
                  newTokenStore.Remove(accessControlEntry.Token, false);
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
            using (requestContext.AcquireReaderLock(this.m_rwLockName))
              this.HardInvalidate(requestContext);
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(56278, "Security", nameof (LocalSecurityNamespace), ex);
          throw;
        }
        finally
        {
          requestContext.TraceLeave(56279, "Security", nameof (LocalSecurityNamespace), nameof (RemoveIdentityACEs));
        }
      }

      public void RemoveAllAccessControlLists(IVssRequestContext requestContext)
      {
        requestContext.TraceEnter(56260, "Security", nameof (LocalSecurityNamespace), nameof (RemoveAllAccessControlLists));
        try
        {
          this.LocalBackingStore.RemoveAllAccessControlLists(requestContext);
          this.NotifyChanged(requestContext, TokenStoreSequenceId.DropCache, false);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(56268, "Security", nameof (LocalSecurityNamespace), ex);
          throw;
        }
        finally
        {
          requestContext.TraceLeave(56269, "Security", nameof (LocalSecurityNamespace), nameof (RemoveAllAccessControlLists));
        }
      }

      private LocalSecurityNamespace.ILocalUserBackingStore LocalBackingStore => (LocalSecurityNamespace.ILocalUserBackingStore) this.m_backingStore;
    }

    protected interface ILocalUserBackingStore : ISecurityNamespaceBackingStore
    {
      void RemoveAllAccessControlLists(IVssRequestContext requestContext);

      TokenStoreSequenceId RemoveIdentityACEs(
        IVssRequestContext requestContext,
        IEnumerable<Guid> tfIds,
        out List<RemovedAccessControlEntry> removed);

      void EnqueueCacheInvalidation(
        IVssRequestContext requestContext,
        TokenStoreSequenceId newSequenceId);
    }

    protected sealed class VirtualLocalUserBackingStore : 
      LocalSecurityNamespace.ILocalUserBackingStore,
      ISecurityNamespaceBackingStore
    {
      private static readonly TokenStoreSequenceId s_virtualHostSequenceId = (TokenStoreSequenceId) 39L;

      public Guid AclStoreId => WellKnownAclStores.User;

      public IQuerySecurityDataResult QuerySecurityData(
        IVssRequestContext requestContext,
        long oldSequenceId)
      {
        return (IQuerySecurityDataResult) new CompositeSecurityNamespace.QuerySecurityDataResult(this.AclStoreId, -1L, LocalSecurityNamespace.VirtualLocalUserBackingStore.s_virtualHostSequenceId, (IEnumerable<BackingStoreAccessControlEntry>) Array.Empty<BackingStoreAccessControlEntry>(), (IEnumerable<string>) Array.Empty<string>());
      }

      public SecurityBackingStoreChangedEventHandler WeakSubscribeToPushInvalidations(
        IVssRequestContext requestContext,
        SecurityBackingStoreChangedEventHandler eventHandler)
      {
        return (SecurityBackingStoreChangedEventHandler) null;
      }

      public bool SupportsPollingInvalidation => true;

      public TokenStoreSequenceId PollForSequenceId(IVssRequestContext requestContext) => LocalSecurityNamespace.VirtualLocalUserBackingStore.s_virtualHostSequenceId;

      public TokenStoreSequenceId SetPermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<IAccessControlEntry> permissions,
        bool merge,
        bool throwOnInvalidIdentity)
      {
        throw new VirtualServiceHostException();
      }

      public TokenStoreSequenceId SetAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<IAccessControlList> acls,
        bool throwOnInvalidIdentity)
      {
        throw new VirtualServiceHostException();
      }

      public TokenStoreSequenceId SetInheritFlag(
        IVssRequestContext requestContext,
        string token,
        bool inheritFlag)
      {
        throw new VirtualServiceHostException();
      }

      public TokenStoreSequenceId RemovePermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<Guid> identityIds)
      {
        throw new VirtualServiceHostException();
      }

      public void RemoveAllAccessControlLists(IVssRequestContext requestContext) => throw new VirtualServiceHostException();

      public TokenStoreSequenceId RemoveAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<string> tokens,
        bool recurse)
      {
        throw new VirtualServiceHostException();
      }

      public TokenStoreSequenceId RenameTokens(
        IVssRequestContext requestContext,
        IEnumerable<TokenRename> renames)
      {
        throw new VirtualServiceHostException();
      }

      public TokenStoreSequenceId RemoveIdentityACEs(
        IVssRequestContext requestContext,
        IEnumerable<Guid> tfIds,
        out List<RemovedAccessControlEntry> removed)
      {
        throw new VirtualServiceHostException();
      }

      public void NotifyChanged(
        IVssRequestContext requestContext,
        TokenStoreSequenceId newSequenceId,
        bool hardInvalidate = false)
      {
      }

      public void EnqueueCacheInvalidation(
        IVssRequestContext requestContext,
        TokenStoreSequenceId newSequenceId)
      {
      }
    }

    protected internal sealed class LocalUserBackingStore : 
      LocalSecurityNamespace.ILocalUserBackingStore,
      ISecurityNamespaceBackingStore
    {
      private readonly SecurityNamespaceDescription m_description;
      private readonly ISecurityDataspaceMapper m_dataspaceMapper;
      private readonly SecuritySettingsService m_settingsService;
      private static readonly VssPerformanceCounter s_refreshesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_LocalSecurityRefreshesPerSec");
      private const string c_area = "Security";
      private const string c_layer = "LocalUserBackingStore";

      public LocalUserBackingStore(
        IVssRequestContext requestContext,
        SecurityNamespaceDescription description,
        ISecurityDataspaceMapper dataspaceMapper)
      {
        ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(description, nameof (description));
        this.m_description = description;
        this.m_dataspaceMapper = dataspaceMapper;
        this.m_settingsService = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>();
      }

      public Guid AclStoreId => WellKnownAclStores.User;

      internal LocalSecurityNamespace.LocalUserBackingStore.RawQuerySecurityDataResult QuerySecurityDataRaw(
        IVssRequestContext requestContext,
        long oldSequenceId)
      {
        LocalSecurityNamespace.LocalUserBackingStore.s_refreshesPerSec.Increment();
        bool usesInheritInformation = this.m_description.NamespaceStructure == SecurityNamespaceStructure.Hierarchical;
        List<DatabaseAccessControlEntry> accessControlEntries = new List<DatabaseAccessControlEntry>();
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        HashSet<Guid> hashSet = vssRequestContext.GetService<IVssSecuritySubjectService>().GetSecuritySubjectEntries(vssRequestContext).Where<SecuritySubjectEntry>((Func<SecuritySubjectEntry, bool>) (s => s.SubjectType != SecuritySubjectType.ServicePrincipal)).ToHashSet<SecuritySubjectEntry, Guid>((Func<SecuritySubjectEntry, Guid>) (s => s.Id));
        IEnumerable<string> noInheritTokens;
        int newSequenceId;
        using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
        {
          using (ResultCollection resultCollection = securityComponent.QuerySecurityData(this.m_description.NamespaceId, usesInheritInformation, checked ((int) oldSequenceId), this.m_description.SeparatorValue))
          {
            foreach (DatabaseAccessControlEntry accessControlEntry in resultCollection.GetCurrent<DatabaseAccessControlEntry>())
            {
              if (hashSet.Contains(accessControlEntry.SubjectId))
              {
                requestContext.Trace(56282, TraceLevel.Error, "Security", nameof (LocalUserBackingStore), "System store only entry is found in the user story of namespace " + this.m_description.DisplayName + ". Violating entry: " + accessControlEntry.ToString() + ".");
              }
              else
              {
                int allow = accessControlEntry.Allow & ~this.m_description.SystemBitMask;
                int deny = accessControlEntry.Deny & ~this.m_description.SystemBitMask;
                if (allow != accessControlEntry.Allow || deny != accessControlEntry.Deny)
                  requestContext.Trace(56281, TraceLevel.Error, "Security", nameof (LocalUserBackingStore), string.Format("SystemBitMask failed to mask reserved bits {0} in namespace {1}. Violating entry: {2}.", (object) this.m_description.SystemBitMask, (object) this.m_description.DisplayName, (object) accessControlEntry.ToString()));
                accessControlEntries.Add(new DatabaseAccessControlEntry(accessControlEntry.SubjectId, accessControlEntry.Token, allow, deny, accessControlEntry.IsDeleted));
              }
            }
            if (usesInheritInformation)
            {
              resultCollection.NextResult();
              noInheritTokens = (IEnumerable<string>) resultCollection.GetCurrent<string>().Items;
            }
            else
              noInheritTokens = Enumerable.Empty<string>();
            resultCollection.NextResult();
            newSequenceId = resultCollection.GetCurrent<int>().First<int>();
          }
        }
        return new LocalSecurityNamespace.LocalUserBackingStore.RawQuerySecurityDataResult(WellKnownAclStores.User, oldSequenceId, (TokenStoreSequenceId) (long) newSequenceId, accessControlEntries, noInheritTokens);
      }

      public IQuerySecurityDataResult QuerySecurityData(
        IVssRequestContext requestContext,
        long oldSequenceId)
      {
        LocalSecurityNamespace.LocalUserBackingStore.RawQuerySecurityDataResult securityDataResult = this.QuerySecurityDataRaw(requestContext, oldSequenceId);
        List<DatabaseAccessControlEntry> accessControlEntries = securityDataResult.AccessControlEntries;
        IDictionary<Guid, IdentityDescriptor> dictionary = BackingStoreAccessControlEntryHelpers.BuildIdentityMap(requestContext, this.m_description.NamespaceId, accessControlEntries.Where<DatabaseAccessControlEntry>((Func<DatabaseAccessControlEntry, bool>) (ace => ace.SubjectId != Guid.Empty)).Select<DatabaseAccessControlEntry, Guid>((Func<DatabaseAccessControlEntry, Guid>) (ace => ace.SubjectId)));
        List<BackingStoreAccessControlEntry> accessControlEntryList = new List<BackingStoreAccessControlEntry>(accessControlEntries.Count);
        foreach (DatabaseAccessControlEntry accessControlEntry in accessControlEntries)
        {
          IdentityDescriptor subject;
          if (accessControlEntry.IsDeleted && Guid.Empty == accessControlEntry.SubjectId)
            subject = (IdentityDescriptor) null;
          else if (!dictionary.TryGetValue(accessControlEntry.SubjectId, out subject) || (IdentityDescriptor) null == subject)
            continue;
          accessControlEntryList.Add(new BackingStoreAccessControlEntry(subject, accessControlEntry.Token, accessControlEntry.Allow, accessControlEntry.Deny, accessControlEntry.IsDeleted));
        }
        return (IQuerySecurityDataResult) new CompositeSecurityNamespace.QuerySecurityDataResult(securityDataResult.AclStoreId, securityDataResult.OldSequenceId, securityDataResult.NewSequenceId, (IEnumerable<BackingStoreAccessControlEntry>) accessControlEntryList, securityDataResult.NoInheritTokens);
      }

      public SecurityBackingStoreChangedEventHandler WeakSubscribeToPushInvalidations(
        IVssRequestContext requestContext,
        SecurityBackingStoreChangedEventHandler eventHandler)
      {
        requestContext.GetService<LocalSecurityInvalidationService>().Register(requestContext, this.m_description, eventHandler);
        return eventHandler;
      }

      public bool SupportsPollingInvalidation => false;

      public TokenStoreSequenceId PollForSequenceId(IVssRequestContext requestContext)
      {
        using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
          return (TokenStoreSequenceId) (long) securityComponent.QuerySequenceId(this.m_description.NamespaceId);
      }

      public TokenStoreSequenceId SetPermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<IAccessControlEntry> permissions,
        bool merge,
        bool throwOnInvalidIdentity)
      {
        this.EnsureDataspaceMapper();
        bool droppedAtLeastOneAce;
        List<DatabaseAccessControlEntry> backingStoreAces = this.GetBackingStoreAces(requestContext, (IEnumerable<IAccessControlList>) new AccessControlList[1]
        {
          new AccessControlList(token, true, permissions)
        }, (merge ? 1 : 0) != 0, (throwOnInvalidIdentity ? 1 : 0) != 0, out droppedAtLeastOneAce);
        TokenStoreSequenceId newSequenceId = TokenStoreSequenceId.NoWorkPerformed;
        if (backingStoreAces.Count > 0)
        {
          using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
            newSequenceId = (TokenStoreSequenceId) (long) securityComponent.SetPermissions(this.m_description.NamespaceId, token, (IEnumerable<DatabaseAccessControlEntry>) backingStoreAces, merge, this.m_description.SeparatorValue);
          this.EnqueueCacheInvalidation(requestContext, newSequenceId);
          if (droppedAtLeastOneAce)
            newSequenceId = TokenStoreSequenceId.UnconditionalRefresh;
        }
        if (!permissions.All<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (ace => ace.IsEmpty)))
          requestContext.LogAuditEvent(SecurityAuditLogConstants.ModifyPermission, SecurityAuditHelper.SecurityData(requestContext, this.m_description, token, permissions));
        return newSequenceId;
      }

      public TokenStoreSequenceId SetAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<IAccessControlList> acls,
        bool throwOnInvalidIdentity)
      {
        this.EnsureDataspaceMapper();
        bool droppedAtLeastOneAce;
        List<DatabaseAccessControlEntry> backingStoreAces = this.GetBackingStoreAces(requestContext, acls, false, throwOnInvalidIdentity, out droppedAtLeastOneAce);
        TokenStoreSequenceId newSequenceId;
        using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
          newSequenceId = (TokenStoreSequenceId) (long) securityComponent.SetAccessControlLists(this.m_description.NamespaceId, acls, (IEnumerable<DatabaseAccessControlEntry>) backingStoreAces, this.m_description.SeparatorValue, this.m_description.NamespaceStructure);
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        if (droppedAtLeastOneAce)
          newSequenceId = TokenStoreSequenceId.UnconditionalRefresh;
        requestContext.LogAuditEvent(SecurityAuditHelper.GetActionTypeForResetOrModify(acls), SecurityAuditHelper.SecurityData(requestContext, this.m_description, acls));
        return newSequenceId;
      }

      public TokenStoreSequenceId SetInheritFlag(
        IVssRequestContext requestContext,
        string token,
        bool inheritFlag)
      {
        this.EnsureDataspaceMapper();
        TokenStoreSequenceId newSequenceId;
        using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
          newSequenceId = (TokenStoreSequenceId) (long) securityComponent.SetInheritFlag(this.m_description.NamespaceId, token, inheritFlag, this.m_description.SeparatorValue);
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        return newSequenceId;
      }

      public TokenStoreSequenceId RemovePermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<Guid> identityIds)
      {
        this.EnsureDataspaceMapper();
        TokenStoreSequenceId newSequenceId;
        using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
          newSequenceId = (TokenStoreSequenceId) (long) securityComponent.RemovePermissions(this.m_description.NamespaceId, token, identityIds, this.m_description.SeparatorValue);
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        requestContext.LogAuditEvent(SecurityAuditLogConstants.RemovePermission, SecurityAuditHelper.SecurityData(this.m_description, token, identityIds));
        return newSequenceId;
      }

      public void RemoveAllAccessControlLists(IVssRequestContext requestContext)
      {
        this.EnsureDataspaceMapper();
        using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
          securityComponent.RemoveAllAccessControlLists(this.m_description.NamespaceId);
        this.EnqueueCacheInvalidation(requestContext, TokenStoreSequenceId.DropCache);
        requestContext.LogAuditEvent(SecurityAuditLogConstants.RemoveAllAccessControlLists, SecurityAuditHelper.SecurityData(this.m_description));
      }

      public TokenStoreSequenceId RemoveAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<string> tokens,
        bool recurse)
      {
        this.EnsureDataspaceMapper();
        TokenStoreSequenceId newSequenceId;
        using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
          newSequenceId = (TokenStoreSequenceId) (long) securityComponent.RemoveAccessControlLists(this.m_description.NamespaceId, tokens, recurse, this.m_description.SeparatorValue);
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        requestContext.LogAuditEvent(SecurityAuditLogConstants.RemoveAccessControlLists, SecurityAuditHelper.SecurityData(this.m_description, tokens, recurse));
        return newSequenceId;
      }

      public TokenStoreSequenceId RenameTokens(
        IVssRequestContext requestContext,
        IEnumerable<TokenRename> renames)
      {
        this.EnsureDataspaceMapper();
        TokenStoreSequenceId newSequenceId;
        using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
        {
          int num = securityComponent.RenameTokens(this.m_description.NamespaceId, renames, this.m_description.SeparatorValue);
          if (-1 == num)
            num = securityComponent.QuerySequenceId(this.m_description.NamespaceId);
          newSequenceId = new TokenStoreSequenceId((long) num);
        }
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        return newSequenceId;
      }

      public TokenStoreSequenceId RemoveIdentityACEs(
        IVssRequestContext requestContext,
        IEnumerable<Guid> tfIds,
        out List<RemovedAccessControlEntry> removed)
      {
        this.EnsureDataspaceMapper();
        TokenStoreSequenceId newSequenceId;
        using (SecurityComponent securityComponent = this.CreateSecurityComponent(requestContext))
        {
          using (ResultCollection resultCollection = securityComponent.RemoveIdentityACEs(this.m_description.NamespaceId, tfIds))
          {
            newSequenceId = (TokenStoreSequenceId) (long) resultCollection.GetCurrent<int>().First<int>();
            resultCollection.NextResult();
            removed = resultCollection.GetCurrent<RemovedAccessControlEntry>().Items;
          }
        }
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        requestContext.LogAuditEvent(SecurityAuditLogConstants.RemoveIdentityACEs, SecurityAuditHelper.SecurityData(this.m_description, tfIds, removed));
        return newSequenceId;
      }

      public void EnqueueCacheInvalidation(
        IVssRequestContext requestContext,
        TokenStoreSequenceId newSequenceId)
      {
        if (!this.m_description.IsRemotable || !requestContext.ExecutionEnvironment.IsHostedDeployment || requestContext.ServiceHost.Is(TeamFoundationHostType.Deployment))
          return;
        SecurityMessage2 message = new SecurityMessage2()
        {
          ServiceOwner = requestContext.ServiceInstanceType(),
          InstanceId = requestContext.ServiceHost.InstanceId,
          NamespaceId = this.m_description.NamespaceId,
          AclStoreId = WellKnownAclStores.User,
          NewSequenceId = newSequenceId.ToArrayForRestReply()
        };
        requestContext.GetService<LocalSecurityService>().EnqueueCacheInvalidation(requestContext, message);
      }

      private SecurityComponent CreateSecurityComponent(IVssRequestContext requestContext)
      {
        SecurityComponent component = requestContext.CreateComponent<SecurityComponent>(this.m_description.DataspaceCategory);
        component.DataspaceMapper = this.m_dataspaceMapper;
        return component;
      }

      private void EnsureDataspaceMapper()
      {
        if (this.m_description.UseTokenTranslator && this.m_dataspaceMapper == null)
          throw new MissingDataspaceMapperException(this.m_description.NamespaceId);
      }

      private List<DatabaseAccessControlEntry> GetBackingStoreAces(
        IVssRequestContext requestContext,
        IEnumerable<IAccessControlList> acls,
        bool merge,
        bool throwOnInvalidIdentity,
        out bool droppedAtLeastOneAce)
      {
        List<DatabaseAccessControlEntry> backingStoreAces = new List<DatabaseAccessControlEntry>();
        Dictionary<IdentityDescriptor, Guid> dictionary = new Dictionary<IdentityDescriptor, Guid>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance);
        droppedAtLeastOneAce = false;
        if (throwOnInvalidIdentity && !this.m_settingsService.Settings.BackingStoreRespectsThrowOnInvalidIdentity)
          throwOnInvalidIdentity = false;
        IdentityService service = requestContext.GetService<IdentityService>();
        List<IdentityDescriptor> list = acls.SelectMany<IAccessControlList, IAccessControlEntry>((Func<IAccessControlList, IEnumerable<IAccessControlEntry>>) (s => s.AccessControlEntries)).Where<IAccessControlEntry>((Func<IAccessControlEntry, bool>) (s => !s.Descriptor.IdentityType.StartsWith("System:", StringComparison.Ordinal) || s.Descriptor.IsSystemServicePrincipalType())).Select<IAccessControlEntry, IdentityDescriptor>((Func<IAccessControlEntry, IdentityDescriptor>) (s => s.Descriptor)).Distinct<IdentityDescriptor>((IEqualityComparer<IdentityDescriptor>) IdentityDescriptorComparer.Instance).ToList<IdentityDescriptor>();
        if (list.Count > 0)
        {
          IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null);
          for (int index = list.Count - 1; index >= 0; --index)
          {
            Microsoft.VisualStudio.Services.Identity.Identity identity = identityList[index];
            if (identity != null)
            {
              dictionary[service.MapToWellKnownIdentifier(identity.Descriptor)] = identity.Id;
              list.RemoveAt(index);
            }
          }
          if (list.Count > 0)
          {
            foreach (Microsoft.VisualStudio.Services.Identity.Identity readIdentity in (IEnumerable<Microsoft.VisualStudio.Services.Identity.Identity>) service.ReadIdentities(requestContext, (IList<IdentityDescriptor>) list, QueryMembership.None, (IEnumerable<string>) null, true))
            {
              if (readIdentity != null)
                dictionary[service.MapToWellKnownIdentifier(readIdentity.Descriptor)] = readIdentity.Id;
            }
          }
        }
        foreach (IAccessControlList acl in acls)
        {
          foreach (IAccessControlEntry accessControlEntry in acl.AccessControlEntries)
          {
            Guid subjectId;
            if (dictionary.TryGetValue(accessControlEntry.Descriptor, out subjectId))
            {
              int allow = accessControlEntry.Allow & ~this.m_description.SystemBitMask;
              int deny = accessControlEntry.Deny & ~this.m_description.SystemBitMask;
              if (allow != accessControlEntry.Allow || deny != accessControlEntry.Deny)
              {
                Exception exception = (Exception) new InvalidPermissionsException(this.m_description.NamespaceId, this.m_description.SystemBitMask);
                requestContext.TraceException(56280, TraceLevel.Error, "Security", nameof (LocalUserBackingStore), exception);
                throw exception;
              }
              backingStoreAces.Add(new DatabaseAccessControlEntry(subjectId, acl.Token, allow, deny, false));
            }
            else if (!accessControlEntry.IsEmpty | merge)
            {
              droppedAtLeastOneAce = true;
              try
              {
                throw new IdentityNotFoundException(accessControlEntry.Descriptor);
              }
              catch (Exception ex)
              {
                requestContext.TraceException(56256, "Security", nameof (LocalUserBackingStore), ex);
                if (throwOnInvalidIdentity)
                  throw;
              }
            }
          }
        }
        return backingStoreAces;
      }

      internal class RawQuerySecurityDataResult
      {
        public Guid AclStoreId;
        public long OldSequenceId;
        public TokenStoreSequenceId NewSequenceId;
        public List<DatabaseAccessControlEntry> AccessControlEntries;
        public IEnumerable<string> NoInheritTokens;

        public RawQuerySecurityDataResult(
          Guid aclStoreId,
          long oldSequenceId,
          TokenStoreSequenceId newSequenceId,
          List<DatabaseAccessControlEntry> accessControlEntries,
          IEnumerable<string> noInheritTokens)
        {
          this.AclStoreId = aclStoreId;
          this.OldSequenceId = oldSequenceId;
          this.NewSequenceId = newSequenceId;
          this.AccessControlEntries = accessControlEntries;
          this.NoInheritTokens = noInheritTokens;
        }
      }
    }

    protected internal sealed class LocalSystemBackingStore : ISecurityNamespaceBackingStore
    {
      private readonly SecurityNamespaceDescription m_description;
      private readonly LocalSecurityInvalidationService m_lis;
      private readonly SecurityTemplateService m_sts;
      private static readonly VssPerformanceCounter s_refreshesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_SystemSecurityRefreshesPerSec");
      private long[] m_lastSeenSequenceId;

      public LocalSystemBackingStore(
        IVssRequestContext requestContext,
        SecurityNamespaceDescription description)
      {
        ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(description, nameof (description));
        this.m_description = description;
        this.m_lis = requestContext.GetService<LocalSecurityInvalidationService>();
        this.m_sts = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecurityTemplateService>();
      }

      public Guid AclStoreId => WellKnownAclStores.System;

      public IQuerySecurityDataResult QuerySecurityData(
        IVssRequestContext requestContext,
        long oldSequenceId)
      {
        LocalSecurityNamespace.LocalSystemBackingStore.s_refreshesPerSec.Increment();
        requestContext = requestContext.Elevate();
        IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Deployment);
        long systemStoreSequenceId = this.m_lis.GetSystemStoreSequenceId(requestContext);
        long sequenceId;
        IEnumerable<SecurityTemplateEntry> securityTemplateEntries = this.m_sts.GetSecurityTemplateEntries(requestContext1, requestContext.ServiceHost.HostType, this.m_description.NamespaceId, this.AclStoreId, out sequenceId);
        return (IQuerySecurityDataResult) new CompositeSecurityNamespace.QuerySecurityDataResult(this.AclStoreId, -1L, new TokenStoreSequenceId(new long[2]
        {
          systemStoreSequenceId,
          sequenceId
        }), (IEnumerable<BackingStoreAccessControlEntry>) LocalSecurityNamespace.LocalSystemBackingStore.AggregateWithBitwiseOr(securityTemplateEntries.SelectMany<SecurityTemplateEntry, BackingStoreAccessControlEntry>((Func<SecurityTemplateEntry, IEnumerable<BackingStoreAccessControlEntry>>) (s => LocalSecurityNamespace.LocalSystemBackingStore.TemplateEntryToAccessControlEntry(requestContext, this.m_description, s)))).ToList<BackingStoreAccessControlEntry>(), (IEnumerable<string>) Array.Empty<string>());
      }

      public SecurityBackingStoreChangedEventHandler WeakSubscribeToPushInvalidations(
        IVssRequestContext requestContext,
        SecurityBackingStoreChangedEventHandler eventHandler)
      {
        return (SecurityBackingStoreChangedEventHandler) null;
      }

      public bool SupportsPollingInvalidation => true;

      public TokenStoreSequenceId PollForSequenceId(IVssRequestContext requestContext)
      {
        long sequenceId = this.m_sts.SequenceId;
        long systemStoreSequenceId = this.m_lis.GetSystemStoreSequenceId(requestContext);
        long[] numArray = this.m_lastSeenSequenceId;
        if (numArray == null || numArray[0] != systemStoreSequenceId || numArray[1] != sequenceId)
        {
          numArray = new long[2]
          {
            systemStoreSequenceId,
            sequenceId
          };
          this.m_lastSeenSequenceId = numArray;
        }
        return new TokenStoreSequenceId(numArray);
      }

      public TokenStoreSequenceId RemoveAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<string> tokens,
        bool recurse)
      {
        throw new NotImplementedException();
      }

      public TokenStoreSequenceId RemovePermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<Guid> identityIds)
      {
        throw new NotImplementedException();
      }

      public TokenStoreSequenceId RenameTokens(
        IVssRequestContext requestContext,
        IEnumerable<TokenRename> renames)
      {
        throw new NotImplementedException();
      }

      public TokenStoreSequenceId SetAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<IAccessControlList> acls,
        bool throwOnInvalidIdentity)
      {
        throw new NotImplementedException();
      }

      public TokenStoreSequenceId SetInheritFlag(
        IVssRequestContext requestContext,
        string token,
        bool inheritFlag)
      {
        throw new NotImplementedException();
      }

      public TokenStoreSequenceId SetPermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<IAccessControlEntry> permissions,
        bool merge,
        bool throwOnInvalidIdentity)
      {
        throw new NotImplementedException();
      }

      private static IEnumerable<BackingStoreAccessControlEntry> TemplateEntryToAccessControlEntry(
        IVssRequestContext requestContext,
        SecurityNamespaceDescription description,
        SecurityTemplateEntry templateEntry)
      {
        bool resiliencyEnabled = !requestContext.IsFeatureEnabled("VisualStudio.Services.Security.DisableSecurityTemplateEntryResiliency");
        SubjectTemplate subjectTemplate;
        TokenTemplate tokenTemplate;
        try
        {
          tokenTemplate = JsonConvert.DeserializeObject<TokenTemplate>(templateEntry.TokenTemplate);
          subjectTemplate = JsonConvert.DeserializeObject<SubjectTemplate>(templateEntry.SubjectTemplate);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(56288, "Security", nameof (LocalSecurityNamespace), ex);
          requestContext.Trace(56289, TraceLevel.Error, "Security", nameof (LocalSecurityNamespace), string.Format("Security template entry with ID {0} failed to parse.", (object) templateEntry.Id));
          yield break;
        }
        IEnumerable<TokenAndContext> source1;
        try
        {
          source1 = tokenTemplate.GetTokens(requestContext, description);
          if (resiliencyEnabled)
            source1 = (IEnumerable<TokenAndContext>) source1.ToList<TokenAndContext>();
        }
        catch (Exception ex)
        {
          if (!resiliencyEnabled)
          {
            throw;
          }
          else
          {
            requestContext.TraceException(56291, "Security", nameof (LocalSecurityNamespace), ex);
            requestContext.Trace(56292, TraceLevel.Error, "Security", nameof (LocalSecurityNamespace), string.Format("Security template entry with ID {0} failed to get tokens.", (object) templateEntry.Id));
            yield break;
          }
        }
        bool loggedSubjectFailure = false;
        foreach (TokenAndContext tokenAndContext in source1)
        {
          TokenAndContext token = tokenAndContext;
          IEnumerable<IdentityDescriptor> source2;
          try
          {
            source2 = subjectTemplate.GetSubjects(requestContext, token.Context);
            if (resiliencyEnabled)
              source2 = (IEnumerable<IdentityDescriptor>) source2.ToList<IdentityDescriptor>();
          }
          catch (Exception ex)
          {
            if (!resiliencyEnabled)
            {
              throw;
            }
            else
            {
              if (!loggedSubjectFailure)
              {
                requestContext.TraceException(56293, "Security", nameof (LocalSecurityNamespace), ex);
                requestContext.Trace(56294, TraceLevel.Error, "Security", nameof (LocalSecurityNamespace), string.Format("Security template entry with ID {0} failed to get subjects.", (object) templateEntry.Id));
                loggedSubjectFailure = true;
                continue;
              }
              continue;
            }
          }
          foreach (IdentityDescriptor subject in source2)
            yield return new BackingStoreAccessControlEntry(subject, token.Token, templateEntry.Allow, templateEntry.Deny, false);
          token = new TokenAndContext();
        }
      }

      private static IEnumerable<BackingStoreAccessControlEntry> AggregateWithBitwiseOr(
        IEnumerable<BackingStoreAccessControlEntry> acesWithDuplicates)
      {
        acesWithDuplicates = (IEnumerable<BackingStoreAccessControlEntry>) acesWithDuplicates.ToList<BackingStoreAccessControlEntry>();
        foreach (IGrouping<LocalSecurityNamespace.LocalSystemBackingStore.AceKey, BackingStoreAccessControlEntry> grouping in acesWithDuplicates.GroupBy<BackingStoreAccessControlEntry, LocalSecurityNamespace.LocalSystemBackingStore.AceKey>((Func<BackingStoreAccessControlEntry, LocalSecurityNamespace.LocalSystemBackingStore.AceKey>) (s => (LocalSecurityNamespace.LocalSystemBackingStore.AceKey) s), LocalSecurityNamespace.LocalSystemBackingStore.AceKey.Comparer))
        {
          int allow = 0;
          int deny = 0;
          foreach (BackingStoreAccessControlEntry accessControlEntry in (IEnumerable<BackingStoreAccessControlEntry>) grouping)
          {
            allow |= accessControlEntry.Allow;
            deny |= accessControlEntry.Deny;
          }
          yield return new BackingStoreAccessControlEntry(grouping.Key.Subject, grouping.Key.Token, allow, deny, false);
        }
      }

      private struct AceKey
      {
        public readonly IdentityDescriptor Subject;
        public readonly string Token;
        public static readonly IEqualityComparer<LocalSecurityNamespace.LocalSystemBackingStore.AceKey> Comparer = (IEqualityComparer<LocalSecurityNamespace.LocalSystemBackingStore.AceKey>) new LocalSecurityNamespace.LocalSystemBackingStore.AceKey.AceKeyComparer();

        public AceKey(IdentityDescriptor subject, string token)
        {
          this.Subject = subject;
          this.Token = token;
        }

        public static implicit operator LocalSecurityNamespace.LocalSystemBackingStore.AceKey(
          BackingStoreAccessControlEntry ace)
        {
          return new LocalSecurityNamespace.LocalSystemBackingStore.AceKey(ace.Subject, ace.Token);
        }

        private class AceKeyComparer : 
          IEqualityComparer<LocalSecurityNamespace.LocalSystemBackingStore.AceKey>
        {
          public bool Equals(
            LocalSecurityNamespace.LocalSystemBackingStore.AceKey x,
            LocalSecurityNamespace.LocalSystemBackingStore.AceKey y)
          {
            return IdentityDescriptorComparer.Instance.Equals(x.Subject, y.Subject) && StringComparer.OrdinalIgnoreCase.Equals(x.Token, y.Token);
          }

          public int GetHashCode(
            LocalSecurityNamespace.LocalSystemBackingStore.AceKey obj)
          {
            return IdentityDescriptorComparer.Instance.GetHashCode(obj.Subject) ^ StringComparer.OrdinalIgnoreCase.GetHashCode(obj.Token ?? "");
          }
        }
      }
    }
  }
}
