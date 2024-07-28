// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Dashboards.CrossProjectSettings.CrossProjectSettingsSecurityHelper
// Assembly: Microsoft.TeamFoundation.Dashboards.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A36A0DA7-561F-410D-8C02-3A33CBE61F1D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Dashboards.Common.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.TeamFoundation.Dashboards.CrossProjectSettings
{
  public class CrossProjectSettingsSecurityHelper
  {
    public static bool HasCrossProjectReadPermission(IVssRequestContext requestContext)
    {
      IVssSecurityNamespace securityNamespace = requestContext.GetService<ITeamFoundationSecurityService>().GetSecurityNamespace(requestContext, CrossProjectSettingsPrivileges.NamespaceId);
      string root = CrossProjectSettingsPrivileges.Root;
      IVssRequestContext requestContext1 = requestContext;
      string token = root;
      return securityNamespace.HasPermission(requestContext1, token, 1);
    }

    public static bool CanUserViewCrossProjectSettings(IVssRequestContext requestContext) => CrossProjectSettingsSecurityHelper.HasCrossProjectReadPermission(requestContext);
  }
}
