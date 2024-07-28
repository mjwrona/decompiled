// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Server.License.StakeholderLicenseHandlerAttribute
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 134B1041-BFA6-49C6-8C6D-CA5ADF31AF54
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Server.dll

using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Licensing;
using Microsoft.VisualStudio.Services.ReleaseManagement.Common.Helpers;
using System;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Server.License
{
  [AttributeUsage(AttributeTargets.Class, Inherited = true)]
  public sealed class StakeholderLicenseHandlerAttribute : ActionFilterAttribute
  {
    public bool SkipStakeholderLicense { get; private set; }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      if (actionContext == null)
        throw new ArgumentNullException(nameof (actionContext));
      if (!(actionContext.ControllerContext.Controller is TfsProjectApiController controller))
        throw new InvalidOperationException("Invalid controller type");
      if (!this.SkipStakeholderLicense && controller.TfsRequestContext.HasNoneOrStakeholderLicense() && !StakeholderLicensingHelper.IsBuildAndReleaseEnabledForStakeholders(controller.TfsRequestContext))
        actionContext.CheckRequestAllowedForAnyUserLicense();
      base.OnActionExecuting(actionContext);
    }

    public StakeholderLicenseHandlerAttribute(bool skipStakeholderLicense) => this.SkipStakeholderLicense = skipStakeholderLicense;

    public StakeholderLicenseHandlerAttribute()
      : this(false)
    {
    }
  }
}
