// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.CosmosJsonDotNetSerializer
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.IO;
using System.Text;

namespace Microsoft.Azure.Cosmos
{
  internal sealed class CosmosJsonDotNetSerializer : CosmosSerializer
  {
    private static readonly Encoding DefaultEncoding = (Encoding) new UTF8Encoding(false, true);
    private readonly JsonSerializerSettings SerializerSettings;

    internal CosmosJsonDotNetSerializer() => this.SerializerSettings = (JsonSerializerSettings) null;

    internal CosmosJsonDotNetSerializer(CosmosSerializationOptions cosmosSerializerOptions)
    {
      if (cosmosSerializerOptions == null)
        this.SerializerSettings = (JsonSerializerSettings) null;
      else
        this.SerializerSettings = new JsonSerializerSettings()
        {
          NullValueHandling = cosmosSerializerOptions.IgnoreNullValues ? NullValueHandling.Ignore : NullValueHandling.Include,
          Formatting = cosmosSerializerOptions.Indented ? Formatting.Indented : Formatting.None,
          ContractResolver = cosmosSerializerOptions.PropertyNamingPolicy == CosmosPropertyNamingPolicy.CamelCase ? (IContractResolver) new CamelCasePropertyNamesContractResolver() : (IContractResolver) null,
          MaxDepth = new int?(64)
        };
    }

    internal CosmosJsonDotNetSerializer(JsonSerializerSettings jsonSerializerSettings) => this.SerializerSettings = jsonSerializerSettings ?? throw new ArgumentNullException(nameof (jsonSerializerSettings));

    public override T FromStream<T>(Stream stream)
    {
      using (stream)
      {
        if (typeof (Stream).IsAssignableFrom(typeof (T)))
          return (T) stream;
        using (StreamReader reader1 = new StreamReader(stream))
        {
          using (JsonTextReader reader2 = new JsonTextReader((TextReader) reader1))
            return this.GetSerializer().Deserialize<T>((JsonReader) reader2);
        }
      }
    }

    public override Stream ToStream<T>(T input)
    {
      MemoryStream stream = new MemoryStream();
      using (StreamWriter streamWriter = new StreamWriter((Stream) stream, CosmosJsonDotNetSerializer.DefaultEncoding, 1024, true))
      {
        using (JsonWriter jsonWriter = (JsonWriter) new JsonTextWriter((TextWriter) streamWriter))
        {
          jsonWriter.Formatting = Formatting.None;
          this.GetSerializer().Serialize(jsonWriter, (object) input);
          jsonWriter.Flush();
          streamWriter.Flush();
        }
      }
      stream.Position = 0L;
      return (Stream) stream;
    }

    private JsonSerializer GetSerializer() => JsonSerializer.Create(this.SerializerSettings);
  }
}
