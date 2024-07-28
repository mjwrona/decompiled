// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Routing.DisallowHttpMethodConstraint
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Routing
{
  public class DisallowHttpMethodConstraint : IHttpRouteConstraint, IRouteConstraint
  {
    private readonly string[] methods;

    public DisallowHttpMethodConstraint(HttpMethod[] methods)
    {
      if (methods == null || methods.Length == 0)
        throw new ArgumentNullException(nameof (methods));
      this.methods = ((IEnumerable<HttpMethod>) methods).Select<HttpMethod, string>((Func<HttpMethod, string>) (x => x.Method)).ToArray<string>();
    }

    public DisallowHttpMethodConstraint(HttpMethod method)
      : this(new HttpMethod[1]{ method })
    {
    }

    public bool Match(
      HttpRequestMessage request,
      IHttpRoute route,
      string parameterName,
      IDictionary<string, object> values,
      HttpRouteDirection routeDirection)
    {
      return !this.Contains(request.Method.Method);
    }

    public bool Match(
      HttpContextBase httpContext,
      Route route,
      string parameterName,
      RouteValueDictionary values,
      RouteDirection routeDirection)
    {
      return !this.Contains(httpContext.Request.HttpMethod);
    }

    private bool Contains(string method) => ((IEnumerable<string>) this.methods).Any<string>((Func<string, bool>) (x => x.Equals(method, StringComparison.OrdinalIgnoreCase)));
  }
}
