// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Server.AzurePublishProfileHelper
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B7D66E3F-07ED-4CF3-859D-36958D465656
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.ServiceEndpoints.Server.dll

using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server;
using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CloudConfiguration;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Server
{
  public static class AzurePublishProfileHelper
  {
    internal static string ServiceEndpointArmUri = string.Empty;
    internal static string AppServicePublishProfileFetchUri = "{0}/publishxml?api-version=2016-08-01";
    internal const string ReadOnlyDisabledSubscription = "ReadOnlyDisabledSubscription";
    internal const string AuthorizationFailed = "AuthorizationFailed";
    internal const string NonRetriable = "NonRetriable";
    internal static HttpMessageHandler HttpMessageHandler;
    private const string c_layer = "AzurePublishProfileHelper";
    private const string SArea = "DistributedTask";
    private const string SLayer = "EndpointService";

    internal static void InitArmDefaults(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint = null)
    {
      if (!requestContext.ExecutionEnvironment.IsDevFabricDeployment || AzureAccessTokenProvider.UseProdAzureResourcesOnDevFabric(requestContext))
        AzurePublishProfileHelper.ServiceEndpointArmUri = "https://management.azure.com";
      else
        AzurePublishProfileHelper.ServiceEndpointArmUri = "https://api-dogfood.resources.windows-int.net/";
    }

    internal static void QueueFetchPublishProfileOperationJob(
      IVssRequestContext requestContext,
      AzurePublishProfileEndpointJobData publishProfileEndpointJobData)
    {
      using (new MethodScope(requestContext, nameof (AzurePublishProfileHelper), nameof (QueueFetchPublishProfileOperationJob)))
      {
        try
        {
          XmlNode xml = TeamFoundationSerializationUtility.SerializeToXml((object) publishProfileEndpointJobData);
          requestContext.GetService<TeamFoundationJobService>().QueueOneTimeJob(requestContext, "AzurePublishProfileEndpointJob", "Microsoft.Azure.DevOps.ServiceEndpoints.Server.Extensions.AzurePublishProfileEndpointJob", xml, JobPriorityLevel.Highest, new TimeSpan(0, 0, 0));
        }
        catch (Exception ex)
        {
          requestContext.TraceException(0, TraceLevel.Error, "DistributedTask", "EndpointService", ex);
        }
      }
    }

    internal static void FetchPublishProfile(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint,
      string vstsToken = null)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (FetchPublishProfile));
      string resourceId;
      endpoint.Data.TryGetValue("resourceId", out resourceId);
      string tenantId1;
      endpoint.Authorization.Parameters.TryGetValue("tenantid", out tenantId1);
      string tenantId2 = AzurePublishProfileHelper.ValidateAndGetTenantId(requestContext, tenantId1);
      AzurePublishProfileHelper.InitArmDefaults(requestContext, endpoint);
      string armAccessToken = AzurePublishProfileHelper.GetArmAccessToken(requestContext, tenantId2, vstsToken);
      string str;
      try
      {
        using (HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, AzurePublishProfileHelper.GetPublishProfileFetchUriForResourceId(requestContext, "/Service/Commerce/AzureResourceManager/BaseUrl", AzurePublishProfileHelper.ServiceEndpointArmUri, resourceId)))
        {
          request.Headers.Add("Authorization", "Bearer " + armAccessToken);
          using (HttpResponseMessage response = AzurePublishProfileHelper.ExecuteHttpRequest(requestContext, request))
            str = AzurePublishProfileHelper.HandleHttpResponse(requestContext, response, ServiceEndpointResources.InsufficientPrivilegesOnSubscription());
        }
      }
      catch (AuthorizationException ex)
      {
        throw new ApplicationException(ServiceEndpointResources.CouldNotFetchPublishProfile((object) ex.Message) + ServiceEndpointResources.CouldNotFetchPublishProfileHelpMessage((object) resourceId), (Exception) ex);
      }
      catch (Exception ex)
      {
        throw new ApplicationException(ServiceEndpointResources.CouldNotFetchPublishProfile((object) ex.Message), ex);
      }
      endpoint.Authorization.Parameters["publishProfile"] = str;
      requestContext.TraceLeave(0, "EndpointService", nameof (FetchPublishProfile));
    }

    internal static Uri GetPublishProfileFetchUriForResourceId(
      IVssRequestContext requestContext,
      string registryPath,
      string defaultValue,
      string resourceId)
    {
      Uri baseUri = AzurePublishProfileHelper.GetBaseUri(requestContext, registryPath, defaultValue);
      string[] strArray = resourceId.Split('/');
      return (strArray[6] + "/" + strArray[7]).ToLowerInvariant() == "microsoft.web/sites" ? new Uri(baseUri, string.Format(AzurePublishProfileHelper.AppServicePublishProfileFetchUri, (object) resourceId)) : (Uri) null;
    }

    internal static Uri GetBaseUri(
      IVssRequestContext requestContext,
      string registryPath,
      string defaultValue)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (GetBaseUri));
      IVssRequestContext vssRequestContext = requestContext.To(TeamFoundationHostType.Deployment);
      string uriString = vssRequestContext.GetService<IVssRegistryService>().GetValue<string>(vssRequestContext, (RegistryQuery) registryPath, defaultValue);
      if (string.IsNullOrEmpty(uriString) && requestContext.ExecutionEnvironment.IsDevFabricDeployment)
        uriString = defaultValue;
      requestContext.TraceLeave(0, "EndpointService", nameof (GetBaseUri));
      return new Uri(uriString);
    }

    internal static string HandleHttpResponse(
      IVssRequestContext requestContext,
      HttpResponseMessage response,
      string errorMessage,
      string helpMessage = null,
      ServiceEndpoint endpoint = null)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (HandleHttpResponse));
      Task<string> task = response.Content.ReadAsStringAsync();
      task.Wait();
      if (!response.IsSuccessStatusCode)
      {
        requestContext.TraceError(34000810, "EndpointService", "Error: {0}", (object) task.Result);
        AzurePublishProfileHelper.ArmErrorContent armErrorContent = JsonConvert.DeserializeObject<AzurePublishProfileHelper.ArmErrorContent>(task.Result);
        if (endpoint != null)
          requestContext.TraceError(34000810, "EndpointService", "Endpoint ID: {0}, ErrorCode: {1}", (object) endpoint.Id, (object) armErrorContent.Error.ErrorCode);
        string message = string.Format((IFormatProvider) CultureInfo.InvariantCulture, " {0}: error code: {1}, inner error code: {2}, inner error message {3}", (object) errorMessage, (object) response.StatusCode.ToString(), (object) armErrorContent.Error.ErrorCode, (object) armErrorContent.Error.ErrorMessage);
        if (!string.IsNullOrEmpty(helpMessage))
          message = message + "<br /> <br />" + helpMessage;
        ApplicationException applicationException = new ApplicationException(message);
        if (response.StatusCode == HttpStatusCode.Forbidden && armErrorContent.Error?.ErrorCode == "AuthorizationFailed" || response.StatusCode == HttpStatusCode.Conflict && armErrorContent.Error?.ErrorCode == "ReadOnlyDisabledSubscription")
          applicationException.Data.Add((object) "NonRetriable", (object) true);
        throw applicationException;
      }
      requestContext.TraceLeave(0, "EndpointService", nameof (HandleHttpResponse));
      return task.Result;
    }

    internal static HttpResponseMessage ExecuteHttpRequest(
      IVssRequestContext requestContext,
      HttpRequestMessage request)
    {
      requestContext.TraceEnter(0, "EndpointService", nameof (ExecuteHttpRequest));
      HttpResponseMessage result;
      using (HttpMessageHandler handler = AzurePublishProfileHelper.HttpMessageHandler ?? (HttpMessageHandler) new HttpClientHandler())
      {
        using (HttpClient httpClient = new HttpClient(handler))
        {
          httpClient.Timeout = new TimeSpan(0, 0, 60);
          result = httpClient.SendAsync(request, HttpCompletionOption.ResponseContentRead).Result;
        }
      }
      requestContext.TraceLeave(0, "EndpointService", nameof (ExecuteHttpRequest));
      return result;
    }

    internal static string GetArmAccessToken(
      IVssRequestContext requestContext,
      string tenantId,
      string vstsToken = null)
    {
      return MsalAzureAccessTokenHelper.GetArmAccessToken(requestContext, tenantId, vstsToken);
    }

    private static string ValidateAndGetTenantId(IVssRequestContext requestContext, string tenantId)
    {
      if (string.IsNullOrEmpty(tenantId))
      {
        requestContext.TraceInfo(0, "EndpointService", "TenantId is null or empty. Retrieving it from the request context");
        tenantId = AadIdentityHelper.GetIdentityTenantId(requestContext.UserContext).ToString();
      }
      return tenantId;
    }

    public static string GetAzDevAccessToken(
      IVssRequestContext requestContext,
      ServiceEndpoint endpoint)
    {
      string vstsToken = (string) null;
      string errorMessage;
      if (endpoint.Authorization.Parameters.ContainsKey("AccessToken") && !VstsAccessTokenHelper.TryGetVstsAccessToken(requestContext, endpoint, out vstsToken, out string _, out string _, out string _, out errorMessage, false))
        throw new InvalidOperationException(errorMessage);
      return vstsToken;
    }

    internal class ArmErrorContent
    {
      [JsonProperty(PropertyName = "error")]
      public AzurePublishProfileHelper.ArmError Error { get; set; }
    }

    internal class ArmError
    {
      [JsonProperty(PropertyName = "code")]
      public string ErrorCode { get; set; }

      [JsonProperty(PropertyName = "message")]
      public string ErrorMessage { get; set; }
    }
  }
}
