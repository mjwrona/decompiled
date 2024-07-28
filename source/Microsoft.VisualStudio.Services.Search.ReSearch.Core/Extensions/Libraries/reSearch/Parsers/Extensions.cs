// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers.Extensions
// Assembly: Microsoft.VisualStudio.Services.Search.ReSearch.Core, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 657A74AE-F2A6-4615-BB2F-7FA1F961B173
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.ReSearch.Core.dll

using Newtonsoft.Json;
using System;
using System.Text;

namespace Microsoft.VisualStudio.Services.Search.Extensions.Libraries.reSearch.Parsers
{
  public static class Extensions
  {
    public static byte[] SerializeToByteArray<T>(this T obj) => (object) obj != null ? Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) obj, Formatting.Indented)) : throw new ArgumentNullException(nameof (obj));

    public static T DeserializeToObject<T>(this byte[] arrayBytes)
    {
      if (arrayBytes == null)
        throw new ArgumentNullException(nameof (arrayBytes));
      JsonSerializerSettings settings = new JsonSerializerSettings()
      {
        TypeNameHandling = TypeNameHandling.None
      };
      return JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(arrayBytes), settings);
    }
  }
}
