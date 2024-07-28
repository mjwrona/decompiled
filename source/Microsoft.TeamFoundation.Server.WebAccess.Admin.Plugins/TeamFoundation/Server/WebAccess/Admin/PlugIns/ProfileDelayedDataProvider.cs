// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileDelayedDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.MarketingPreferences;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProfileDelayedDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProfileAboutViewDelayed";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      IClientLocationProviderService service = requestContext.GetService<IClientLocationProviderService>();
      service.AddLocation(requestContext, providerContext.SharedData, UserConstants.UserServicePrincipalId, new TeamFoundationHostType?(TeamFoundationHostType.Deployment));
      service.AddLocation(requestContext, providerContext.SharedData, AexServiceConstants.ServiceInstanceTypeId, new TeamFoundationHostType?(TeamFoundationHostType.Deployment));
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      Microsoft.VisualStudio.Services.MarketingPreferences.MarketingPreferences marketingPreferences = requestContext.GetService<IMarketingPreferencesService>().GetMarketingPreferences(requestContext, userIdentity.SubjectDescriptor);
      ProfileDelayedData data;
      try
      {
        data = new ProfileDelayedData()
        {
          NamedSessionToken = OrganizationAdminDataProviderHelper.GetSessionToken(requestContext, TeamFoundationHostType.Deployment, "sessiontoken-profileadmin"),
          MarketingPreferences = marketingPreferences
        };
      }
      catch (Exception ex)
      {
        data = (ProfileDelayedData) null;
        requestContext.TraceException(10050079, "ProfileSettings", "DataProvider", ex);
      }
      return (object) data;
    }
  }
}
