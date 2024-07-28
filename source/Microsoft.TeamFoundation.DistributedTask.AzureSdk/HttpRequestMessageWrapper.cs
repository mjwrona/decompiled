// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.HttpRequestMessageWrapper
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk
{
  [DataContract]
  public class HttpRequestMessageWrapper : HttpMessageWrapper
  {
    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public HttpMethod Method { get; protected set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public Uri RequestUri { get; protected set; }

    [DataMember(IsRequired = false, EmitDefaultValue = false)]
    public IDictionary<string, object> Properties { get; } = (IDictionary<string, object>) new Dictionary<string, object>();

    public static HttpRequestMessageWrapper CreateFrom(Microsoft.Rest.HttpRequestMessageWrapper azureRequest)
    {
      if (azureRequest == null)
        return (HttpRequestMessageWrapper) null;
      HttpRequestMessageWrapper from = new HttpRequestMessageWrapper()
      {
        Method = azureRequest.Method,
        RequestUri = azureRequest.RequestUri
      };
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) azureRequest.Properties)
        from.Properties.Add(property.Key, property.Value);
      from.Content = azureRequest.Content;
      foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in azureRequest.Headers.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (header => !header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))))
        from.Headers.Add(keyValuePair.Key, keyValuePair.Value);
      return from;
    }

    public static async Task<HttpRequestMessageWrapper> CreateFromAsync(
      HttpRequestMessage azureRequest)
    {
      if (azureRequest == null)
        return (HttpRequestMessageWrapper) null;
      HttpRequestMessageWrapper request = new HttpRequestMessageWrapper()
      {
        Method = azureRequest.Method,
        RequestUri = azureRequest.RequestUri
      };
      foreach (KeyValuePair<string, object> property in (IEnumerable<KeyValuePair<string, object>>) azureRequest.Properties)
        request.Properties.Add(property.Key, property.Value);
      if (azureRequest.Content != null)
      {
        try
        {
          HttpRequestMessageWrapper requestMessageWrapper = request;
          requestMessageWrapper.Content = await azureRequest.Content.ReadAsStringAsync();
          requestMessageWrapper = (HttpRequestMessageWrapper) null;
        }
        catch (ObjectDisposedException ex)
        {
        }
      }
      foreach (KeyValuePair<string, IEnumerable<string>> keyValuePair in azureRequest.Headers.Where<KeyValuePair<string, IEnumerable<string>>>((Func<KeyValuePair<string, IEnumerable<string>>, bool>) (header => !header.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase))))
        request.Headers.Add(keyValuePair.Key, keyValuePair.Value);
      return request;
    }
  }
}
