// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.HttpClientExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.Shared.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9764DF62-33FE-41B6-9E79-DE201B497BE0
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.dll

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Packaging.Shared.WebApi
{
  public static class HttpClientExtensions
  {
    public static async Task<T> GetJsonObject<T>(this HttpClient httpClient, Uri queryServiceUri)
    {
      T jsonObject;
      using (HttpResponseMessage response = await httpClient.GetAsync(queryServiceUri))
        jsonObject = response.IsJsonResponse() ? JsonConvert.DeserializeObject<T>(await response.Content.ReadAsStringAsync()) : throw new InvalidDataException("Expected a JSON response but got something else");
      return jsonObject;
    }
  }
}
