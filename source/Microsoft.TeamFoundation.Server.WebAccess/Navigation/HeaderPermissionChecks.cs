// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Navigation.HeaderPermissionChecks
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Server.WebAccess.Navigation
{
  public static class HeaderPermissionChecks
  {
    public static bool UserHasAdminSettingsPermissions(IVssRequestContext requestContext)
    {
      FeatureContext featureContext = requestContext.FeatureContext();
      bool flag = false;
      if (featureContext.IsAdminFeatureAvailable)
      {
        if (featureContext.AreStandardFeaturesAvailable)
          flag = true;
        else if (!requestContext.ExecutionEnvironment.IsHostedDeployment)
        {
          IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
          flag = vssRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 2, false);
        }
      }
      return flag;
    }
  }
}
