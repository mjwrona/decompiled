// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RestrictAadServicePrincipalsMvcAttribute
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebPlatform.Sdk.Server;
using System.Web.Mvc;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RestrictAadServicePrincipalsMvcAttribute : ActionFilterAttribute
  {
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;

    public RestrictAadServicePrincipalsMvcAttribute()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public RestrictAadServicePrincipalsMvcAttribute(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
      if (filterContext.Controller is IPlatformController controller)
      {
        IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
        if (tfsRequestContext.UserContext != (IdentityDescriptor) null && tfsRequestContext.UserContext.IsAadServicePrincipalType() && this.aadServicePrincipalConfigurationHelper.IsRestrictionOfCertainApisForServicePrincipalsEnabled(tfsRequestContext))
          throw new InvalidAccessException("Service principals are not allowed to perform this action.");
      }
      base.OnActionExecuting(filterContext);
    }
  }
}
