// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.ArtifactScopeHelper
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public static class ArtifactScopeHelper
  {
    public static ArtifactScopeType GetScopeTypeFromScopeId(string scopeId)
    {
      string lowerInvariant = scopeId?.ToLowerInvariant();
      if (lowerInvariant != null)
      {
        switch (lowerInvariant.Length)
        {
          case 3:
            switch (lowerInvariant[0])
            {
              case 'i':
                if (lowerInvariant == "ivy")
                  return ArtifactScopeType.Ivy;
                break;
              case 'n':
                if (lowerInvariant == "npm")
                  return ArtifactScopeType.Npm;
                break;
            }
            break;
          case 4:
            switch (lowerInvariant[0])
            {
              case 'd':
                if (lowerInvariant == "drop")
                  return ArtifactScopeType.Drop;
                break;
              case 'p':
                if (lowerInvariant == "pypi")
                  return ArtifactScopeType.PyPi;
                break;
            }
            break;
          case 5:
            switch (lowerInvariant[0])
            {
              case 'c':
                if (lowerInvariant == "cargo")
                  return ArtifactScopeType.Cargo;
                break;
              case 'm':
                if (lowerInvariant == "maven")
                  return ArtifactScopeType.Maven;
                break;
              case 'n':
                if (lowerInvariant == "nuget")
                  return ArtifactScopeType.NuGet;
                break;
              case 'u':
                if (lowerInvariant == "upack")
                  return ArtifactScopeType.UPack;
                break;
            }
            break;
          case 6:
            if (lowerInvariant == "symbol")
              return ArtifactScopeType.Symbol;
            break;
          case 9:
            switch (lowerInvariant[0])
            {
              case 'b':
                if (lowerInvariant == "buildlogs")
                  return ArtifactScopeType.BuildLogs;
                break;
              case 'p':
                if (lowerInvariant == "packaging")
                  return ArtifactScopeType.Packaging;
                break;
            }
            break;
          case 13:
            if (lowerInvariant == "pipelinecache")
              return ArtifactScopeType.PipelineCache;
            break;
          case 14:
            if (lowerInvariant == "buildartifacts")
              return ArtifactScopeType.BuildArtifacts;
            break;
          case 16:
            if (lowerInvariant == "pipelineartifact")
              return ArtifactScopeType.PipelineArtifact;
            break;
        }
      }
      return ArtifactScopeType.Others;
    }
  }
}
