// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.UrlHostResolutionHelper
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Web;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class UrlHostResolutionHelper
  {
    public static string GetFirstPathSegment(
      IVssRequestContext requestContext,
      Uri requestUri,
      string applicationVirtualPath,
      out string absoluteVirtualPath,
      bool allowDots = false)
    {
      string str = HttpUtility.UrlDecode(requestUri.AbsolutePath);
      int num1 = 1;
      if (applicationVirtualPath.Length > 1 && str.StartsWith(applicationVirtualPath, StringComparison.OrdinalIgnoreCase))
        num1 = applicationVirtualPath.Length + (!applicationVirtualPath.EndsWith("/") ? 1 : 0);
      if (num1 >= str.Length)
      {
        absoluteVirtualPath = (string) null;
        return (string) null;
      }
      if (str[num1] == '_')
      {
        absoluteVirtualPath = (string) null;
        return (string) null;
      }
      int num2 = str.IndexOf('/', num1);
      if (num2 == -1)
        num2 = str.Length;
      string name = str.Substring(num1, num2 - num1);
      if (string.IsNullOrEmpty(name) || !ForbiddenHostNames.IsNameAllowed(name, TeamFoundationHostType.Application | TeamFoundationHostType.ProjectCollection))
      {
        absoluteVirtualPath = (string) null;
        return (string) null;
      }
      if (!allowDots && requestContext.ExecutionEnvironment.IsHostedDeployment && name.IndexOf('.') >= 0)
      {
        absoluteVirtualPath = (string) null;
        return (string) null;
      }
      absoluteVirtualPath = VirtualPathUtility.AppendTrailingSlash(str.Substring(0, Math.Min(num2 + 1, str.Length)));
      return name;
    }

    internal static bool ParseHostIdFromPathSegment(string pathSegment, out Guid hostId)
    {
      hostId = Guid.Empty;
      return pathSegment.Length == 37 && (pathSegment[0].Equals('A') || pathSegment[0].Equals('a')) && Guid.TryParse(pathSegment.Substring(1, 36), out hostId);
    }
  }
}
