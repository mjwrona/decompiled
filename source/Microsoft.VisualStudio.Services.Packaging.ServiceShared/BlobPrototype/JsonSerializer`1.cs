// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JsonSerializer`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class JsonSerializer<T> : ISerializer<T>
  {
    private readonly JsonSerializerSettings settings;

    public JsonSerializer(JsonSerializerSettings settings) => this.settings = settings;

    public T Deserialize(Stream stream)
    {
      stream.Position = 0L;
      using (StreamReader reader1 = new StreamReader(stream, Encoding.UTF8))
      {
        using (JsonReader reader2 = (JsonReader) new JsonTextReader((TextReader) reader1))
          return JsonSerializer.Create(this.settings).Deserialize<T>(reader2);
      }
    }

    public byte[] Serialize(T input) => Encoding.UTF8.GetBytes(JsonConvert.SerializeObject((object) input, this.settings));
  }
}
