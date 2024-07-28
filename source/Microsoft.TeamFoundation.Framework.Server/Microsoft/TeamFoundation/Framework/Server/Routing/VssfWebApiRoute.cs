// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Routing.VssfWebApiRoute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web.Http.Routing;

namespace Microsoft.TeamFoundation.Framework.Server.Routing
{
  public class VssfWebApiRoute : HttpRoute
  {
    private const string c_matchedValuesDictionaryKey = "VssfWebApiRoute.MatchedValues";
    internal const string c_requestHostRelativePathPartsKey = "HostRelativeRequestPathParts";
    private Microsoft.TeamFoundation.Framework.Server.Routing.MVC.ParsedRoute m_parsedRoute;
    private Dictionary<string, Regex> m_stringConstraintRegexs;

    public VssfWebApiRoute(
      string routeTemplate,
      HttpRouteValueDictionary defaults,
      HttpRouteValueDictionary constraints,
      HttpRouteValueDictionary dataTokens,
      HttpMessageHandler messageHandler)
      : base(routeTemplate, defaults, constraints, dataTokens, messageHandler)
    {
      this.m_parsedRoute = Microsoft.TeamFoundation.Framework.Server.Routing.MVC.RouteParser.Parse(routeTemplate);
      if (constraints == null || constraints.Count <= 0)
        return;
      this.m_stringConstraintRegexs = new Dictionary<string, Regex>();
      foreach (KeyValuePair<string, object> constraint in (Dictionary<string, object>) constraints)
      {
        if (constraint.Value is string key)
          this.m_stringConstraintRegexs[key] = new Regex("^(" + key + ")$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
      }
    }

    public override IHttpRouteData GetRouteData(string virtualPathRoot, HttpRequestMessage request)
    {
      object obj1;
      IDictionary<string, IList<string>> dictionary;
      if (request.Properties.TryGetValue("HostRelativeRequestPathParts", out obj1))
      {
        dictionary = (IDictionary<string, IList<string>>) obj1;
      }
      else
      {
        dictionary = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>((IEqualityComparer<string>) StringComparer.Ordinal);
        request.Properties["HostRelativeRequestPathParts"] = (object) dictionary;
      }
      IList<string> pathSegmentStrings;
      if (!dictionary.TryGetValue(request.RequestUri.OriginalString, out pathSegmentStrings))
      {
        string url = (string) null;
        IVssRequestContext ivssRequestContext = request.GetIVssRequestContext();
        if (ivssRequestContext != null)
        {
          RoutePerformanceTimingUtil.StartRouteMatchingTimer(ivssRequestContext);
          string str = ivssRequestContext.RelativePath();
          if (VssfWebApiRoute.DecodeUriString(request.RequestUri.AbsolutePath, ivssRequestContext).IndexOf(str, StringComparison.Ordinal) < 0)
            url = (string) null;
          else
            url = str.TrimStart('/');
        }
        if (url == null)
        {
          string absolutePath = request.RequestUri.AbsolutePath;
          if (!absolutePath.StartsWith(virtualPathRoot, StringComparison.OrdinalIgnoreCase))
            return (IHttpRouteData) null;
          int length = virtualPathRoot.Length;
          url = VssfWebApiRoute.DecodeUriString(absolutePath.Length <= length || absolutePath[length] != '/' ? absolutePath.Substring(length) : absolutePath.Substring(length + 1), ivssRequestContext);
        }
        pathSegmentStrings = Microsoft.TeamFoundation.Framework.Server.Routing.MVC.RouteParser.SplitUrlToPathSegmentStrings(url);
        dictionary[request.RequestUri.OriginalString] = pathSegmentStrings;
      }
      object obj2;
      HttpRouteValueDictionary routeValueDictionary;
      if (request.Properties.TryGetValue("VssfWebApiRoute.MatchedValues", out obj2))
      {
        routeValueDictionary = (HttpRouteValueDictionary) obj2;
        routeValueDictionary.Clear();
      }
      else
      {
        routeValueDictionary = new HttpRouteValueDictionary();
        request.Properties["VssfWebApiRoute.MatchedValues"] = (object) routeValueDictionary;
      }
      if (!this.m_parsedRoute.Match((IDictionary<string, object>) routeValueDictionary, pathSegmentStrings, this.Defaults))
        return (IHttpRouteData) null;
      if (this.Constraints != null)
      {
        foreach (KeyValuePair<string, object> constraint in (IEnumerable<KeyValuePair<string, object>>) this.Constraints)
        {
          Regex regex;
          if (constraint.Value is string key && this.m_stringConstraintRegexs != null && this.m_stringConstraintRegexs.TryGetValue(key, out regex))
          {
            object obj3;
            routeValueDictionary.TryGetValue(constraint.Key, out obj3);
            string input = Convert.ToString(obj3, (IFormatProvider) CultureInfo.InvariantCulture);
            if (!regex.IsMatch(input))
              return (IHttpRouteData) null;
          }
          else if (!this.ProcessConstraint(request, constraint.Value, constraint.Key, routeValueDictionary, HttpRouteDirection.UriResolution))
            return (IHttpRouteData) null;
        }
      }
      HttpRouteData routeData = new HttpRouteData((IHttpRoute) this, routeValueDictionary);
      RoutePerformanceTimingUtil.StopRouteMatchingTimer(request);
      return (IHttpRouteData) routeData;
    }

    internal static string DecodeUriString(
      string originalRequestPath,
      IVssRequestContext requestContext)
    {
      return Uri.UnescapeDataString(originalRequestPath);
    }
  }
}
