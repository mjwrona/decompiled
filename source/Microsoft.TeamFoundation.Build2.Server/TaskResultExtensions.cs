// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.TaskResultExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;

namespace Microsoft.TeamFoundation.Build2.Server
{
  internal static class TaskResultExtensions
  {
    internal static BuildResult ToBuildResult(this TaskResult taskResult)
    {
      switch (taskResult)
      {
        case TaskResult.Succeeded:
          return BuildResult.Succeeded;
        case TaskResult.SucceededWithIssues:
          return BuildResult.PartiallySucceeded;
        case TaskResult.Failed:
          return BuildResult.Failed;
        case TaskResult.Canceled:
        case TaskResult.Abandoned:
          return BuildResult.Canceled;
        default:
          return BuildResult.None;
      }
    }
  }
}
