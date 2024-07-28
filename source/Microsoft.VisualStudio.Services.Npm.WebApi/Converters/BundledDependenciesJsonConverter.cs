// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Npm.WebApi.Converters.BundledDependenciesJsonConverter
// Assembly: Microsoft.VisualStudio.Services.Npm.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 639B57A1-1338-429F-9659-38C0A0394E05
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Npm.WebApi.dll

using Microsoft.VisualStudio.Services.Npm.WebApi.Types;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Npm.WebApi.Converters
{
  public class BundledDependenciesJsonConverter : JsonConverter<BundledDependencies>
  {
    public override BundledDependencies ReadJson(
      JsonReader reader,
      Type objectType,
      BundledDependencies existingValue,
      bool hasExistingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (BundledDependencies) null;
      if (reader.TokenType != JsonToken.Boolean)
        return new BundledDependencies((IReadOnlyList<string>) serializer.Deserialize<string[]>(reader));
      return !(bool) reader.Value ? BundledDependencies.None : BundledDependencies.All;
    }

    public override void WriteJson(
      JsonWriter writer,
      BundledDependencies value,
      JsonSerializer serializer)
    {
      if (value == null)
        writer.WriteNull();
      else if (value.BundleAllDependencies)
        writer.WriteValue(true);
      else if (!value.List.Any<string>())
        writer.WriteValue(false);
      else
        serializer.Serialize(writer, (object) value.List);
    }
  }
}
