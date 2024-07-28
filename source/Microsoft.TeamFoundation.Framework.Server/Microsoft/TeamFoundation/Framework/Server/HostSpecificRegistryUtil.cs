// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HostSpecificRegistryUtil
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class HostSpecificRegistryUtil
  {
    public static T GetPerHostRegistry<T>(
      this IVssRequestContext requestContext,
      Guid hostId,
      string pathBase,
      string pathKey,
      bool fallThru,
      T defaultValue)
    {
      pathBase = HostSpecificRegistryUtil.AddTail(pathBase);
      requestContext = requestContext.To(TeamFoundationHostType.Deployment);
      IVssRegistryService service = requestContext.GetService<IVssRegistryService>();
      string hostRegistryPath = HostSpecificRegistryUtil.GetPerHostRegistryPath(hostId, pathBase, pathKey);
      string str = service.GetValue(requestContext, (RegistryQuery) hostRegistryPath, false, (string) null);
      if (str == null & fallThru)
      {
        string query = pathBase + HostSpecificRegistryUtil.RemoveHead(pathKey);
        str = service.GetValue(requestContext, (RegistryQuery) query, false, (string) null);
      }
      return str != null ? RegistryUtility.FromString<T>(str) : defaultValue;
    }

    public static string GetPerHostRegistryPath(Guid hostId, string pathBase, string pathKey) => HostSpecificRegistryUtil.AddTail(pathBase) + hostId.ToString("n") + HostSpecificRegistryUtil.AddHead(pathKey);

    private static string AddTail(string path) => !path.EndsWith("/") ? path + "/" : path;

    private static string AddHead(string path) => !path.StartsWith("/") ? "/" + path : path;

    private static string RemoveHead(string path) => !path.StartsWith("/") ? path : path.Substring(1);
  }
}
