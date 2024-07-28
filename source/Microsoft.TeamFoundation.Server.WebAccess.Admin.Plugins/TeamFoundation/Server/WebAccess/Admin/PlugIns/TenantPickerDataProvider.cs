// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.TenantPickerDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Framework.Server.Authentication;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.VisualStudio.Services.Aad;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class TenantPickerDataProvider : IExtensionDataProvider
  {
    private const string devFabricOldDomain = "vsts.me";
    private const string devFabricNewDomain = "codedev.ms";
    private const string prodOldDomain = "visualstudio.com";
    private const string prodNewDomain = "dev.azure.com";

    public string Name => "Admin.TenantPicker";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      string str1 = string.Empty;
      string str2 = string.Empty;
      Guid guid = Guid.Empty;
      string absoluteUri;
      try
      {
        IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
        absoluteUri = new UriBuilder(vssRequestContext.GetService<ILocationService>().GetLocationServiceUrl(vssRequestContext, ServiceInstanceTypes.SPS, AccessMappingConstants.ClientAccessMappingMoniker))
        {
          Path = "_signedin"
        }.Uri.AbsoluteUri;
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050080, TraceLevel.Error, "TenantPicker", "Service", ex);
        return (object) null;
      }
      try
      {
        AadTenant tenant = OrganizationAdminDataProviderHelper.GetTenant(requestContext);
        if (tenant != null)
        {
          str1 = tenant.DisplayName;
          guid = tenant.ObjectId;
          IEnumerable<AadDomain> verifiedDomains = tenant.VerifiedDomains;
          str2 = verifiedDomains != null ? verifiedDomains.Where<AadDomain>((Func<AadDomain, bool>) (vd => vd.IsDefault)).First<AadDomain>().Name : (string) null;
        }
      }
      catch (Exception ex)
      {
        requestContext.TraceException(10050081, TraceLevel.Error, "TenantPicker", "Service", ex);
      }
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
        IsGuestUserInAad = userIdentity.MetaType == IdentityMetaType.Guest,
        Tenant = tenantData
      };
      string siteRootUrl = requestContext.RequestUri().Scheme + Uri.SchemeDelimiter + TenantPickerDataProvider.GetUrlHost(requestContext);
      UserTenantData userTenantData = TenantPickerDataProvider.FetchUserTenantData(requestContext, absoluteUri, siteRootUrl);
      if (userTenantData.UserTenants.Any<TenantData>() && userIdentity.IsMsaOrFpmsa() && tenantData.Id != Guid.Empty)
        userTenantData.UserTenants.Add(new TenantData()
        {
          Id = Guid.Empty,
          DisplayName = "Microsoft Account",
          Domain = userData.Email,
          AuthUrl = new AadAuthUrlUtility.AuthUrlBuilder()
          {
            Tenant = "live.com",
            DomainHint = "live.com",
            RedirectLocation = absoluteUri,
            QueryString = ((IDictionary<string, string>) new Dictionary<string, string>()
            {
              {
                "reply_to",
                siteRootUrl
              }
            })
          }.Build(requestContext)
        });
      userTenantData.UserTenants.Remove(userTenantData.UserTenants.FirstOrDefault<TenantData>((Func<TenantData, bool>) (ut => ut.Id == tenantData.Id)));
      return (object) new TenantPickerData()
      {
        UserTenantData = userTenantData,
        User = userData
      };
    }

    private static UserTenantData FetchUserTenantData(
      IVssRequestContext requestContext,
      string spsSignedInUri,
      string siteRootUrl)
    {
      List<AadTenant> list = OrganizationAdminDataProviderHelper.GetTenants(requestContext).ToList<AadTenant>();
      return new UserTenantData()
      {
        UserContext = requestContext.UserContext != (IdentityDescriptor) null ? requestContext.UserContext.ToString() : string.Empty,
        UserTenants = list.Select<AadTenant, TenantData>((Func<AadTenant, TenantData>) (ut => new TenantData()
        {
          Id = ut.ObjectId,
          DisplayName = ut.DisplayName,
          AuthUrl = new AadAuthUrlUtility.AuthUrlBuilder()
          {
            Tenant = ut.ObjectId.ToString(),
            RedirectLocation = spsSignedInUri,
            QueryString = ((IDictionary<string, string>) new Dictionary<string, string>()
            {
              {
                "reply_to",
                siteRootUrl
              }
            })
          }.Build(requestContext)
        })).ToList<TenantData>()
      };
    }

    private static string GetUrlHost(IVssRequestContext requestContext)
    {
      string host = requestContext.RequestUri().Host;
      if (host.ToLower().Contains("visualstudio.com"))
        return "dev.azure.com";
      return host.ToLower().Contains("vsts.me") ? "codedev.ms" : host;
    }
  }
}
