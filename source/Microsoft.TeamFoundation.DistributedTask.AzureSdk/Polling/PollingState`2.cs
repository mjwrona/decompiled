// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingState`2
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using Microsoft.Rest;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling
{
  public class PollingState<TBody, THeader>
    where TBody : class
    where THeader : class
  {
    private string m_status;
    private HttpResponseMessage m_response;
    private readonly int? m_retryTimeout;
    private AzureOperationDetails m_azureOperationDetails;
    private Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader> m_azureOperationResponse;
    private Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException m_cloudException;
    public const int c_defaultDelayInMilliseconds = 30000;

    public string AzureAsyncOperationHeaderLink { get; set; }

    public AzureOperationDetails AzureOperationDetails
    {
      get
      {
        if (this.m_azureOperationDetails == null)
        {
          try
          {
            HttpResponseMessage response = this.Response;
            string json;
            if (response == null)
            {
              json = (string) null;
            }
            else
            {
              HttpContent content = response.Content;
              json = content != null ? content.AsString() : (string) null;
            }
            this.m_azureOperationDetails = new AzureOperationDetails(JObject.Parse(json));
          }
          catch
          {
          }
        }
        return this.m_azureOperationDetails;
      }
    }

    public Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader> AzureOperationResponse
    {
      get
      {
        if (this.m_azureOperationResponse == null)
        {
          Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader> operationResponse = new Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader>();
          operationResponse.Body = this.Resource;
          operationResponse.Headers = this.ResourceHeaders;
          operationResponse.Request = this.Request;
          operationResponse.Response = this.Response;
          this.m_azureOperationResponse = operationResponse;
        }
        return this.m_azureOperationResponse;
      }
    }

    public Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException CloudException
    {
      get
      {
        if (this.m_cloudException == null)
          this.m_cloudException = new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("The Long Running Operation failed. Polling Status: " + this.ToString())
          {
            Body = this.Error,
            Request = Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpRequestMessageWrapper.CreateFrom(new Microsoft.Rest.HttpRequestMessageWrapper(this.Request, this.Request.Content.AsString())),
            Response = Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpResponseMessageWrapper.CreateFrom(new Microsoft.Rest.HttpResponseMessageWrapper(this.Response, this.Response.Content.AsString()))
          };
        return this.m_cloudException;
      }
    }

    public int DelayInMilliseconds
    {
      get
      {
        if (this.m_retryTimeout.HasValue)
          return this.m_retryTimeout.Value * 1000;
        return this.Response != null && this.Response.Headers.Contains("Retry-After") ? int.Parse(this.Response.Headers.GetValues("Retry-After").FirstOrDefault<string>(), (IFormatProvider) CultureInfo.InvariantCulture) * 1000 : 30000;
      }
    }

    public Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudError Error { get; set; }

    public string LocationHeaderLink { get; set; }

    public HttpRequestMessage Request { get; set; }

    public TBody Resource { get; set; }

    public THeader ResourceHeaders { get; set; }

    public HttpResponseMessage Response
    {
      get => this.m_response;
      set
      {
        this.m_response = value;
        if (this.m_response == null)
          return;
        if (this.m_response.Headers.Contains("Azure-AsyncOperation"))
          this.AzureAsyncOperationHeaderLink = this.m_response.Headers.GetValues("Azure-AsyncOperation").FirstOrDefault<string>();
        if (!this.m_response.Headers.Contains("Location"))
          return;
        this.LocationHeaderLink = this.m_response.Headers.GetValues("Location").FirstOrDefault<string>();
      }
    }

    public string Status
    {
      get => this.m_status;
      set => this.m_status = value ?? throw new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("No Provisioning State");
    }

    public PollingState(HttpOperationResponse<TBody, THeader> response, int? retryTimeout)
    {
      this.m_retryTimeout = retryTimeout;
      this.Response = response.Response;
      this.Request = response.Request;
      this.Resource = response.Body;
      this.ResourceHeaders = response.Headers;
      HttpResponseMessage response1 = response.Response;
      string str;
      if (response1 == null)
      {
        str = (string) null;
      }
      else
      {
        HttpContent content = response1.Content;
        str = content != null ? content.AsString() : (string) null;
      }
      string json = str;
      JObject jobject = (JObject) null;
      if (!string.IsNullOrEmpty(json))
      {
        try
        {
          jobject = JObject.Parse(json);
        }
        catch (JsonException ex)
        {
        }
      }
      switch (this.Response.StatusCode)
      {
        case HttpStatusCode.OK:
          this.Status = (string) jobject?["properties"]?[(object) "provisioningState"] ?? "Succeeded";
          break;
        case HttpStatusCode.Created:
          this.Status = (string) jobject?["properties"]?[(object) "provisioningState"] ?? "InProgress";
          break;
        case HttpStatusCode.Accepted:
          this.Status = "InProgress";
          break;
        case HttpStatusCode.NoContent:
          this.Status = "Succeeded";
          break;
        default:
          this.Status = "Failed";
          break;
      }
    }

    public override string ToString() => string.Format("Polling Status: {0}, AzureOperationId: {1}, Retry-After: {2}, Method: {3}, StatusCode: {4}, AzureAsyncOperationHeaderLink: {5}, LocationHeaderLink: {6}", (object) this.Status, (object) (this.AzureOperationDetails?.Name ?? string.Empty), (object) this.DelayInMilliseconds, (object) this.Request.Method.Method, (object) this.Response.StatusCode, (object) (this.AzureAsyncOperationHeaderLink ?? string.Empty), (object) (this.LocationHeaderLink ?? string.Empty));
  }
}
