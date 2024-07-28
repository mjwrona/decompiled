// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.GitHubConnectionCreator
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Build2.Routes;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.ExternalIntegration.Utilities;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public class GitHubConnectionCreator : IPipelineConnectionCreator
  {
    private const string c_layer = "GitHubConnectionCreator";
    private const string c_serviceEndpointTag_provider = "pipelinesSourceProvider";
    private const string c_avatarUrl = "AvatarUrl";

    public string RepositoryType => "GitHub";

    public virtual string IdentityName => PipelinesResources.GitHubProviderUserName();

    public virtual string IdentityRole => "GitHub App";

    public void PreCreateConnection(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      CreatePipelineConnectionInputs inputs)
    {
      string installationId = this.GetInstallationId(requestContext, (IDictionary<string, string>) inputs.ProviderData);
      if (provider.EventsHandler == null || string.IsNullOrEmpty(installationId))
        return;
      PipelinesHostIdMappingManager.AddRoute(requestContext, provider, installationId);
    }

    public virtual Microsoft.TeamFoundation.Build2.Server.BuildRepository CreateBuildRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string repositoryName,
      string serviceEndpointId)
    {
      return BuildServiceHelper.CreateBuildRepository(requestContext, projectId, this.RepositoryType, repositoryName, repositoryName, new Guid?(new Guid(serviceEndpointId)));
    }

    public virtual ServiceEndpoint CreateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      string defaultName,
      IDictionary<string, string> providerData)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (GitHubConnectionCreator), "{0}-github(project={1})", (object) nameof (CreateServiceEndpoint), (object) projectId);
      IDistributedTaskInstalledAppService service = requestContext.GetService<IDistributedTaskInstalledAppService>();
      string installationId1 = this.GetInstallationId(requestContext, providerData);
      string str1 = (string) null;
      string str2 = (string) null;
      IVssRequestContext requestContext1 = requestContext;
      string appId = GitHubAppConstants.AppId;
      string installationId2 = installationId1;
      DistributedTaskInstalledAppData installedAppData;
      ref DistributedTaskInstalledAppData local = ref installedAppData;
      GitHubData.V3.InstallationDetails installationDetails;
      if (service.TryGetInstallationData(requestContext1, appId, installationId2, out local) && JsonUtilities.TryDeserialize<GitHubData.V3.InstallationDetails>(installedAppData.Data, out installationDetails))
      {
        str1 = installationDetails?.Account?.Login;
        str2 = installationDetails?.Account?.Avatar_url;
      }
      string str3 = "InstallationToken";
      string resourceName = !string.IsNullOrEmpty(str1) ? str1 : defaultName;
      ServiceEndpoint serviceEndpoint = new ServiceEndpoint()
      {
        Id = Guid.Empty,
        Type = "GitHub",
        Name = ServiceEndpointHelper.GetNextAvailableEndpointName(requestContext, projectId, resourceName),
        Url = new GitHubRoot().Uri,
        Authorization = new EndpointAuthorization()
        {
          Scheme = str3,
          Parameters = {
            {
              "IdToken",
              installationId1
            },
            {
              "IdSignature",
              GitHubSecretManagementHelper.GetJwtSignature(requestContext, installationId1)
            }
          }
        },
        Data = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        {
          {
            "pipelinesSourceProvider",
            GitHubProviderConstants.ProviderId
          }
        }
      };
      if (!string.IsNullOrEmpty(str2))
        serviceEndpoint.Data.Add("AvatarUrl", str2);
      if (requestContext.ServiceHost.Is(TeamFoundationHostType.ProjectCollection))
        requestContext.GetService<ITeamFoundationEventService>().PublishNotification(requestContext, (object) new ProviderServiceConnectionEvent()
        {
          Action = "Created",
          AppName = GitHubProviderConstants.ProviderId
        });
      return serviceEndpoint;
    }

    public ServiceEndpoint CreateUserEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      string login,
      string avatarUrl,
      string accessToken,
      string accessTokenType)
    {
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (GitHubConnectionCreator), "{0}-github(project={1}, login={2}, scheme=OAuth, type={3})", (object) nameof (CreateUserEndpoint), (object) projectId, (object) login, (object) accessTokenType);
      ServiceEndpoint userEndpoint = new ServiceEndpoint()
      {
        Id = Guid.Empty,
        Type = "GitHub",
        Name = ServiceEndpointHelper.GetNextAvailableEndpointName(requestContext, projectId, login),
        Url = new GitHubRoot().Uri,
        Authorization = new EndpointAuthorization()
        {
          Scheme = "OAuth",
          Parameters = {
            {
              "AccessToken",
              accessToken
            },
            {
              "AccessTokenType",
              accessTokenType
            },
            {
              "OAuthAccessTokenIsSupplied",
              bool.TrueString
            }
          }
        },
        Data = (IDictionary<string, string>) new Dictionary<string, string>((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase)
        {
          {
            "pipelinesSourceProvider",
            GitHubProviderConstants.ProviderId
          },
          {
            GitHubAppConstants.LoginPropertyName,
            login
          }
        }
      };
      if (!string.IsNullOrEmpty(avatarUrl))
        userEndpoint.Data.Add("AvatarUrl", avatarUrl);
      return userEndpoint;
    }

    public bool IsProviderEndpoint(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      string installationId = null)
    {
      requestContext.Trace(34001100, TraceLevel.Info, "Build2", nameof (GitHubConnectionCreator), "{endpoint.Id}" + serviceEndpoint?.Id.ToString(), (object) ("{endpoint.Scheme}" + serviceEndpoint?.Authorization?.Scheme));
      string b;
      if (((string.Equals(serviceEndpoint.Type, "GitHub", StringComparison.OrdinalIgnoreCase) ? 1 : 0) & (serviceEndpoint.Authorization == null ? (false ? 1 : 0) : (serviceEndpoint.Authorization.Scheme.Equals("InstallationToken", StringComparison.OrdinalIgnoreCase) ? 1 : 0))) != 0 && serviceEndpoint.Authorization.Parameters.TryGetValue("IdToken", out b))
      {
        if (!string.IsNullOrEmpty(installationId))
        {
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Connections.IsProviderEndpoint, nameof (GitHubConnectionCreator), "Endpoint matched on type, scheme, and token.");
          return string.Equals(installationId, b, StringComparison.Ordinal);
        }
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Connections.IsProviderEndpoint, nameof (GitHubConnectionCreator), "Endpoint matched on type and scheme.");
        return true;
      }
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Connections.IsProviderEndpoint, nameof (GitHubConnectionCreator), "Endpoint NOT a match.");
      return false;
    }

    public bool IsProviderDefinition(IVssRequestContext requestContext, Microsoft.TeamFoundation.Build2.Server.BuildDefinition definition)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build2.Server.BuildDefinition>(definition, nameof (definition));
      return this.IsProviderRepository(requestContext, definition.Repository, definition.ProjectId);
    }

    public bool IsProviderRepository(
      IVssRequestContext requestContext,
      Microsoft.TeamFoundation.Build2.Server.BuildRepository repository,
      Guid projectId)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Build2.Server.BuildRepository>(repository, nameof (repository));
      Guid serviceEndpointId;
      if (!repository.TryGetServiceEndpointId(out serviceEndpointId) || serviceEndpointId == Guid.Empty)
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Connections.IsProviderDefinition, nameof (GitHubConnectionCreator), "Definition does NOT match the endpoint.");
        return false;
      }
      if (requestContext.GetService<IServiceEndpointService2>().GetServiceEndpoint(requestContext, projectId, serviceEndpointId).AllowsWebhooks(requestContext))
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Connections.IsProviderDefinition, nameof (GitHubConnectionCreator), "Definition endpoint does NOT allow webhooks.");
        return false;
      }
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Connections.IsProviderDefinition, nameof (GitHubConnectionCreator), "Definition endpoint is a match.");
      return true;
    }

    public virtual string GetInstallationId(
      IVssRequestContext requestContext,
      IDictionary<string, string> providerData)
    {
      string installationId;
      if (providerData != null && providerData.TryGetValue(GitHubConstants.InstallationId, out installationId))
        return installationId;
      throw new ArgumentException("'" + GitHubConstants.InstallationId + "' is a required value in the ProviderData dictionary, but was not found.");
    }

    public string GetInstallationId(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint)
    {
      string str;
      return ((string.Equals(serviceEndpoint.Type, "GitHub", StringComparison.OrdinalIgnoreCase) ? 1 : 0) & (serviceEndpoint.Authorization == null ? (false ? 1 : 0) : (serviceEndpoint.Authorization.Scheme.Equals("InstallationToken", StringComparison.OrdinalIgnoreCase) ? 1 : 0))) != 0 && serviceEndpoint.Authorization.Parameters.TryGetValue("IdToken", out str) ? str : (string) null;
    }

    public string GetRedirectUrl(
      IVssRequestContext requestContext,
      CreatePipelineConnectionInputs inputs,
      ServiceEndpoint endpoint)
    {
      ArgumentUtility.CheckForNull<CreatePipelineConnectionInputs>(inputs, nameof (inputs));
      ArgumentUtility.CheckForNull<TeamProject>(inputs.Project, "Project");
      if (!string.IsNullOrEmpty(inputs.RedirectUrl))
        return inputs.RedirectUrl;
      IBuildRouteService service = requestContext.GetService<IBuildRouteService>();
      Guid projectId;
      string pipelineId;
      string nonce;
      if (UserServiceHelper.TryGetEditContext(requestContext, out projectId, out pipelineId, out nonce, out string _))
      {
        if (inputs.Project.Id != projectId)
        {
          pipelineId = (string) null;
          nonce = (string) null;
        }
        return service.GetEditPipelineDesignerUrl(requestContext, inputs.Project.Id, pipelineId, nonce);
      }
      string repositoryId;
      string telemetrySession;
      if (!UserServiceHelper.TryGetContinuationContext(requestContext, out projectId, out repositoryId, out telemetrySession) || inputs.Project.Id != projectId)
        repositoryId = (string) null;
      return service.GetNewDefinitionDesignerUrl(requestContext, inputs.Project.Id, this.RepositoryType, endpoint == null || !(endpoint.Id != Guid.Empty) ? (string) null : endpoint.Id.ToString("D"), (IEnumerable<Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType>) new Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType[2]
      {
        Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.ContinuousIntegration,
        Microsoft.TeamFoundation.Build.WebApi.DefinitionTriggerType.PullRequest
      }, true, inputs.RequestSource, repositoryId, telemetrySession);
    }
  }
}
