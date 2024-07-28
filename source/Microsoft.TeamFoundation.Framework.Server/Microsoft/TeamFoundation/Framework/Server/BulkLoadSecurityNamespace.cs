// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.BulkLoadSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public abstract class BulkLoadSecurityNamespace : CompositeSecurityNamespace
  {
    private readonly InitialLoadGate m_initialLoadGate;
    private int m_invalidationId;
    private readonly ILockName m_aclStoresLock;
    private BulkLoadSecurityNamespace.BulkLoadAclStores m_aclStores;
    private const string c_area = "Security";
    private const string c_layer = "BulkLoadSecurityNamespace";

    public BulkLoadSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description,
      ISecurityNamespaceExtension extension)
      : base(requestContext, description, extension)
    {
      SecuritySettingsService service = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>();
      this.m_initialLoadGate = new InitialLoadGate(service.Settings.InitialLoadGateSize, service.Settings.LoadGateWaiterLimit);
      this.m_invalidationId = 0;
      this.m_aclStoresLock = requestContext.ServiceHost.CreateUniqueLockName(string.Format("{0}/{1}", (object) this.GetType().FullName, (object) "aclStores"));
    }

    private BulkLoadSecurityNamespace.BulkLoadAclStores ExecuteBulkLoad(
      IVssRequestContext requestContext)
    {
      requestContext.TraceEnter(56480, "Security", nameof (BulkLoadSecurityNamespace), nameof (ExecuteBulkLoad));
      try
      {
        using (new TraceWatch(requestContext, 56343, TraceLevel.Error, TimeSpan.FromSeconds(60.0), "Security", nameof (BulkLoadSecurityNamespace), "Namespace: '{0}' ({1}), Remoted: {2}", new object[3]
        {
          (object) this.m_description.DisplayName,
          (object) this.m_description.NamespaceId,
          (object) this.m_description.IsRemoted
        }))
        {
          int invalidationId;
          using (requestContext.AcquireReaderLock(this.m_aclStoresLock))
            invalidationId = this.m_invalidationId;
          IEnumerable<CachingAclStore> aclStores;
          IEnumerable<BulkLoadSecurityNamespace.BulkLoadInvalidation> invalidations;
          this.ExecuteBulkLoadImpl(requestContext, out aclStores, out invalidations);
          foreach (CachingAclStore cachingAclStore in aclStores)
            cachingAclStore.QueryAclCount(requestContext);
          foreach (BulkLoadSecurityNamespace.BulkLoadInvalidation loadInvalidation in invalidations)
          {
            if (loadInvalidation.AclStoreId == Guid.Empty)
            {
              foreach (CachingAclStore cachingAclStore in aclStores)
                cachingAclStore.NotifyChanged(requestContext, loadInvalidation.NewSequenceId, loadInvalidation.HardInvalidate);
            }
            else
            {
              foreach (CachingAclStore cachingAclStore in aclStores)
              {
                if (cachingAclStore.AclStoreId == loadInvalidation.AclStoreId)
                  cachingAclStore.NotifyChanged(requestContext, loadInvalidation.NewSequenceId, loadInvalidation.HardInvalidate);
              }
            }
          }
          return this.CreateAclStores(requestContext, aclStores, invalidationId);
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(56482, "Security", nameof (BulkLoadSecurityNamespace), ex);
        throw;
      }
      finally
      {
        requestContext.TraceLeave(56481, "Security", nameof (BulkLoadSecurityNamespace), nameof (ExecuteBulkLoad));
      }
    }

    protected abstract void ExecuteBulkLoadImpl(
      IVssRequestContext requestContext,
      out IEnumerable<CachingAclStore> aclStores,
      out IEnumerable<BulkLoadSecurityNamespace.BulkLoadInvalidation> invalidations);

    protected virtual BulkLoadSecurityNamespace.BulkLoadAclStores CreateAclStores(
      IVssRequestContext requestContext,
      IEnumerable<CachingAclStore> aclStores,
      int initialInvalidationId)
    {
      CachingAclStore cachingAclStore = aclStores.First<CachingAclStore>((Func<CachingAclStore, bool>) (s => s.AclStoreId == WellKnownAclStores.User));
      return new BulkLoadSecurityNamespace.BulkLoadAclStores((IEnumerable<IQueryableAclStore>) aclStores, (IQueryableAclStore) cachingAclStore, (IMutableAclStore) cachingAclStore, initialInvalidationId);
    }

    public bool IsCacheable => true;

    internal int InitialLoadGateMaxCount => this.m_initialLoadGate.MaxCount;

    internal int InitialLoadGateWaiterCount => this.m_initialLoadGate.Waiters;

    public override void OnDataChanged(IVssRequestContext requestContext)
    {
      CompositeSecurityNamespace.CompositeAclStores aclStores1 = (CompositeSecurityNamespace.CompositeAclStores) this.m_aclStores;
      if (aclStores1 != null)
      {
        CompositeSecurityNamespace.RelayOnDataChanged(requestContext, aclStores1);
      }
      else
      {
        using (requestContext.AcquireReaderLock(this.m_aclStoresLock))
        {
          CompositeSecurityNamespace.CompositeAclStores aclStores2 = (CompositeSecurityNamespace.CompositeAclStores) this.m_aclStores;
          if (aclStores2 != null)
            CompositeSecurityNamespace.RelayOnDataChanged(requestContext, aclStores2);
          else
            Interlocked.Increment(ref this.m_invalidationId);
        }
      }
    }

    protected override CompositeSecurityNamespace.CompositeAclStores GetAclStores(
      IVssRequestContext requestContext)
    {
      BulkLoadSecurityNamespace.BulkLoadAclStores aclStores1 = this.m_aclStores;
      if (aclStores1 != null)
        return (CompositeSecurityNamespace.CompositeAclStores) aclStores1;
      bool flag = false;
      try
      {
        flag = this.m_initialLoadGate.Wait(requestContext.CancellationToken);
        if (this.InitialLoadGateWaiterCount != 0 && this.InitialLoadGateWaiterCount % 10 == 0)
          requestContext.TraceAlways(10390002, TraceLevel.Info, "Security", nameof (BulkLoadSecurityNamespace), string.Format("Number of waiters outside of the remote namespace initial load gate: {0}", (object) this.InitialLoadGateWaiterCount));
        BulkLoadSecurityNamespace.BulkLoadAclStores aclStores2 = this.m_aclStores;
        if (aclStores2 != null)
          return (CompositeSecurityNamespace.CompositeAclStores) aclStores2;
        BulkLoadSecurityNamespace.BulkLoadAclStores aclStores3 = this.ExecuteBulkLoad(requestContext);
        using (requestContext.AcquireWriterLock(this.m_aclStoresLock))
        {
          if (this.m_aclStores != null)
            return (CompositeSecurityNamespace.CompositeAclStores) this.m_aclStores;
          if (this.m_invalidationId > aclStores3.InitialInvalidationId)
          {
            using (requestContext.AcquireExemptionLock())
              CompositeSecurityNamespace.RelayOnDataChanged(requestContext, (CompositeSecurityNamespace.CompositeAclStores) aclStores3);
          }
          this.m_aclStores = aclStores3;
          return (CompositeSecurityNamespace.CompositeAclStores) aclStores3;
        }
      }
      finally
      {
        if (flag)
          this.m_initialLoadGate.Release();
      }
    }

    protected struct BulkLoadInvalidation
    {
      public readonly Guid AclStoreId;
      public readonly TokenStoreSequenceId NewSequenceId;
      public readonly bool HardInvalidate;

      public BulkLoadInvalidation(
        Guid aclStoreId,
        TokenStoreSequenceId newSequenceId,
        bool hardInvalidate = false)
      {
        this.AclStoreId = aclStoreId;
        this.NewSequenceId = newSequenceId;
        this.HardInvalidate = hardInvalidate;
      }
    }

    protected class BulkLoadAclStores : CompositeSecurityNamespace.CompositeAclStores
    {
      public readonly int InitialInvalidationId;

      public BulkLoadAclStores(
        IEnumerable<IQueryableAclStore> queryableStores,
        IQueryableAclStore primaryQueryableStore,
        IMutableAclStore mutableStore,
        int initialInvalidationId)
        : base(queryableStores, primaryQueryableStore, mutableStore)
      {
        this.InitialInvalidationId = initialInvalidationId;
      }
    }

    protected abstract class BulkLoadSecurityNamespaceBackingStore : ISecurityNamespaceBackingStore
    {
      protected readonly Guid m_namespaceId;
      protected readonly SecurityNamespaceDescription m_description;
      protected readonly Guid m_aclStoreId;
      private IQuerySecurityDataResult m_initialSecurityData;

      public BulkLoadSecurityNamespaceBackingStore(
        IVssRequestContext requestContext,
        SecurityNamespaceDescription description,
        Guid aclStoreId,
        IQuerySecurityDataResult initialSecurityData = null)
      {
        ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
        ArgumentUtility.CheckForNull<SecurityNamespaceDescription>(description, nameof (description));
        ArgumentUtility.CheckForEmptyGuid(aclStoreId, nameof (aclStoreId));
        if (initialSecurityData != null && initialSecurityData.AclStoreId != aclStoreId)
          throw new ArgumentException();
        this.m_description = description;
        this.m_namespaceId = description.NamespaceId;
        this.m_aclStoreId = aclStoreId;
        this.m_initialSecurityData = initialSecurityData;
      }

      public Guid AclStoreId => this.m_aclStoreId;

      public IQuerySecurityDataResult QuerySecurityData(
        IVssRequestContext requestContext,
        long oldSequenceId)
      {
        IQuerySecurityDataResult securityDataResult = this.m_initialSecurityData ?? this.QuerySecurityDataImpl(requestContext, oldSequenceId);
        this.m_initialSecurityData = (IQuerySecurityDataResult) null;
        return securityDataResult;
      }

      protected abstract IQuerySecurityDataResult QuerySecurityDataImpl(
        IVssRequestContext requestContext,
        long oldSequenceId);

      public virtual SecurityBackingStoreChangedEventHandler WeakSubscribeToPushInvalidations(
        IVssRequestContext requestContext,
        SecurityBackingStoreChangedEventHandler eventHandler)
      {
        return (SecurityBackingStoreChangedEventHandler) null;
      }

      public abstract bool SupportsPollingInvalidation { get; }

      public abstract TokenStoreSequenceId PollForSequenceId(IVssRequestContext requestContext);

      public abstract TokenStoreSequenceId SetPermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<IAccessControlEntry> permissions,
        bool merge,
        bool throwOnInvalidIdentity);

      public abstract TokenStoreSequenceId SetAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<IAccessControlList> acls,
        bool throwOnInvalidIdentity);

      public abstract TokenStoreSequenceId SetInheritFlag(
        IVssRequestContext requestContext,
        string token,
        bool inheritFlag);

      public abstract TokenStoreSequenceId RemovePermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<Guid> identityIds);

      public abstract TokenStoreSequenceId RemoveAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<string> tokens,
        bool recurse);

      public abstract TokenStoreSequenceId RenameTokens(
        IVssRequestContext requestContext,
        IEnumerable<TokenRename> renames);
    }
  }
}
