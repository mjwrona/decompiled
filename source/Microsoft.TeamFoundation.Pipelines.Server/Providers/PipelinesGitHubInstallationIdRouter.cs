// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.PipelinesGitHubInstallationIdRouter
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  internal class PipelinesGitHubInstallationIdRouter : 
    GitHubInstallationIdRouter,
    IPipelineHostIdMappingRouter,
    IHostIdMappingRouter
  {
    protected override string Layer => nameof (PipelinesGitHubInstallationIdRouter);

    protected override string QualifierName => "full_name";

    public bool TryExtractMappingData(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      out HostIdMappingData mappingData)
    {
      return this.TryExtractMappingDataForSingleRepository(requestContext, definition, definition.Repository, out mappingData);
    }

    public bool TryExtractMappingDataForSingleRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildRepository repository,
      out HostIdMappingData mappingData)
    {
      string installationId = string.Empty;
      mappingData = (HostIdMappingData) null;
      int num = this.TryGetInstallationIdForRepository(requestContext, definition, repository, out installationId) ? 1 : 0;
      if (num == 0)
        return num != 0;
      mappingData = new HostIdMappingData()
      {
        PropertyName = GitHubConstants.InstallationId,
        Id = installationId,
        Qualifier = repository.Name
      };
      return num != 0;
    }

    public bool TryExtractMappingDataForRepositories(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      List<BuildRepository> repoList,
      out List<HostIdMappingData> mappingDataList)
    {
      ArgumentUtility.CheckForNull<List<BuildRepository>>(repoList, nameof (repoList));
      mappingDataList = new List<HostIdMappingData>();
      foreach (BuildRepository repo in repoList)
      {
        HostIdMappingData mappingData;
        if (this.TryExtractMappingDataForSingleRepository(requestContext, definition, repo, out mappingData))
        {
          mappingDataList.Add(mappingData);
        }
        else
        {
          requestContext.TraceAlways(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.EventsRouting.Mapping, TraceLevel.Info, Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Area, this.Layer, "Failed to extract mapping for repository name '" + repo.Name + "' from definition '" + definition.Name + ".");
          return false;
        }
      }
      return true;
    }

    public bool TryParseRepoIdFromRoutingKey(string key, out string repositoryId)
    {
      repositoryId = key.Substring(key.LastIndexOf("}-") + 2);
      return !string.IsNullOrEmpty(repositoryId);
    }

    private bool TryGetInstallationIdForRepository(
      IVssRequestContext requestContext,
      BuildDefinition definition,
      BuildRepository repository,
      out string installationId)
    {
      ArgumentUtility.CheckForNull<BuildDefinition>(definition, nameof (definition));
      ArgumentUtility.CheckForNull<BuildRepository>(repository, nameof (repository));
      installationId = "";
      Guid serviceEndpointId;
      if (!repository.TryGetServiceEndpointId(out serviceEndpointId) || serviceEndpointId == Guid.Empty)
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.EventsRouting.Mapping, this.Layer, "Cannot get service endpoint");
        return false;
      }
      IVssRequestContext vssRequestContext = requestContext.Elevate();
      ServiceEndpoint serviceEndpoint = vssRequestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(vssRequestContext, definition.ProjectId, serviceEndpointId);
      if (serviceEndpoint.AllowsWebhooks(requestContext))
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.EventsRouting.Mapping, this.Layer, "Definition endpoint ALLOWS webhooks.");
        return false;
      }
      if (serviceEndpoint.Authorization.Parameters.TryGetValue("IdToken", out installationId))
        return true;
      requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.EventsRouting.Mapping, this.Layer, "Unable to extract installationId from endpoint.");
      return false;
    }
  }
}
