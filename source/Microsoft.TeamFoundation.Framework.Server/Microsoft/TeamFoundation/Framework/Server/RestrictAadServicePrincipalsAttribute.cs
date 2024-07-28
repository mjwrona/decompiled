// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RestrictAadServicePrincipalsAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Identity;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RestrictAadServicePrincipalsAttribute : ActionFilterAttribute
  {
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;

    public string WhiteListCategoryName { get; set; }

    public RestrictAadServicePrincipalsAttribute()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public RestrictAadServicePrincipalsAttribute(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (!(actionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      if (tfsRequestContext.UserContext != (IdentityDescriptor) null && tfsRequestContext.UserContext.IsAadServicePrincipalType() && this.aadServicePrincipalConfigurationHelper.IsRestrictionOfCertainApisForServicePrincipalsEnabled(tfsRequestContext) && (string.IsNullOrEmpty(this.WhiteListCategoryName) || !this.aadServicePrincipalConfigurationHelper.IsAadServicePrincipalWhitelistedForCategory(tfsRequestContext, this.WhiteListCategoryName, tfsRequestContext.GetUserCuid())))
        throw new InvalidAccessException("Service principals are not allowed to perform this action.");
    }
  }
}
