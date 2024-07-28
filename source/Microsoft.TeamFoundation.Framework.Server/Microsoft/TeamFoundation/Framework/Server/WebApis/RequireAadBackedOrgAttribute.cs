// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApis.RequireAadBackedOrgAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Organization;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server.WebApis
{
  public class RequireAadBackedOrgAttribute : ActionFilterAttribute
  {
    private readonly IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper;

    public RequireAadBackedOrgAttribute()
      : this(AadServicePrincipalConfigurationHelper.Instance)
    {
    }

    public RequireAadBackedOrgAttribute(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper)
    {
      this.aadServicePrincipalConfigurationHelper = aadServicePrincipalConfigurationHelper;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (!(actionContext.ControllerContext.Controller is TfsApiController controller))
        return;
      IVssRequestContext tfsRequestContext = controller.TfsRequestContext;
      if (this.aadServicePrincipalConfigurationHelper.IsRequireAadBackedOrgAttributeEnabled(tfsRequestContext) && !tfsRequestContext.IsOrganizationAadBacked())
        throw new InvalidOperationException(FrameworkResources.RequireAadBackedOrg());
    }
  }
}
