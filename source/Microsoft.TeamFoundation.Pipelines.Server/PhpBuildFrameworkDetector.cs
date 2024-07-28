// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PhpBuildFrameworkDetector
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
  public class PhpBuildFrameworkDetector : IBuildFrameworkDetector
  {
    public static readonly string Id = "php";

    public bool TryDetect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType,
      out DetectedBuildFramework detectedBuildFramework)
    {
      using (new Tracer<PhpBuildFrameworkDetector>(requestContext, TracePoints.BuildFrameworkDetection.PhpTryDetectEnter, TracePoints.BuildFrameworkDetection.PhpTryDetectLeave, nameof (TryDetect)))
      {
        ArgumentUtility.CheckForNull<TreeAnalysis>(treeAnalysis, nameof (treeAnalysis));
        detectedBuildFramework = (DetectedBuildFramework) null;
        if (PhpBuildFrameworkDetector.LooksLikePhp(treeAnalysis))
        {
          Dictionary<string, string> settings = new Dictionary<string, string>();
          this.SetWorkingDirectory(treeAnalysis, settings);
          detectedBuildFramework = new DetectedBuildFramework(PhpBuildFrameworkDetector.Id, string.Empty, (IReadOnlyDictionary<string, string>) settings);
        }
        return detectedBuildFramework != null;
      }
    }

    private void SetWorkingDirectory(TreeAnalysis treeAnalysis, Dictionary<string, string> settings)
    {
    }

    private static bool LooksLikePhp(TreeAnalysis treeAnalysis)
    {
      if (treeAnalysis.NodeDictionary.ContainsKey("composer.json") || treeAnalysis.NodeDictionary.ContainsKey("phpunit.xml") || treeAnalysis.NodeDictionary.ContainsKey("index.php"))
        return true;
      if (treeAnalysis.NodeDictionary.ContainsKey("composer.lock"))
      {
        if (!((IEnumerable<string>) new string[7]
        {
          "default.htm",
          "default.html",
          "default.asp",
          "default.aspx",
          "index.htm",
          "index.html",
          "iisstart.htm"
        }).Any<string>(new Func<string, bool>(treeAnalysis.NodeDictionary.ContainsKey)))
          return true;
      }
      return false;
    }

    public static class Settings
    {
      public static readonly string WorkingDirectory = "workingDirectory";
    }
  }
}
