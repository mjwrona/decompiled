// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.RestrictAadServicePrincipalsDescriptorAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Globalization;
using System.Web;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class RestrictAadServicePrincipalsDescriptorAttribute : ActionFilterAttribute
  {
    public string ParameterName { get; set; }

    public RestrictAadServicePrincipalsDescriptorAttribute()
      : this(AadServicePrincipalConfigurationHelper.Instance, string.Empty)
    {
    }

    public RestrictAadServicePrincipalsDescriptorAttribute(string parameterName)
      : this(AadServicePrincipalConfigurationHelper.Instance, parameterName)
    {
    }

    public RestrictAadServicePrincipalsDescriptorAttribute(
      IAadServicePrincipalConfigurationHelper aadServicePrincipalConfigurationHelper,
      string parameterName)
    {
      this.ParameterName = parameterName;
    }

    public override void OnActionExecuting(HttpActionContext actionContext)
    {
      object obj;
      if (actionContext.ControllerContext != null && actionContext.ControllerContext.Controller is TfsApiController && actionContext.ControllerContext.RouteData != null && actionContext.ControllerContext.RouteData.Values.TryGetValue(this.ParameterName, out obj) && obj != null && SubjectDescriptor.FromString(HttpUtility.UrlDecode(Convert.ToString(obj, (IFormatProvider) CultureInfo.InvariantCulture))).IsAadServicePrincipalType())
        throw new SubjectDescriptorNotSupportedException("This action cannot be performed for aad service principals.");
    }
  }
}
