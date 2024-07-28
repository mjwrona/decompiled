// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RemoteSecurityNamespace
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server.AuditLog;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.Security.Client;
using Microsoft.VisualStudio.Services.Security.Messages;
using Microsoft.VisualStudio.Services.Security.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RemoteSecurityNamespace : 
    BulkLoadSecurityNamespace,
    IRemoteSecurityNamespace,
    IVssSecurityNamespace,
    IMutableSecurityNamespace,
    IQueryableSecurityNamespace
  {
    private readonly Guid m_serviceOwner;
    private const string c_area = "Security";
    private const string c_layer = "RemoteSecurityNamespace";

    public RemoteSecurityNamespace(
      IVssRequestContext requestContext,
      SecurityNamespaceDescription description,
      ISecurityNamespaceExtension extension,
      Guid serviceOwner)
      : base(requestContext, description, extension)
    {
      requestContext.CheckHostedDeployment();
      this.m_serviceOwner = serviceOwner;
    }

    protected override void ExecuteBulkLoadImpl(
      IVssRequestContext requestContext,
      out IEnumerable<CachingAclStore> aclStores,
      out IEnumerable<BulkLoadSecurityNamespace.BulkLoadInvalidation> invalidations)
    {
      List<CachingAclStore> cachingAclStoreList = new List<CachingAclStore>();
      List<BulkLoadSecurityNamespace.BulkLoadInvalidation> invalidationsList = new List<BulkLoadSecurityNamespace.BulkLoadInvalidation>();
      Guid domainId = requestContext.GetService<IdentityService>().DomainId;
      RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler handler = (RemoteSecurityInvalidationService.RemoteNamespaceChangedEventHandler) ((invalidateContext, aclStoreId, newSequenceId) => invalidationsList.Add(new BulkLoadSecurityNamespace.BulkLoadInvalidation(aclStoreId, newSequenceId)));
      SecuritySettingsService.SecurityServiceSettings settings = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>().Settings;
      RemoteSecurityInvalidationService service = requestContext.GetService<RemoteSecurityInvalidationService>();
      service.RegisterStrong(requestContext, this.m_serviceOwner, this.m_description.NamespaceId, handler);
      try
      {
        foreach (Microsoft.VisualStudio.Services.Security.SecurityNamespaceData securityNamespaceData in (List<Microsoft.VisualStudio.Services.Security.SecurityNamespaceData>) requestContext.Elevate().GetClient<SecurityBackingStoreHttpClient>(this.m_serviceOwner).QuerySecurityDataAsync(this.m_description.NamespaceId, !settings.AllowDescriptorRequestsFromSecurityBackingStore).SyncResult<SecurityNamespaceDataCollection>())
        {
          if (domainId != securityNamespaceData.IdentityDomain)
            throw new InvalidRemoteSecurityNamespaceException(FrameworkResources.RemoteDoesNotMatchIdentityDomain());
          CompositeSecurityNamespace.QuerySecurityDataResult initialSecurityData = new CompositeSecurityNamespace.QuerySecurityDataResult(securityNamespaceData.AclStoreId, securityNamespaceData.OldSequenceId, new TokenStoreSequenceId(securityNamespaceData.NewSequenceId), RemoteSecurityNamespace.MapToBackingStoreAces(requestContext, this.m_description.NamespaceId, securityNamespaceData.AccessControlEntries), securityNamespaceData.NoInheritTokens);
          RemoteSecurityNamespace.RemoteSecurityNamespaceBackingStore backingStore = new RemoteSecurityNamespace.RemoteSecurityNamespaceBackingStore(requestContext, this.m_description, securityNamespaceData.AclStoreId, this.m_serviceOwner, (IQuerySecurityDataResult) initialSecurityData);
          CachingAclStore cachingAclStore = new CachingAclStore(requestContext, this.m_description, (ISecurityNamespaceBackingStore) backingStore, settings.CacheLifetimeInMilliseconds);
          cachingAclStoreList.Add(cachingAclStore);
        }
      }
      finally
      {
        service.UnregisterStrong(requestContext, this.m_serviceOwner, this.m_description.NamespaceId, handler);
      }
      aclStores = (IEnumerable<CachingAclStore>) cachingAclStoreList;
      invalidations = (IEnumerable<BulkLoadSecurityNamespace.BulkLoadInvalidation>) invalidationsList;
    }

    public Guid ServiceOwner => this.m_serviceOwner;

    protected static IEnumerable<BackingStoreAccessControlEntry> MapToBackingStoreAces(
      IVssRequestContext requestContext,
      Guid namespaceId,
      IEnumerable<RemoteBackingStoreAccessControlEntry> remoteAces)
    {
      RemoteBackingStoreAccessControlEntry accessControlEntry = remoteAces.FirstOrDefault<RemoteBackingStoreAccessControlEntry>((Func<RemoteBackingStoreAccessControlEntry, bool>) (s => s.Subject != null));
      if (accessControlEntry != null && accessControlEntry.Subject.IndexOf(';') < 0)
      {
        IDictionary<Guid, IdentityDescriptor> dictionary = BackingStoreAccessControlEntryHelpers.BuildIdentityMap(requestContext, namespaceId, GetSubjectIds(remoteAces.Select<RemoteBackingStoreAccessControlEntry, string>((Func<RemoteBackingStoreAccessControlEntry, string>) (s => s.Subject)).Where<string>((Func<string, bool>) (s => s != null))));
        List<BackingStoreAccessControlEntry> backingStoreAces = new List<BackingStoreAccessControlEntry>();
        foreach (RemoteBackingStoreAccessControlEntry remoteAce in remoteAces)
        {
          Guid result;
          if (Guid.TryParse(remoteAce.Subject, out result))
          {
            IdentityDescriptor subject;
            if (remoteAce.IsDeleted && Guid.Empty == result)
              subject = (IdentityDescriptor) null;
            else if (!BackingStoreAccessControlEntryHelpers.GroupWellKnownVSIDs.VsidToDescriptor.TryGetValue(result, out subject) && (!dictionary.TryGetValue(result, out subject) || (IdentityDescriptor) null == subject))
              continue;
            backingStoreAces.Add(new BackingStoreAccessControlEntry(subject, remoteAce.Token, remoteAce.Allow, remoteAce.Deny, remoteAce.IsDeleted));
          }
        }
        return (IEnumerable<BackingStoreAccessControlEntry>) backingStoreAces;
      }
      List<BackingStoreAccessControlEntry> backingStoreAces1 = new List<BackingStoreAccessControlEntry>();
      foreach (RemoteBackingStoreAccessControlEntry remoteAce in remoteAces)
      {
        IdentityDescriptor identityDescriptor;
        if (remoteAce.IsDeleted)
        {
          identityDescriptor = (IdentityDescriptor) null;
        }
        else
        {
          identityDescriptor = IdentityDescriptor.FromString(remoteAce.Subject);
          if (identityDescriptor.IsClaimsIdentityType() && ServicePrincipals.IsServicePrincipal(requestContext, identityDescriptor, false, out Guid _))
            identityDescriptor = new IdentityDescriptor("System:ServicePrincipal", identityDescriptor.Identifier);
        }
        backingStoreAces1.Add(new BackingStoreAccessControlEntry(identityDescriptor, remoteAce.Token, remoteAce.Allow, remoteAce.Deny, remoteAce.IsDeleted));
      }
      return (IEnumerable<BackingStoreAccessControlEntry>) backingStoreAces1;

      static IEnumerable<Guid> GetSubjectIds(IEnumerable<string> subjectStrings)
      {
        foreach (string subjectString in subjectStrings)
        {
          Guid result;
          if (Guid.TryParse(subjectString, out result) && Guid.Empty != result)
            yield return result;
        }
      }
    }

    protected class RemoteSecurityNamespaceBackingStore : 
      BulkLoadSecurityNamespace.BulkLoadSecurityNamespaceBackingStore
    {
      private readonly Guid m_serviceOwner;
      private static readonly VssPerformanceCounter s_refreshesPerSec = VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_Service_RemoteSecurityRefreshesPerSec");

      public RemoteSecurityNamespaceBackingStore(
        IVssRequestContext requestContext,
        SecurityNamespaceDescription description,
        Guid aclStoreId,
        Guid serviceOwner,
        IQuerySecurityDataResult initialSecurityData = null)
        : base(requestContext, description, aclStoreId, initialSecurityData)
      {
        ArgumentUtility.CheckForEmptyGuid(serviceOwner, nameof (serviceOwner));
        this.m_serviceOwner = serviceOwner;
      }

      protected override IQuerySecurityDataResult QuerySecurityDataImpl(
        IVssRequestContext requestContext,
        long oldSequenceId)
      {
        RemoteSecurityNamespace.RemoteSecurityNamespaceBackingStore.s_refreshesPerSec.Increment();
        SecuritySettingsService.SecurityServiceSettings settings = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>().Settings;
        IdentityService service = requestContext.GetService<IdentityService>();
        Microsoft.VisualStudio.Services.Security.SecurityNamespaceData securityNamespaceData = this.GetClient(requestContext).QuerySecurityDataAsync(this.m_namespaceId, this.m_aclStoreId, (long) checked ((int) oldSequenceId), !settings.AllowDescriptorRequestsFromSecurityBackingStore).SyncResult<Microsoft.VisualStudio.Services.Security.SecurityNamespaceData>();
        if (service.DomainId != securityNamespaceData.IdentityDomain)
          throw new InvalidRemoteSecurityNamespaceException(FrameworkResources.RemoteDoesNotMatchIdentityDomain());
        return (IQuerySecurityDataResult) new CompositeSecurityNamespace.QuerySecurityDataResult(securityNamespaceData.AclStoreId, securityNamespaceData.OldSequenceId, new TokenStoreSequenceId(securityNamespaceData.NewSequenceId), RemoteSecurityNamespace.MapToBackingStoreAces(requestContext, this.m_namespaceId, securityNamespaceData.AccessControlEntries), securityNamespaceData.NoInheritTokens);
      }

      public override SecurityBackingStoreChangedEventHandler WeakSubscribeToPushInvalidations(
        IVssRequestContext requestContext,
        SecurityBackingStoreChangedEventHandler eventHandler)
      {
        requestContext.GetService<RemoteSecurityInvalidationService>().RegisterWeak(requestContext, this.m_serviceOwner, this.m_namespaceId, this.m_aclStoreId, eventHandler);
        return eventHandler;
      }

      public override bool SupportsPollingInvalidation => false;

      public override TokenStoreSequenceId PollForSequenceId(IVssRequestContext requestContext)
      {
        SecuritySettingsService.SecurityServiceSettings settings = requestContext.To(TeamFoundationHostType.Deployment).GetService<SecuritySettingsService>().Settings;
        return new TokenStoreSequenceId(this.GetClient(requestContext).QuerySecurityDataAsync(this.m_namespaceId, this.m_aclStoreId, (long) int.MaxValue, !settings.AllowDescriptorRequestsFromSecurityBackingStore).SyncResult<Microsoft.VisualStudio.Services.Security.SecurityNamespaceData>().NewSequenceId);
      }

      public override TokenStoreSequenceId SetPermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<IAccessControlEntry> permissions,
        bool merge,
        bool throwOnInvalidIdentity)
      {
        requestContext.LogAuditEvent(SecurityAuditLogConstants.ModifyPermission, SecurityAuditHelper.SecurityData(requestContext, this.m_description, token, permissions));
        TokenStoreSequenceId newSequenceId = (TokenStoreSequenceId) this.GetClient(requestContext).SetPermissionsAsync(this.m_namespaceId, token, SecurityConverter.Convert(permissions), merge, throwOnInvalidIdentity).SyncResult<long>();
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        return newSequenceId;
      }

      public override TokenStoreSequenceId SetAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<IAccessControlList> acls,
        bool throwOnInvalidIdentity)
      {
        requestContext.LogAuditEvent(SecurityAuditLogConstants.ModifyAccessControlLists, SecurityAuditHelper.SecurityData(requestContext, this.m_description, acls));
        TokenStoreSequenceId newSequenceId = (TokenStoreSequenceId) this.GetClient(requestContext).SetAccessControlListsAsync(this.m_namespaceId, SecurityConverter.Convert(acls), Enumerable.Empty<Microsoft.VisualStudio.Services.Security.AccessControlEntry>(), throwOnInvalidIdentity).SyncResult<long>();
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        return newSequenceId;
      }

      public override TokenStoreSequenceId SetInheritFlag(
        IVssRequestContext requestContext,
        string token,
        bool inheritFlag)
      {
        TokenStoreSequenceId newSequenceId = (TokenStoreSequenceId) this.GetClient(requestContext).SetInheritFlagAsync(this.m_namespaceId, token, inheritFlag).SyncResult<long>();
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        return newSequenceId;
      }

      public override TokenStoreSequenceId RemovePermissions(
        IVssRequestContext requestContext,
        string token,
        IEnumerable<Guid> identityIds)
      {
        requestContext.LogAuditEvent(SecurityAuditLogConstants.RemovePermission, SecurityAuditHelper.SecurityData(this.m_description, token, identityIds));
        TokenStoreSequenceId newSequenceId = (TokenStoreSequenceId) this.GetClient(requestContext).RemovePermissionsAsync(this.m_namespaceId, token, identityIds).SyncResult<long>();
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        return newSequenceId;
      }

      public override TokenStoreSequenceId RemoveAccessControlLists(
        IVssRequestContext requestContext,
        IEnumerable<string> tokens,
        bool recurse)
      {
        requestContext.LogAuditEvent(SecurityAuditLogConstants.RemoveAccessControlLists, SecurityAuditHelper.SecurityData(this.m_description, tokens, recurse));
        TokenStoreSequenceId newSequenceId = (TokenStoreSequenceId) this.GetClient(requestContext).RemoveAccessControlListsAsync(this.m_namespaceId, tokens, recurse).SyncResult<long>();
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        return newSequenceId;
      }

      public override TokenStoreSequenceId RenameTokens(
        IVssRequestContext requestContext,
        IEnumerable<TokenRename> renames)
      {
        TokenStoreSequenceId newSequenceId = (TokenStoreSequenceId) this.GetClient(requestContext).RenameTokensAsync(this.m_namespaceId, SecurityConverter.Convert(renames)).SyncResult<long>();
        this.EnqueueCacheInvalidation(requestContext, newSequenceId);
        return newSequenceId;
      }

      private void EnqueueCacheInvalidation(
        IVssRequestContext requestContext,
        TokenStoreSequenceId newSequenceId)
      {
        ITeamFoundationSqlNotificationService service = requestContext.GetService<ITeamFoundationSqlNotificationService>();
        RemoteSecurityNamespaceDataChangedMessage objectToSerialize = new RemoteSecurityNamespaceDataChangedMessage()
        {
          ServiceOwner = this.m_serviceOwner,
          NamespaceId = this.m_namespaceId,
          NewSequenceId = checked ((int) newSequenceId.ToScalarForInvalidation())
        };
        IVssRequestContext requestContext1 = requestContext;
        Guid securityDataChanged = SqlNotificationEventClasses.RemoteSecurityDataChanged;
        string eventData = TeamFoundationSerializationUtility.SerializeToString<RemoteSecurityNamespaceDataChangedMessage>(objectToSerialize);
        service.SendNotification(requestContext1, securityDataChanged, eventData);
      }

      protected virtual SecurityBackingStoreHttpClient GetClient(IVssRequestContext requestContext) => requestContext.Elevate().GetClient<SecurityBackingStoreHttpClient>(this.m_serviceOwner);
    }
  }
}
