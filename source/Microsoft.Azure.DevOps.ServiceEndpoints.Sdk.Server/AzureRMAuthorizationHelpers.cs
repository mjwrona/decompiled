// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.AzureRMAuthorizationHelpers
// Assembly: Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 002C83BC-B53E-470A-8038-76E47B5E5BF3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.ServiceEndpoints.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

namespace Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server
{
  public class AzureRMAuthorizationHelpers
  {
    protected readonly ServiceEndpoint _serviceEndpoint;
    protected readonly IVssRequestContext _requestContext;
    protected const int MaxSize = 2097152;
    private string _authToken;
    private string _authorityUrl;
    private readonly int TimeoutInSeconds = 20000;
    private readonly AzureEndpointServicePrincipalAuthorizer _authorizer;

    public AzureRMAuthorizationHelpers(
      IVssRequestContext requestContext,
      ServiceEndpoint serviceEndpoint)
    {
      this._serviceEndpoint = serviceEndpoint;
      this._requestContext = requestContext;
      this._authorizer = new AzureEndpointServicePrincipalAuthorizer(requestContext, serviceEndpoint);
    }

    public string GetStorageAccessKey(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      if (!this._serviceEndpoint.Authorization.Parameters.ContainsKey("ServicePrincipalId") || !this._serviceEndpoint.Authorization.Parameters.ContainsKey("ServicePrincipalKey") || !this._serviceEndpoint.Authorization.Parameters.ContainsKey("TenantId"))
        throw new AzureStorageAccessKeyFetchFailedException(ServiceEndpointSdkResources.NoAzureServicePrincipal());
      if (this._serviceEndpoint.Data != null && !this._serviceEndpoint.Data.ContainsKey("subscriptionId"))
        throw new AzureStorageAccessKeyFetchFailedException(ServiceEndpointSdkResources.NoSubscriptionId());
      string storageAccountName = (string) JObject.FromObject(context.ReplacementObject).GetValue(expression.Expression.Trim());
      if (string.IsNullOrEmpty(storageAccountName))
        throw new AzureStorageAccessKeyFetchFailedException(ServiceEndpointSdkResources.NoStorageAccountName());
      List<string> source = this.ListStorageAccountAccessKeys(this.GetStorageAccountResourceId(this._serviceEndpoint.Data["subscriptionId"], storageAccountName));
      return source != null && source.Count<string>() > 0 ? source[0] : throw new AzureStorageAccessKeyFetchFailedException(ServiceEndpointSdkResources.NoStorageAccessKeyFound());
    }

    public string GetKubeConfig(
      MustacheTemplatedExpression expression,
      MustacheEvaluationContext context)
    {
      if (!this._serviceEndpoint.Authorization.Parameters.ContainsKey("ServicePrincipalId") || !this._serviceEndpoint.Authorization.Parameters.ContainsKey("ServicePrincipalKey") || !this._serviceEndpoint.Authorization.Parameters.ContainsKey("TenantId"))
        throw new AzureKubeConfigFetchFailedException(ServiceEndpointSdkResources.NoAzureServicePrincipal());
      string clusterId = (string) JObject.FromObject(context.ReplacementObject).GetValue(expression.Expression.Trim());
      JObject jobject = !string.IsNullOrEmpty(clusterId) ? this.GetAccessProfile(clusterId) : throw new AzureKubeConfigFetchFailedException(ServiceEndpointSdkResources.NoClusterId());
      JToken jtoken;
      string s;
      if (jobject != null && jobject.TryGetValue("properties", out jtoken) && jtoken is JObject container && container.TryGetValue<string>("kubeConfig", out s))
        return Encoding.UTF8.GetString(Convert.FromBase64String(s));
      throw new AzureKubeConfigFetchFailedException(ServiceEndpointSdkResources.NoKubeConfig());
    }

    private JObject GetAccessProfile(string clusterId)
    {
      HttpWebRequest request = (HttpWebRequest) WebRequest.Create(string.Format("{0}/{1}/accessProfiles/clusterAdmin?api-version=2017-08-31", (object) this.GetAuthorityUrl(), (object) clusterId));
      request.Method = "GET";
      request.ReadWriteTimeout = this.TimeoutInSeconds;
      this.AuthorizeRequest(request);
      HttpWebResponse response;
      try
      {
        response = (HttpWebResponse) request.GetResponse();
      }
      catch (WebException ex)
      {
        throw new AzureKubeConfigFetchFailedException(ex.Message);
      }
      return this.CreateResponseBodyObject(response);
    }

