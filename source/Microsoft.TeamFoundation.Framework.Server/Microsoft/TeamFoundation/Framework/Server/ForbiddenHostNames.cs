// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.ForbiddenHostNames
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Framework.Server
{
  public static class ForbiddenHostNames
  {
    internal static readonly string[] ForbiddenChildPaths = new string[2]
    {
      "~/TeamFoundationServer/",
      "~/web/"
    };
    private static HashSet<string> s_forbiddenNames = new HashSet<string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    static ForbiddenHostNames()
    {
      ForbiddenHostNames.s_forbiddenNames.Add("services");
      ForbiddenHostNames.s_forbiddenNames.Add("signalr");
      ForbiddenHostNames.s_forbiddenNames.Add("teamfoundation");
      foreach (string forbiddenChildPath in ForbiddenHostNames.ForbiddenChildPaths)
      {
        char[] chArray = new char[1]{ '~' };
        ForbiddenHostNames.Add(forbiddenChildPath.TrimStart(chArray).TrimStart('/').TrimEnd('/'));
      }
      foreach (string virtualDirectory in TeamFoundationHostManagementService.s_v1ServiceVirtualDirectories)
      {
        char[] chArray = new char[1]{ '~' };
        ForbiddenHostNames.Add(virtualDirectory.TrimStart(chArray).TrimStart('/').TrimEnd('/'));
      }
    }

    private static void Add(string name) => ForbiddenHostNames.s_forbiddenNames.Add(name);

    public static bool IsNameAllowed(string name, TeamFoundationHostType hostType) => !ForbiddenHostNames.s_forbiddenNames.Contains(name);
  }
}
