// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.DetectedBuildFrameworkWeightingService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class DetectedBuildFrameworkWeightingService : 
    IDetectedBuildFrameworkWeightingService,
    IVssFrameworkService
  {
    private static readonly IReadOnlyDictionary<string, int> s_frameworkMatchQuality = (IReadOnlyDictionary<string, int>) new Dictionary<string, int>()
    {
      {
        DockerBuildFrameworkDetector.Id,
        20000
      },
      {
        DotNetCoreBuildFrameworkDetector.Id,
        19000
      },
      {
        MsBuildBuildFrameworkDetector.Id,
        18000
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Maven,
        17000
      },
      {
        NodeJsBuildFrameworkDetector.Id,
        16000
      },
      {
        PythonBuildFrameworkDetector.Id,
        15000
      },
      {
        PhpBuildFrameworkDetector.Id,
        14000
      },
      {
        PowershellBuildFrameworkDetector.Id,
        13000
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Gradle,
        13000
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Static,
        12000
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.XCode,
        11000
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Make,
        10000
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Go,
        9000
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Ant,
        8000
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Ruby,
        7000
      },
      {
        FallbackBuildFrameworkDetector.FutureDetectedBuildFrameworkIds.Jekyll,
        6000
      }
    };

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IReadOnlyList<DetectedBuildFramework> AssignWeights(
      IReadOnlyList<DetectedBuildFramework> detectedBuildFrameworks)
    {
      ArgumentUtility.CheckForNull<IReadOnlyList<DetectedBuildFramework>>(detectedBuildFrameworks, nameof (detectedBuildFrameworks));
      foreach (DetectedBuildFramework detectedBuildFramework in (IEnumerable<DetectedBuildFramework>) detectedBuildFrameworks)
        detectedBuildFramework.Weight = this.GetWeight(detectedBuildFramework);
      return (IReadOnlyList<DetectedBuildFramework>) detectedBuildFrameworks.OrderByDescending<DetectedBuildFramework, int>((Func<DetectedBuildFramework, int>) (f => f.Weight)).ToList<DetectedBuildFramework>();
    }

    private int GetWeight(DetectedBuildFramework detectedBuildFramework)
    {
      ArgumentUtility.CheckForNull<DetectedBuildFramework>(detectedBuildFramework, nameof (detectedBuildFramework));
      int weight;
      if (!DetectedBuildFrameworkWeightingService.s_frameworkMatchQuality.TryGetValue(detectedBuildFramework.Id, out weight))
        throw new ArgumentException("Unexpected build framework identifier: " + detectedBuildFramework.Id, "Id");
      return weight;
    }
  }
}
