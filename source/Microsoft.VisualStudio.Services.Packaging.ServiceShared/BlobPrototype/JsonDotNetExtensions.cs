// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.JsonDotNetExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Newtonsoft.Json;
using System.IO;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class JsonDotNetExtensions
  {
    public static string ReadExpectedStringOrNullProperty(
      this JsonReader reader,
      string serializedPropertyName)
    {
      reader.ReadExpectedPropertyNameOnly(serializedPropertyName);
      if (!reader.Read())
        throw new InvalidDataException(Resources.Error_ValueNotFoundForProperty((object) serializedPropertyName));
      return reader.Value?.ToString();
    }

    public static void ReadExpectedPropertyNameOnly(
      this JsonReader reader,
      string serializedPropertyName)
    {
      reader.Read();
      if (reader.TokenType != JsonToken.PropertyName || reader.Value.ToString() != serializedPropertyName)
        throw new InvalidDataException(Resources.Error_PropertyNotFound((object) serializedPropertyName));
    }

    public static string ReadExpectedStringProperty(
      this JsonReader reader,
      string serializedPropertyName)
    {
      return reader.ReadExpectedStringOrNullProperty(serializedPropertyName) ?? throw new InvalidDataException(Resources.Error_ValueCannotBeNull((object) serializedPropertyName));
    }

    public static string ReadPropertyNameOnly(this JsonReader reader)
    {
      reader.Read();
      if (reader.TokenType == JsonToken.EndObject)
        return (string) null;
      return reader.TokenType == JsonToken.PropertyName ? reader.Value.ToString() : throw new InvalidDataException(Resources.Error_TokenNotAProperty((object) reader.TokenType));
    }

    public static string ReadStringOrNull(this JsonReader reader)
    {
      if (!reader.Read())
        throw new InvalidDataException(Resources.Error_ValueNotFound());
      return reader.Value?.ToString();
    }

    public static void MoveToDepth(this JsonReader reader, int depth)
    {
      do
        ;
      while (reader.Depth > depth && reader.Read());
    }
  }
}
