// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.FallbackBuildFrameworkDetector
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.Server.Helpers;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class FallbackBuildFrameworkDetector : IBuildFrameworkDetector
  {
    private const string c_layer = "FallbackBuildFrameworkDetector";
    private static readonly Dictionary<string, string> s_TemplateIdToBuildFramework = new Dictionary<string, string>()
    {
      [TemplateIds.Workflow.DockerImage] = DockerBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.DockerBuild] = DockerBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.DockerContainer] = DockerBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.DockerContainer] = DockerBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.DockerContainerToAcr] = DockerBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.DockerContainerToAks] = DockerBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.DockerContainerFunctionApp] = DockerBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.DockerContainerWebapp] = DockerBuildFrameworkDetector.Id,
      [TemplateIds.Ant] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Ant,
      [TemplateIds.Android] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Gradle,
      [TemplateIds.Gradle] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Gradle,
      [TemplateIds.Pipelines.Gcc] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Make,
      [TemplateIds.Workflow.C_Cpp] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Make,
      [TemplateIds.Go] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Go,
      [TemplateIds.Maven] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Maven,
      [TemplateIds.Pipelines.MavenWebAppToLinuxOnAzure] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Maven,
      [TemplateIds.NodeJs] = NodeJsBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.NodeJsWithVue] = NodeJsBuildFrameworkDetector.Id,
      [TemplateIds.NodeJsWithWebpack] = NodeJsBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.NodeJsWithReact] = NodeJsBuildFrameworkDetector.Id,
      [TemplateIds.NodeJsWithAngular] = NodeJsBuildFrameworkDetector.Id,
      [TemplateIds.NodeJsWithGrunt] = NodeJsBuildFrameworkDetector.Id,
      [TemplateIds.NodeJsWithGulp] = NodeJsBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.JekyllContainer] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Jekyll,
      [TemplateIds.Workflow.Jekyll] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Jekyll,
      [TemplateIds.Php] = PhpBuildFrameworkDetector.Id,
      [TemplateIds.PythonPackage] = PythonBuildFrameworkDetector.Id,
      [TemplateIds.PythonDjango] = PythonBuildFrameworkDetector.Id,
      [TemplateIds.Ruby] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Ruby,
      [TemplateIds.Xcode] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.XCode,
      [TemplateIds.Html] = FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Static,
      [TemplateIds.Aspnet] = MsBuildBuildFrameworkDetector.Id,
      [TemplateIds.AspnetCore] = MsBuildBuildFrameworkDetector.Id,
      [TemplateIds.AspnetCoreNetFramework] = MsBuildBuildFrameworkDetector.Id,
      [TemplateIds.NetDesktop] = MsBuildBuildFrameworkDetector.Id,
      [TemplateIds.UniversalWindowsPlatform] = MsBuildBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.XamarinAndroid] = MsBuildBuildFrameworkDetector.Id,
      [TemplateIds.Pipelines.XamarinIos] = MsBuildBuildFrameworkDetector.Id,
      [TemplateIds.Workflow.Xamarin] = MsBuildBuildFrameworkDetector.Id
    };

    internal static bool IsFallbackBuildFrameworkDetectorId(string id) => string.Equals(FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Ant, id, StringComparison.OrdinalIgnoreCase) || string.Equals(FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Go, id, StringComparison.OrdinalIgnoreCase) || string.Equals(FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Gradle, id, StringComparison.OrdinalIgnoreCase) || string.Equals(FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Jekyll, id, StringComparison.OrdinalIgnoreCase) || string.Equals(FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Make, id, StringComparison.OrdinalIgnoreCase) || string.Equals(FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Maven, id, StringComparison.OrdinalIgnoreCase) || string.Equals(FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Ruby, id, StringComparison.OrdinalIgnoreCase) || string.Equals(FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Static, id, StringComparison.OrdinalIgnoreCase) || string.Equals(FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.XCode, id, StringComparison.OrdinalIgnoreCase);

    public bool TryDetect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType,
      out DetectedBuildFramework detectedBuildFramework)
    {
      using (new Tracer<FallbackBuildFrameworkDetector>(requestContext, TracePoints.BuildFrameworkDetection.FallbackTryDetectEnter, TracePoints.BuildFrameworkDetection.FallbackTryDetectLeave, nameof (TryDetect)))
      {
        string key = TemplateRules.GetRuleIds().Select<string, (string, int)>((Func<string, (string, int)>) (id => (id, TemplateRules.GetMatchQuality(id, treeAnalysis)))).Where<(string, int)>((Func<(string, int), bool>) (match => match.quality > 0)).OrderByDescending<(string, int), int>((Func<(string, int), int>) (match => match.quality)).Select<(string, int), string>((Func<(string, int), string>) (match => match.id)).FirstOrDefault<string>();
        if (key == null)
        {
          detectedBuildFramework = (DetectedBuildFramework) null;
          return false;
        }
        string id1;
        if (!FallbackBuildFrameworkDetector.s_TemplateIdToBuildFramework.TryGetValue(key, out id1))
        {
          requestContext.TraceWarning(TracePoints.BuildFrameworkDetection.FallbackTryDetectError, nameof (FallbackBuildFrameworkDetector), "Failed to map the template rule '{0}' to a build framework", (object) key);
          detectedBuildFramework = (DetectedBuildFramework) null;
          return false;
        }
        detectedBuildFramework = new DetectedBuildFramework(id1, string.Empty);
        return true;
      }
    }

    public static class FutureDetectedBuildFrameworkIds
    {
      public static readonly string Ant = "ant";
      public static readonly string Go = "go";
      public static readonly string Gradle = "gradle";
      public static readonly string Jekyll = "jekyll";
      public static readonly string Make = "make";
      public static readonly string Maven = "maven";
      public static readonly string Ruby = "ruby";
      public static readonly string Static = "static";
      public static readonly string XCode = "xcode";
    }
  }
}
