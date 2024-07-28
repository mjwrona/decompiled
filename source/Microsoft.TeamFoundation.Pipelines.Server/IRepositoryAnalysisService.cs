// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.IRepositoryAnalysisService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  [DefaultServiceImplementation(typeof (RepositoryAnalysisService))]
  public interface IRepositoryAnalysisService : IVssFrameworkService
  {
    ConfigurationFile GetExistingConfigurationFile(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch,
      string path);

    IReadOnlyList<Template> GetSuggestedConfigurationFiles(
      IVssRequestContext requestContext,
      RepositoryContext repositoryContext,
      IReadOnlyList<DetectedBuildFramework> detectedBuildFrameworks);

    string GetRecommendedConfigurationPath(IVssRequestContext requestContext, HashSet<string> files);

    string PrimaryConfigurationPath { get; }

    IEnumerable<string> FindExistingConfigurationFilePaths(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryType,
      string repository,
      Guid? connectionId,
      string branch,
      Func<string, bool> validationFunction);
  }
}
