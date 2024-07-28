// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.RequireAccountAdminAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using System.Diagnostics;
using System.Web;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class RequireAccountAdminAttribute : ActionFilterAttribute
  {
    private const string s_area = "WebAccessAttribute";
    private const string s_layer = "RequireAccountAdminAttribute";

    public RequireAccountAdminAttribute()
      : this(false)
    {
    }

    public RequireAccountAdminAttribute(bool allowTroubleshoot) => this.AllowTroubleshoot = allowTroubleshoot;

    public bool AllowTroubleshoot { get; set; }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      if (filterContext.Controller is TfsController controller)
      {
        if (!controller.TfsRequestContext.IsAccountAdmin() && !this.AllowTroubleshootPermission(controller))
          throw new HttpException(404, PlatformResources.PageNotFound);
      }
      else
      {
        TeamFoundationTracingService.TraceRaw(520042, TraceLevel.Error, "WebAccessAttribute", nameof (RequireAccountAdminAttribute), "No TfsController associated with filter context. Defaulting to 404 response.");
        throw new HttpException(404, PlatformResources.PageNotFound);
      }
    }

    private bool AllowTroubleshootPermission(TfsController tfsController)
    {
      if (!this.AllowTroubleshoot)
        return false;
      IVssRequestContext vssRequestContext = tfsController.TfsRequestContext.To(TeamFoundationHostType.Deployment);
      vssRequestContext.Elevate();
      return vssRequestContext.GetService<TeamFoundationSecurityService>().GetSecurityNamespace(vssRequestContext, FrameworkSecurity.DiagnosticNamespaceId).HasPermission(vssRequestContext, FrameworkSecurity.DiagnosticNamespaceToken, 4);
    }
  }
}
