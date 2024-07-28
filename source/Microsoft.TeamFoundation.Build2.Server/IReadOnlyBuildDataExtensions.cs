// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IReadOnlyBuildDataExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.Azure.Pipelines.Server.ObjectModel;
using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class IReadOnlyBuildDataExtensions
  {
    public static ISecuredObject ToSecuredObject(this IReadOnlyBuildData build) => (ISecuredObject) new SecuredObject(Security.BuildNamespaceId, BuildPermissions.ViewBuilds, build.GetToken());

    internal static string GetToken(this IReadOnlyBuildData build)
    {
      ArgumentUtility.CheckForNull<IReadOnlyBuildData>(build, nameof (build));
      ArgumentUtility.CheckForNull<MinimalBuildDefinition>(build.Definition, "build.Definition");
      ArgumentUtility.CheckForNull<Uri>(build.Uri, "build.Uri");
      return build.Definition.GetToken() + "/*" + build.Uri.AbsolutePath;
    }

    public static string GetProjectName(
      this IReadOnlyBuildData build,
      IVssRequestContext requestContext)
    {
      string projectName;
      return build != null && requestContext.GetService<IProjectService>().TryGetProjectName(requestContext, build.ProjectId, out projectName) ? projectName : (string) null;
    }

    public static string GetWebUrl(this IReadOnlyBuildData build, IVssRequestContext requestContext)
    {
      ArgumentUtility.CheckForNull<IReadOnlyBuildData>(build, nameof (build));
      return requestContext.GetService<IBuildRouteService>().GetBuildWebUrl(requestContext, build.ProjectId, build.Id);
    }

    public static RunState GetPipelineRunState(this IReadOnlyBuildData build)
    {
      BuildStatus? status = build.Status;
      if (status.HasValue)
      {
        switch (status.GetValueOrDefault())
        {
          case BuildStatus.InProgress:
          case BuildStatus.Postponed:
          case BuildStatus.NotStarted:
            return RunState.InProgress;
          case BuildStatus.Completed:
            return RunState.Completed;
          case BuildStatus.Cancelling:
            return RunState.Canceling;
        }
      }
      return RunState.InProgress;
    }

    public static RunResult? GetPipelineRunResult(this IReadOnlyBuildData build)
    {
      BuildStatus? status = build.Status;
      BuildStatus buildStatus = BuildStatus.Completed;
      if (!(status.GetValueOrDefault() == buildStatus & status.HasValue))
        return new RunResult?();
      BuildResult? result = build.Result;
      if (result.HasValue)
      {
        switch (result.GetValueOrDefault())
        {
          case BuildResult.Succeeded:
          case BuildResult.PartiallySucceeded:
            return new RunResult?(RunResult.Succeeded);
          case BuildResult.Failed:
            return new RunResult?(RunResult.Failed);
          case BuildResult.Canceled:
            return new RunResult?(RunResult.Canceled);
        }
      }
      return new RunResult?(RunResult.Succeeded);
    }
  }
}
