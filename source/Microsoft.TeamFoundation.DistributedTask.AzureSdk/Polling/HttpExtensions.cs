// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling.HttpExtensions
// Assembly: Microsoft.TeamFoundation.DistributedTask.AzureSdk, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 84D2B88A-971A-412D-9BB4-BAAD1599A5AE
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.DistributedTask.AzureSdk.dll

using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.TeamFoundation.DistributedTask.AzureSdk.Polling
{
  public static class HttpExtensions
  {
    public static string AsString(this HttpContent content) => content?.ReadAsStringAsync().ConfigureAwait(false).GetAwaiter().GetResult();

    public static JObject ToJson(this HttpHeaders headers) => headers == null || !headers.Any<KeyValuePair<string, IEnumerable<string>>>() ? new JObject() : headers.ToDictionary<KeyValuePair<string, IEnumerable<string>>, string, IEnumerable<string>>((Func<KeyValuePair<string, IEnumerable<string>>, string>) (kvp => kvp.Key), (Func<KeyValuePair<string, IEnumerable<string>>, IEnumerable<string>>) (kvp => kvp.Value)).ToJson();

    public static JObject ToJson(
      this IDictionary<string, IEnumerable<string>> headers)
    {
      if (headers == null || !headers.Any<KeyValuePair<string, IEnumerable<string>>>())
        return new JObject();
      JObject json = new JObject();
      foreach (KeyValuePair<string, IEnumerable<string>> header in (IEnumerable<KeyValuePair<string, IEnumerable<string>>>) headers)
        json[header.Key] = header.Value.Count<string>() <= 1 ? (JToken) header.Value.FirstOrDefault<string>() : (JToken) new JArray((object) header.Value);
      return json;
    }
  }
}
