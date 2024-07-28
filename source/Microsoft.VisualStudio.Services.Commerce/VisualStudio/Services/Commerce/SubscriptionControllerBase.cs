// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.SubscriptionControllerBase
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Http;
using System.Web.Http.Controllers;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [ControllerApiVersion(1.0)]
  [RequestContentTypeRestriction(AllowXml = true)]
  public abstract class SubscriptionControllerBase : CommerceControllerBase
  {
    internal override string Layer => nameof (SubscriptionControllerBase);

    public SubscriptionControllerBase()
    {
    }

    internal SubscriptionControllerBase(HttpControllerContext controllerContext) => this.Initialize(controllerContext);

    [HttpGet]
    public virtual IEnumerable<ISubscriptionAccount> GetAccounts(
      AccountProviderNamespace providerNamespaceId,
      Guid subscriptionId)
    {
      return this.TfsRequestContext.GetService<ISubscriptionService>().GetAccounts(this.TfsRequestContext, subscriptionId, providerNamespaceId);
    }

    [HttpGet]
    public virtual IEnumerable<IAzureSubscription> GetAzureSubscriptions(
      [FromUri(Name = "ids")] List<Guid> ids,
      AccountProviderNamespace providerNamespaceId)
    {
      return this.TfsRequestContext.GetService<ISubscriptionService>().GetAzureSubscriptions(this.TfsRequestContext, ids, providerNamespaceId);
    }

    [HttpGet]
    public virtual IEnumerable<ISubscriptionAccount> GetAccountsByIdentity(
      AccountProviderNamespace providerNamespaceId,
      Guid memberId,
      bool queryOnlyOwnerAccounts,
      bool inlcudeDisabledAccounts,
      bool includeMSAAccounts,
      [FromUri] List<Guid> serviceOwners)
    {
      return this.TfsRequestContext.GetService<ISubscriptionService>().GetAccounts(this.TfsRequestContext, providerNamespaceId, new Guid?(memberId), queryOnlyOwnerAccounts, inlcudeDisabledAccounts, includeMSAAccounts, (IEnumerable<Guid>) serviceOwners);
    }

    [HttpGet]
    public virtual IEnumerable<ISubscriptionAccount> GetAccountsByIdentityForOfferId(
      AccountProviderNamespace providerNamespaceId,
      Guid memberId,
      bool queryOnlyOwnerAccounts,
      bool inlcudeDisabledAccounts,
      bool includeMSAAccounts,
      [FromUri] List<Guid> serviceOwners,
      string galleryId,
      bool addUnlinkedSubscription = false,
      bool queryAccountsByUpn = false)
    {
      return this.TfsRequestContext.GetService<ISubscriptionService>().GetAccounts(this.TfsRequestContext, providerNamespaceId, new Guid?(memberId), queryOnlyOwnerAccounts, inlcudeDisabledAccounts, includeMSAAccounts, (IEnumerable<Guid>) serviceOwners, galleryId, addUnlinkedSubscription, queryAccountsByUpn);
    }

    [HttpGet]
    public virtual IEnumerable<ISubscriptionAccount> GetAzureSubscriptionForUser(
      Guid? subscriptionId = null,
      bool queryAcrossTenants = false)
    {
      return this.TfsRequestContext.GetService<ISubscriptionService>().GetAzureSubscriptionForUser(this.TfsRequestContext, subscriptionId, queryAcrossTenants);
    }

    [HttpGet]
    public virtual ISubscriptionAccount GetAzureSubscriptionForPurchase(
      Guid subscriptionId,
      string galleryItemId,
      Guid? accountId = null,
      Guid? subscriptionTenantId = null)
    {
      if (!this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableSubscriptionTenantCheck") || !subscriptionTenantId.HasValue || !(subscriptionTenantId.Value != Guid.Empty))
        return this.TfsRequestContext.GetService<ISubscriptionService>().GetAzureSubscriptionForPurchase(this.TfsRequestContext, subscriptionId, galleryItemId, accountId);
      if (this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.EnableUseSubscriptionTenantFromRequestContext"))
      {
        this.TfsRequestContext.Items.Add(nameof (subscriptionTenantId), (object) subscriptionTenantId);
        return this.TfsRequestContext.GetService<ISubscriptionService>().GetAzureSubscriptionForPurchase(this.TfsRequestContext, subscriptionId, galleryItemId, accountId);
      }
      ISubscriptionAccount response = (ISubscriptionAccount) null;
      SubscriptionHelper.WithUserInSubscriptionTenantContext(this.TfsRequestContext, subscriptionTenantId.Value, (Action<IVssRequestContext>) (userInTenantContext => response = userInTenantContext.GetService<ISubscriptionService>().GetAzureSubscriptionForPurchase(userInTenantContext, subscriptionId, galleryItemId, accountId)));
      return response;
    }

    [HttpPost]
    [ClientIgnore]
    public void CreateSubscription(AzureSubscriptionInternal subscription) => throw new NotImplementedException();

    [HttpPatch]
    public virtual void ChangeSubscriptionAccount(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      bool hydrate = false)
    {
      this.TfsRequestContext.GetService<ISubscriptionService>().ChangeSubscriptionCollection(this.TfsRequestContext, subscriptionId, providerNamespaceId, accountId, hydrate);
    }

    [HttpPut]
    public virtual void LinkAccount(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId,
      bool hydrate = false)
    {
      this.TfsRequestContext.GetService<ISubscriptionService>().LinkCollection(this.TfsRequestContext, subscriptionId, providerNamespaceId, accountId, new Guid?(ownerId), hydrate);
    }

    [HttpDelete]
    public virtual void UnlinkAccount(
      Guid subscriptionId,
      AccountProviderNamespace providerNamespaceId,
      Guid accountId,
      Guid ownerId)
    {
      CommerceUtil.SetRequestSource(this.TfsRequestContext, "SubscriptionUnlink");
      this.TfsRequestContext.GetService<ISubscriptionService>().UnlinkCollection(this.TfsRequestContext, subscriptionId, providerNamespaceId, accountId, new Guid?(ownerId), true);
    }

    [HttpGet]
    [ClientResponseType(typeof (SubscriptionAccount), null, null)]
    public virtual ISubscriptionAccount GetSubscriptionAccount(
      AccountProviderNamespace providerNamespaceId,
      Guid accountId)
    {
      return this.TfsRequestContext.GetService<ISubscriptionService>().GetSubscriptionAccount(this.TfsRequestContext, providerNamespaceId, accountId);
    }

    [HttpGet]
    [ClientResponseType(typeof (SubscriptionAccount), null, null)]
    public virtual ISubscriptionAccount GetSubscriptionAccountByName(
      AccountProviderNamespace providerNamespaceId,
      string accountName,
      Guid subscriptionId,
      [FromUri] IEnumerable<Guid> serviceOwners)
    {
      this.TfsRequestContext.TraceAlways(5109214, TraceLevel.Info, this.Area, this.Layer, string.Format("Subscription controller GetSubscriptionAccountByName. Parameters providerNamespaceId={0}, account={1}, subscription={2}", (object) providerNamespaceId, (object) accountName, (object) subscriptionId));
      return this.TfsRequestContext.GetService<PlatformSubscriptionService>().GetAzureSubscriptionFromName(this.TfsRequestContext, subscriptionId, accountName, providerNamespaceId, serviceOwners);
    }

    [HttpGet]
    public virtual bool IsAssignmentBillingEnabled(Guid accountId) => this.TfsRequestContext.GetService<PlatformSubscriptionService>().IsAssignmentBillingEnabled(this.TfsRequestContext, accountId);

    [HttpGet]
    public virtual bool IsPortalStaticPageEnabled(Guid directoryId)
    {
      this.TfsRequestContext.TraceAlways(5109275, TraceLevel.Info, this.Area, this.Layer, "Enter IsPortalStaticPageEnabled for directory {0}", (object) directoryId);
      bool flag = CommerceDeploymentHelper.IsPortalStaticPageEnabled(this.TfsRequestContext, directoryId, 5109274, this.Area, this.Layer);
      this.TfsRequestContext.TraceAlways(5109276, TraceLevel.Info, this.Area, this.Layer, "PortalStaticPage is {0} for directory {1}", flag ? (object) "enabled" : (object) "disabled", (object) directoryId);
      return flag;
    }
  }
}
