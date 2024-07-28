// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationResponse
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using System.Net.Http;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling
{
  [DataContract]
  public class AzureOperationResponse
  {
    [DataMember]
    public string RequestId { get; set; }

    [DataMember]
    public Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpRequestMessageWrapper Request { get; set; }

    [DataMember]
    public Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpResponseMessageWrapper Response { get; set; }

    public static AzureOperationResponse CreateFrom(Microsoft.Rest.Azure.AzureOperationResponse azureOperationResponse)
    {
      if (azureOperationResponse == null)
        return (AzureOperationResponse) null;
      AzureOperationResponse from = new AzureOperationResponse();
      from.RequestId = azureOperationResponse.RequestId;
      HttpRequestMessage request1 = azureOperationResponse.Request;
      HttpRequestMessage request2 = azureOperationResponse.Request;
      string content1;
      if (request2 == null)
      {
        content1 = (string) null;
      }
      else
      {
        HttpContent content2 = request2.Content;
        content1 = content2 != null ? content2.AsString() : (string) null;
      }
      from.Request = Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpRequestMessageWrapper.CreateFrom(new Microsoft.Rest.HttpRequestMessageWrapper(request1, content1));
      HttpResponseMessage response1 = azureOperationResponse.Response;
      HttpResponseMessage response2 = azureOperationResponse.Response;
      string content3;
      if (response2 == null)
      {
        content3 = (string) null;
      }
      else
      {
        HttpContent content4 = response2.Content;
        content3 = content4 != null ? content4.AsString() : (string) null;
      }
      from.Response = Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpResponseMessageWrapper.CreateFrom(new Microsoft.Rest.HttpResponseMessageWrapper(response1, content3));
      return from;
    }
  }
}
