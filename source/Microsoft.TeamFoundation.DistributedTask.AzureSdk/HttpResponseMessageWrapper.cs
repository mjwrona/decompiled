// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpResponseMessageWrapper
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk
{
  [DataContract]
  public class HttpResponseMessageWrapper : HttpMessageWrapper
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public HttpStatusCode StatusCode { get; set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public string ReasonPhrase { get; protected set; }

    public static HttpResponseMessageWrapper CreateFrom(Microsoft.Rest.HttpResponseMessageWrapper azureResponse)
    {
      if (azureResponse == null)
        return (HttpResponseMessageWrapper) null;
      HttpResponseMessageWrapper responseMessageWrapper = new HttpResponseMessageWrapper();
      responseMessageWrapper.StatusCode = azureResponse.StatusCode;
      responseMessageWrapper.ReasonPhrase = azureResponse.ReasonPhrase;
      responseMessageWrapper.Content = azureResponse.Content;
      HttpResponseMessageWrapper from = responseMessageWrapper;
      foreach (KeyValuePair<string, IEnumerable<string>> header in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) azureResponse.Headers)
        from.Headers.Add(header.Key, header.Value);
      return from;
    }

    public static async Task<HttpResponseMessageWrapper> CreateFromAsync(
      HttpResponseMessage response)
    {
      if (response == null)
        return (HttpResponseMessageWrapper) null;
      HttpResponseMessageWrapper responseMessageWrapper1 = new HttpResponseMessageWrapper();
      responseMessageWrapper1.StatusCode = response.StatusCode;
      responseMessageWrapper1.ReasonPhrase = response.ReasonPhrase;
      HttpResponseMessageWrapper responseMessageWrapper2 = responseMessageWrapper1;
      responseMessageWrapper2.Content = await response.Content.ReadAsStringAsync();
      HttpResponseMessageWrapper fromAsync = responseMessageWrapper1;
      responseMessageWrapper2 = (HttpResponseMessageWrapper) null;
      responseMessageWrapper1 = (HttpResponseMessageWrapper) null;
      foreach (KeyValuePair<string, IEnumerable<string>> header in (HttpHeaders) response.Headers)
        fromAsync.Headers.Add(header.Key, header.Value);
      return fromAsync;
    }
  }
}
