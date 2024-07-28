// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution.JsonIPv4SubnetConverter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.TeamFoundation.Framework.Server.HttpModule.IpResolution
{
  internal sealed class JsonIPv4SubnetConverter : JsonConverter<IPv4Subnet>
  {
    public override IPv4Subnet ReadJson(
      JsonReader reader,
      Type objectType,
      IPv4Subnet existingValue,
      bool hasExistingValue,
      JsonSerializer serializer)
    {
      return reader != null && reader.TokenType == JsonToken.String ? IPv4Subnet.Parse(reader.Value.ToString()) : throw new JsonException(string.Format("Unexpected token type {0} during parsing of type IPv4Subnet", (object) (JsonToken) (reader != null ? (int) reader.TokenType : 0)));
    }

    public override void WriteJson(JsonWriter writer, IPv4Subnet value, JsonSerializer serializer)
    {
      if (writer == null)
        return;
      if (value == null)
        writer.WriteNull();
      else
        writer.WriteValue(value.ToString());
    }
  }
}
