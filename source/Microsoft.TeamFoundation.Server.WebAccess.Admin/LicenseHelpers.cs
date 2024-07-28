// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Admin.LicenseHelpers
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Admin, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 31215F45-B8A9-42A7-99A7-F8CB77B7D405
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Admin.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Aad;
using System;
using System.Diagnostics;
using System.Globalization;

namespace Microsoft.TeamFoundation.Server.WebAccess.Admin
{
  public static class LicenseHelpers
  {
    private const int WebAccessExceptionEaten = 599999;

    public static bool HasManageLicensesPermission(IVssRequestContext requestContext)
    {
      if (requestContext.ExecutionEnvironment.IsHostedDeployment)
        return false;
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Application);
      return vssRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.FrameworkNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.FrameworkNamespaceToken, 2, false);
    }

    internal static string GetAadUserWarningMessage(IVssRequestContext tfsRequestContext)
    {
      string userWarningMessage = (string) null;
      if (tfsRequestContext.IsFeatureEnabled("WebAccess.EnableAddAadUserWarning"))
      {
        try
        {
          userWarningMessage = string.Format((IFormatProvider) CultureInfo.CurrentCulture, AdminServerResources.AddAADUserWithTenantName, (object) tfsRequestContext.GetService<AadService>().GetTenant(tfsRequestContext, new GetTenantRequest()).Tenant.DisplayName);
        }
        catch (AadException ex)
        {
          tfsRequestContext.TraceException(599999, nameof (LicenseHelpers), "AddAadUserWarningMessage", (Exception) ex);
        }
        catch (Exception ex)
        {
          tfsRequestContext.Trace(599999, TraceLevel.Error, nameof (LicenseHelpers), "AddAadUserWarningMessage", "something realy went wrong, AadService should not throw the Exception:{0}", (object) ex);
        }
      }
      if (string.IsNullOrEmpty(userWarningMessage))
        userWarningMessage = AdminServerResources.AddAADUser;
      return userWarningMessage;
    }
  }
}
