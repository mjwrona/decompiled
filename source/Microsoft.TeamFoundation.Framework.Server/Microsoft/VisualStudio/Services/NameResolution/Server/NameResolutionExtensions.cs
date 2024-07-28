// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.NameResolution.Server.NameResolutionExtensions
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.TeamFoundation.Framework.Server;

namespace Microsoft.VisualStudio.Services.NameResolution.Server
{
  internal static class NameResolutionExtensions
  {
    public static TeamFoundationHostType GetHostType(this NameResolutionEntry entry)
    {
      switch (entry.Namespace)
      {
        case "Deployment":
          return TeamFoundationHostType.Deployment;
        case "Organization":
          return TeamFoundationHostType.Application;
        case "Collection":
        case "GlobalCollection":
          return TeamFoundationHostType.ProjectCollection;
        default:
          return TeamFoundationHostType.Unknown;
      }
    }
  }
}
