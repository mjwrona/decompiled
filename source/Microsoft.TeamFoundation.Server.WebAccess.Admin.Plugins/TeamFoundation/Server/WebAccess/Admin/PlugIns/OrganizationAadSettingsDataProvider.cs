// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.OrganizationAadSettingsDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Navigation;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.Organization;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class OrganizationAadSettingsDataProvider : IExtensionDataProvider
  {
    private readonly string s_area = "IdentityDomainTransfer";
    private readonly string s_layer = nameof (OrganizationAadSettingsDataProvider);
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;

    public string Name => "Admin.OrganizationAdminAad";

    public OrganizationAadSettingsDataProvider()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public OrganizationAadSettingsDataProvider(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      IClientLocationProviderService service = requestContext.GetService<IClientLocationProviderService>();
      service.AddLocation(requestContext, providerContext.SharedData, AexServiceConstants.ServiceInstanceTypeId, new TeamFoundationHostType?(TeamFoundationHostType.Deployment));
      service.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.ProjectCollection));
      service.AddLocation(requestContext, providerContext.SharedData, ServiceInstanceTypes.SPS, new TeamFoundationHostType?(TeamFoundationHostType.Application));
      string str1 = string.Empty;
      string str2 = string.Empty;
      Guid guid = Guid.Empty;
      IVssRequestContext vssRequestContext1 = requestContext.To(TeamFoundationHostType.Application);
      Microsoft.VisualStudio.Services.Organization.Organization organization = vssRequestContext1.GetService<IOrganizationService>().GetOrganization(vssRequestContext1, (IEnumerable<string>) null);
      if (organization != null && organization.TenantId != Guid.Empty)
      {
        guid = organization.TenantId;
        try
        {
          AadTenant tenant = OrganizationAdminDataProviderHelper.GetTenant(vssRequestContext1);
          if (tenant != null)
          {
            str1 = tenant.DisplayName;
            IEnumerable<AadDomain> verifiedDomains = tenant.VerifiedDomains;
            str2 = verifiedDomains != null ? verifiedDomains.Where<AadDomain>((Func<AadDomain, bool>) (vd => vd.IsDefault)).First<AadDomain>().Name : (string) null;
          }
        }
        catch (Exception ex)
        {
          requestContext.TraceException(10050051, TraceLevel.Error, "OrganizationAAD", "Service", ex);
        }
      }
      IVssRequestContext vssRequestContext2 = requestContext.To(TeamFoundationHostType.Deployment);
      string locationServiceUrl = vssRequestContext2.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext2, ServiceInstanceTypes.SPS, AccessMappingConstants.PublicAccessMappingMoniker);
      Collection collection = requestContext.GetService<ICollectionService>().GetCollection(requestContext, Enumerable.Empty<string>());
      string str3 = vssRequestContext2.GetService<IUrlHostResolutionService>().GetHostUri(vssRequestContext2, collection.Id, ServiceInstanceTypes.TFS)?.ToString();
      TenantData tenantData = new TenantData()
      {
        DisplayName = str1,
        Id = guid,
        Domain = str2
      };
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      UserData userData = new UserData()
      {
        Name = userIdentity.DisplayName,
        Id = userIdentity.Id,
        Email = userIdentity.Properties.ContainsKey("mail") ? userIdentity.Properties["mail"].ToString() : "",
        IsMsa = userIdentity.IsMsa(),
        IsGuestUserInAad = userIdentity.MetaType == IdentityMetaType.Guest
      };
      bool flag1 = OrganizationAdminDataProviderHelper.CheckTenantLinkingPermission(requestContext);
      bool flag2 = requestContext.IsMicrosoftTenant();
      string str4 = str3;
      string str5 = locationServiceUrl + "_signout?redirectUrl=" + str4 + "_settings/organizationAad";
      if (requestContext.IsFeatureEnabled("VisualStudio.Services.Identity.NewLinkingUnlinkingFlow.UseSpsUrl.Enable"))
        str5 = UserSettingsContext.GetSignOutUrl(requestContext) + "?redirectUrl=" + str3 + "_settings/organizationAad";
      requestContext.Trace(80920, TraceLevel.Info, this.s_area, this.s_layer, "Using sps singout url of " + str5);
      return (object) new OrganizationAadSettingsData()
      {
        OrgnizationTenantData = tenantData,
        User = userData,
        OrganizationName = collection.Name,
        OrganizationId = collection.Id,
        SpsSignoutUrl = str5,
        HasModifyPermissions = flag1,
        IsMicrosoftTenant = flag2,
        SupportServicePrincipals = this.aadServicePrincipalConfigurationHelper.IsOrgGroupMembershipSupportServicePrincipalsEnabled(requestContext)
      };
    }
  }
}
