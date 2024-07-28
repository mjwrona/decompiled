// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DockerBuildFrameworkDetector
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class DockerBuildFrameworkDetector : IBuildFrameworkDetector
  {
    private const string c_dockerfileFileName = "Dockerfile";
    private const string c_dockerComposeFileName = "docker-compose.yml";
    public static readonly string Id = "docker";

    public bool TryDetect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType,
      out DetectedBuildFramework detectedBuildFramework)
    {
      using (new Tracer<DockerBuildFrameworkDetector>(requestContext, TracePoints.BuildFrameworkDetection.DockerTryDetectEnter, TracePoints.BuildFrameworkDetection.DockerTryDetectLeave, nameof (TryDetect)))
      {
        ArgumentUtility.CheckForNull<TreeAnalysis>(treeAnalysis, nameof (treeAnalysis));
        DetectedBuildTarget[] array = this.GetDetectedBuildTargets(requestContext, treeAnalysis, fileContentsProvider, detectionType).ToArray<DetectedBuildTarget>();
        if (!((IEnumerable<DetectedBuildTarget>) array).Any<DetectedBuildTarget>())
        {
          detectedBuildFramework = (DetectedBuildFramework) null;
          return false;
        }
        detectedBuildFramework = new DetectedBuildFramework(DockerBuildFrameworkDetector.Id, string.Empty, (IReadOnlyList<DetectedBuildTarget>) array);
        return true;
      }
    }

    private IEnumerable<DetectedBuildTarget> GetDetectedBuildTargets(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType)
    {
      DetectedBuildTarget detectedBuildTarget;
      if (this.TryGetComposeFileBuildTarget(treeAnalysis, out detectedBuildTarget))
        yield return detectedBuildTarget;
      foreach (DetectedBuildTarget dockerfileBuildTarget in DockerBuildFrameworkDetector.GetDockerfileBuildTargets(requestContext, treeAnalysis, fileContentsProvider, detectionType))
        yield return dockerfileBuildTarget;
    }

    private bool TryGetComposeFileBuildTarget(
      TreeAnalysis treeAnalysis,
      out DetectedBuildTarget detectedBuildTarget)
    {
      FilePath filePath;
      if (!treeAnalysis.TryGetFilePath("docker-compose.yml", out filePath))
      {
        detectedBuildTarget = (DetectedBuildTarget) null;
        return false;
      }
      detectedBuildTarget = new DetectedBuildTarget(DockerBuildFrameworkDetector.Settings.WellKnownTypes.Compose, filePath.ToString());
      return true;
    }

    private static IEnumerable<DetectedBuildTarget> GetDockerfileBuildTargets(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType)
    {
      int maxFilesToRead = DockerBuildFrameworkDetector.GetMaxFilesToRead(requestContext, detectionType);
      return treeAnalysis.NodeDictionary.Where<KeyValuePair<string, IList<TreeNode>>>((Func<KeyValuePair<string, IList<TreeNode>>, bool>) (kvp => DockerBuildFrameworkDetector.IsDockerfileName(kvp.Key))).SelectMany<KeyValuePair<string, IList<TreeNode>>, TreeNode>((Func<KeyValuePair<string, IList<TreeNode>>, IEnumerable<TreeNode>>) (kvp => (IEnumerable<TreeNode>) kvp.Value)).Where<TreeNode>((Func<TreeNode, bool>) (n => !n.IsDirectory)).Select<TreeNode, FilePath>((Func<TreeNode, FilePath>) (node => new FilePath(node.Path))).Select<FilePath, DetectedBuildTarget>(new Func<FilePath, int, DetectedBuildTarget>(ToBuildTarget));

      DetectedBuildTarget ToBuildTarget(FilePath filePath, int index)
      {
        Dictionary<string, string> settings = new Dictionary<string, string>();
        string contents;
        if (index < maxFilesToRead && fileContentsProvider.TryGetFileContents(requestContext, filePath.ToString(), out contents) && !string.IsNullOrEmpty(contents))
        {
          IDockerfileInfo dockerfileInfo = DockerfileParser.Parse(requestContext, contents);
          List<string> list1 = dockerfileInfo.BuildStages.Select<IDockerBuildStage, string>((Func<IDockerBuildStage, string>) (s => s.BaseImage.Image)).ToList<string>();
          if (list1.Any<string>())
            settings[DockerBuildFrameworkDetector.Settings.BaseImages] = string.Join(",", (IEnumerable<string>) list1);
          List<string> list2 = dockerfileInfo.BuildStages.SelectMany<IDockerBuildStage, string>((Func<IDockerBuildStage, IEnumerable<string>>) (s => (IEnumerable<string>) s.ExposedPorts)).ToList<string>();
          if (list2.Any<string>())
            settings[DockerBuildFrameworkDetector.Settings.ExposedPorts] = string.Join(",", (IEnumerable<string>) list2);
        }
        return new DetectedBuildTarget(DockerBuildFrameworkDetector.Settings.WellKnownTypes.Dockerfile, filePath.ToString(), (IReadOnlyDictionary<string, string>) settings);
      }
    }

    private static bool IsDockerfileName(string filename) => filename.Equals("Dockerfile", StringComparison.OrdinalIgnoreCase) || filename.StartsWith("Dockerfile.", StringComparison.OrdinalIgnoreCase);

    private static int GetMaxFilesToRead(
      IVssRequestContext requestContext,
      BuildFrameworkDetectionType detectionType)
    {
      return detectionType == BuildFrameworkDetectionType.Shallow ? 1 : requestContext.GetService<IVssRegistryService>().GetValue<int>(requestContext, (RegistryQuery) "/Service/Pipelines/DockerBuildFrameworkDetector/MaxFilesToRead", 50);
    }

    public static class Settings
    {
      public static readonly string BaseImages = DockerBuildFrameworkDetector.Id + "." + DockerBuildFrameworkDetector.Settings.WellKnownTypes.Dockerfile + ".baseImage";
      public static readonly string ExposedPorts = DockerBuildFrameworkDetector.Id + "." + DockerBuildFrameworkDetector.Settings.WellKnownTypes.Dockerfile + ".exposedPort";

      public static class WellKnownTypes
      {
        public static readonly string Dockerfile = nameof (Dockerfile);
        public static readonly string Compose = nameof (Compose);
      }
    }
  }
}
