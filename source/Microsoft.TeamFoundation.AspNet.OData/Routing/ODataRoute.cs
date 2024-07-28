// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Routing.ODataRoute
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Web.Http.Routing;

namespace Microsoft.AspNet.OData.Routing
{
  public class ODataRoute : HttpRoute
  {
    private static readonly string _escapedHashMark = Uri.EscapeDataString("#");
    private static readonly string _escapedQuestionMark = Uri.EscapeDataString("?");

    public ODataRoute(string routePrefix, ODataPathRouteConstraint pathConstraint)
      : this(routePrefix, (IHttpRouteConstraint) pathConstraint, (HttpRouteValueDictionary) null, (HttpRouteValueDictionary) null, (HttpRouteValueDictionary) null, (HttpMessageHandler) null)
    {
    }

    public ODataRoute(string routePrefix, IHttpRouteConstraint routeConstraint)
      : this(routePrefix, routeConstraint, (HttpRouteValueDictionary) null, (HttpRouteValueDictionary) null, (HttpRouteValueDictionary) null, (HttpMessageHandler) null)
    {
    }

    public ODataRoute(
      string routePrefix,
      ODataPathRouteConstraint pathConstraint,
      HttpRouteValueDictionary defaults,
      HttpRouteValueDictionary constraints,
      HttpRouteValueDictionary dataTokens,
      HttpMessageHandler handler)
      : this(routePrefix, (IHttpRouteConstraint) pathConstraint, defaults, constraints, dataTokens, handler)
    {
    }

    public ODataRoute(
      string routePrefix,
      IHttpRouteConstraint routeConstraint,
      HttpRouteValueDictionary defaults,
      HttpRouteValueDictionary constraints,
      HttpRouteValueDictionary dataTokens,
      HttpMessageHandler handler)
      : base(ODataRoute.GetRouteTemplate(routePrefix), defaults, constraints, dataTokens, handler)
    {
      this.RouteConstraint = routeConstraint;
      this.Initialize(routePrefix, routeConstraint as ODataPathRouteConstraint);
      if (routeConstraint != null)
        this.Constraints.Add(ODataRouteConstants.ConstraintName, (object) routeConstraint);
      this.Constraints.Add(ODataRouteConstants.VersionConstraintName, (object) new ODataVersionConstraint());
    }

    public IHttpRouteConstraint RouteConstraint { get; private set; }

    public override IHttpVirtualPathData GetVirtualPath(
      HttpRequestMessage request,
      IDictionary<string, object> values)
    {
      object obj;
      if (values == null || !values.Keys.Contains<string>(HttpRoute.HttpRouteKey, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase) || !values.TryGetValue(ODataRouteConstants.ODataPath, out obj) || !(obj is string odataPath))
        return (IHttpVirtualPathData) null;
      return !this.CanGenerateDirectLink ? base.GetVirtualPath(request, values) : (IHttpVirtualPathData) this.GenerateLinkDirectly(odataPath);
    }

    [Obsolete("The version constraint is relaxed by default")]
    public ODataRoute HasRelaxedODataVersionConstraint() => this.SetODataVersionConstraint(true);

    private ODataRoute SetODataVersionConstraint(bool isRelaxedMatch)
    {
      object obj;
      if (this.Constraints.TryGetValue(ODataRouteConstants.VersionConstraintName, out obj))
        ((ODataVersionConstraint) obj).IsRelaxedMatch = isRelaxedMatch;
      return this;
    }

    internal HttpVirtualPathData GenerateLinkDirectly(string odataPath) => new HttpVirtualPathData((IHttpRoute) this, ODataRoute.UriEncode(ODataRoute.CombinePathSegments(this.RoutePrefix, odataPath)));

    private void Initialize(string routePrefix, ODataPathRouteConstraint pathRouteConstraint)
    {
      this.RoutePrefix = routePrefix;
      this.PathRouteConstraint = pathRouteConstraint;
      this.CanGenerateDirectLink = routePrefix == null || routePrefix.IndexOf('{') == -1;
    }

    public string RoutePrefix { get; private set; }

    public ODataPathRouteConstraint PathRouteConstraint { get; private set; }

    internal bool CanGenerateDirectLink { get; private set; }

    private static string GetRouteTemplate(string prefix) => !string.IsNullOrEmpty(prefix) ? prefix + "/" + ODataRouteConstants.ODataPathTemplate : ODataRouteConstants.ODataPathTemplate;

    private static string CombinePathSegments(string routePrefix, string odataPath)
    {
      if (string.IsNullOrEmpty(routePrefix))
        return odataPath;
      return !string.IsNullOrEmpty(odataPath) ? routePrefix + "/" + odataPath : routePrefix;
    }

    private static string UriEncode(string str) => Uri.EscapeUriString(str).Replace("#", ODataRoute._escapedHashMark).Replace("?", ODataRoute._escapedQuestionMark);
  }
}
