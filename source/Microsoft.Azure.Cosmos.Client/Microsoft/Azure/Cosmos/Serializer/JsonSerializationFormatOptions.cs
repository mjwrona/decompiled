// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Cosmos.Serializer.JsonSerializationFormatOptions
// Assembly: Microsoft.Azure.Cosmos.Client, Version=3.31.2.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35
// MVID: 16FBD598-821A-4D2D-8F97-7046A72AA497
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Cosmos.Client.dll

using Microsoft.Azure.Cosmos.Json;
using System;

namespace Microsoft.Azure.Cosmos.Serializer
{
  internal abstract class JsonSerializationFormatOptions
  {
    protected JsonSerializationFormatOptions(JsonSerializationFormat jsonSerializationFormat) => this.JsonSerializationFormat = jsonSerializationFormat;

    public JsonSerializationFormat JsonSerializationFormat { get; }

    public static JsonSerializationFormatOptions Create(
      JsonSerializationFormat jsonSerializationFormat)
    {
      return (JsonSerializationFormatOptions) new JsonSerializationFormatOptions.NativelySupportedJsonSerializationFormatOptions(jsonSerializationFormat);
    }

    public static JsonSerializationFormatOptions Create(
      JsonSerializationFormat jsonSerializationFormat,
      JsonSerializationFormatOptions.CreateNavigator createNavigator)
    {
      return (JsonSerializationFormatOptions) new JsonSerializationFormatOptions.CustomJsonSerializationFormatOptions(jsonSerializationFormat, createNavigator);
    }

    public delegate IJsonNavigator CreateNavigator(ReadOnlyMemory<byte> content);

    public sealed class NativelySupportedJsonSerializationFormatOptions : 
      JsonSerializationFormatOptions
    {
      public NativelySupportedJsonSerializationFormatOptions(
        JsonSerializationFormat jsonSerializationFormat)
        : base(jsonSerializationFormat)
      {
      }
    }

    public sealed class CustomJsonSerializationFormatOptions : JsonSerializationFormatOptions
    {
      public CustomJsonSerializationFormatOptions(
        JsonSerializationFormat jsonSerializationFormat,
        JsonSerializationFormatOptions.CreateNavigator createNavigator)
        : base(jsonSerializationFormat)
      {
        this.createNavigator = createNavigator ?? throw new ArgumentNullException(nameof (jsonSerializationFormat));
      }

      public JsonSerializationFormatOptions.CreateNavigator createNavigator { get; }
    }
  }
}
