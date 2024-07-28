// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationResponse`2
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using System.Net.Http;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling
{
  [DataContract]
  public class AzureOperationResponse<TBody, THeader> : AzureOperationResponse<TBody>
  {
    [DataMember]
    public THeader Headers { get; set; }

    public static AzureOperationResponse<TBody, THeader> CreateFrom(
      Microsoft.Rest.Azure.AzureOperationResponse<TBody, THeader> azureOperationResponse)
    {
      if (azureOperationResponse == null)
        return (AzureOperationResponse<TBody, THeader>) null;
      AzureOperationResponse<TBody, THeader> from = new AzureOperationResponse<TBody, THeader>();
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
      from.Body = azureOperationResponse.Body;
      from.Headers = azureOperationResponse.Headers;
      return from;
    }
  }
}
