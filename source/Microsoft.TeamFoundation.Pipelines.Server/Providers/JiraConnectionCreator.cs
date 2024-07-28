// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraConnectionCreator
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira;
using Microsoft.TeamFoundation.Build2.Server;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Pipelines.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Jwt;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public class JiraConnectionCreator : IPipelineConnectionCreator
  {
    private const string c_layer = "JiraConnectionCreator";

    public string RepositoryType => (string) null;

    public string IdentityName => (string) null;

    public string IdentityRole => JiraProviderConstants.IdentityRole;

    public void PreCreateConnection(
      IVssRequestContext requestContext,
      IPipelineSourceProvider provider,
      CreatePipelineConnectionInputs inputs)
    {
      string installationId = this.GetInstallationId(requestContext, (IDictionary<string, string>) inputs.ProviderData);
      ArgumentUtility.CheckIsValidURI(installationId, UriKind.Absolute, "jiraAccountUrl");
      string jiraAccountName = JiraHelper.GetJiraAccountName(installationId);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (JiraConnectionCreator), "{0}-jira(project={1}, account url={2})", (object) nameof (PreCreateConnection), (object) inputs.Project.Id, (object) installationId);
      ArgumentUtility.CheckForNull<TeamProject>(inputs.Project, "Project");
      if (!this.IsProjectAdmin(requestContext, inputs.Project.Id))
        throw new UnauthorizedAccessException(PipelinesResources.JiraConnectAppConnectionCreateAccessDenied());
      string jwtToken = this.GetJWTToken(inputs);
      if (JiraConnectionCreator.ValidateJWTWithOAuthConfiguration(requestContext, jiraAccountName, jwtToken, inputs))
        return;
      JiraInstallationData secretsFromKeyVault = JiraHelper.GetSecretsFromKeyVault(requestContext, jiraAccountName);
      if (secretsFromKeyVault == null)
        throw new InvalidRequestException(PipelinesResources.JiraAppInstallationDataNotFound());
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (JiraConnectionCreator), "{0}- Got secerts from the key vault for jira account {1})", (object) nameof (PreCreateConnection), (object) jiraAccountName);
      JiraJsonWebTokenHelper.ValidateJsonWebToken(requestContext, jwtToken, secretsFromKeyVault.ClientKey, secretsFromKeyVault.SharedSecret);
      OAuthConfigurationParams configurationParams = new OAuthConfigurationParams()
      {
        Name = JiraConnectionCreator.GetNextAvailableOAuthConfigurationName(requestContext, jiraAccountName),
        ClientId = secretsFromKeyVault.ClientKey,
        ClientSecret = secretsFromKeyVault.SharedSecret,
        EndpointType = "Jira",
        Url = new Uri(installationId)
      };
      AuthConfiguration authConfiguration = requestContext.GetService<IOAuthConfigurationService2>().CreateAuthConfiguration(requestContext.Elevate(), configurationParams);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (JiraConnectionCreator), "{0}- Created collection configuration. Id={1})", (object) nameof (PreCreateConnection), (object) authConfiguration.Id);
      string str = new JiraConfigurationData()
      {
        Id = authConfiguration.Id
      }.Serialize<JiraConfigurationData>();
      requestContext.GetService<IDistributedTaskInstalledAppService>().AddInstallation(requestContext, provider.ExternalApp.AppId, jiraAccountName, new DistributedTaskInstalledAppData()
      {
        BillingHostId = requestContext.ServiceHost.InstanceId,
        Data = str
      });
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (JiraConnectionCreator), "{0}- Adding host id mapping in MPS for jira account {1})", (object) nameof (PreCreateConnection), (object) jiraAccountName);
      JiraHelper.DeleteReverseLookupEntryFromLocalStore(requestContext, jiraAccountName);
      requestContext.GetService<IHostIdMappingService>().AddRoute(requestContext, (IHostIdMappingProviderData) provider, jiraAccountName);
    }

    public ServiceEndpoint CreateServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      string defaultName,
      IDictionary<string, string> providerData)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<IDictionary<string, string>>(providerData, nameof (providerData));
      string installationId = this.GetInstallationId(requestContext, providerData);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(installationId, "jiraAccountUrl");
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (JiraConnectionCreator), "{0}-jira(project={1}, account url={2})", (object) nameof (CreateServiceEndpoint), (object) projectId, (object) installationId);
      Guid instanceId = requestContext.ServiceHost.InstanceId;
      string jiraAccountName = JiraHelper.GetJiraAccountName(installationId);
      DistributedTaskInstalledAppData appData;
      if (!requestContext.GetService<IDistributedTaskInstalledAppService>().TryGetInstallationData(requestContext, "com.azure.devops.integration.jira", jiraAccountName, out appData))
        throw new InvalidRequestException(PipelinesResources.JiraAppInstallationDataNotFound());
      JiraConfigurationData jiraConfigurationData = JsonUtilities.Deserialize<JiraConfigurationData>(appData.Data);
      ServiceEndpoint serviceEndpoint = JiraConnectionCreator.GetServiceEndpoint(requestContext, projectId, installationId, jiraConfigurationData);
      requestContext.GetService<IServiceEndpointService2>().CreateServiceEndpoint(requestContext.Elevate(), projectId, serviceEndpoint);
      return (ServiceEndpoint) null;
    }

    public string GetInstallationId(
      IVssRequestContext requestContext,
      IDictionary<string, string> providerData)
    {
      string installationId;
      if (providerData != null && providerData.TryGetValue("jiraAccountUrl", out installationId))
        return installationId;
      throw new ArgumentException("'jiraAccountUrl' is a required value in the ProviderData dictionary, but was not found.");
    }

    public string GetInstallationId(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint)
    {
      throw new NotImplementedException();
    }

    public string GetRedirectUrl(
      IVssRequestContext requestContext,
      CreatePipelineConnectionInputs inputs,
      ServiceEndpoint endpoint)
    {
      ArgumentUtility.CheckForNull<CreatePipelineConnectionInputs>(inputs, nameof (inputs));
      return inputs.RedirectUrl;
    }

    public ServiceEndpoint CreateUserEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      string login,
      string avatarUrl,
      string accessToken,
      string accessTokenType)
    {
      throw new NotImplementedException();
    }

    public bool IsProviderEndpoint(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint,
      string installationId = null)
    {
      throw new NotImplementedException();
    }

    public bool IsProviderDefinition(IVssRequestContext requestContext, BuildDefinition definition) => throw new NotImplementedException();

    public bool IsProviderRepository(
      IVssRequestContext requestContext,
      BuildRepository repository,
      Guid projectId)
    {
      throw new NotImplementedException();
    }

    public BuildRepository CreateBuildRepository(
      IVssRequestContext requestContext,
      Guid projectId,
      string repositoryId,
      string repositoryName,
      string serviceEndpointId)
    {
      throw new NotImplementedException();
    }

    private static ServiceEndpoint GetServiceEndpoint(
      IVssRequestContext requestContext,
      Guid projectId,
      string jiraAccountUrl,
      JiraConfigurationData jiraConfigurationData)
    {
      string jiraAccountName = JiraHelper.GetJiraAccountName(jiraAccountUrl);
      return new ServiceEndpoint()
      {
        Id = Guid.Empty,
        Type = "Jira",
        Name = Microsoft.TeamFoundation.Pipelines.Server.ServiceEndpointHelper.GetNextAvailableEndpointName(requestContext, projectId, jiraAccountName, true),
        Authorization = new EndpointAuthorization()
        {
          Scheme = "JiraConnectApp",
          Parameters = {
            {
              "ConfigurationId",
              jiraConfigurationData.Id.ToString()
            }
          }
        },
        Url = new Uri(jiraAccountUrl),
        IsReady = true
      };
    }

    private string GetJWTToken(CreatePipelineConnectionInputs inputs)
    {
      string jwtToken;
      if (inputs.ProviderData != null && inputs.ProviderData.TryGetValue("jwt", out jwtToken))
        return jwtToken;
      throw new ArgumentException("'jwt' is a required value in the ProviderData dictionary, but was not found.");
    }

    private static void CheckConnectionExistOrNot(
      IVssRequestContext requestContext,
      CreatePipelineConnectionInputs inputs,
      Guid configId)
    {
      IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Guid id = inputs.Project.Id;
      foreach (ServiceEndpoint queryServiceEndpoint in service.QueryServiceEndpoints(requestContext1, id, "Jira", (IEnumerable<string>) new List<string>()
      {
        "JiraConnectApp"
      }, (IEnumerable<Guid>) null, (string) null, false))
      {
        if (queryServiceEndpoint.Authorization != null && queryServiceEndpoint.Authorization.Parameters != null && queryServiceEndpoint.Authorization.Parameters.ContainsKey("ConfigurationId") && queryServiceEndpoint.Authorization.Parameters["ConfigurationId"].Equals(configId.ToString(), StringComparison.OrdinalIgnoreCase))
          throw new InvalidRequestException(string.Format(PipelinesResources.JiraConnectAppConnectAlreadyExist((object) inputs.Project.Name)));
      }
    }

    private static string GetNextAvailableOAuthConfigurationName(
      IVssRequestContext context,
      string resourceName)
    {
      context.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (JiraConnectionCreator), "GetNextAvailableOAuthConfigurationName(" + resourceName + ")");
      HashSet<string> hashSet = context.GetService<IOAuthConfigurationService2>().GetAuthConfigurations(context, (string) null, OAuthConfigurationActionFilter.None).Select<AuthConfiguration, string>((Func<AuthConfiguration, string>) (e => e.Name)).Distinct<string>().ToHashSet<string>();
      return Microsoft.TeamFoundation.Pipelines.Server.ServiceEndpointHelper.GetArtifactName(resourceName, hashSet);
    }

    private bool IsProjectAdmin(IVssRequestContext requestContext, Guid projectId)
    {
      Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
      IdentityService service = requestContext.GetService<IdentityService>();
      IdentityScope scope = service.GetScope(requestContext, projectId);
      IdentityDescriptor groupDescriptor = IdentityMapper.MapFromWellKnownIdentifier(GroupWellKnownIdentityDescriptors.NamespaceAdministratorsGroup, scope.Id);
      return service.IsMember(requestContext, groupDescriptor, userIdentity.Descriptor);
    }

    private static bool ValidateJWTWithOAuthConfiguration(
      IVssRequestContext requestContext,
      string jiraAccountName,
      string jwt,
      CreatePipelineConnectionInputs inputs)
    {
      JiraConfigurationData configurationData = JiraHelper.GetJiraConfigurationData(requestContext, jiraAccountName);
      if (configurationData == null)
        return false;
      Guid id = configurationData.Id;
      JiraInstallationData collectionConfiguration = JiraHelper.GetSecretsFromCollectionConfiguration(requestContext, id);
      if (collectionConfiguration == null)
        return false;
      try
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.CreateConnection, nameof (JiraConnectionCreator), "{0}- Got secerts from collection config Id {1})", (object) nameof (ValidateJWTWithOAuthConfiguration), (object) id);
        JiraJsonWebTokenHelper.ValidateJsonWebToken(requestContext, jwt, collectionConfiguration.ClientKey, collectionConfiguration.SharedSecret);
        JiraConnectionCreator.CheckConnectionExistOrNot(requestContext, inputs, id);
        return true;
      }
      catch (SignatureValidationException ex)
      {
        IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
        IHostIdMappingService service = context.GetService<IHostIdMappingService>();
        HostIdMappingData hostIdMappingData = JiraHelper.GetHostIdMappingData(jiraAccountName, requestContext.ServiceHost.InstanceId.ToString());
        IVssRequestContext deploymentRequestContext = context;
        string providerId = JiraProviderConstants.ProviderId;
        HostIdMappingData mappingData = hostIdMappingData;
        Guid? hostId = service.GetHostId(deploymentRequestContext, providerId, mappingData, true);
        if (hostId.HasValue)
        {
          Guid? nullable = hostId;
          Guid empty = Guid.Empty;
          if ((nullable.HasValue ? (nullable.HasValue ? (nullable.GetValueOrDefault() == empty ? 1 : 0) : 1) : 0) == 0)
            throw;
        }
        return false;
      }
    }
  }
}
