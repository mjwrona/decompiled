// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DetectedBuildFrameworkExtensions
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public static class DetectedBuildFrameworkExtensions
  {
    public static Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildFramework ToWebApi(
      this DetectedBuildFramework detectedBuildFramework)
    {
      ArgumentUtility.CheckForNull<DetectedBuildFramework>(detectedBuildFramework, nameof (detectedBuildFramework));
      return new Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildFramework()
      {
        Id = detectedBuildFramework.Id,
        Version = detectedBuildFramework.Version,
        Settings = detectedBuildFramework.Settings,
        BuildTargets = (IReadOnlyList<Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildTarget>) detectedBuildFramework.BuildTargets.Select<DetectedBuildTarget, Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildTarget>((Func<DetectedBuildTarget, Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildTarget>) (bt => bt.ToWebApi())).ToList<Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildTarget>()
      };
    }

    public static Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildTarget ToWebApi(
      this DetectedBuildTarget detectedBuildTarget)
    {
      ArgumentUtility.CheckForNull<DetectedBuildTarget>(detectedBuildTarget, nameof (detectedBuildTarget));
      return new Microsoft.TeamFoundation.Pipelines.WebApi.DetectedBuildTarget()
      {
        Type = detectedBuildTarget.Type,
        Path = detectedBuildTarget.Path,
        Settings = detectedBuildTarget.Settings
      };
    }
  }
}
