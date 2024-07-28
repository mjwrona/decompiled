// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns.ProfileAboutViewDataProvider
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 362E2629-6AF5-42CD-95A4-09FE50F477E2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.Admin.Plugins.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Navigation;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ExtensionManagement.Sdk.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using Microsoft.VisualStudio.Services.Profile;
using Microsoft.VisualStudio.Services.Users;
using Microsoft.VisualStudio.Services.Users.Server;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin.PlugIns
{
  public class ProfileAboutViewDataProvider : IExtensionDataProvider
  {
    public string Name => "Admin.ProfileAboutView";

    public object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext,
      Contribution contribution)
    {
      ProfileUserData data;
      try
      {
        IUserService service = requestContext.GetService<IUserService>();
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        IVssRequestContext requestContext1 = requestContext;
        SubjectDescriptor subjectDescriptor = userIdentity.SubjectDescriptor;
        User user = service.GetUser(requestContext1, subjectDescriptor);
        ProfileRegion profileRegion = new ProfileRegion()
        {
          Name = user.Country,
          Code = user.Country
        };
        IdentityData identityData = new IdentityData()
        {
          Id = userIdentity.Id,
          SubjectDescriptor = userIdentity.SubjectDescriptor.ToString()
        };
        string str = UserSettingsContext.GetAexProfileUrl(requestContext);
        if (!string.IsNullOrWhiteSpace(str))
          str = str.Replace("o~msft~vsts~usercard", "o~msft~old~vsts~profile");
        data = new ProfileUserData()
        {
          DisplayName = user.DisplayName,
          Email = !string.IsNullOrWhiteSpace(user.UnconfirmedMail) ? user.UnconfirmedMail : user.Mail,
          EmailConfirmationPending = !string.IsNullOrWhiteSpace(user.UnconfirmedMail),
          Region = profileRegion,
          Identity = identityData,
          OldProfileUrl = str
        };
        requestContext.GetService<IClientFeatureProviderService>().AddFeatureState(requestContext, providerContext.SharedData, "ms.vss-admin-web.user-profile-sync-feature");
      }
      catch (Exception ex)
      {
        data = (ProfileUserData) null;
        requestContext.TraceException(10050068, "ProfileSettings", "DataProvider", ex);
      }
      return (object) data;
    }
  }
}
