// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.NodeJsBuildFrameworkDetector
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class NodeJsBuildFrameworkDetector : IBuildFrameworkDetector
  {
    public static readonly string Id = "nodejs";

    public bool TryDetect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType,
      out DetectedBuildFramework detectedBuildFramework)
    {
      using (new Tracer<NodeJsBuildFrameworkDetector>(requestContext, TracePoints.BuildFrameworkDetection.NodeJsTryDetectEnter, TracePoints.BuildFrameworkDetection.NodeJsTryDetectLeave, nameof (TryDetect)))
      {
        ArgumentUtility.CheckForNull<TreeAnalysis>(treeAnalysis, nameof (treeAnalysis));
        detectedBuildFramework = (DetectedBuildFramework) null;
        if (NodeJsBuildFrameworkDetector.LooksLikeNodeJs(treeAnalysis))
        {
          List<DetectedBuildTarget> buildTargets = new List<DetectedBuildTarget>(this.GetTaskRunners(requestContext, treeAnalysis));
          detectedBuildFramework = new DetectedBuildFramework(NodeJsBuildFrameworkDetector.Id, string.Empty, (IReadOnlyList<DetectedBuildTarget>) buildTargets);
        }
        return detectedBuildFramework != null;
      }
    }

    private static bool LooksLikeNodeJs(TreeAnalysis treeAnalysis) => treeAnalysis.NodeDictionary.ContainsKey("package.json");

    private IEnumerable<DetectedBuildTarget> GetTaskRunners(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis)
    {
      FilePath filePath;
      if (treeAnalysis.TryGetFilePath("gulpfile.js", out filePath))
        yield return new DetectedBuildTarget(NodeJsBuildFrameworkDetector.Settings.WellKnownTypes.Gulp, filePath.ToString());
      if (treeAnalysis.TryGetFilePath("gruntfile.js", out filePath))
        yield return new DetectedBuildTarget(NodeJsBuildFrameworkDetector.Settings.WellKnownTypes.Grunt, filePath.ToString());
      foreach (AzureFunctionAppDetector.FunctionAppInfo azureFunctionApp in new AzureFunctionAppDetector().GetAzureFunctionApps(requestContext, treeAnalysis))
        yield return new DetectedBuildTarget(NodeJsBuildFrameworkDetector.Settings.WellKnownTypes.AzureFunctionApp, azureFunctionApp.Host.ToString());
    }

    public static class Settings
    {
      public static class WellKnownTypes
      {
        public static readonly string Gulp = "gulp";
        public static readonly string Grunt = "grunt";
        public static readonly string AzureFunctionApp = "azureFunctionApp";
      }
    }
  }
}
