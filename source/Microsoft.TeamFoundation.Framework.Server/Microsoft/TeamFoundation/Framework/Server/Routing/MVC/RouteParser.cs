// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.Routing.MVC.RouteParser
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.Framework.Server.Routing.MVC
{
  internal static class RouteParser
  {
    private static string GetLiteral(string segmentLiteral)
    {
      string str = segmentLiteral.Replace("{{", "").Replace("}}", "");
      return str.Contains("{") || str.Contains("}") ? (string) null : segmentLiteral.Replace("{{", "{").Replace("}}", "}");
    }

    private static int IndexOfFirstOpenParameter(string segment, int startIndex)
    {
      while (true)
      {
        startIndex = segment.IndexOf('{', startIndex);
        if (startIndex != -1)
        {
          if (startIndex + 1 != segment.Length && (startIndex + 1 >= segment.Length || segment[startIndex + 1] == '{'))
            startIndex += 2;
          else
            goto label_3;
        }
        else
          break;
      }
      return -1;
label_3:
      return startIndex;
    }

    internal static bool IsSeparator(string s) => string.Equals(s, "/", StringComparison.Ordinal);

    private static bool IsValidParameterName(string parameterName)
    {
      if (parameterName.Length == 0)
        return false;
      for (int index = 0; index < parameterName.Length; ++index)
      {
        switch (parameterName[index])
        {
          case '/':
          case '{':
          case '}':
            return false;
          default:
            continue;
        }
      }
      return true;
    }

    internal static bool IsInvalidRouteUrl(string routeUrl) => routeUrl.StartsWith("~", StringComparison.Ordinal) || routeUrl.StartsWith("/", StringComparison.Ordinal) || routeUrl.IndexOf('?') != -1;

    public static ParsedRoute Parse(string routeUrl)
    {
      if (routeUrl == null)
        routeUrl = string.Empty;
      IList<string> stringList = !RouteParser.IsInvalidRouteUrl(routeUrl) ? RouteParser.SplitUrlToPathSegmentStrings(routeUrl) : throw new ArgumentException("Invalid route url", nameof (routeUrl));
      Exception exception = RouteParser.ValidateUrlParts(stringList);
      if (exception != null)
        throw exception;
      return new ParsedRoute(RouteParser.SplitUrlToPathSegments(stringList));
    }

    private static IList<PathSubsegment> ParseUrlSegment(string segment, out Exception exception)
    {
      int startIndex = 0;
      List<PathSubsegment> urlSegment = new List<PathSubsegment>();
      int num1;
      for (; startIndex < segment.Length; startIndex = num1 + 1)
      {
        int num2 = RouteParser.IndexOfFirstOpenParameter(segment, startIndex);
        if (num2 == -1)
        {
          string literal = RouteParser.GetLiteral(segment.Substring(startIndex));
          if (literal == null)
          {
            exception = (Exception) new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Route mismatched parameter", (object) segment), "routeUrl");
            return (IList<PathSubsegment>) null;
          }
          if (literal.Length > 0)
          {
            urlSegment.Add((PathSubsegment) new LiteralSubsegment(literal));
            break;
          }
          break;
        }
        num1 = segment.IndexOf('}', num2 + 1);
        if (num1 == -1)
        {
          exception = (Exception) new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Route mismatched parameter", (object) segment), "routeUrl");
          return (IList<PathSubsegment>) null;
        }
        string literal1 = RouteParser.GetLiteral(segment.Substring(startIndex, num2 - startIndex));
        if (literal1 == null)
        {
          exception = (Exception) new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Route mismatched parameter", (object) segment), "routeUrl");
          return (IList<PathSubsegment>) null;
        }
        if (literal1.Length > 0)
          urlSegment.Add((PathSubsegment) new LiteralSubsegment(literal1));
        string parameterName = segment.Substring(num2 + 1, num1 - num2 - 1);
        urlSegment.Add((PathSubsegment) new ParameterSubsegment(parameterName));
      }
      exception = (Exception) null;
      return (IList<PathSubsegment>) urlSegment;
    }

    private static IList<PathSegment> SplitUrlToPathSegments(IList<string> urlParts)
    {
      List<PathSegment> pathSegments = new List<PathSegment>();
      foreach (string urlPart in (IEnumerable<string>) urlParts)
      {
        if (RouteParser.IsSeparator(urlPart))
        {
          pathSegments.Add((PathSegment) new SeparatorPathSegment());
        }
        else
        {
          IList<PathSubsegment> urlSegment = RouteParser.ParseUrlSegment(urlPart, out Exception _);
          pathSegments.Add((PathSegment) new ContentPathSegment(urlSegment));
        }
      }
      return (IList<PathSegment>) pathSegments;
    }

    internal static IList<string> SplitUrlToPathSegmentStrings(string url)
    {
      List<string> pathSegmentStrings = new List<string>();
      if (string.IsNullOrEmpty(url))
        return (IList<string>) pathSegmentStrings;
      int num;
      for (int startIndex = 0; startIndex < url.Length; startIndex = num + 1)
      {
        num = url.IndexOf('/', startIndex);
        if (num == -1)
        {
          string str = url.Substring(startIndex);
          if (str.Length > 0)
          {
            pathSegmentStrings.Add(str);
            break;
          }
          break;
        }
        string str1 = url.Substring(startIndex, num - startIndex);
        if (str1.Length > 0)
          pathSegmentStrings.Add(str1);
        pathSegmentStrings.Add("/");
      }
      return (IList<string>) pathSegmentStrings;
    }

    private static Exception ValidateUrlParts(IList<string> pathSegments)
    {
      HashSet<string> usedParameterNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      bool? nullable = new bool?();
      bool flag1 = false;
      foreach (string pathSegment in (IEnumerable<string>) pathSegments)
      {
        if (flag1)
          return (Exception) new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Catch all route must be last"), "routeUrl");
        bool flag2;
        if (!nullable.HasValue)
        {
          nullable = new bool?(RouteParser.IsSeparator(pathSegment));
          flag2 = nullable.Value;
        }
        else
        {
          flag2 = RouteParser.IsSeparator(pathSegment);
          if (flag2 && nullable.Value)
            return (Exception) new ArgumentException("Route cannot have consecutive separators", "routeUrl");
          nullable = new bool?(flag2);
        }
        if (!flag2)
        {
          Exception exception;
          IList<PathSubsegment> urlSegment = RouteParser.ParseUrlSegment(pathSegment, out exception);
          if (exception != null)
            return exception;
          exception = RouteParser.ValidateUrlSegment(urlSegment, usedParameterNames, pathSegment);
          if (exception != null)
            return exception;
          flag1 = urlSegment.Any<PathSubsegment>((Func<PathSubsegment, bool>) (seg => seg is ParameterSubsegment && ((ParameterSubsegment) seg).IsCatchAll));
        }
      }
      return (Exception) null;
    }

    private static Exception ValidateUrlSegment(
      IList<PathSubsegment> pathSubsegments,
      HashSet<string> usedParameterNames,
      string pathSegment)
    {
      bool flag = false;
      Type type = (Type) null;
      foreach (PathSubsegment pathSubsegment in (IEnumerable<PathSubsegment>) pathSubsegments)
      {
        if (type != (Type) null && type == pathSubsegment.GetType())
          return (Exception) new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Route cannot have consecutive parameters"), "routeUrl");
        type = pathSubsegment.GetType();
        if (!(pathSubsegment is LiteralSubsegment) && pathSubsegment is ParameterSubsegment parameterSubsegment)
        {
          string parameterName = parameterSubsegment.ParameterName;
          if (parameterSubsegment.IsCatchAll)
            flag = true;
          if (!RouteParser.IsValidParameterName(parameterName))
            return (Exception) new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Invalid route parameter name", (object) parameterName), "routeUrl");
          if (usedParameterNames.Contains(parameterName))
            return (Exception) new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentUICulture, "Repeated route parameter", (object) parameterName), "routeUrl");
          usedParameterNames.Add(parameterName);
        }
      }
      return flag && pathSubsegments.Count != 1 ? (Exception) new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, "Route cannot have catch-all in multi-segment"), "routeUrl") : (Exception) null;
    }
  }
}
