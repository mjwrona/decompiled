// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.AccessReadConsistencyFilterAttribute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public sealed class AccessReadConsistencyFilterAttribute : ActionFilterAttribute
  {
    internal const string FeatureFlagRestApiReadConsistencyLevel = "VisualStudio.Services.RestApi.ReadConsistencyLevel";
    private readonly string m_FeatureFlag;

    public AccessReadConsistencyFilterAttribute(string featureFlag = null) => this.m_FeatureFlag = featureFlag;

    public override void OnActionExecuting(HttpActionContext filterContext)
    {
      base.OnActionExecuting(filterContext);
      HttpRequestMessage request = filterContext?.Request;
      object obj;
      IEnumerable<string> values;
      VssReadConsistencyLevel result;
      if (request == null || request.Properties == null || request.Headers == null || !(filterContext?.ControllerContext?.Controller is TfsApiController) || !request.Properties.TryGetValue(TfsApiPropertyKeys.TfsRequestContext, out obj) || !(obj is IVssRequestContext requestContext) || !request.Headers.TryGetValues("X-VSS-ReadConsistencyLevel", out values) || !Enum.TryParse<VssReadConsistencyLevel>(values.FirstOrDefault<string>(), out result) || !requestContext.IsFeatureEnabled("VisualStudio.Services.RestApi.ReadConsistencyLevel") || this.m_FeatureFlag != null && !requestContext.IsFeatureEnabled(this.m_FeatureFlag))
        return;
      requestContext.Items["ReadConsistencyLevel"] = (object) result;
    }
  }
}
