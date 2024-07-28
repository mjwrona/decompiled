// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.NavigationLevelExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.Platform, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6A2C403-5081-466C-A570-9B50BFA8E213
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.Platform.dll

using System;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public static class NavigationLevelExtensions
  {
    public static void DecomposeContextLevels(
      this NavigationContextLevels levels,
      Action<NavigationContextLevels> action)
    {
      if (levels.HasFlag((Enum) NavigationContextLevels.Deployment))
        action(NavigationContextLevels.Deployment);
      if (levels.HasFlag((Enum) NavigationContextLevels.Application))
        action(NavigationContextLevels.Application);
      if (levels.HasFlag((Enum) NavigationContextLevels.Collection))
        action(NavigationContextLevels.Collection);
      if (levels.HasFlag((Enum) NavigationContextLevels.Project))
        action(NavigationContextLevels.Project);
      if (!levels.HasFlag((Enum) NavigationContextLevels.Team))
        return;
      action(NavigationContextLevels.Team);
    }
  }
}
