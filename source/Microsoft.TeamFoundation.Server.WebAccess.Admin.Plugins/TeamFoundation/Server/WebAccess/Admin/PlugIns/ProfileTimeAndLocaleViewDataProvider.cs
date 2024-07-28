// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileTimeAndLocaleViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.Models;
using Microsoft.TeamFoundation.Server.WebAccess.Navigation;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.Web.Profile.Builders;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProfileTimeAndLocaleViewDataProvider : IExtensionDataProvider
  {
    private static readonly string TFSUserQueryPattern = "TFS.*";
    private static readonly string TimeZoneQueryName = "TFS.TimeZone";
    private static readonly string CultureQueryName = "TFS.Culture";
    private static readonly string DatePatternQueryName = "TFS.DatePattern";
    private static readonly string TimePatternQueryName = "TFS.TimePattern";

    public string Name => "Admin.ProfileTimeAndLocaleView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      requestContext.GetService<IClientLocationProviderService>().AddLocation(requestContext, providerContext.SharedData, UserConstants.UserServicePrincipalId, new TeamFoundationHostType?(TeamFoundationHostType.Deployment));
      ProfileUserPreferencesData data = new ProfileUserPreferencesData();
      try
      {
        IUserService service = requestContext.GetService<IUserService>();
        Guid userId = requestContext.GetUserId();
        SubjectDescriptor descriptor = service.GetUser(requestContext, userId).Descriptor;
        IdentityData identityData = new IdentityData()
        {
          Id = userId,
          SubjectDescriptor = descriptor.ToString()
        };
        data.UserIdentity = identityData;
        IList<UserAttribute> userAttributeList = service.QueryAttributes(requestContext, descriptor, ProfileTimeAndLocaleViewDataProvider.TFSUserQueryPattern);
        ProfilePreferencesModel availablePreferences = ProfilePreferencesBuilder.GetAvailablePreferences(requestContext, HttpContextFactory.Current);
        availablePreferences.Cultures = (IList<CultureInfoModel>) availablePreferences.Cultures.GroupBy<CultureInfoModel, string>((Func<CultureInfoModel, string>) (element => element.Language)).Select<IGrouping<string, CultureInfoModel>, CultureInfoModel>((Func<IGrouping<string, CultureInfoModel>, CultureInfoModel>) (group => group.FirstOrDefault<CultureInfoModel>())).ToList<CultureInfoModel>();
        foreach (UserAttribute userAttribute in (IEnumerable<UserAttribute>) userAttributeList)
        {
          if (userAttribute.Name.Equals(ProfileTimeAndLocaleViewDataProvider.TimeZoneQueryName))
            data.TimeZone = userAttribute.Value;
          else if (userAttribute.Name.Equals(ProfileTimeAndLocaleViewDataProvider.CultureQueryName))
            data.Culture = userAttribute.Value;
          else if (userAttribute.Name.Equals(ProfileTimeAndLocaleViewDataProvider.DatePatternQueryName))
            data.DatePattern = userAttribute.Value;
          else if (userAttribute.Name.Equals(ProfileTimeAndLocaleViewDataProvider.TimePatternQueryName))
            data.TimePattern = userAttribute.Value;
        }
        data.TimeZone = !string.IsNullOrWhiteSpace(data.TimeZone) ? data.TimeZone : requestContext.GetCollectionTimeZone().Id;
        data.Culture = !string.IsNullOrWhiteSpace(data.Culture) ? data.Culture : availablePreferences.Cultures.FirstOrDefault<CultureInfoModel>().Language;
        data.DatePattern = !string.IsNullOrWhiteSpace(data.DatePattern) ? data.DatePattern : availablePreferences.Cultures.FirstOrDefault<CultureInfoModel>().OptionalCalendars[0].DateFormats[0].Format;
        data.TimePattern = !string.IsNullOrWhiteSpace(data.TimePattern) ? data.TimePattern : availablePreferences.Cultures.FirstOrDefault<CultureInfoModel>().OptionalCalendars[0].TimeFormats[0].Format;
        data.OldProfileUrl = UserSettingsContext.GetAexProfileUrl(requestContext);
        if (!string.IsNullOrWhiteSpace(data.OldProfileUrl))
          data.OldProfileUrl = data.OldProfileUrl.Replace("o~msft~vsts~usercard", "o~msft~old~vsts~profile");
        data.NamedSessionToken = OrganizationAdminDataProviderHelper.GetSessionToken(requestContext, TeamFoundationHostType.Deployment, "sessiontoken-profileadmin");
        data.ProfilePreferences = availablePreferences;
      }
      catch (Exception ex)
      {
        data = (ProfileUserPreferencesData) null;
        requestContext.TraceException(10050067, "ProfileSettings", "DataProvider", ex);
      }
      return (object) data;
    }
  }
}
