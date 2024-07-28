// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.Providers.JiraHelper
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira;
using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server;
using Microsoft.TeamFoundation.DistributedTask.Server.Exceptions;
using Microsoft.TeamFoundation.ExternalIntegration.HostIdMapping;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location;
using Microsoft.VisualStudio.Services.Location.Server;
using Microsoft.VisualStudio.Services.NameResolution.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;

namespace Microsoft.TeamFoundation.Pipelines.Server.Providers
{
  public static class JiraHelper
  {
    private const string c_layer = "JiraHelper";

    internal static HostIdMappingData GetHostIdMappingData(string jiraAccountName, string hostId = "") => new HostIdMappingData()
    {
      Id = jiraAccountName,
      PropertyName = nameof (jiraAccountName),
      Qualifier = hostId
    };

    internal static void ValidateInstallationData(
      IVssRequestContext requestContext,
      object taskArgs)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      string str = (string) taskArgs;
      ArgumentUtility.CheckStringForNullOrEmpty(str, "jiraBaseUrl");
      string jiraAccountName = JiraHelper.GetJiraAccountName(str);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Validating the data for jira account {1}.", (object) nameof (ValidateInstallationData), (object) jiraAccountName);
      string key = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}-backup", (object) jiraAccountName);
      JiraInstallationData secretsFromKeyVault = JiraHelper.GetSecretsFromKeyVault(requestContext, key);
      if (secretsFromKeyVault == null)
        return;
      bool flag = JiraHelper.AreSecretsValid(requestContext, str, secretsFromKeyVault.ClientKey, secretsFromKeyVault.SharedSecret);
      if (flag)
      {
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Secerts stored in backup key are valid for jira account {1}.", (object) nameof (ValidateInstallationData), (object) jiraAccountName);
        JiraHelper.SetSecertInKeyVault(requestContext, jiraAccountName, JsonUtility.ToString((object) secretsFromKeyVault));
      }
      JiraHelper.DeleteDataFromKeyVault(requestContext, key);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Validated data for jira account {1}.", (object) nameof (ValidateInstallationData), (object) jiraAccountName, (object) flag);
    }

    private static bool AreSecretsValid(
      IVssRequestContext requestContext,
      string jiraBaseUrl,
      string clientKey,
      string sharedSecret)
    {
      JiraHttpClient jiraHttpClient = JiraHttpClientFactory.Create(requestContext);
      JiraAuthentication authentication = new JiraAuthentication(new Uri(jiraBaseUrl), clientKey, sharedSecret);
      for (int index = 1; index < 40; ++index)
      {
        switch (jiraHttpClient.IsAuthDataValid(authentication))
        {
          case HttpStatusCode.OK:
          case HttpStatusCode.Forbidden:
            return true;
          case HttpStatusCode.Unauthorized:
            return false;
          default:
            Thread.Sleep(1000);
            continue;
        }
      }
      return false;
    }

    internal static JiraInstallationData GetSecretsFromKeyVault(
      IVssRequestContext requestContext,
      string key)
    {
      string secret = requestContext.GetService<IJiraConnectAppKeyVaultProvider2>().GetSecret(requestContext, key);
      return secret != null ? JsonUtilities.Deserialize<JiraInstallationData>(secret) : (JiraInstallationData) null;
    }

    internal static void SetSecertInKeyVault(
      IVssRequestContext requestContext,
      string key,
      string value)
    {
      requestContext.GetService<IJiraConnectAppKeyVaultProvider2>().SetSecret(requestContext, key, value);
    }

    internal static JiraInstallationData GetSecretsFromCollectionConfiguration(
      IVssRequestContext requestContext,
      Guid configId)
    {
      AuthConfiguration authConfiguration = JiraHelper.GetAuthConfiguration(requestContext, configId);
      if (authConfiguration == null)
        return (JiraInstallationData) null;
      return new JiraInstallationData()
      {
        ClientKey = authConfiguration.ClientId,
        SharedSecret = authConfiguration.ClientSecret
      };
    }

    private static AuthConfiguration GetAuthConfiguration(
      IVssRequestContext requestContext,
      Guid configId)
    {
      return requestContext.GetService<IOAuthConfigurationService2>().GetAuthConfiguration(requestContext.Elevate(), configId);
    }

    public static string GetJiraAccountName(string jiraAccountUrl)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(jiraAccountUrl, nameof (jiraAccountUrl));
      ArgumentUtility.CheckIsValidURI(jiraAccountUrl, UriKind.Absolute, nameof (jiraAccountUrl));
      return new Uri(jiraAccountUrl).Host.Split('.')[0].ToLowerInvariant();
    }

    public static void ValidateJWT(
      IVssRequestContext requestContext,
      string jiraBaseUrl,
      string jwt)
    {
      string jiraAccountName = JiraHelper.GetJiraAccountName(jiraBaseUrl);
      JiraInstallationData secretsFromKeyVault = JiraHelper.GetSecretsFromKeyVault(requestContext, jiraAccountName);
      if (secretsFromKeyVault == null)
      {
        requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "JiraAppInstallationDataNotFound - Unable to validate JWT token throwed exception for account '" + jiraAccountName + "'.");
        throw new InvalidRequestException(PipelinesResources.JiraAppInstallationDataNotFound());
      }
      JiraJsonWebTokenHelper.ValidateJsonWebToken(requestContext, jwt, secretsFromKeyVault.ClientKey, secretsFromKeyVault.SharedSecret);
    }

    private static void DeleteDataFromKeyVault(
      IVssRequestContext requestContext,
      string key,
      bool testFlag = false)
    {
      TimeSpan timeout = testFlag ? TimeSpan.Zero : TimeSpan.FromMinutes(4.0);
      IJiraConnectAppKeyVaultProvider2 service = requestContext.GetService<IJiraConnectAppKeyVaultProvider2>();
      service.DeleteSecret(requestContext, key);
      Thread.Sleep(timeout);
      service.PurgeSecret(requestContext, key);
    }

    public static IEnumerable<JiraConfiguration> GetJiraConfigurations(
      IVssRequestContext requestContext,
      string jiraBaseUrl)
    {
      string jiraAccountName = JiraHelper.GetJiraAccountName(jiraBaseUrl);
      IEnumerable<Guid> accountNameMappingData = JiraHelper.GetJiraAccountNameMappingData(requestContext.Elevate(), jiraAccountName);
      List<JiraConfiguration> jiraConfigurations = new List<JiraConfiguration>();
      if (accountNameMappingData.IsNullOrEmpty<Guid>())
        return (IEnumerable<JiraConfiguration>) jiraConfigurations;
      INameResolutionService service = requestContext.GetService<INameResolutionService>();
      foreach (Guid hostId in accountNameMappingData)
      {
        string str = JiraHelper.ResolveName(requestContext, service, hostId);
        jiraConfigurations.Add(new JiraConfiguration("JiraConfiguration", 1)
        {
          HostId = hostId,
          HostName = str
        });
      }
      return (IEnumerable<JiraConfiguration>) jiraConfigurations;
    }

    private static string ResolveName(
      IVssRequestContext requestContext,
      INameResolutionService nameResolutionService,
      Guid hostId)
    {
      try
      {
        string name = nameResolutionService.GetPrimaryEntryForValue(requestContext, hostId)?.Name;
        if (!string.IsNullOrWhiteSpace(name))
          return name;
        requestContext.TraceAlways(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.DataProvider.GetConfigurationDataProvider, TraceLevel.Error, Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Area, nameof (JiraHelper), "Failed to resolve name for host ID: {0}", (object) hostId.ToString());
      }
      catch (Exception ex)
      {
        requestContext.TraceException(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.DataProvider.GetConfigurationDataProvider, Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Area, nameof (JiraHelper), ex);
      }
      return hostId.ToString();
    }

    internal static void DeleteProjectConnection(
      IVssRequestContext requestContext,
      Guid hostId,
      Guid projectId,
      string jiraBaseUrl)
    {
      string jiraAccountName = JiraHelper.GetJiraAccountName(jiraBaseUrl);
      JiraConfigurationData configurationData = JiraHelper.GetJiraConfigurationData(requestContext, jiraAccountName);
      if (configurationData == null)
        throw new InvalidRequestException(PipelinesResources.JiraAppInstallationDataNotFound());
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Query for service endpoint type Jira account {1}.", (object) nameof (DeleteProjectConnection), (object) jiraAccountName);
      IServiceEndpointService2 service = requestContext.GetService<IServiceEndpointService2>();
      IServiceEndpointService2 endpointService2 = service;
      IVssRequestContext requestContext1 = requestContext.Elevate();
      Guid scopeIdentifier = projectId;
      foreach (ServiceEndpoint queryServiceEndpoint in endpointService2.QueryServiceEndpoints(requestContext1, scopeIdentifier, "Jira", (IEnumerable<string>) new List<string>()
      {
        "JiraConnectApp"
      }, (IEnumerable<Guid>) null, (string) null, false))
      {
        string g;
        if (queryServiceEndpoint.Authorization != null && queryServiceEndpoint.Authorization.Parameters != null && queryServiceEndpoint.Authorization.Parameters.TryGetValue("ConfigurationId", out g) && configurationData.Id.Equals(new Guid(g)))
        {
          requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Deleting the service endpoint {1} of project {2}.", (object) nameof (DeleteProjectConnection), (object) queryServiceEndpoint.Id, (object) projectId);
          service.DeleteServiceEndpoint(requestContext.Elevate(), projectId, queryServiceEndpoint.Id);
        }
      }
    }

    public static void DeleteJiraAccountData(
      IVssRequestContext requestContext,
      JiraLifecycleEventData jiraEventData,
      bool unitTestThreadTimeSpanFlag = false)
    {
      string jiraAccountName = JiraHelper.GetJiraAccountName(jiraEventData.BaseUrl);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Deleting the jira account {1} data.", (object) nameof (DeleteJiraAccountData), (object) jiraAccountName);
      string vsassetsAccessMapping = PipelineConstants.VsassetsAccessMapping;
      AccessMapping accessMapping = requestContext.GetService<ILocationService>().GetAccessMapping(requestContext, vsassetsAccessMapping);
      if (accessMapping == null)
        return;
      IEnumerable<Guid> accountNameMappingData = JiraHelper.GetJiraAccountNameMappingData(requestContext, jiraAccountName);
      if (!accountNameMappingData.IsNullOrEmpty<Guid>())
      {
        JiraInstallationData secretsFromKeyVault = JiraHelper.GetSecretsFromKeyVault(requestContext, jiraAccountName);
        if (secretsFromKeyVault != null)
        {
          foreach (Guid guid in accountNameMappingData)
          {
            requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Deleting the jira account data for the hostId {1}.", (object) nameof (DeleteJiraAccountData), (object) guid);
            jiraEventData.HostId = guid;
            jiraEventData.EventType = "deleteaccount";
            if (!JiraHelper.SendEvent(requestContext, accessMapping.AccessPoint, jiraEventData, secretsFromKeyVault))
              requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Deleting the jira account request failed for host {1}.", (object) nameof (DeleteJiraAccountData), (object) guid);
          }
        }
      }
      JiraHelper.DeleteDataFromKeyVault(requestContext, jiraAccountName, unitTestThreadTimeSpanFlag);
      JiraHelper.DeleteHostIdMappings(requestContext, jiraAccountName);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Deleted jira account data.", (object) nameof (DeleteJiraAccountData));
    }

    public static void DeleteLinkedJiraConfiguration(
      IVssRequestContext requestContext,
      JiraLifecycleEventData jiraEventData)
    {
      string jiraAccountName = JiraHelper.GetJiraAccountName(jiraEventData.BaseUrl);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Deleting jira account {1} collection config data.", (object) nameof (DeleteLinkedJiraConfiguration), (object) jiraAccountName);
      JiraConfigurationData configurationData = JiraHelper.GetJiraConfigurationData(requestContext, jiraAccountName);
      if (configurationData != null)
      {
        try
        {
          requestContext.GetService<IOAuthConfigurationService2>().DeleteAuthConfiguration(requestContext.Elevate(), configurationData.Id);
        }
        catch (Exception ex)
        {
          requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), string.Format("{0}- failed with exception '{1}' while deleting configId '{2}'", (object) nameof (DeleteLinkedJiraConfiguration), (object) ex, (object) configurationData.Id));
        }
      }
      requestContext.GetService<IDistributedTaskInstalledAppService>().RemoveInstallation(requestContext.Elevate(), "com.azure.devops.integration.jira", jiraAccountName);
      JiraHelper.DeleteHostIdMapping(requestContext.Elevate(), jiraAccountName, jiraEventData.HostId.ToString());
      JiraHelper.DeleteReverseLookupEntryFromLocalStore(requestContext.Elevate(), jiraAccountName);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Deleted jira account {1} collection config data.", (object) nameof (DeleteLinkedJiraConfiguration), (object) jiraAccountName);
    }

    public static bool SendEvent(
      IVssRequestContext requestContext,
      string appBaseUrl,
      JiraLifecycleEventData jiraEventData,
      JiraInstallationData jiraInstallationData)
    {
      string str1 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}_{1}_Jira_HttpRequest", (object) jiraEventData.HostId, (object) jiraEventData.EventType);
      string requestUri = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "{0}/_apis/public/Pipelines/Events?provider={1}&api-version=5.0-preview.1", (object) appBaseUrl, (object) JiraProviderConstants.ProviderId);
      JiraAuthentication authentication = new JiraAuthentication(new Uri(jiraEventData.BaseUrl), jiraInstallationData.ClientKey, jiraInstallationData.SharedSecret);
      try
      {
        using (HttpClient httpClient = new HttpClient())
        {
          httpClient.Timeout = TimeSpan.FromSeconds(120.0);
          using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, requestUri))
          {
            using (StringContent stringContent = new StringContent(JsonUtility.ToString((object) jiraEventData), Encoding.UTF8, "application/json"))
            {
              request.Content = (HttpContent) stringContent;
              string str2 = string.Format((IFormatProvider) CultureInfo.InvariantCulture, "JWT {0}", (object) JiraJsonWebTokenHelper.GenerateJsonWebToken(request, authentication, jiraInstallationData.ClientKey));
              request.Headers.Add("Authorization", str2);
              request.Headers.Add("User-Agent", str1);
              TaskExtensions.SyncResult(httpClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead));
            }
          }
        }
        requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Delete collection config request sent jira account {1}.", (object) nameof (SendEvent), (object) jiraEventData.BaseUrl);
        return true;
      }
      catch (Exception ex)
      {
        requestContext.TraceError(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), string.Format("{0}- Sending delete collection config request throwed exception '{1}' for account '{2}'.", (object) nameof (SendEvent), (object) ex, (object) jiraEventData.BaseUrl));
      }
      return false;
    }

    internal static JiraConfigurationData GetJiraConfigurationData(
      IVssRequestContext requestContext,
      string jiraAccountName)
    {
      DistributedTaskInstalledAppData appData;
      return !requestContext.GetService<IDistributedTaskInstalledAppService>().TryGetInstallationData(requestContext, "com.azure.devops.integration.jira", jiraAccountName, out appData) ? (JiraConfigurationData) null : JsonUtilities.Deserialize<JiraConfigurationData>(appData.Data);
    }

    internal static void DeleteReverseLookupEntryFromLocalStore(
      IVssRequestContext requestContext,
      string jiraAccountName)
    {
      IVssRequestContext context = requestContext.To(TeamFoundationHostType.Deployment);
      NameResolutionStore service = context.GetService<NameResolutionStore>();
      string str1 = JiraProviderConstants.ProviderId + "-jiraAccountName-reverseLookup";
      string str2 = "{" + jiraAccountName + "}";
      IVssRequestContext requestContext1 = context;
      string @namespace = str1;
      string name = str2;
      service.DeleteEntry(requestContext1, @namespace, name);
    }

    private static void DeleteHostIdMappings(
      IVssRequestContext requestContext,
      string jiraAccountName)
    {
      HostIdMappingData hostIdMappingData = JiraHelper.GetHostIdMappingData(jiraAccountName);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Deleting the jira account {1} mapping from MPS", (object) nameof (DeleteHostIdMappings), (object) jiraAccountName);
      requestContext.GetService<IHostIdMappingService>().RemoveHostIdMappings(requestContext, JiraProviderConstants.ProviderId, hostIdMappingData);
    }

    private static void DeleteHostIdMapping(
      IVssRequestContext requestContext,
      string jiraAccountName,
      string hostId)
    {
      HostIdMappingData hostIdMappingData = JiraHelper.GetHostIdMappingData(jiraAccountName, hostId);
      requestContext.TraceInfo(Microsoft.TeamFoundation.Pipelines.Server.TracePoints.Provider.HandleEvent, nameof (JiraHelper), "{0}- Deleting the jira account-hostId: {1}-{2} mapping from MPS", (object) "DeleteHostIdMappings", (object) jiraAccountName, (object) hostId);
      requestContext.GetService<IHostIdMappingService>().RemoveHostIdMapping(requestContext, JiraProviderConstants.ProviderId, hostIdMappingData);
    }

    private static IEnumerable<Guid> GetJiraAccountNameMappingData(
      IVssRequestContext requestContext,
      string jiraAccountName)
    {
      JiraHelper.DeleteReverseLookupEntryFromLocalStore(requestContext, jiraAccountName);
      IHostIdMappingService service = requestContext.GetService<IHostIdMappingService>();
      HostIdMappingData hostIdMappingData = JiraHelper.GetHostIdMappingData(jiraAccountName);
      IVssRequestContext deploymentRequestContext = requestContext;
      string providerId = JiraProviderConstants.ProviderId;
      HostIdMappingData mappingData = hostIdMappingData;
      return service.GetHostIds(deploymentRequestContext, providerId, mappingData);
    }
  }
}