    private string GetStorageAccountResourceId(string subscriptionId, string storageAccountName)
    {
      HttpWebRequest request = (HttpWebRequest) WebRequest.Create(string.Format("{0}/subscriptions/{1}/providers/Microsoft.Storage/storageAccounts?api-version=2015-06-15", (object) this.GetAuthorityUrl(), (object) subscriptionId));
      request.Method = "GET";
      this.AuthorizeRequest(request);
      request.ReadWriteTimeout = this.TimeoutInSeconds;
      HttpWebResponse response;
      try
      {
        response = (HttpWebResponse) request.GetResponse();
      }
      catch (WebException ex)
      {
        throw new AzureStorageAccessKeyFetchFailedException(ex.Message);
      }
      JArray source;
      if (this.CreateResponseBodyObject(response).TryGetValue<JArray>("value", out source) && source != null && source.Count<JToken>() > 0)
      {
        JObject jobject = (JObject) null;
        for (int index = 0; index < source.Count<JToken>(); ++index)
        {
          string a;
          if ((source[index] as JObject).TryGetValue<string>("name", out a) && string.Equals(a, storageAccountName, StringComparison.OrdinalIgnoreCase))
          {
            jobject = source[index] as JObject;
            break;
          }
        }
        if (jobject != null)
          return (string) jobject.GetValue("id");
      }
      throw new AzureStorageAccessKeyFetchFailedException(ServiceEndpointSdkResources.StorageAccountNotFound());
    }

    private List<string> ListStorageAccountAccessKeys(string storageId, string apiVersion = "2017-06-01")
    {
      HttpWebRequest request = (HttpWebRequest) WebRequest.Create(string.Format("{0}{1}/listKeys?api-version={2}", (object) this.GetAuthorityUrl(), (object) storageId, (object) apiVersion));
      request.Method = "POST";
      request.ContentLength = 0L;
      request.ContentType = "application/json";
      request.ReadWriteTimeout = this.TimeoutInSeconds;
      this.AuthorizeRequest(request);
      HttpWebResponse response;
      try
      {
        response = (HttpWebResponse) request.GetResponse();
      }
      catch (WebException ex)
      {
        throw new AzureStorageAccessKeyFetchFailedException(ex.Message);
      }
      return this.ExtractKeys(this.CreateResponseBodyObject(response));
    }

    private void AuthorizeRequest(HttpWebRequest request)
    {
      if (string.IsNullOrEmpty(this._authToken))
      {
        this._authorizer.AuthorizeRequest(request, (string) null);
        this._authToken = request.Headers["Authorization"];
      }
      else
        request.Headers.Set("Authorization", this._authToken);
    }

    private string GetAuthorityUrl()
    {
      if (string.IsNullOrEmpty(this._authorityUrl))
        this._authorityUrl = this._authorizer.ServiceEndpoint.GetEnvironmentDataUrl(this._requestContext, "environmentUrl");
      return string.IsNullOrEmpty(this._authorityUrl) ? this._serviceEndpoint.Url.ToString() : this._authorityUrl;
    }

    private JObject CreateResponseBodyObject(HttpWebResponse response)
    {
      char[] buffer = new char[2097152];
      StreamReader streamReader = new StreamReader(response.GetResponseStream());
      int length = streamReader.ReadBlock(buffer, 0, 2097152);
      if (!streamReader.EndOfStream)
        throw new InvalidEndpointResponseException(ServiceEndpointSdkResources.ResponseSizeExceeded());
      return (JObject) JsonConvert.DeserializeObject(new string(buffer, 0, length));
    }

    private List<string> ExtractKeys(JObject responseBodyObject)
    {
      List<string> keys = new List<string>();
      JArray jarray;
      if (responseBodyObject.TryGetValue<JArray>("keys", out jarray))
      {
        for (int index = 0; index < jarray.Count; ++index)
        {
          string str;
          if (jarray[index] != null && (jarray[index] as JObject).TryGetValue<string>("value", out str))
            keys.Add(str);
        }
      }
      return keys;
    }
  }
}
