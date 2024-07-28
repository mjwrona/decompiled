// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Sha256IdJsonSerializer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class Sha256IdJsonSerializer : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (Sha256Id);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.String)
        throw this.MakeException(reader);
      Sha256Id result;
      if (!Sha256Id.TryParse((string) reader.Value, out result))
        throw this.MakeException(reader);
      return (object) result;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      Sha256Id sha256Id = (Sha256Id) value;
      writer.WriteValue(sha256Id.ToString());
    }

    private JsonReaderException MakeException(JsonReader reader) => !(reader is JsonTextReader jsonTextReader) ? new JsonReaderException(Resources.Format("Sha256IdMustBeStringNoLine", (object) reader.Path)) : new JsonReaderException(Resources.Format("Sha256IdMustBeString", (object) reader.Path, (object) jsonTextReader.LineNumber, (object) jsonTextReader.LinePosition));
  }
}
