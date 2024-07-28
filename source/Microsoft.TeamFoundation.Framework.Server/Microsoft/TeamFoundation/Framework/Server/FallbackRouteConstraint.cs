// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.FallbackRouteConstraint
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public class FallbackRouteConstraint : IHttpRouteConstraint
  {
    public bool Match(
      HttpRequestMessage request,
      IHttpRoute route,
      string parameterName,
      IDictionary<string, object> values,
      HttpRouteDirection routeDirection)
    {
      IVssRequestContext ivssRequestContext = request.GetIVssRequestContext();
      if (ivssRequestContext != null)
      {
        ApiResourceVersion apiResourceVersion = request.GetApiResourceVersion();
        IEnumerable<string> strings;
        if (apiResourceVersion != null && ivssRequestContext.Items.TryGetValue<IEnumerable<string>>(RequestContextItemsKeys.RoutesMatchedExceptVersion, out strings) && strings.Any<string>())
        {
          if (!strings.Skip<string>(1).Any<string>())
            throw new VssVersionOutOfRangeException(apiResourceVersion, strings.First<string>());
          throw new VssVersionOutOfRangeException(apiResourceVersion, strings);
        }
      }
      return true;
    }
  }
}
