// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.ISubscriptionService
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [DefaultServiceImplementation(typeof (FrameworkSubscriptionService))]
  public interface ISubscriptionService : IVssFrameworkService
  {
    IEnumerable<IAzureSubscription> GetAzureSubscriptions(
      IVssRequestContext requestContext,
      List<Guid> subscriptionIds,
      AccountProviderNamespace providerNamespaceId);

    IEnumerable<ISubscriptionAccount> GetAccounts(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId);

    void CreateSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      int anniversaryDay,
      SubscriptionStatus status,
      SubscriptionSource source);

    void LinkCollection(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid collectionId,
      Guid? ownerId,
      bool hydrate = false);

    void UnlinkCollection(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid collectionId,
      Guid? ownerId,
      bool hydrate = false);

    IEnumerable<ISubscriptionAccount> GetAccounts(
      IVssRequestContext requestContext,
      AccountProviderNamespace providerNamespace,
      Guid? memberId,
      bool queryOnlyOwnerAccounts,
      bool inlcudeDisabledAccounts = false,
      bool includeMSAAccounts = false,
      IEnumerable<Guid> serviceOwners = null,
      string galleryId = null,
      bool addUnlinkedSubscription = false,
      bool queryAccountsByUpn = false);

    ISubscriptionAccount GetSubscriptionAccount(
      IVssRequestContext requestContext,
      AccountProviderNamespace providerNamespace,
      Guid accountId);

    IEnumerable<ISubscriptionAccount> GetAzureSubscriptionForUser(
      IVssRequestContext requestContext,
      Guid? subscriptionId = null,
      bool queryAcrossTenants = false);

    ISubscriptionAccount GetAzureSubscriptionForPurchase(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string galleryItemId,
      Guid? collectionId = null);

    void ChangeSubscriptionCollection(
      IVssRequestContext tfsRequestContext,
      Guid newSubscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      bool hydrate,
      bool performOnlyValidations = false);

    void CreateSubscription(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      SubscriptionStatus status,
      SubscriptionSource source);

    ISubscriptionAccount GetAzureSubscriptionFromName(
      IVssRequestContext requestContext,
      Guid subscriptionId,
      string accountName,
      AccountProviderNamespace providerNamespaceId,
      IEnumerable<Guid> serviceOwners);

    bool IsAssignmentBillingEnabled(IVssRequestContext requestContext, Guid accountId);

    bool IsProjectCollectionAdmin(
      IVssRequestContext requestContext,
      Guid memberId,
      Guid collectionId);
  }
}
