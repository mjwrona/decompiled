// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.Facade.JsonStreamBuilder
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Gallery.Server.Facade
{
  public static class JsonStreamBuilder
  {
    public static Stream Create<T>(T value)
    {
      MemoryStream memoryStream = new MemoryStream();
      using (StreamWriter streamWriter = new StreamWriter((Stream) memoryStream, (Encoding) new UTF8Encoding(false), 1024, true))
      {
        JsonTextWriter jsonTextWriter1 = new JsonTextWriter((TextWriter) streamWriter);
        jsonTextWriter1.Formatting = Formatting.None;
        using (JsonTextWriter jsonTextWriter2 = jsonTextWriter1)
        {
          JsonSerializerFactory.New().Serialize((JsonWriter) jsonTextWriter2, (object) value);
          jsonTextWriter2.Flush();
        }
      }
      memoryStream.Seek(0L, SeekOrigin.Begin);
      return (Stream) memoryStream;
    }
  }
}
