// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Commerce.PassthroughController
// Assembly: Microsoft.VisualStudio.Services.Commerce, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1A903DA-646E-45AC-891B-7946BA20E824
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Commerce.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.HostAcquisition;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.NameResolution;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.UserAccountMapping;
using Microsoft.VisualStudio.Services.UserMapping;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Commerce
{
  [XmlFormatter]
  public class PassthroughController : CommerceControllerBase
  {
    [ExcludeFromCodeCoverage]
    internal virtual Microsoft.VisualStudio.Services.Identity.Identity ConstructIdentityFromEmail(
      IVssRequestContext TfsRequestContext,
      string emailAddress)
    {
      return IdentityHelper.GetOrCreateBindPendingIdentity(TfsRequestContext.Elevate(), "Windows Live ID", CommerceIdentityHelper.GetEmailFromUpn(emailAddress), callerName: nameof (ConstructIdentityFromEmail));
    }

    [ExcludeFromCodeCoverage]
    internal virtual Microsoft.VisualStudio.Services.Identity.Identity GetIdentity(
      IVssRequestContext requestContext,
      string email,
      string domain,
      string puid)
    {
      return IdentityHelper.GetOrCreateBindPendingIdentity(requestContext.Elevate(), domain, CommerceIdentityHelper.GetEmailFromUpn(email), puid, callerName: nameof (GetIdentity));
    }

    [HttpPost]
    [CaptureRdfeOperationId]
    [TraceFilter(5105571, 5105575)]
    [TraceExceptions(5105574)]
    [TraceRequest(5105572)]
    [TraceResponse(5105573)]
    public AccountList QueryAccountsByOwner(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName,
      QueryAccountsByOwnershipInput input)
    {
      this.TfsRequestContext.GetService<IPermissionCheckerService>().CheckPermission(this.TfsRequestContext, 1, CommerceSecurity.CommerceSecurityNamespaceId);
      PlatformSubscriptionService subscriptionService = this.TfsRequestContext.GetService<PlatformSubscriptionService>();
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = this.ConstructIdentityFromEmail(this.TfsRequestContext, input.EmailAddress);
      List<string> stringList = new List<string>();
      ITeamFoundationHostManagementService service = this.TfsRequestContext.GetService<ITeamFoundationHostManagementService>();
      foreach (Guid hostId in UserAccountMappingMigrationHelper.QueryAccountIds(this.TfsRequestContext, userIdentity, UserRole.Owner).Where<Guid>((Func<Guid, bool>) (collectionId => subscriptionService.GetAzureResourceAccountByCollectionId(this.TfsRequestContext, collectionId) == null)))
      {
        HostProperties hostProperties = service.QueryServiceHostPropertiesCached(this.TfsRequestContext, hostId);
        if (hostProperties != null)
        {
          stringList.Add(hostProperties.Name);
          this.TfsRequestContext.Trace(5108825, TraceLevel.Info, this.Area, this.Layer, string.Format("Discovered host name {0} for host ID {1} owned by {2}", (object) hostProperties.Name, (object) hostId, (object) userIdentity.MasterId));
        }
        else if (this.TfsRequestContext.IsCommerceService())
        {
          NameResolutionEntry primaryEntryForValue = this.TfsRequestContext.GetService<INameResolutionService>().GetPrimaryEntryForValue(this.TfsRequestContext, hostId);
          if (primaryEntryForValue != null)
          {
            stringList.Add(primaryEntryForValue.Name);
            this.TfsRequestContext.TraceAlways(5108825, TraceLevel.Info, this.Area, this.Layer, string.Format("Discovered host name {0} in SPS for host ID {1} owned by {2} initiated from Commerce Service", (object) primaryEntryForValue.Name, (object) hostId, (object) userIdentity.MasterId));
          }
        }
      }
      return new AccountList() { AccountNames = stringList };
    }

    [HttpPost]
    [CaptureRdfeOperationId]
    [TraceFilter(5106320, 5106324)]
    [TraceExceptions(5106321)]
    [TraceRequest(5106322)]
    [TraceResponse(5106323)]
    public AzureRegionList GetRegionList(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException();
      IPermissionCheckerService service = this.TfsRequestContext.GetService<IPermissionCheckerService>();
      IVssRequestContext context = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid securityNamespaceId = CommerceSecurity.CommerceSecurityNamespaceId;
      service.CheckPermission(tfsRequestContext, 1, securityNamespaceId);
      IEnumerable<string> source = context.GetService<IHostAcquisitionService>().GetRegions(context).Where<Region>((Func<Region, bool>) (x => !this.IsRegionExcluded(x.DisplayName))).Select<Region, string>((Func<Region, string>) (region => region.DisplayName));
      return new AzureRegionList()
      {
        Regions = source.ToList<string>()
      };
    }

    [HttpPost]
    [CaptureRdfeOperationId]
    [TraceFilter(5105576, 5105580)]
    [TraceExceptions(5105579)]
    [TraceResponse(5105578)]
    public AccountTenantList QueryAccountsByOwnerWithTenant(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName,
      QueryAccountsByOwnershipWithTenantInput input)
    {
      AccountTenantList accountTenantList = new AccountTenantList();
      accountTenantList.AccountNames = new List<KeyValuePair>();
      accountTenantList.AccountNames.AddRange(this.QueryAccountsByOwnerWithTenantExtended(subscriptionId, cloudServiceName, resourceType, resourceName, input).Accounts.Select<AccountInfo, KeyValuePair>((Func<AccountInfo, KeyValuePair>) (x => new KeyValuePair(x.Name, x.TenantId))));
      return accountTenantList;
    }

    [HttpPost]
    [CaptureRdfeOperationId]
    [TraceFilter(5106440, 5106450)]
    [TraceExceptions(5106449)]
    [TraceResponse(5106442)]
    public AccountInfoList QueryAccountsByOwnerWithTenantExtended(
      string subscriptionId,
      string cloudServiceName,
      string resourceType,
      string resourceName,
      QueryAccountsByOwnershipWithTenantInput input)
    {
      if (this.TfsRequestContext.ExecutionEnvironment.IsOnPremisesDeployment)
        throw new InvalidOperationException();
      this.TfsRequestContext.GetService<IPermissionCheckerService>().CheckPermission(this.TfsRequestContext, 1, CommerceSecurity.CommerceSecurityNamespaceId);
      PlatformSubscriptionService subscriptionService = this.TfsRequestContext.GetService<PlatformSubscriptionService>();
      IdentityService service = this.TfsRequestContext.GetService<IdentityService>();
      AccountInfoList accountList = new AccountInfoList()
      {
        Accounts = new List<AccountInfo>()
      };
      List<Microsoft.VisualStudio.Services.Identity.Identity> list = service.ReadIdentities(this.TfsRequestContext, IdentitySearchFilter.AccountName, input.EmailAddress, QueryMembership.None, (IEnumerable<string>) null).ToList<Microsoft.VisualStudio.Services.Identity.Identity>();
      if (!string.IsNullOrEmpty(input.Puid))
      {
        IdentityDescriptor identityDescriptor = new IdentityDescriptor("Microsoft.IdentityModel.Claims.ClaimsIdentity", input.Puid + "@Live.com");
        Microsoft.VisualStudio.Services.Identity.Identity msaIdentity = service.ReadIdentities(this.TfsRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityDescriptor
        }, QueryMembership.None, (IEnumerable<string>) null).FirstOrDefault<Microsoft.VisualStudio.Services.Identity.Identity>();
        if (msaIdentity != null && list.All<Microsoft.VisualStudio.Services.Identity.Identity>((Func<Microsoft.VisualStudio.Services.Identity.Identity, bool>) (x => x.MasterId != msaIdentity.MasterId)))
          list.Add(msaIdentity);
      }
      Guid.TryParse(input.IdentityDomain, out Guid _);
      IVssRequestContext requestContext = this.TfsRequestContext.Elevate();
      foreach (Microsoft.VisualStudio.Services.Identity.Identity identity1 in list)
      {
        Microsoft.VisualStudio.Services.Identity.Identity identity = identity1;
        IList<Guid> guidList = UserAccountMappingMigrationHelper.QueryAccountIds(this.TfsRequestContext, identity, UserRole.Owner);
        ICommerceRegionHandler commerceRegionHandler = this.TfsRequestContext.GetExtension<ICommerceRegionHandler>();
        foreach (Guid guid in (IEnumerable<Guid>) guidList)
        {
          Guid accountId = guid;
          CollectionHelper.WithCollectionContext(requestContext, accountId, (Action<IVssRequestContext>) (collectionContext =>
          {
            Collection collection = collectionContext.GetService<ICollectionService>().GetCollection(collectionContext, (IEnumerable<string>) null);
            if (collection == null || collection.Status != CollectionStatus.Enabled)
              return;
            Guid organizationAadTenantId = collectionContext.GetOrganizationAadTenantId();
            if (organizationAadTenantId != domainGuid && organizationAadTenantId != Guid.Empty || subscriptionService.GetAzureResourceAccountByCollectionId(collectionContext, accountId) != null)
              return;
            string domainString = organizationAadTenantId == Guid.Empty ? "Windows Live ID" : organizationAadTenantId.ToString();
            string regionString = commerceRegionHandler.GetRegionDisplayName(this.TfsRequestContext, collection.PreferredRegion);
            if (!this.IsRegionExcluded(regionString))
              accountList.Accounts.Add(new AccountInfo()
              {
                Name = collection.Name,
                Region = regionString,
                TenantId = domainString,
                AccountId = accountId.ToString()
              });
            this.TfsRequestContext.TraceConditionally(5108826, TraceLevel.Info, this.Area, this.Layer, (Func<string>) (() => string.Format("Discovered account name {0} region {1} domain {2} with ID {3} owned by {4}", (object) collection.Name, (object) regionString, (object) domainString, (object) accountId, (object) identity.MasterId)));
          }), method: nameof (QueryAccountsByOwnerWithTenantExtended));
        }
      }
      return accountList;
    }

    private bool IsRegionExcluded(string regionName) => this.TfsRequestContext.IsFeatureEnabled("VisualStudio.Services.Commerce.HideRegionsInAux") && regionName.Equals("South India", StringComparison.OrdinalIgnoreCase);

    internal override string Layer => nameof (PassthroughController);
  }
}
