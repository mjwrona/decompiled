// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts.DownloadStepExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.TeamFoundation.DistributedTask.Pipelines;
using Microsoft.TeamFoundation.DistributedTask.Pipelines.Artifacts;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts
{
  public static class DownloadStepExtensions
  {
    public static bool IsDownloadBuildStepExists(this IReadOnlyList<JobStep> steps)
    {
      foreach (JobStep step1 in (IEnumerable<JobStep>) steps)
      {
        if (step1 is TaskStep step2 && step2.IsDownloadBuildTask())
          return true;
      }
      return false;
    }

    public static bool IsDownloadBuildTask(this Step step) => step is TaskStep taskStep && taskStep.Reference != null && taskStep.Reference.Name.Equals("downloadBuild", StringComparison.OrdinalIgnoreCase);

    public static bool IsDownloadStepDisabled(this Step step)
    {
      string a;
      return step is TaskStep taskStep && taskStep.Inputs.TryGetValue("alias", out a) && string.Equals(a, "none", StringComparison.OrdinalIgnoreCase) && (step.IsDownloadBuildTask() || step.IsDownloadTask());
    }

    public static bool IsDownloadTask(this Step step) => step is TaskStep taskStep && taskStep.Reference != null && taskStep.Reference.Id.Equals(PipelineArtifactConstants.DownloadTask.Id) && taskStep.Reference.Version == (string) PipelineArtifactConstants.DownloadTask.Version;

    public static bool IsDownloadCurrentPipelineArtifactStep(this Step step)
    {
      string a;
      return step is TaskStep step1 && step1.IsDownloadTask() && step1.Inputs.TryGetValue("alias", out a) && string.Equals(a, "current", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsDownloadPipelineArtifactStepDisabled(this TaskStep step)
    {
      string a;
      return step.IsDownloadTask() && step.Inputs.TryGetValue("alias", out a) && string.Equals(a, "none", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsDownloadExternalPipelineArtifactStep(this TaskStep step)
    {
      string str;
      return step.IsDownloadTask() && step.Inputs != null && step.Inputs.TryGetValue("alias", out str) && !string.IsNullOrEmpty(str) && !str.Equals("current", StringComparison.OrdinalIgnoreCase) && !str.Equals("none", StringComparison.OrdinalIgnoreCase);
    }

    public static bool IsGetPackageTask(this Step step) => step is TaskStep taskStep && taskStep.Reference != null && taskStep.Reference.Name.Equals("getPackage", StringComparison.OrdinalIgnoreCase);

    public static string GetAliasFromTaskStep(this TaskStep step)
    {
      string str;
      return !step.Inputs.TryGetValue("alias", out str) ? string.Empty : str;
    }

    public static bool IsDownloadPipelineArtifactStepExists(this IReadOnlyList<JobStep> steps)
    {
      foreach (JobStep step1 in (IEnumerable<JobStep>) steps)
      {
        if (step1 is TaskStep step2 && step2.IsDownloadTask())
          return true;
      }
      return false;
    }

    public static void Merge(
      this IDictionary<string, string> first,
      IDictionary<string, string> second)
    {
      foreach (string key in (IEnumerable<string>) (second?.Keys ?? (ICollection<string>) new List<string>()))
        first[key] = second[key];
    }

    public static void Merge(
      this IDictionary<string, string> first,
      IReadOnlyDictionary<string, string> second)
    {
      foreach (string key in second?.Keys ?? (IEnumerable<string>) new List<string>())
        first[key] = second[key];
    }
  }
}
