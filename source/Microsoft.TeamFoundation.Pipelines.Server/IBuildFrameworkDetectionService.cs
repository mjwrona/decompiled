// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.IBuildFrameworkDetectionService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (BuildFrameworkDetectionService))]
  public interface IBuildFrameworkDetectionService : IVssFrameworkService
  {
    IReadOnlyList<DetectedBuildFramework> Detect(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch,
      BuildFrameworkDetectionType detectionType);

    IReadOnlyList<DetectedBuildFramework> Detect(
      IVssRequestContext requestContext,
      TreeAnalysis treeAnalysis,
      IFileContentsProvider fileContentsProvider,
      BuildFrameworkDetectionType detectionType);

    IReadOnlyList<DetectedBuildFramework> Detect(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch,
      TreeAnalysis treeAnalysis,
      BuildFrameworkDetectionType detectionType);
  }
}
