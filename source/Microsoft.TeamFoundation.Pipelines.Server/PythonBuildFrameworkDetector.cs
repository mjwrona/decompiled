// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PythonBuildFrameworkDetector
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
  public class PythonBuildFrameworkDetector : IBuildFrameworkDetector
  {
    public static readonly string Id = "python";

    public bool TryDetect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType,
      out DetectedBuildFramework detectedBuildFramework)
    {
      using (new Tracer<PythonBuildFrameworkDetector>(requestContext, TracePoints.BuildFrameworkDetection.PythonTryDetectEnter, TracePoints.BuildFrameworkDetection.PythonTryDetectLeave, nameof (TryDetect)))
      {
        ArgumentUtility.CheckForNull<TreeAnalysis>(treeAnalysis, nameof (treeAnalysis));
        detectedBuildFramework = (DetectedBuildFramework) null;
        if (!PythonBuildFrameworkDetector.LooksLikePython(treeAnalysis))
          return false;
        List<DetectedBuildTarget> functionAppBuildTargets = PythonBuildFrameworkDetector.GetAzureFunctionAppBuildTargets(requestContext, treeAnalysis);
        Dictionary<string, string> frameworkSettings = PythonBuildFrameworkDetector.GetWebFrameworkSettings(requestContext, treeAnalysis, fileContentsProvider);
        if (!functionAppBuildTargets.Any<DetectedBuildTarget>() && !frameworkSettings.Any<KeyValuePair<string, string>>())
          return false;
        string pythonVersion = this.GetPythonVersion(requestContext, treeAnalysis, fileContentsProvider);
        detectedBuildFramework = new DetectedBuildFramework(PythonBuildFrameworkDetector.Id, pythonVersion, (IReadOnlyDictionary<string, string>) frameworkSettings, (IReadOnlyList<DetectedBuildTarget>) functionAppBuildTargets);
        return true;
      }
    }

    private static Dictionary<string, string> GetWebFrameworkSettings(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider)
    {
      Dictionary<string, string> settings = new Dictionary<string, string>();
      if (PythonBuildFrameworkDetector.LooksLikeDjango(treeAnalysis))
      {
        PythonBuildFrameworkDetector.SetDjangoSettings(treeAnalysis, settings);
        return settings;
      }
      string contents;
      if (!treeAnalysis.TryGetFileContents(requestContext, fileContentsProvider, "requirements.txt", out contents) || string.IsNullOrEmpty(contents))
        return settings;
      string[] array = new PythonRequirementsParser().EnumeratePackages(contents).ToArray<string>();
      if (PythonBuildFrameworkDetector.LooksLikeFlask((IReadOnlyList<string>) array))
        PythonBuildFrameworkDetector.SetFlaskSettings(settings);
      else if (PythonBuildFrameworkDetector.LooksLikeBottle((IReadOnlyList<string>) array))
        PythonBuildFrameworkDetector.SetBottleSettings(settings);
      return settings;
    }

    private static List<DetectedBuildTarget> GetAzureFunctionAppBuildTargets(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis)
    {
      return new AzureFunctionAppDetector().GetAzureFunctionApps(requestContext, treeAnalysis).Select<AzureFunctionAppDetector.FunctionAppInfo, DetectedBuildTarget>((Func<AzureFunctionAppDetector.FunctionAppInfo, DetectedBuildTarget>) (app => new DetectedBuildTarget(PythonBuildFrameworkDetector.WellKnownTypes.AzureFunctionApp, app.Host.ToString()))).ToList<DetectedBuildTarget>();
    }

    private static void SetDjangoSettings(
      TreeAnalysis treeAnalysis,
      Dictionary<string, string> settings)
    {
      settings[PythonBuildFrameworkDetector.Settings.WebFramework] = PythonBuildFrameworkDetector.WebFrameworks.Django;
      FilePath filePath;
      if (!treeAnalysis.TryGetFilePath("settings.py", out filePath))
        return;
      settings[PythonBuildFrameworkDetector.Settings.DjangoSettings] = filePath.ToString();
    }

    private static void SetFlaskSettings(Dictionary<string, string> settings) => settings[PythonBuildFrameworkDetector.Settings.WebFramework] = PythonBuildFrameworkDetector.WebFrameworks.Flask;

    private static void SetBottleSettings(Dictionary<string, string> settings) => settings[PythonBuildFrameworkDetector.Settings.WebFramework] = PythonBuildFrameworkDetector.WebFrameworks.Bottle;

    private string GetPythonVersion(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider)
    {
      string contents;
      Version result;
      return !treeAnalysis.TryGetFileContents(requestContext, fileContentsProvider, "runtime.txt", out contents) || !contents.StartsWith("python-", StringComparison.OrdinalIgnoreCase) || !Version.TryParse(contents.Replace("python-", string.Empty), out result) ? string.Empty : result.ToString();
    }

    private static bool LooksLikePython(TreeAnalysis treeAnalysis) => treeAnalysis.FileTypes.Contains(".py");

    private static bool LooksLikeDjango(TreeAnalysis treeAnalysis) => treeAnalysis.NodeDictionary.ContainsKey("manage.py");

    private static bool LooksLikeFlask(IReadOnlyList<string> packageNames) => packageNames.Contains<string>("flask", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    private static bool LooksLikeBottle(IReadOnlyList<string> packageNames) => packageNames.Contains<string>("bottle", (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);

    public static class Settings
    {
      public static readonly string WorkingDirectory = "workingDirectory";
      public static readonly string Version = PythonBuildFrameworkDetector.Id + ".version";
      public static readonly string WebFramework = PythonBuildFrameworkDetector.Id + ".webFramework";
      public static readonly string DjangoSettings = PythonBuildFrameworkDetector.Id + ".django.settings";
      public static readonly string FlaskProject = PythonBuildFrameworkDetector.Id + ".flask.project";
    }

    public static class WebFrameworks
    {
      public static readonly string Django = "django";
      public static readonly string Bottle = "bottle";
      public static readonly string Flask = "flask";
    }

    public static class WellKnownTypes
    {
      public static readonly string AzureFunctionApp = "azureFunctionApp";
    }
  }
}
