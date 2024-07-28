// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.BuildArtifactExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class BuildArtifactExtensions
  {
    public static void TryInferType(this BuildArtifactResource resource)
    {
      if (resource == null || !string.IsNullOrEmpty(resource.Type) || string.IsNullOrEmpty(resource.Data))
        return;
      if (resource.Data.StartsWith("$/", StringComparison.OrdinalIgnoreCase))
        resource.Type = "VersionControl";
      else if (resource.Data.StartsWith("#", StringComparison.OrdinalIgnoreCase))
      {
        resource.Type = "Container";
      }
      else
      {
        Uri result = (Uri) null;
        if (!Uri.TryCreate(resource.Data, UriKind.RelativeOrAbsolute, out result))
          return;
        if (result.IsFile || result.IsUnc)
          resource.Type = "FilePath";
        if (result.IsAbsoluteUri)
          resource.DownloadUrl = result.AbsoluteUri;
        else
          resource.DownloadUrl = resource.Data;
      }
    }

    public static List<BuildArtifact> GetUniqueArtifacts(this IList<BuildArtifact> artifacts) => artifacts.OrderBy<BuildArtifact, DateTime>((Func<BuildArtifact, DateTime>) (x => x.SourceCreatedDate)).Distinct<BuildArtifact>((IEqualityComparer<BuildArtifact>) BuildArtifactEqualityComparer.Default).ToList<BuildArtifact>();
  }
}
