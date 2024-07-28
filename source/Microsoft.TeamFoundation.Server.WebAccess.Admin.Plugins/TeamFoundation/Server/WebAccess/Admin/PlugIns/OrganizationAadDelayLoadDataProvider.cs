// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.OrganizationAadDelayLoadDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.TenantPolicy;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class OrganizationAadDelayLoadDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.OrganizationAdminAadDelayLoad";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      AadTenantPolicy aadTenantPolicy1 = OrganizationAadDelayLoadDataProvider.FetchAadTenantPolicy(requestContext, "TenantPolicy.OrganizationCreationRestriction");
      AadTenantPolicy aadTenantPolicy2 = OrganizationAadDelayLoadDataProvider.FetchAadTenantPolicy(requestContext, "TenantPolicy.RestrictGlobalPersonalAccessToken");
      AadTenantPolicy aadTenantPolicy3 = OrganizationAadDelayLoadDataProvider.FetchAadTenantPolicy(requestContext, "TenantPolicy.RestrictFullScopePersonalAccessToken");
      AadTenantPolicy aadTenantPolicy4 = OrganizationAadDelayLoadDataProvider.FetchAadTenantPolicy(requestContext, "TenantPolicy.RestrictPersonalAccessTokenLifespan");
      AadTenantPolicy aadTenantPolicy5 = OrganizationAadDelayLoadDataProvider.FetchAadTenantPolicy(requestContext, "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation", true);
      return (object) new OrganizationAadDelayLoadSettingsData()
      {
        OrgCreationPolicy = aadTenantPolicy1,
        GlobalTokenPolicy = aadTenantPolicy2,
        FullScopeTokenPolicy = aadTenantPolicy3,
        TokenLifespanPolicy = aadTenantPolicy4,
        LeakedTokenAutoRevocationPolicy = aadTenantPolicy5
      };
    }

    private static AadTenantPolicy FetchAadTenantPolicy(
      IVssRequestContext requestContext,
      string policyName,
      bool enabledByDefault = false)
    {
      try
      {
        if (PolicyPermissionHelper.Instance.HasPermission(requestContext, policyName))
        {
          Policy policy = requestContext.GetService<FrameworkTenantPolicyService>().GetPolicy(requestContext, policyName);
          return new AadTenantPolicy()
          {
            IsPolicyEnabled = policy == null ? enabledByDefault : policy.Value,
            PolicyName = policyName,
            Properties = OrganizationAadDelayLoadDataProvider.GetPolicyProperties(requestContext, policy, policyName)
          };
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050085, TraceLevel.Error, "OrganizationAAD", "Service", ex, string.Format("Failed to fetch tenant policy {0} with exception: {1}", (object) policyName, (object) ex));
      }
      return (AadTenantPolicy) null;
    }

    private static List<Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject> GetAllowedList(
      IVssRequestContext requestContext,
      Policy policyData)
    {
      List<Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject> allowedList = new List<Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>();
      if (policyData?.Properties != null)
      {
        string json;
        policyData.Properties.TryGetValue("AllowedUsersAndGroupObjectIds", out json);
        if (!string.IsNullOrWhiteSpace(json))
        {
          IEnumerable<Guid> source = Enumerable.Empty<Guid>();
          try
          {
            source = (IEnumerable<Guid>) JsonUtilities.Deserialize<ISet<Guid>>(json);
          }
          catch (Exception ex)
          {
            requestContext.TraceException(10050086, TraceLevel.Error, "OrganizationAAD", "Service", ex);
          }
          if (source.Any<Guid>())
          {
            AadService service = requestContext.GetService<AadService>();
            GetUsersWithIdsResponse<Guid> usersWithIds = service.GetUsersWithIds<Guid>(requestContext, new GetUsersWithIdsRequest<Guid>()
            {
              Identifiers = source
            });
            allowedList.AddRange(usersWithIds != null ? (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>) usersWithIds.Users.Select<KeyValuePair<Guid, AadUser>, AadUser>((Func<KeyValuePair<Guid, AadUser>, AadUser>) (user => user.Value)).Where<AadUser>((Func<AadUser, bool>) (user => user != null)).Select<AadUser, Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>((Func<AadUser, Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>) (user => new Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject()
            {
              DisplayName = user.DisplayName,
              IsUser = true,
              ObjectId = user.ObjectId
            })).ToList<Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>() : (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>) null);
            GetGroupsWithIdsResponse<Guid> groupsWithIds = service.GetGroupsWithIds<Guid>(requestContext, new GetGroupsWithIdsRequest<Guid>()
            {
              Identifiers = source
            });
            allowedList.AddRange(groupsWithIds != null ? (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>) groupsWithIds.Groups.Select<KeyValuePair<Guid, AadGroup>, AadGroup>((Func<KeyValuePair<Guid, AadGroup>, AadGroup>) (group => group.Value)).Where<AadGroup>((Func<AadGroup, bool>) (group => group != null)).Select<AadGroup, Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>((Func<AadGroup, Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>) (group => new Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject()
            {
              DisplayName = group.DisplayName,
              IsUser = false,
              ObjectId = group.ObjectId
            })).ToList<Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>() : (IEnumerable<Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models.AadObject>) null);
          }
        }
      }
      return allowedList;
    }

    private static string GetErrorMessage(Policy policyData)
    {
      string empty = string.Empty;
      if (policyData?.Properties != null)
        policyData.Properties.TryGetValue("ErrorMessage", out empty);
      return empty;
    }

    private static int GetMaxPATLifespanDays(Policy policyData)
    {
      int result = 30;
      string s = result.ToString();
      if (policyData?.Properties != null)
        policyData.Properties.TryGetValue("MaxPatLifespanInDays", out s);
      int.TryParse(s, out result);
      return result;
    }

    private static Dictionary<string, object> GetPolicyProperties(
      IVssRequestContext requestContext,
      Policy policyData,
      string policyName)
    {
      Dictionary<string, object> policyProperties = new Dictionary<string, object>();
      switch (policyName)
      {
        case "TenantPolicy.OrganizationCreationRestriction":
          policyProperties.Add("AllowedUsersAndGroupObjectIds", (object) OrganizationAadDelayLoadDataProvider.GetAllowedList(requestContext, policyData));
          policyProperties.Add("ErrorMessage", (object) OrganizationAadDelayLoadDataProvider.GetErrorMessage(policyData));
          goto case "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation";
        case "TenantPolicy.RestrictGlobalPersonalAccessToken":
          policyProperties.Add("AllowedUsersAndGroupObjectIds", (object) OrganizationAadDelayLoadDataProvider.GetAllowedList(requestContext, policyData));
          goto case "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation";
        case "TenantPolicy.RestrictFullScopePersonalAccessToken":
          policyProperties.Add("AllowedUsersAndGroupObjectIds", (object) OrganizationAadDelayLoadDataProvider.GetAllowedList(requestContext, policyData));
          goto case "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation";
        case "TenantPolicy.RestrictPersonalAccessTokenLifespan":
          policyProperties.Add("MaxPatLifespanInDays", (object) OrganizationAadDelayLoadDataProvider.GetMaxPATLifespanDays(policyData));
          policyProperties.Add("AllowedUsersAndGroupObjectIds", (object) OrganizationAadDelayLoadDataProvider.GetAllowedList(requestContext, policyData));
          goto case "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation";
        case "TenantPolicy.EnableLeakedPersonalAccessTokenAutoRevocation":
          return policyProperties;
        default:
          throw new Exception("Could not load properties data for $" + policyName);
      }
    }
  }
}
