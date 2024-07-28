// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Account.ApiAccountController
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Account, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AC21A176-69BE-407E-B3DD-80612369F784
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Account.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.WebAccess.Routing;
using Microsoft.VisualStudio.Services.Account;
using Microsoft.VisualStudio.Services.Commerce;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Security;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Account
{
  [SupportedRouteArea("Api", NavigationContextLevels.Deployment | NavigationContextLevels.Application | NavigationContextLevels.Collection)]
  [Microsoft.TeamFoundation.Server.WebAccess.DemandFeature("00000000-0000-0000-0000-000000000000", false)]
  public class ApiAccountController : AccountAreaController
  {
    private const string c_area = "HostingAccount";
    private const string c_layer = "ApiAccountController";
    private const string c_CheckAllowTeamAdminMaterialization = "VisualStudio.Services.AdminEngagement.OrganizationPolicies.TeamAdminsMaterialization";

    [HttpGet]
    [TfsTraceFilter(504221, 504222)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult GetAccountSettings()
    {
      this.Response.Cache.SetCacheability(HttpCacheability.NoCache);
      return (ActionResult) this.Json((object) ApiAccountController.GetAccountSettingsModelInternal(this.TfsRequestContext).ToJson(), JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [TfsTraceFilter(504231, 504239)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult EnableAccountTrialMode() => throw new AccountTrialException("This feature is no longer supported.");

    [HttpPost]
    [TfsTraceFilter(504241, 504249)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult MarkAccountForDelete(string accountName, string accountDeleteReason)
    {
      JsObject data = new JsObject();
      try
      {
        this.TfsRequestContext.Trace(504242, TraceLevel.Info, "HostingAccount", nameof (ApiAccountController), "MarkAccountForDelete: started {0}", (object) accountName);
        if (string.IsNullOrEmpty(accountName))
          throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid input: AccountName - {0}", (object) accountName));
        if (this.UpdateAccountForSoftDelete(this.TfsRequestContext, accountName, accountDeleteReason))
        {
          IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
          string locationServiceUrl = vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, ServiceInstanceTypes.TFS, this.TfsRequestContext.UseDevOpsDomainUrls() ? AccessMappingConstants.ServicePathMappingMoniker : AccessMappingConstants.RootDomainMappingMoniker);
          data["Status"] = (object) "true";
          data["redirectUrl"] = (object) locationServiceUrl;
        }
        else
        {
          data["Status"] = (object) "false";
          data["ErrorMessage"] = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, AccountServerResources.AccountDeletePleaseTryAgainLater);
        }
      }
      catch (UnableToDeleteAzureLinkedAccountException ex)
      {
        this.TfsRequestContext.TraceException(504244, TraceLevel.Error, "HostingAccount", nameof (ApiAccountController), (Exception) ex);
        this.LogException((Exception) ex);
        data["Status"] = (object) false;
        data["Html"] = (object) true;
        data["ErrorMessage"] = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.GetUiErrorMessageForSoftDelete((Exception) ex));
      }
      catch (Exception ex)
      {
        this.TfsRequestContext.TraceException(504243, TraceLevel.Error, "HostingAccount", nameof (ApiAccountController), ex);
        this.LogException(ex);
        data["Status"] = (object) "false";
        data["ErrorMessage"] = (object) string.Format((IFormatProvider) CultureInfo.CurrentCulture, this.GetUiErrorMessageForSoftDelete(ex));
      }
      return (ActionResult) this.Json((object) data);
    }

    [HttpGet]
    [TfsTraceFilter(504241, 504249)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult GetAccountDeleteReason()
    {
      JsObject data = new JsObject();
      try
      {
        IList<AccountDeleteReason> accountDeleteReasonList = (IList<AccountDeleteReason>) new List<AccountDeleteReason>()
        {
          new AccountDeleteReason()
          {
            key = 1,
            DisplayText = AccountServerResources.AccountDeleteReason1
          },
          new AccountDeleteReason()
          {
            key = 2,
            DisplayText = AccountServerResources.AccountDeleteReason2
          },
          new AccountDeleteReason()
          {
            key = 3,
            DisplayText = AccountServerResources.AccountDeleteReason3
          },
          new AccountDeleteReason()
          {
            key = 4,
            DisplayText = AccountServerResources.AccountDeleteReason4
          },
          new AccountDeleteReason()
          {
            key = 5,
            DisplayText = AccountServerResources.AccountDeleteReason5
          },
          new AccountDeleteReason()
          {
            key = 6,
            DisplayText = AccountServerResources.AccountDeleteReason6
          }
        };
        data["data"] = (object) accountDeleteReasonList;
        data["Status"] = (object) "true";
      }
      catch (Exception ex)
      {
        this.LogException(ex);
        data["Status"] = (object) "false";
      }
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpGet]
    [TfsTraceFilter(504261, 504269)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult GetSubscriptionId()
    {
      this.TfsRequestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Application);
      ISubscriptionAccount subscriptionAccount = vssRequestContext.GetService<ISubscriptionService>().GetSubscriptionAccount(vssRequestContext, AccountProviderNamespace.VisualStudioOnline, this.TfsRequestContext.ServiceHost.InstanceId);
      JsObject data = new JsObject();
      data["subscriptionId"] = (object) (Guid?) subscriptionAccount?.SubscriptionId;
      return (ActionResult) this.Json((object) data, JsonRequestBehavior.AllowGet);
    }

    [HttpPost]
    [TfsTraceFilter(504701, 504710)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult UpdateAccountAdvancedSettings(string newAccountName)
    {
      this.TfsRequestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      AccountNameValidator.ValidateAccountName(newAccountName);
      JsObject data = new JsObject();
      this.TfsRequestContext.Trace(504702, TraceLevel.Info, "HostingAccount", nameof (ApiAccountController), "UpdateAccountAdvancedSettings: started {0}", (object) newAccountName);
      if (string.IsNullOrEmpty(newAccountName))
        throw new ArgumentNullException(string.Format((IFormatProvider) CultureInfo.InvariantCulture, "Invalid input: AccountName - {0}", (object) newAccountName));
      ApiAccountController.UpdateAccountAdvancedSettingsInternal(this.TfsRequestContext, newAccountName);
      Collection collection = this.TfsRequestContext.GetService<ICollectionService>().GetCollection(this.TfsRequestContext, (IEnumerable<string>) null);
      IVssRequestContext vssRequestContext = this.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      string str = vssRequestContext.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext, collection.Id).ToString();
      data["Status"] = (object) "true";
      data["NewAccountUrl"] = (object) (str + "_admin/_home/settings");
      return (ActionResult) this.Json((object) data);
    }

    [HttpPost]
    [ValidateInput(false)]
    [TfsTraceFilter(504223, 504224)]
    [AcceptNavigationLevels(NavigationContextLevels.Collection)]
    public ActionResult UpdateAccountSettings(
      string timeZoneId,
      string accountOwnerId,
      string privacyUrl,
      bool? basicAuthDisabled = null,
      bool? oAuthDisabled = null,
      bool? guestUserDisabled = null,
      bool? publicKeyDisabled = null,
      bool? policyPPEnabled = null,
      bool? policyOPEnabled = null,
      bool? policyEnforceConditionalAccessPolicy = null,
      bool? policyAllowGitHubInvitations = null,
      bool? policyAllowRequestAccess = null,
      bool? policyAllowTeamAdminsInvitations = null)
    {
      ApiAccountController.UpdateAccountSettingsInternal(this.TfsRequestContext, timeZoneId, accountOwnerId, privacyUrl, basicAuthDisabled, oAuthDisabled, guestUserDisabled, publicKeyDisabled, policyPPEnabled, policyOPEnabled, policyEnforceConditionalAccessPolicy, policyAllowGitHubInvitations, policyAllowRequestAccess, policyAllowTeamAdminsInvitations);
      JsObject data = new JsObject();
      data["success"] = (object) true;
      return (ActionResult) this.Json((object) data);
    }

    internal static AccountSettingsModel GetAccountSettingsModelInternal(
      IVssRequestContext requestContext)
    {
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      IVssRequestContext context = requestContext.Elevate();
      TimeZoneInfo collectionTimeZone = requestContext.GetCollectionTimeZone();
      Collection collection = requestContext.GetService<ICollectionService>().GetCollection(context, (IEnumerable<string>) new string[2]
      {
        "Microsoft.VisualStudio.Services.Account.TfsAccountRegionDisplayName",
        "SystemProperty.PrivacyUrl"
      });
      IDictionary<string, PolicyModel> policies = (IDictionary<string, PolicyModel>) new Dictionary<string, PolicyModel>();
      IOrganizationPolicyService service = requestContext.GetService<IOrganizationPolicyService>();
      bool defaultValue = requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationPolicies.AltCredDefaultToTrue");
      Policy<bool> policy1 = service.GetPolicy<bool>(requestContext, "Policy.DisallowBasicAuthentication", defaultValue);
      PolicyInfo policyInfo1 = service.GetPolicyInfo(requestContext, "Policy.DisallowBasicAuthentication");
      policies.Add("Policy.DisallowBasicAuthentication", new PolicyModel(policyInfo1.Description, policy1.EffectiveValue, policyInfo1.MoreInfoLink?.ToString()));
      Policy<bool> policy2 = service.GetPolicy<bool>(requestContext, "Policy.DisallowOAuthAuthentication", false);
      PolicyInfo policyInfo2 = service.GetPolicyInfo(requestContext, "Policy.DisallowOAuthAuthentication");
      policies.Add("Policy.DisallowOAuthAuthentication", new PolicyModel(policyInfo2.Description, policy2.EffectiveValue, policyInfo2.MoreInfoLink?.ToString()));
      bool policyDefaultValue = AadGuestUserAccessHelper.DisallowAadGuestUserAccessPolicyDefaultValue;
      Policy<bool> policy3 = service.GetPolicy<bool>(requestContext, "Policy.DisallowAadGuestUserAccess", policyDefaultValue);
      PolicyInfo policyInfo3 = service.GetPolicyInfo(requestContext, "Policy.DisallowAadGuestUserAccess");
      policies.Add("Policy.DisallowAadGuestUserAccess", new PolicyModel(policyInfo3.Description, policy3.EffectiveValue, policyInfo3.MoreInfoLink?.ToString()));
      Policy<bool> policy4 = service.GetPolicy<bool>(requestContext, "Policy.DisallowSecureShell", false);
      PolicyInfo policyInfo4 = service.GetPolicyInfo(requestContext, "Policy.DisallowSecureShell");
      policies.Add("Policy.DisallowSecureShell", new PolicyModel(policyInfo4.Description, policy4.EffectiveValue, policyInfo4.MoreInfoLink?.ToString()));
      Policy<bool> policy5 = service.GetPolicy<bool>(requestContext, "Policy.EnforceAADConditionalAccess", false);
      PolicyInfo policyInfo5 = service.GetPolicyInfo(requestContext, "Policy.EnforceAADConditionalAccess");
      policies.Add("Policy.EnforceAADConditionalAccess", new PolicyModel(policyInfo5.Description, policy5.EffectiveValue, policyInfo5.MoreInfoLink?.ToString()));
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess"))
      {
        Policy<bool> policy6 = service.GetPolicy<bool>(requestContext, "Policy.AllowAnonymousAccess", false);
        PolicyInfo policyInfo6 = service.GetPolicyInfo(requestContext, "Policy.AllowAnonymousAccess");
        policies.Add("Policy.PPEnabled", new PolicyModel(policyInfo6.Description, policy6.EffectiveValue, policyInfo6.MoreInfoLink?.ToString()));
      }
      Policy<bool> policy7 = service.GetPolicy<bool>(requestContext, "Policy.AllowRequestAccessToken", true);
      PolicyInfo policyInfo7 = service.GetPolicyInfo(requestContext, "Policy.AllowRequestAccessToken");
      policies.Add("Policy.AllowRequestAccessToken", new PolicyModel(policyInfo7.Description, policy7.EffectiveValue, policyInfo7.MoreInfoLink?.ToString()));
      if (requestContext.IsOrganizationActivated())
      {
        Policy<bool> policy8 = service.GetPolicy<bool>(requestContext, "Policy.AllowOrgAccess", true);
        PolicyInfo policyInfo8 = service.GetPolicyInfo(requestContext, "Policy.AllowOrgAccess");
        policies.Add("Policy.OPEnabled", new PolicyModel(policyInfo8.Description, policy8.EffectiveValue, policyInfo8.MoreInfoLink?.ToString()));
      }
      Policy<bool> policy9 = service.GetPolicy<bool>(requestContext, "Policy.AllowGitHubInvitationsAccessToken", false);
      PolicyInfo policyInfo9 = service.GetPolicyInfo(requestContext, "Policy.AllowGitHubInvitationsAccessToken");
      policies.Add("Policy.AllowGitHubInvitationsAccessToken", new PolicyModel(policyInfo9.Description, policy9.EffectiveValue, policyInfo9.MoreInfoLink?.ToString()));
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationPolicies.TeamAdminsMaterialization"))
      {
        Policy<bool> policy10 = service.GetPolicy<bool>(requestContext, "Policy.AllowTeamAdminsInvitationsAccessToken", true);
        PolicyInfo policyInfo10 = service.GetPolicyInfo(requestContext, "Policy.AllowTeamAdminsInvitationsAccessToken");
        policies.Add("Policy.AllowTeamAdminsInvitationsAccessToken", new PolicyModel(policyInfo10.Description, policy10.EffectiveValue, policyInfo10.MoreInfoLink?.ToString()));
      }
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      TeamFoundationIdentity ownerIdentity = ((IEnumerable<TeamFoundationIdentity>) vssRequestContext.GetService<ITeamFoundationIdentityService>().ReadIdentities(vssRequestContext, new Guid[1]
      {
        collection.Owner
      }, MembershipQuery.None, ReadIdentityOptions.None, (IEnumerable<string>) null)).FirstOrDefault<TeamFoundationIdentity>();
      string locationServiceUrl = requestContext.GetService<ILocationService>().GetLocationServiceUrl(requestContext, Microsoft.VisualStudio.Services.Location.LocationServiceConstants.SelfReferenceIdentifier, AccessMappingConstants.PublicAccessMappingMoniker);
      string region = collection.Properties.GetValue<string>("Microsoft.VisualStudio.Services.Account.TfsAccountRegionDisplayName", string.Empty);
      string privacyUrl = collection.Properties.GetValue<string>("SystemProperty.PrivacyUrl", string.Empty);
      ISubscriptionAccount subscriptionAccount = vssRequestContext.GetService<ISubscriptionService>().GetSubscriptionAccount(vssRequestContext, AccountProviderNamespace.VisualStudioOnline, requestContext.ServiceHost.InstanceId);
      bool? devopsDomainUrls = new bool?();
      string targetAccountUrl = (string) null;
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.HostResolution.AllowCodexDomainMigration"))
      {
        devopsDomainUrls = new bool?(requestContext.UseDevOpsDomainUrls());
        targetAccountUrl = (!devopsDomainUrls.Value ? (IHostUriData) new DevOpsCollectionHostUriData(collection.Name) : (IHostUriData) new RootMappingHostUriData(collection.Name)).BuildUri(requestContext.To(TeamFoundationHostType.Deployment), ServiceInstanceTypes.TFS)?.ToString();
        if (targetAccountUrl == null)
        {
          requestContext.Trace(504228, TraceLevel.Error, "HostingAccount", nameof (ApiAccountController), "Failed to resolve target account Url, devopsDomainUrls={0}", (object) devopsDomainUrls);
          devopsDomainUrls = new bool?();
        }
      }
      return new AccountSettingsModel(collectionTimeZone.Id, ownerIdentity.TeamFoundationId, collection.Name, locationServiceUrl, ownerIdentity, region, privacyUrl, (Guid?) subscriptionAccount?.SubscriptionId, devopsDomainUrls, targetAccountUrl, policies);
    }

    private bool UpdateAccountForSoftDelete(
      IVssRequestContext requestContext,
      string accountName,
      string accountDeleteReason)
    {
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      Collection collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) null);
      if (string.IsNullOrEmpty(accountName) || !VssStringComparer.HostingAccountPropertyName.Equals(accountName, collection.Name))
        throw new AccountPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AccountServerResources.AccountNameNotMatched, (object) accountName));
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Application);
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = vssRequestContext1.GetService<IdentityService>().ReadIdentities(vssRequestContext1, (IList<Guid>) new Guid[1]
      {
        collection.Owner
      }, QueryMembership.None, (IEnumerable<string>) null)[0];
      if (userIdentity.Id != readIdentity.Id && !IdentityDescriptorComparer.Instance.Equals(userIdentity.Descriptor, readIdentity.Descriptor))
        throw new TeamFoundationSecurityServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AccountServerResources.AccessDeniedDeleteNotAccountOwner, (object) userIdentity.ProviderDisplayName));
      requestContext.Trace(504226, TraceLevel.Info, "HostingAccount", nameof (ApiAccountController), "Account SoftDelete from {0} by {2}", (object) collection.Name, (object) accountName);
      string name = collection.Name;
      IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
      int num = vssRequestContext1.GetService<IOrganizationService>().DeleteCollection(vssRequestContext1, collection.Id) ? 1 : 0;
      if (num != 0)
      {
        this.TraceAccountSoftDelete(vssRequestContext2, collection, name, userIdentity, accountDeleteReason);
        return num != 0;
      }
      vssRequestContext2.Trace(504244, TraceLevel.Error, "HostingAccount", nameof (ApiAccountController), string.Format("Account softDelete fail for account: {0}", (object) collection.Id));
      return num != 0;
    }

    private void TraceAccountSoftDelete(
      IVssRequestContext deploymentRequestContext,
      Collection account,
      string orignalaccountName,
      Microsoft.VisualStudio.Services.Identity.Identity user,
      string accountDeleteReason)
    {
      try
      {
        CustomerIntelligenceData properties = new CustomerIntelligenceData();
        properties.Add(CustomerIntelligenceProperty.AccountId, (object) account.Id);
        properties.Add(CustomerIntelligenceProperty.AccountName, orignalaccountName);
        properties.Add(CustomerIntelligenceProperty.NewAccountName, account.Name);
        properties.Add(CustomerIntelligenceProperty.UserId, (object) user.Id);
        properties.Add(CustomerIntelligenceProperty.UserCUID, (object) IdentityCuidHelper.ComputeCuid(deploymentRequestContext, (IReadOnlyVssIdentity) user));
        properties.Add(CustomerIntelligenceProperty.AccountSoftDeleteReason, string.IsNullOrEmpty(accountDeleteReason) ? string.Empty : accountDeleteReason);
        deploymentRequestContext.GetService<CustomerIntelligenceService>().Publish(deploymentRequestContext, CustomerIntelligenceArea.Account, CustomerIntelligenceFeature.AccountSoftDeleted, properties);
      }
      catch (Exception ex)
      {
        deploymentRequestContext.TraceException(5000008, "HostingAccount", nameof (ApiAccountController), ex);
      }
    }

    private string GetUiErrorMessageForSoftDelete(Exception ex)
    {
      switch (ex)
      {
        case ArgumentNullException _:
          return ex.Message;
        case AccountPropertyException _:
          return ex.Message;
        case TeamFoundationSecurityServiceException _:
          return ex.Message;
        case AccessCheckException _:
          return AccountServerResources.AccessDenied;
        case UnableToDeleteAzureLinkedAccountException _:
          return AccountServerResources.DeleteUnlinkedAccountErrorMessage;
        default:
          return AccountServerResources.AccountDeletePleaseTryAgainLater;
      }
    }

    internal static void UpdateAccountAdvancedSettingsInternal(
      IVssRequestContext requestContext,
      string accountName)
    {
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      if (string.IsNullOrEmpty(accountName))
        return;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      Guid collectionId;
      if (HostNameResolver.TryGetCollectionServiceHostId(vssRequestContext, accountName, out collectionId) && collectionId != requestContext.ServiceHost.InstanceId)
        throw new AccountPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AccountServerResources.RenameAccountInUse, (object) accountName));
      TeamFoundationIdentityService service1 = requestContext.GetService<TeamFoundationIdentityService>();
      TeamFoundationIdentity readIdentity = service1.ReadIdentities(requestContext, new Guid[1]
      {
        requestContext.GetUserId()
      })[0];
      if (!service1.IsMember(requestContext, GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, readIdentity.Descriptor))
        throw new TeamFoundationSecurityServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AccountServerResources.AccessDeniedRenameNotAccountOwner, (object) readIdentity.ProviderDisplayName));
      ICollectionService service2 = requestContext.GetService<ICollectionService>();
      Collection collection = service2.GetCollection(requestContext, (IEnumerable<string>) null);
      service2.RenameCollection(requestContext, accountName);
      requestContext.Trace(504227, TraceLevel.Info, "HostingAccount", nameof (ApiAccountController), "Account renamed from {0} to {1} by {2}", (object) collection.Name, (object) accountName, (object) requestContext.AuthenticatedUserName);
      vssRequestContext.GetService<IHostSyncService>().EnsureHostUpdated(vssRequestContext, requestContext.ServiceHost.InstanceId);
    }

    internal static void UpdateAccountSettingsInternal(
      IVssRequestContext requestContext,
      string timeZoneId,
      string accountOwnerId,
      string privacyUrl,
      bool? basicAuthDisabled = null,
      bool? oAuthDisabled = null,
      bool? guestUserDisabled = null,
      bool? publicKeyDisabled = null,
      bool? policyPPEnabled = null,
      bool? policyOPEnabled = null,
      bool? policyEnforceAADConditionalAccess = null,
      bool? policyAllowGitHubInvitations = null,
      bool? policyAllowRequestAccess = null,
      bool? policyAllowTeamAdminsInvitations = null)
    {
      requestContext.CheckServiceHostType(TeamFoundationHostType.ProjectCollection);
      TeamFoundationIdentityService service1 = requestContext.GetService<TeamFoundationIdentityService>();
      if (!string.IsNullOrEmpty(timeZoneId))
      {
        ICollectionPreferencesService service2 = requestContext.GetService<ICollectionPreferencesService>();
        TimeZoneInfo systemTimeZoneById = TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        try
        {
          CollectionPreferences collectionPreferences = service2.GetCollectionPreferences(requestContext) ?? new CollectionPreferences();
          collectionPreferences.TimeZone = systemTimeZoneById;
          service2.SetCollectionPreferences(requestContext, collectionPreferences);
        }
        catch (OrganizationServiceSecurityException ex)
        {
          throw new OrganizationServiceSecurityException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AccountServerResources.AccountSpecificSettingNoPermission, (object) service1.ReadRequestIdentity(requestContext).DisplayName, (object) AccountServerResources.AccountSettingTimeZone));
        }
      }
      if (oAuthDisabled.HasValue || basicAuthDisabled.HasValue || guestUserDisabled.HasValue || publicKeyDisabled.HasValue || policyPPEnabled.HasValue || policyOPEnabled.HasValue || policyEnforceAADConditionalAccess.HasValue || policyAllowGitHubInvitations.HasValue || policyAllowRequestAccess.HasValue || policyAllowTeamAdminsInvitations.HasValue)
      {
        IOrganizationPolicyService service3 = requestContext.GetService<IOrganizationPolicyService>();
        bool defaultValue1 = requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationPolicies.AltCredDefaultToTrue");
        bool defaultValue2 = requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.PolicyAllowRequestAccess.DefaultValue");
        bool policyDefaultValue = AadGuestUserAccessHelper.DisallowAadGuestUserAccessPolicyDefaultValue;
        Policy<bool> policy1 = service3.GetPolicy<bool>(requestContext, "Policy.DisallowBasicAuthentication", defaultValue1);
        Policy<bool> policy2 = service3.GetPolicy<bool>(requestContext, "Policy.DisallowOAuthAuthentication", false);
        Policy<bool> policy3 = service3.GetPolicy<bool>(requestContext, "Policy.DisallowAadGuestUserAccess", policyDefaultValue);
        Policy<bool> policy4 = service3.GetPolicy<bool>(requestContext, "Policy.DisallowSecureShell", false);
        Policy<bool> policy5 = service3.GetPolicy<bool>(requestContext, "Policy.AllowAnonymousAccess", false);
        Policy<bool> policy6 = service3.GetPolicy<bool>(requestContext, "Policy.AllowOrgAccess", true);
        Policy<bool> policy7 = service3.GetPolicy<bool>(requestContext, "Policy.EnforceAADConditionalAccess", false);
        Policy<bool> policy8 = service3.GetPolicy<bool>(requestContext, "Policy.AllowGitHubInvitationsAccessToken", false);
        Policy<bool> policy9 = service3.GetPolicy<bool>(requestContext, "Policy.AllowRequestAccessToken", defaultValue2);
        Policy<bool> policy10 = service3.GetPolicy<bool>(requestContext, "Policy.AllowTeamAdminsInvitationsAccessToken", true);
        try
        {
          ApiAccountController.CheckAndSetPolicyValue(requestContext, policy1, basicAuthDisabled);
          ApiAccountController.CheckAndSetPolicyValue(requestContext, policy2, oAuthDisabled);
          ApiAccountController.CheckAndSetPolicyValue(requestContext, policy3, guestUserDisabled);
          ApiAccountController.CheckAndSetPolicyValue(requestContext, policy4, publicKeyDisabled);
          ApiAccountController.CheckAndSetPolicyValue(requestContext, policy7, policyEnforceAADConditionalAccess);
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.AnonymousAccess"))
            ApiAccountController.CheckAndSetPolicyValue(requestContext, policy5, policyPPEnabled);
          if (requestContext.IsOrganizationActivated())
            ApiAccountController.CheckAndSetPolicyValue(requestContext, policy6, policyOPEnabled);
          ApiAccountController.CheckAndSetPolicyValue(requestContext, policy9, policyAllowRequestAccess);
          ApiAccountController.CheckAndSetPolicyValue(requestContext, policy8, policyAllowGitHubInvitations);
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationPolicies.TeamAdminsMaterialization"))
            ApiAccountController.CheckAndSetPolicyValue(requestContext, policy10, policyAllowTeamAdminsInvitations);
        }
        catch (OrganizationServiceSecurityException ex)
        {
          throw new OrganizationServiceSecurityException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AccountServerResources.AccountSpecificSettingNoPermission, (object) service1.ReadRequestIdentity(requestContext).DisplayName, (object) AccountServerResources.AccountSettingPolicy));
        }
      }
      if (privacyUrl != null)
      {
        ICollectionService service4 = requestContext.GetService<ICollectionService>();
        PropertyBag propertyBag = new PropertyBag();
        propertyBag.Add("SystemProperty.PrivacyUrl", (object) privacyUrl);
        IVssRequestContext context = requestContext;
        PropertyBag properties = propertyBag;
        service4.UpdateProperties(context, properties);
      }
      if (string.IsNullOrEmpty(accountOwnerId))
        return;
      Collection collection = ApiAccountController.HasReassignAccountPermission(requestContext) ? requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) null) : throw new TeamFoundationSecurityServiceException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, AccountServerResources.AccessDeniedAccountReassignment, (object) service1.ReadRequestIdentity(requestContext).DisplayName));
      ApiAccountController.CheckAndSetNewAccountOwner(requestContext, accountOwnerId, collection.Owner);
    }

    private static bool CheckAndSetNewAccountOwner(
      IVssRequestContext requestContext,
      string newOwnerIdAsString,
      Guid existingOwnerId)
    {
      if (string.IsNullOrEmpty(newOwnerIdAsString))
        return false;
      Guid guid = new Guid(newOwnerIdAsString);
      if (existingOwnerId == guid)
        return false;
      IList<Microsoft.VisualStudio.Services.Identity.Identity> identityList = requestContext.GetService<IdentityService>().ReadIdentities(requestContext, (IList<Guid>) new Guid[2]
      {
        guid,
        existingOwnerId
      }, QueryMembership.None, (IEnumerable<string>) null);
      if (identityList.Count != 2 || identityList.Contains((Microsoft.VisualStudio.Services.Identity.Identity) null))
        throw new IdentityNotFoundException(guid);
      if (IdentityDescriptorComparer.Instance.Equals(identityList[0].Descriptor, identityList[1].Descriptor))
        return false;
      Microsoft.VisualStudio.Services.Identity.Identity readIdentity = identityList[0];
      if (!requestContext.IsFeatureEnabled("WebAccess.Account.UpdateAccountSettings.UseLocalIdToUpdateCollectionOwner") && identityList[0].MasterId == IdentityConstants.LinkedId)
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        readIdentity = vssRequestContext.GetService<IdentityService>().ReadIdentities(vssRequestContext, (IList<IdentityDescriptor>) new IdentityDescriptor[1]
        {
          identityList[0].Descriptor
        }, QueryMembership.None, (IEnumerable<string>) null)[0];
        guid = readIdentity.Id;
      }
      if (readIdentity.IsContainer || IdentityValidation.IsBindPendingType(readIdentity.Descriptor) || IdentityValidation.IsUnauthenticatedType(readIdentity.Descriptor))
        throw new IdentityServiceException(AccountServerResources.BindPendingNotAllowed);
      return IdentityHelper.IsUserIdentity(requestContext, (IReadOnlyVssIdentity) readIdentity) ? requestContext.GetService<ICollectionService>().UpdateCollectionOwner(requestContext, guid) : throw new IdentityServiceException(AccountServerResources.OnlyUserIdentitiesAllowed);
    }

    private static bool CheckAndSetPolicyValue(
      IVssRequestContext requestContext,
      Policy<bool> policy,
      bool? newPolicyValue)
    {
      if (!newPolicyValue.HasValue || !policy.IsValueUndefined && policy.Value == newPolicyValue.Value)
        return false;
      requestContext.GetService<IOrganizationPolicyService>().SetPolicyValue<bool>(requestContext, policy.Name, newPolicyValue.Value);
      return true;
    }

    private static bool HasReassignAccountPermission(IVssRequestContext collectionContext) => collectionContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(collectionContext, AccountAdminSecurity.NamespaceId).HasPermission(collectionContext, AccountAdminSecurity.OwnershipToken, 4, false);
  }
}
