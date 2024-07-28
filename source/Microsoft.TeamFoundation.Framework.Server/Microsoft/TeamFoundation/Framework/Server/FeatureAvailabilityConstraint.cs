// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FeatureAvailabilityConstraint
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FeatureAvailabilityConstraint : IHttpRouteConstraint
  {
    private readonly string m_featureName;

    public FeatureAvailabilityConstraint(string featureName) => this.m_featureName = featureName;

    public bool Match(
      HttpRequestMessage request,
      IHttpRoute route,
      string parameterName,
      IDictionary<string, object> values,
      HttpRouteDirection routeDirection)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(this.m_featureName, "featureName");
      return request.GetIVssRequestContext().IsFeatureEnabled(this.m_featureName);
    }
  }
}
