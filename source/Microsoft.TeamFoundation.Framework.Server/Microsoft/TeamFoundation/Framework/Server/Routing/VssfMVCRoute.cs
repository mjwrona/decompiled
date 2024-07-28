// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Routing.VssfMVCRoute
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Routing;

namespace Microsoft.TeamFoundation.Framework.Server.Routing
{
  public class VssfMVCRoute : Route
  {
    private const string c_matchedValuesDictionaryKey = "VssfMVCRoute.MatchedValues";
    private const string c_requestHostRelativePathPartsKey = "HostRelativeRequestPathParts";
    private static readonly char[] c_relativeTrimChars = new char[2]
    {
      '~',
      '/'
    };
    private Microsoft.TeamFoundation.Framework.Server.Routing.MVC.ParsedRoute m_parsedRoute;
    private Dictionary<string, Regex> m_stringConstraintRegexs;
    private bool m_useServiceHostForVirtualPath;

    public VssfMVCRoute(
      string routeTemplate,
      RouteValueDictionary defaults,
      RouteValueDictionary constraints,
      RouteValueDictionary dataTokens,
      IRouteHandler routeHandler,
      string matchRouteTemplate = null,
      bool useServiceHostForVirtualPath = true)
      : base(routeTemplate, defaults, constraints, dataTokens, routeHandler)
    {
      if (matchRouteTemplate == null)
        matchRouteTemplate = routeTemplate;
      if (constraints != null && constraints.Count > 0)
      {
        this.m_stringConstraintRegexs = new Dictionary<string, Regex>();
        foreach (KeyValuePair<string, object> constraint in constraints)
        {
          if (constraint.Value is string key)
            this.m_stringConstraintRegexs[key] = new Regex("^(" + key + ")$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);
        }
      }
      this.m_parsedRoute = Microsoft.TeamFoundation.Framework.Server.Routing.MVC.RouteParser.Parse(matchRouteTemplate);
      this.m_useServiceHostForVirtualPath = useServiceHostForVirtualPath;
    }

    public override RouteData GetRouteData(HttpContextBase httpContext)
    {
      if (!(httpContext.Items[(object) "HostRelativeRequestPathParts"] is IDictionary<string, IList<string>> dictionary))
      {
        dictionary = (IDictionary<string, IList<string>>) new Dictionary<string, IList<string>>((IEqualityComparer<string>) StringComparer.Ordinal);
        httpContext.Items[(object) "HostRelativeRequestPathParts"] = (object) dictionary;
      }
      IList<string> pathSegmentStrings;
      if (!dictionary.TryGetValue(httpContext.Request.AppRelativeCurrentExecutionFilePath, out pathSegmentStrings))
      {
        string url = (string) null;
        if (httpContext.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext requestContext)
        {
          RoutePerformanceTimingUtil.StartRouteMatchingTimer(requestContext);
          string str = requestContext.RelativePath();
          if ((httpContext.Request.AppRelativeCurrentExecutionFilePath + httpContext.Request.PathInfo).IndexOf(str, StringComparison.OrdinalIgnoreCase) < 0)
            url = (string) null;
          else
            url = str.TrimStart('/');
        }
        if (url == null)
          url = httpContext.Request.AppRelativeCurrentExecutionFilePath.Substring(2) + httpContext.Request.PathInfo;
        pathSegmentStrings = Microsoft.TeamFoundation.Framework.Server.Routing.MVC.RouteParser.SplitUrlToPathSegmentStrings(url);
        dictionary[httpContext.Request.AppRelativeCurrentExecutionFilePath] = pathSegmentStrings;
      }
      if (!(httpContext.Items[(object) "VssfMVCRoute.MatchedValues"] is RouteValueDictionary routeValueDictionary))
      {
        routeValueDictionary = new RouteValueDictionary();
        httpContext.Items[(object) "VssfMVCRoute.MatchedValues"] = (object) routeValueDictionary;
      }
      else
        routeValueDictionary.Clear();
      if (!this.m_parsedRoute.Match((IDictionary<string, object>) routeValueDictionary, pathSegmentStrings, (IDictionary<string, object>) this.Defaults))
        return (RouteData) null;
      RouteData routeData = new RouteData((RouteBase) this, this.RouteHandler);
      if (this.Constraints != null)
      {
        foreach (KeyValuePair<string, object> constraint in this.Constraints)
        {
          Regex regex;
          if (constraint.Value is string key && this.m_stringConstraintRegexs != null && this.m_stringConstraintRegexs.TryGetValue(key, out regex))
          {
            object obj;
            routeValueDictionary.TryGetValue(constraint.Key, out obj);
            string input = Convert.ToString(obj, (IFormatProvider) CultureInfo.InvariantCulture);
            if (!regex.IsMatch(input))
              return (RouteData) null;
          }
          else if (!this.ProcessConstraint(httpContext, constraint.Value, constraint.Key, routeValueDictionary, RouteDirection.IncomingRequest))
            return (RouteData) null;
        }
      }
      foreach (KeyValuePair<string, object> keyValuePair in routeValueDictionary)
        routeData.Values.Add(keyValuePair.Key, keyValuePair.Value);
      if (this.DataTokens != null)
      {
        foreach (KeyValuePair<string, object> dataToken in this.DataTokens)
          routeData.DataTokens[dataToken.Key] = dataToken.Value;
      }
      RoutePerformanceTimingUtil.StopRouteMatchingTimer(httpContext);
      return routeData;
    }

    public override VirtualPathData GetVirtualPath(
      RequestContext requestContext,
      RouteValueDictionary values)
    {
      VirtualPathData virtualPath = base.GetVirtualPath(requestContext, values);
      if (!this.m_useServiceHostForVirtualPath)
        return virtualPath;
      if (virtualPath == null)
        return (VirtualPathData) null;
      if (!(requestContext.HttpContext.Items[(object) HttpContextConstants.IVssRequestContext] is IVssRequestContext requestContext1))
        return (VirtualPathData) null;
      string path1 = VirtualPathUtility.ToAppRelative(requestContext1.VirtualPath()).TrimStart(VssfMVCRoute.c_relativeTrimChars);
      if (path1.StartsWith("/"))
        path1 = path1.Substring(1);
      virtualPath.VirtualPath = PathUtility.Combine(path1, virtualPath.VirtualPath);
      return virtualPath;
    }
  }
}
