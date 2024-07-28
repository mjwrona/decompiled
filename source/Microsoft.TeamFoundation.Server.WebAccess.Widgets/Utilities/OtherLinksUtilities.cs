// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Widgets.Utilities.OtherLinksUtilities
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Widgets, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: DD4C24BB-2646-4C82-B0E8-494FC53AC01D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Widgets.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ExtensionManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.Server.WebAccess.Widgets.Utilities
{
  public class OtherLinksUtilities
  {
    public static object GetData(
      IVssRequestContext requestContext,
      DataProviderContext providerContext)
    {
      string str1 = "";
      string str2 = "";
      string str3 = "";
      int num = 0;
      bool flag = false;
      IVssRequestContext requestContext1 = requestContext.To(TeamFoundationHostType.Application);
      TfsWebContext requestWebContext = WebContextFactory.GetCurrentRequestWebContext<TfsWebContext>();
      Guid projectId = requestWebContext.CurrentProjectGuid != Guid.Empty ? requestWebContext.CurrentProjectGuid : WebPageDataProviderUtil.GetPageSource(requestContext).Project.Id;
      if (requestWebContext != null)
      {
        if (!requestWebContext.IsHosted)
        {
          TeamProjectSettings teamProjectSettings = TeamProjectSettings.GetTeamProjectSettings(requestContext1, projectId);
          if (teamProjectSettings != null)
          {
            if (teamProjectSettings.Portal != (Uri) null)
              str1 = teamProjectSettings.Portal.AbsoluteUri;
            if (teamProjectSettings.Guidance != (Uri) null)
              str2 = teamProjectSettings.Guidance.AbsoluteUri;
            if (teamProjectSettings.ReportManagerFolderUrl != (Uri) null)
              str3 = teamProjectSettings.ReportManagerFolderUrl.AbsoluteUri;
          }
        }
        if (requestWebContext.FeatureContext != null)
        {
          flag = requestWebContext.FeatureContext.IsFeatureAvailable(LicenseFeatures.AdvancedHomePageId);
          num = (int) requestWebContext.FeatureContext.GetFeatureMode(LicenseFeatures.FeedbackId);
        }
      }
      OtherLinksInfo andCheckPermission = OtherLinksInfo.CreateAndCheckPermission(requestContext, projectId);
      andCheckPermission.showTeamLinks = flag;
      andCheckPermission.portalUrl = str1;
      andCheckPermission.guidanceUrl = str2;
      andCheckPermission.reportUrl = str3;
      andCheckPermission.feedbackMode = num;
      return (object) andCheckPermission;
    }
  }
}
