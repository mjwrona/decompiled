// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models.CloudException
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Models
{
  [Serializable]
  public class CloudException : Exception
  {
    public Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpRequestMessageWrapper Request { get; set; }

    public Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpResponseMessageWrapper Response { get; set; }

    public CloudError Body { get; set; }

    public string RequestId { get; set; }

    public CloudException()
    {
    }

    public CloudException(string message)
      : this(message, (Exception) null)
    {
    }

    public CloudException(string message, Exception innerException)
      : base(SecretUtility.ScrubSecrets(message), innerException)
    {
    }

    protected CloudException(SerializationInfo info, StreamingContext context)
      : base(info, context)
    {
      this.Request = (Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpRequestMessageWrapper) info.GetValue(nameof (Request), typeof (Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpRequestMessageWrapper));
      this.Response = (Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpResponseMessageWrapper) info.GetValue(nameof (Response), typeof (Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpResponseMessageWrapper));
      this.Body = (CloudError) info.GetValue(nameof (Body), typeof (CloudError));
      this.RequestId = info.GetString(nameof (RequestId));
    }

    public override void GetObjectData(SerializationInfo info, StreamingContext context)
    {
      base.GetObjectData(info, context);
      info.AddValue("Request", (object) this.Request);
      info.AddValue("Response", (object) this.Response);
      info.AddValue("Body", (object) this.Body);
      info.AddValue("RequestId", (object) this.RequestId);
    }

    public static CloudException CreateFrom(Microsoft.Rest.Azure.CloudException azureCloudException)
    {
      if (azureCloudException == null)
        return (CloudException) null;
      return new CloudException(azureCloudException.Message, (Exception) azureCloudException)
      {
        Request = Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpRequestMessageWrapper.CreateFrom(azureCloudException.Request),
        Response = Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpResponseMessageWrapper.CreateFrom(azureCloudException.Response),
        Body = CloudError.CreateFrom(azureCloudException.Body),
        RequestId = azureCloudException.RequestId
      };
    }
  }
}
