// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.DataProviders.AdminPolicyDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Helpers;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.FeatureAvailability;
using Microsoft.VisualStudio.Services.FeatureAvailability.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.Organization.Client;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.DataProviders
{
  public class AdminPolicyDataProvider : IExtensionDataProvider
  {
    private static readonly IList<string> s_commonApplicationConnectionPolicies = (IList<string>) new List<string>()
    {
      "Policy.DisallowBasicAuthentication",
      "Policy.DisallowOAuthAuthentication",
      "Policy.DisallowSecureShell"
    };
    private static readonly IList<string> s_aadOnlySecurityPolicies = (IList<string>) new List<string>()
    {
      "Policy.LogAuditEvents"
    };
    private static readonly IList<string> s_collectionPolicies = (IList<string>) new List<string>()
    {
      "Policy.DisallowBasicAuthentication",
      "Policy.DisallowOAuthAuthentication",
      "Policy.DisallowSecureShell",
      "Policy.DisallowAadGuestUserAccess",
      "Policy.AllowAnonymousAccess",
      "Policy.AllowOrgAccess",
      "Policy.EnforceAADConditionalAccess",
      "Policy.AllowGitHubInvitationsAccessToken",
      "Policy.AllowRequestAccessToken"
    };
    private static readonly IList<string> s_organizationPolicies = (IList<string>) new List<string>()
    {
      "Policy.DisallowAadGuestUserAccess",
      "Policy.AllowAnonymousAccess",
      "Policy.AllowOrgAccess",
      "Policy.EnforceAADConditionalAccess"
    };
    private static readonly IList<string> s_userPolicies = (IList<string>) new List<string>()
    {
      "Policy.DisallowAadGuestUserAccess"
    };
    private static readonly IDictionary<PolicyType, IList<string>> s_knownAadCollectionPolicies = (IDictionary<PolicyType, IList<string>>) new Dictionary<PolicyType, IList<string>>()
    {
      {
        PolicyType.ApplicationConnectionPolicies,
        AdminPolicyDataProvider.s_commonApplicationConnectionPolicies
      },
      {
        PolicyType.SecurityPolicies,
        AdminPolicyDataProvider.s_aadOnlySecurityPolicies
      },
      {
        PolicyType.UserPolicies,
        AdminPolicyDataProvider.s_userPolicies
      }
    };
    private static readonly IDictionary<PolicyType, IList<string>> s_knownMsaCollectionPolicies = (IDictionary<PolicyType, IList<string>>) new Dictionary<PolicyType, IList<string>>()
    {
      {
        PolicyType.ApplicationConnectionPolicies,
        AdminPolicyDataProvider.s_commonApplicationConnectionPolicies
      }
    };
    private static readonly IDictionary<PolicyType, IList<string>> s_knownOrganizationPoliciesWithoutDirectoryPolicies = (IDictionary<PolicyType, IList<string>>) new Dictionary<PolicyType, IList<string>>()
    {
      {
        PolicyType.ApplicationConnectionPolicies,
        AdminPolicyDataProvider.s_commonApplicationConnectionPolicies
      },
      {
        PolicyType.SecurityPolicies,
        AdminPolicyDataProvider.s_aadOnlySecurityPolicies
      },
      {
        PolicyType.UserPolicies,
        AdminPolicyDataProvider.s_userPolicies
      }
    };
    private static readonly IDictionary<PolicyType, IList<string>> s_knownOrganizationPolicies = (IDictionary<PolicyType, IList<string>>) new Dictionary<PolicyType, IList<string>>()
    {
      {
        PolicyType.ApplicationConnectionPolicies,
        AdminPolicyDataProvider.s_commonApplicationConnectionPolicies
      },
      {
        PolicyType.SecurityPolicies,
        AdminPolicyDataProvider.s_aadOnlySecurityPolicies
      }
    };
    private static readonly IList<string> s_invertedPolicies = (IList<string>) new List<string>()
    {
      "Policy.DisallowBasicAuthentication",
      "Policy.DisallowOAuthAuthentication",
      "Policy.DisallowAadGuestUserAccess",
      "Policy.DisallowSecureShell"
    };
    private static readonly IList<string> s_defaultTruePolicies = (IList<string>) new List<string>()
    {
      "Policy.AllowOrgAccess",
      "Policy.AllowRequestAccessToken",
      "Policy.AllowTeamAdminsInvitationsAccessToken",
      "Policy.ArtifactsExternalPackageProtectionToken"
    };

    public virtual string Name => "Admin.Organization.Administration.Policy";

    public virtual object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Application);
      Microsoft.VisualStudio.Services.Organization.Organization organization = context.GetService<IOrganizationService>().GetOrganization(context, (IEnumerable<string>) null);
      IOrganizationPolicyService service = requestContext.GetService<IOrganizationPolicyService>();
      PolicyCollection policyCollection = new PolicyCollection();
      IDictionary<PolicyType, IList<string>> applicablePolicies = this.GetApplicablePolicies(requestContext, organization);
      foreach (PolicyType key in (IEnumerable<PolicyType>) applicablePolicies.Keys)
      {
        IList<PolicyData> policyDataList = (IList<PolicyData>) new List<PolicyData>();
        foreach (string policyName in (IEnumerable<string>) applicablePolicies[key])
        {
          bool defaultValue = AdminPolicyDataProvider.s_defaultTruePolicies.Contains(policyName);
          if (requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationPolicies.AltCredDefaultToTrue") && policyName == "Policy.DisallowBasicAuthentication")
            defaultValue = true;
          if (policyName == "Policy.DisallowAadGuestUserAccess")
            defaultValue = AadGuestUserAccessHelper.DisallowAadGuestUserAccessPolicyDefaultValue;
          Policy<bool> policy = service.GetPolicy<bool>(requestContext.Elevate(), policyName, defaultValue);
          if ((!requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationPolicies.HideAltCredForMSFT") || !requestContext.IsMicrosoftTenant()) && !requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationPolicies.HideAltCredForAllDisabled") || !(policy.Name == "Policy.DisallowBasicAuthentication") || !policy.Value)
          {
            Microsoft.VisualStudio.Services.Organization.PolicyInfo policyInfo = service.GetPolicyInfo(requestContext.Elevate(), policyName);
            PolicyData policyData = new PolicyData()
            {
              Policy = AdminPolicyDataProvider.HandleInvertedPolicyCases(requestContext, policy),
              LearnMoreLink = policyInfo.MoreInfoLink?.ToString(),
              Description = policyInfo.Description,
              ApplicableServiceHost = (int) AdminPolicyDataProvider.GetApplicableServiceHost(policyName)
            };
            policyDataList.Add(policyData);
          }
        }
        switch (key)
        {
          case PolicyType.ApplicationConnectionPolicies:
            policyCollection.ApplicationConnection = policyDataList;
            continue;
          case PolicyType.SecurityPolicies:
            policyCollection.Security = policyDataList;
            continue;
          case PolicyType.UserPolicies:
            policyCollection.User = policyDataList;
            continue;
          default:
            continue;
        }
      }
      bool flag = requestContext.GetUserIdentity().MetaType == IdentityMetaType.Guest;
      string empty1 = string.Empty;
      string empty2 = string.Empty;
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
      {
        empty2 = requestContext.ServiceHost.InstanceId.ToString();
        empty1 = requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) new string[1]
        {
          "SystemProperty.RequestAccessUrl"
        }).Properties.GetValue<string>("SystemProperty.RequestAccessUrl", string.Empty);
      }
      return (object) new PoliciesData()
      {
        Policies = policyCollection,
        PermissionBits = AdminPolicyDataProvider.GetPolicyPermissionBits(requestContext),
        IsOrganizationActivated = organization.IsActivated,
        InvertedPolicies = AdminPolicyDataProvider.s_invertedPolicies,
        IsMicrosoftTenant = requestContext.IsMicrosoftTenant(),
        IsGuestUser = flag,
        HostId = empty2,
        RequestAccessUrl = empty1
      };
    }

    private static int GetPolicyPermissionBits(IVssRequestContext requestContext)
    {
      if (!requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        return OrganizationAdministrationDataProviderHelper.GetModifyPropertiesPermissionBits(requestContext);
      Microsoft.VisualStudio.Services.Organization.Collection collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext, (IEnumerable<string>) null);
      return OrganizationAdministrationDataProviderHelper.GetModifyPermissionBits(requestContext, new Guid?(collection.Id));
    }

    private static Policy HandleInvertedPolicyCases(
      IVssRequestContext requestContext,
      Policy<bool> originalPolicy)
    {
      Policy client = originalPolicy.ToClient<bool>();
      if (AdminPolicyDataProvider.s_invertedPolicies.Contains(originalPolicy.Name))
      {
        client.EffectiveValue = (object) !(bool) client.EffectiveValue;
        if (client.ParentPolicy != null)
          client.ParentPolicy.EffectiveValue = (object) !(bool) client.ParentPolicy.EffectiveValue;
      }
      return client;
    }

    private static HostType GetApplicableServiceHost(string policyName)
    {
      HostType applicableServiceHost = HostType.None;
      if (AdminPolicyDataProvider.s_collectionPolicies.Contains(policyName))
        applicableServiceHost |= HostType.Collection;
      if (AdminPolicyDataProvider.s_organizationPolicies.Contains(policyName))
        applicableServiceHost |= HostType.Organization;
      return applicableServiceHost;
    }

    private IDictionary<PolicyType, IList<string>> GetApplicablePolicies(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Organization.Organization organization)
    {
      FeatureFlag featureFlag = requestContext.GetClient<FeatureAvailabilityHttpClient>(ServiceInstanceTypes.SPS).GetFeatureFlagByNameAsync("VisualStudio.Services.Identity.AnonymousAccess").SyncResult<FeatureFlag>();
      bool flag = featureFlag != null && featureFlag.EffectiveState == "On";
      if (organization.TenantId == Guid.Empty)
      {
        IDictionary<PolicyType, IList<string>> dictionary = (IDictionary<PolicyType, IList<string>>) AdminPolicyDataProvider.s_knownMsaCollectionPolicies.ToDictionary<KeyValuePair<PolicyType, IList<string>>, PolicyType, IList<string>>((Func<KeyValuePair<PolicyType, IList<string>>, PolicyType>) (x => x.Key), (Func<KeyValuePair<PolicyType, IList<string>>, IList<string>>) (x => (IList<string>) new List<string>((IEnumerable<string>) x.Value)));
        if (flag)
          dictionary.Add(PolicyType.SecurityPolicies, (IList<string>) new List<string>()
          {
            "Policy.AllowAnonymousAccess"
          });
        dictionary.Add(PolicyType.UserPolicies, (IList<string>) new List<string>()
        {
          "Policy.AllowGitHubInvitationsAccessToken"
        });
        return dictionary;
      }
      IDictionary<PolicyType, IList<string>> dictionary1 = (IDictionary<PolicyType, IList<string>>) (requestContext.ServiceHost.Is(TeamFoundationHostType.Application) ? (IEnumerable<KeyValuePair<PolicyType, IList<string>>>) AdminPolicyDataProvider.s_knownOrganizationPoliciesWithoutDirectoryPolicies : (IEnumerable<KeyValuePair<PolicyType, IList<string>>>) AdminPolicyDataProvider.s_knownAadCollectionPolicies).ToDictionary<KeyValuePair<PolicyType, IList<string>>, PolicyType, IList<string>>((Func<KeyValuePair<PolicyType, IList<string>>, PolicyType>) (x => x.Key), (Func<KeyValuePair<PolicyType, IList<string>>, IList<string>>) (x => (IList<string>) new List<string>((IEnumerable<string>) x.Value)));
      if (flag)
        dictionary1[PolicyType.SecurityPolicies].Add("Policy.AllowAnonymousAccess");
      if (this.AllowProjectVisibilityForMicrosoftTenant(requestContext, organization))
        dictionary1[PolicyType.SecurityPolicies].Add("Policy.AllowOrgAccess");
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.AdminEngagement.OrganizationPolicies.TeamAdminsMaterialization"))
        dictionary1[PolicyType.UserPolicies].Add("Policy.AllowTeamAdminsInvitationsAccessToken");
      dictionary1[PolicyType.SecurityPolicies].Add("Policy.ArtifactsExternalPackageProtectionToken");
      dictionary1[PolicyType.UserPolicies].Add("Policy.AllowRequestAccessToken");
      dictionary1[PolicyType.SecurityPolicies].Add("Policy.EnforceAADConditionalAccess");
      return dictionary1;
    }

    private bool AllowProjectVisibilityForMicrosoftTenant(
      IVssRequestContext requestContext,
      Microsoft.VisualStudio.Services.Organization.Organization organization)
    {
      if (organization.IsActivated)
        return true;
      return requestContext.IsFeatureEnabled("Microsoft.VisualStudio.Services.Identity.UseTenantAsBoundaryForOidProjectVisibility") && requestContext.IsMicrosoftTenant();
    }
  }
}
