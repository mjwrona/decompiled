// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.TreeAnalysisProviderService
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.TeamFoundation.Build.WebApi;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class TreeAnalysisProviderService : ITreeAnalysisProviderService, IVssFrameworkService
  {
    private const long c_treeAnalysisTimeoutMs = 5000;
    private const string c_layer = "TreeAnalysisProviderService";

    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public TreeAnalysis GetTreeAnalysis(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid? connectionId,
      string repositoryType,
      string repository,
      string branch)
    {
      IBuildSourceProvider sourceProvider = requestContext.GetService<IBuildSourceProviderService>().GetSourceProvider(requestContext, repositoryType);
      long timeoutFromRegistry = TreeAnalysisProviderService.GetTimeoutFromRegistry(requestContext);
      try
      {
        return new TreeAnalysis.Builder(sourceProvider.GetTreeTraversalResult(requestContext, projectId, connectionId, repository, branch, timeoutFromRegistry) ?? throw new BuildFrameworkDetectionException(PipelinesResources.BuildFrameworkDetectionFailedNoTreeAnalysis())).Build();
      }
      catch (ExternalSourceProviderException ex) when (ex.InnerException is TimeoutException)
      {
        requestContext.TraceError(TracePoints.RepositoryAnalysis.FindRecommendations, nameof (TreeAnalysisProviderService), "Getting tree analysis failed: " + ex.Message);
        return (TreeAnalysis) null;
      }
    }

    private static long GetTimeoutFromRegistry(IVssRequestContext requestContext) => requestContext.GetService<IVssRegistryService>().GetValue<long>(requestContext, (RegistryQuery) "/Service/Pipelines/Settings/TreeAnalysisTimeoutMs", true, 5000L);
  }
}
