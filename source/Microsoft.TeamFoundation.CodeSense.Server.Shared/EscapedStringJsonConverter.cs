// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.CodeSense.Server.EscapedStringJsonConverter
// Assembly: Microsoft.TeamFoundation.CodeSense.Server.Shared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 548902A5-AE61-4BC7-8D52-315B40AB5900
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.CodeSense.Server.Shared.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.IO;

namespace Microsoft.TeamFoundation.CodeSense.Server
{
  public class EscapedStringJsonConverter : VssSecureJsonConverter
  {
    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      using (StringWriter stringWriter = new StringWriter())
      {
        serializer.Serialize((TextWriter) stringWriter, value);
        writer.WriteValue(stringWriter.ToString());
      }
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      string s = (string) reader.Value;
      if (s == null)
        return (object) null;
      using (StringReader reader1 = new StringReader(s))
        return serializer.Deserialize((TextReader) reader1, objectType);
    }

    public override bool CanConvert(Type objectType) => true;
  }
}
