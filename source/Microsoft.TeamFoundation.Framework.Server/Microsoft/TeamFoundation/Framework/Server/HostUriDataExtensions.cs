// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostUriDataExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.Common;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HostUriDataExtensions
  {
    public static string AbsoluteVirtualPath(this IHostUriData uriData)
    {
      string path2 = uriData.RelativeVirtualPath;
      if (path2 != "/")
        path2 = PathUtility.Combine(UrlHostResolutionService.ApplicationVirtualPath, path2);
      return path2;
    }

    internal static string ComputeWebApplicationRelativeDirectory(this IHostUriData uriData)
    {
      if (uriData is HostUriData hostUriData && hostUriData.VirtualPathInAccessPoint)
        return (string) null;
      string applicationVirtualPath = UrlHostResolutionService.ApplicationVirtualPath;
      string str = uriData.AbsoluteVirtualPath().Substring(applicationVirtualPath.Length);
      if (string.IsNullOrEmpty(str))
        return (string) null;
      return str.TrimStart('~', '/');
    }
  }
}
