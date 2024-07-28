// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.Alerts.AlertsExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Alerts, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 0FF2CB39-6514-430A-A4E9-A45535A580D6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Alerts.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Integration.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.VisualStudio.Services.Notifications.Server;
using System;
using System.Diagnostics;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess.Alerts
{
  public static class AlertsExtensions
  {
    public static bool IsAlertAdministrator(this HtmlHelper htmlHelper)
    {
      try
      {
        IVssRequestContext tfsRequestContext = htmlHelper.ViewContext.TfsWebContext().TfsRequestContext;
        return tfsRequestContext.GetService<INotificationSubscriptionService>().CallerHasAdminPermissions(tfsRequestContext);
      }
      catch (Exception ex)
      {
        TeamFoundationTracingService.TraceExceptionRaw(520015, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, ex);
        return false;
      }
    }

    public static MvcHtmlString AlertsAdminViewOptions(this HtmlHelper htmlHelper) => htmlHelper.JsonIsland((object) new
    {
      usesGitVersionControl = AlertsExtensions.DoesProjectUseGitVersionControl(htmlHelper.ViewContext.TfsWebContext()),
      usesVCVersionControl = AlertsExtensions.DoesProjectUseVCVersionControl(htmlHelper.ViewContext.TfsWebContext())
    }, (object) new{ @class = "options" });

    public static bool DoesProjectUseGitVersionControl(TfsWebContext tfsWebContext) => AlertsExtensions.CheckSourceControlCapability(tfsWebContext, "SourceControlGitEnabled");

    public static bool DoesProjectUseVCVersionControl(TfsWebContext tfsWebContext) => AlertsExtensions.CheckSourceControlCapability(tfsWebContext, "SourceControlTfvcEnabled");

    private static bool CheckSourceControlCapability(TfsWebContext tfsWebContext, string capability)
    {
      bool flag = false;
      CatalogNode catalogNode = tfsWebContext.TfsRequestContext.GetService<CommonStructureService>().QueryProjectCatalogNode(tfsWebContext.TfsRequestContext, tfsWebContext.CurrentProjectUri.AbsoluteUri);
      string str;
      if (catalogNode != null && catalogNode.Resource != null && catalogNode.Resource.Properties != null && catalogNode.Resource.Properties.TryGetValue(capability, out str))
        flag = str != null && str.Equals(bool.TrueString);
      return flag;
    }
  }
}
