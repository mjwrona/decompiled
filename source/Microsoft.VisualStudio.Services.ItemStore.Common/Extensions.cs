// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ItemStore.Common.Extensions
// Assembly: Microsoft.VisualStudio.Services.ItemStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 44753C0C-D541-4975-AF3F-2B606DE6FF70
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ItemStore.Common.dll

using Microsoft.VisualStudio.Services.Content.Common;
using Newtonsoft.Json;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.ItemStore.Common
{
  public static class Extensions
  {
    public static async Task<T> ReadItemAsync<T>(this HttpContent content) where T : Item
    {
      string str;
      if (content == null)
        str = string.Empty;
      else
        str = await content.ReadAsStringAsync().ConfigureAwait(false);
      string content1 = str;
      T obj;
      try
      {
        obj = string.IsNullOrWhiteSpace(content1) ? default (T) : Item.FromJson<T>(content1);
      }
      catch (JsonReaderException ex)
      {
        throw new WrappedJsonReaderException("Unable to read content as json", ex, content1);
      }
      return obj;
    }

    public static void WriteItem(this HttpResponseMessage response, StoredItem item) => response.Content = Extensions.CreateContent(item);

    public static void WriteItem(this HttpRequestMessage request, StoredItem item) => request.Content = Extensions.CreateContent(item);

    public static async Task<AssociationsStatus> AssociateAsync(
      this ItemHttpClientBase client,
      object routeValues,
      AssociationsItem spec,
      Guid resourceId,
      CancellationToken cancellationToken)
    {
      return await (await client.AssociateAsync(spec, resourceId, routeValues, cancellationToken: cancellationToken).ConfigureAwait(false)).Content.ReadItemAsync<AssociationsStatus>().ConfigureAwait(false);
    }

    private static HttpContent CreateContent(StoredItem item) => item != null ? (HttpContent) new StringContent(item.ToJson().ToString(), StrictEncodingWithoutBOM.UTF8, "application/json") : (HttpContent) null;
  }
}
