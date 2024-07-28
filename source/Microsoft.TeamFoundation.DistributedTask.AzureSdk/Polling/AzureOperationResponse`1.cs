// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.AzureOperationResponse`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling
{
  [DataContract]
  public class AzureOperationResponse<TBody> : AzureOperationResponse
  {
    [DataMember]
    public TBody Body { get; set; }

    public static async Task<AzureOperationResponse<TBody>> CreateFromAsync(
      Microsoft.Rest.Azure.AzureOperationResponse<TBody> azureOperationResponse)
    {
      if (azureOperationResponse == null)
        return (AzureOperationResponse<TBody>) null;
      AzureOperationResponse<TBody> fromAsync = new AzureOperationResponse<TBody>();
      fromAsync.RequestId = azureOperationResponse.RequestId;
      AzureOperationResponse<TBody> operationResponse1 = fromAsync;
      operationResponse1.Request = await HttpRequestMessageWrapper.CreateFromAsync(azureOperationResponse.Request).ConfigureAwait(false);
      AzureOperationResponse<TBody> operationResponse2 = fromAsync;
      operationResponse2.Response = await HttpResponseMessageWrapper.CreateFromAsync(azureOperationResponse.Response).ConfigureAwait(false);
      fromAsync.Body = azureOperationResponse.Body;
      return fromAsync;
    }
  }
}
