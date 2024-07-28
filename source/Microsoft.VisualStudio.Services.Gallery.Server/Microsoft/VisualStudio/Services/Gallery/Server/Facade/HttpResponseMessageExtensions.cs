// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.HttpResponseMessageExtensions
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Newtonsoft.Json;
using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade
{
  public static class HttpResponseMessageExtensions
  {
    private static readonly JsonSerializer serializer = JsonSerializerFactory.New();

    public static async Task<T> GetResponseFromStream<T>(this HttpResponseMessage response)
    {
      if (response == null)
        throw new ArgumentNullException(nameof (response));
      T responseFromStream;
      using (Stream stream = await response.Content.ReadAsStreamAsync())
      {
        using (StreamReader reader1 = new StreamReader(stream))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
            responseFromStream = HttpResponseMessageExtensions.serializer.Deserialize<T>((JsonReader) reader2);
        }
      }
      return responseFromStream;
    }
  }
}
