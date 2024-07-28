// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PowershellBuildFrameworkDetector
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
  public class PowershellBuildFrameworkDetector : IBuildFrameworkDetector
  {
    public static readonly string Id = "powershell";

    public bool TryDetect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType,
      out DetectedBuildFramework detectedBuildFramework)
    {
      using (new Tracer<PowershellBuildFrameworkDetector>(requestContext, TracePoints.BuildFrameworkDetection.PowershellTryDetectEnter, TracePoints.BuildFrameworkDetection.PowershellTryDetectLeave, nameof (TryDetect)))
      {
        ArgumentUtility.CheckForNull<TreeAnalysis>(treeAnalysis, nameof (treeAnalysis));
        detectedBuildFramework = (DetectedBuildFramework) null;
        if (!this.LooksLikePowershell(treeAnalysis))
          return false;
        List<DetectedBuildTarget> functionAppBuildTargets = this.GetAzureFunctionAppBuildTargets(requestContext, treeAnalysis);
        if (!functionAppBuildTargets.Any<DetectedBuildTarget>())
          return false;
        detectedBuildFramework = new DetectedBuildFramework(PowershellBuildFrameworkDetector.Id, string.Empty, (IReadOnlyDictionary<string, string>) new Dictionary<string, string>(), (IReadOnlyList<DetectedBuildTarget>) functionAppBuildTargets);
        return true;
      }
    }

    private List<DetectedBuildTarget> GetAzureFunctionAppBuildTargets(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis)
    {
      return new AzureFunctionAppDetector().GetAzureFunctionApps(requestContext, treeAnalysis).Select<AzureFunctionAppDetector.FunctionAppInfo, DetectedBuildTarget>((Func<AzureFunctionAppDetector.FunctionAppInfo, DetectedBuildTarget>) (app => new DetectedBuildTarget(PowershellBuildFrameworkDetector.WellKnownTypes.AzureFunctionApp, app.Host.ToString()))).ToList<DetectedBuildTarget>();
    }

    private bool LooksLikePowershell(TreeAnalysis treeAnalysis) => treeAnalysis.FileTypes.Contains(".ps1");

    public static class Settings
    {
      public static readonly string WorkingDirectory = "workingDirectory";
    }

    public static class WellKnownTypes
    {
      public static readonly string AzureFunctionApp = "azureFunctionAppPowershell";
    }
  }
}
