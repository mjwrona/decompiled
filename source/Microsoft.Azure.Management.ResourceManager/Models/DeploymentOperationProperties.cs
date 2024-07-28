// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Management.ResourceManager.Models.DeploymentOperationProperties
// Assembly: Microsoft.Azure.Management.ResourceManager, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: ABBAD935-2366-4053-A43B-1C3AE5FDB3D3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Management.ResourceManager.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.Azure.Management.ResourceManager.Models
{
  public class DeploymentOperationProperties
  {
    public DeploymentOperationProperties()
    {
    }

    public DeploymentOperationProperties(
      string provisioningState = null,
      DateTime? timestamp = null,
      string duration = null,
      string serviceRequestId = null,
      string statusCode = null,
      object statusMessage = null,
      TargetResource targetResource = null,
      HttpMessage request = null,
      HttpMessage response = null)
    {
      this.ProvisioningState = provisioningState;
      this.Timestamp = timestamp;
      this.Duration = duration;
      this.ServiceRequestId = serviceRequestId;
      this.StatusCode = statusCode;
      this.StatusMessage = statusMessage;
      this.TargetResource = targetResource;
      this.Request = request;
      this.Response = response;
    }

    [JsonProperty(PropertyName = "provisioningState")]
    public string ProvisioningState { get; private set; }

    [JsonProperty(PropertyName = "timestamp")]
    public DateTime? Timestamp { get; private set; }

    [JsonProperty(PropertyName = "duration")]
    public string Duration { get; private set; }

    [JsonProperty(PropertyName = "serviceRequestId")]
    public string ServiceRequestId { get; private set; }

    [JsonProperty(PropertyName = "statusCode")]
    public string StatusCode { get; private set; }

    [JsonProperty(PropertyName = "statusMessage")]
    public object StatusMessage { get; private set; }

    [JsonProperty(PropertyName = "targetResource")]
    public TargetResource TargetResource { get; private set; }

    [JsonProperty(PropertyName = "request")]
    public HttpMessage Request { get; private set; }

    [JsonProperty(PropertyName = "response")]
    public HttpMessage Response { get; private set; }
  }
}
