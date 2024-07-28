// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities.BuildExtensions
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.TeamFoundation.Build.WebApi;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Utilities
{
  public static class BuildExtensions
  {
    public static bool IsValidBuild(this Microsoft.TeamFoundation.Build.WebApi.Build build)
    {
      if (build != null)
      {
        BuildStatus? status = build.Status;
        BuildStatus buildStatus = BuildStatus.Completed;
        if (status.GetValueOrDefault() == buildStatus & status.HasValue)
        {
          BuildResult? result1 = build.Result;
          BuildResult buildResult1 = BuildResult.PartiallySucceeded;
          if (!(result1.GetValueOrDefault() == buildResult1 & result1.HasValue))
          {
            BuildResult? result2 = build.Result;
            BuildResult buildResult2 = BuildResult.Succeeded;
            if (!(result2.GetValueOrDefault() == buildResult2 & result2.HasValue))
              goto label_5;
          }
          return !build.Deleted;
        }
      }
label_5:
      return false;
    }
  }
}
