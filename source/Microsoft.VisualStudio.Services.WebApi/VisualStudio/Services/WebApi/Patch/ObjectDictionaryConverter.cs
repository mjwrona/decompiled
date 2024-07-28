// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.WebApi.Patch.ObjectDictionaryConverter
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.WebApi.Patch
{
  internal class ObjectDictionaryConverter : CustomCreationConverter<IDictionary<string, object>>
  {
    public override IDictionary<string, object> Create(Type objectType) => (IDictionary<string, object>) new Dictionary<string, object>();

    public override bool CanConvert(Type objectType) => objectType == typeof (object) || base.CanConvert(objectType);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return reader.TokenType == JsonToken.StartObject || reader.TokenType == JsonToken.Null ? base.ReadJson(reader, objectType, existingValue, serializer) : serializer.Deserialize(reader);
    }
  }
}
