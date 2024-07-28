// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.PollingStateWrapper
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using Microsoft.Rest.Azure;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling
{
  [DataContract]
  public class PollingStateWrapper
  {
    [DataMember]
    public string AzureAsyncOperationHeaderLink { get; set; }

    [DataMember]
    public AzureOperationDetails AzureOperationDetails { get; set; }

    [DataMember]
    public Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudError Error { get; set; }

    [DataMember]
    public HttpMethod HttpMethod { get; set; }

    [DataMember]
    public HttpStatusCode HttpStatusCode { get; set; }

    [DataMember]
    public string LocationHeaderLink { get; set; }

    [DataMember]
    public string RequestId { get; set; }

    [DataMember]
    public Uri RequestUri { get; set; }

    [DataMember]
    public JObject Resource { get; set; }

    [DataMember]
    public int RetryAfter { get; set; }

    [DataMember]
    public string Status { get; set; }

    public PollingStateWrapper()
    {
    }

    public PollingStateWrapper(Microsoft.Rest.Azure.AzureOperationResponse operation)
    {
      this.HttpMethod = operation.Request.Method;
      this.HttpStatusCode = operation.Response.StatusCode;
      this.RequestId = operation.RequestId;
      this.RequestUri = operation.Request.RequestUri;
      this.UpdateFromResponseHeaders(operation.Response);
      HttpResponseMessage response = operation.Response;
      string str;
      if (response == null)
      {
        str = (string) null;
      }
      else
      {
        HttpContent content = response.Content;
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
      switch (operation.Response.StatusCode)
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

    public Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException CreateCloudException() => new Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException("The Long Running Operation failed. Polling Status: " + this.ToString())
    {
      Body = this.Error,
      RequestId = this.RequestId
    };

    public void UpdateFromResponseHeaders(HttpResponseMessage response)
    {
      if (response == null)
        return;
      if (response.Headers.Contains("Azure-AsyncOperation"))
        this.AzureAsyncOperationHeaderLink = response.Headers.GetValues("Azure-AsyncOperation").FirstOrDefault<string>();
      if (response.Headers.Contains("Location"))
        this.LocationHeaderLink = response.Headers.GetValues("Location").FirstOrDefault<string>();
      if (!response.Headers.Contains("Retry-After"))
        return;
      this.RetryAfter = int.Parse(response.Headers.GetValues("Retry-After").FirstOrDefault<string>(), (IFormatProvider) CultureInfo.InvariantCulture);
    }

    public void UpdateStateFrom(
      Microsoft.Rest.Azure.AzureOperationResponse<AzureAsyncOperation, object> operation)
    {
      this.Status = operation.Body.Status;
      this.UpdateFromResponseHeaders(operation.Response);
      HttpResponseMessage response = operation.Response;
      string str;
      if (response == null)
      {
        str = (string) null;
      }
      else
      {
        HttpContent content = response.Content;
        str = content != null ? content.AsString() : (string) null;
      }
      string json = str;
      try
      {
        this.Resource = JObject.Parse(json);
        this.AzureOperationDetails = new AzureOperationDetails(this.Resource);
      }
      catch
      {
      }
    }

    public void UpdateStateFrom(PollingStateWrapper updatedState)
    {
      if (!string.IsNullOrEmpty(updatedState.AzureAsyncOperationHeaderLink))
        this.AzureAsyncOperationHeaderLink = updatedState.AzureAsyncOperationHeaderLink;
      if (updatedState.AzureOperationDetails != null)
      {
        if (this.AzureOperationDetails == null)
        {
          this.AzureOperationDetails = updatedState.AzureOperationDetails;
        }
        else
        {
          this.AzureOperationDetails.Name = !string.IsNullOrEmpty(updatedState.AzureOperationDetails.Name) ? updatedState.AzureOperationDetails.Name : this.AzureOperationDetails.Name;
          DateTime? nullable1;
          if (updatedState.AzureOperationDetails.StartTime.HasValue)
          {
            nullable1 = updatedState.AzureOperationDetails.StartTime;
            DateTime minValue = DateTime.MinValue;
            if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != minValue ? 1 : 0) : 0) : 1) != 0)
              this.AzureOperationDetails.StartTime = updatedState.AzureOperationDetails.StartTime;
          }
          nullable1 = updatedState.AzureOperationDetails.EndTime;
          if (nullable1.HasValue)
          {
            nullable1 = updatedState.AzureOperationDetails.EndTime;
            DateTime minValue = DateTime.MinValue;
            if ((nullable1.HasValue ? (nullable1.HasValue ? (nullable1.GetValueOrDefault() != minValue ? 1 : 0) : 0) : 1) != 0)
              this.AzureOperationDetails.StartTime = updatedState.AzureOperationDetails.StartTime;
          }
          AzureOperationDetails operationDetails1 = this.AzureOperationDetails;
          nullable1 = updatedState.AzureOperationDetails.StartTime;
          DateTime? nullable2 = nullable1 ?? this.AzureOperationDetails.StartTime;
          operationDetails1.StartTime = nullable2;
          AzureOperationDetails operationDetails2 = this.AzureOperationDetails;
          nullable1 = updatedState.AzureOperationDetails.EndTime;
          DateTime? nullable3 = nullable1 ?? this.AzureOperationDetails.EndTime;
          operationDetails2.EndTime = nullable3;
          this.AzureOperationDetails.Status = !string.IsNullOrEmpty(updatedState.AzureOperationDetails.Status) ? updatedState.AzureOperationDetails.Status : this.AzureOperationDetails.Status;
        }
      }
      if (updatedState.Error != null)
        this.Error = updatedState.Error;
      if (!string.IsNullOrEmpty(updatedState.LocationHeaderLink))
        this.LocationHeaderLink = updatedState.LocationHeaderLink;
      if (updatedState.Resource != null)
        this.Resource = updatedState.Resource;
      if (updatedState.RetryAfter > this.RetryAfter)
        this.RetryAfter = updatedState.RetryAfter;
      if (string.IsNullOrEmpty(updatedState.Status))
        return;
      this.Status = updatedState.Status;
    }

    public override string ToString() => string.Format("Polling Status: {0}, AzureOperationId: {1}, Retry-After: {2}, Method: {3}, StatusCode: {4}, AzureAsyncOperationHeaderLink: {5}, LocationHeaderLink: {6}", (object) this.Status, (object) (this.AzureOperationDetails?.Name ?? string.Empty), (object) this.RetryAfter, (object) this.HttpMethod, (object) this.HttpStatusCode, (object) (this.AzureAsyncOperationHeaderLink ?? string.Empty), (object) (this.LocationHeaderLink ?? string.Empty));
  }
}
