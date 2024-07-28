// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.JsonConverterWithCallbacks`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.VisualStudio.Services.Common;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class JsonConverterWithCallbacks<T> : JsonConverter
  {
    private readonly JsonDeserializationCallback<T> m_deserializeCallback;
    private readonly JsonSerializationCallback<T> m_serializeCallback;

    public JsonConverterWithCallbacks(JsonDeserializationCallback<T> deserializeCallback)
      : this(deserializeCallback, JsonConverterWithCallbacks<T>.\u003C\u003EO.\u003C0\u003E__SerializeNotImplemented ?? (JsonConverterWithCallbacks<T>.\u003C\u003EO.\u003C0\u003E__SerializeNotImplemented = new JsonSerializationCallback<T>(JsonConverterWithCallbacks<T>.SerializeNotImplemented)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public JsonConverterWithCallbacks(
      JsonDeserializationCallback<T> deserializeCallback,
      JsonSerializationCallback<T> serializeCallback)
    {
      ArgumentUtility.CheckForNull<JsonDeserializationCallback<T>>(deserializeCallback, nameof (deserializeCallback));
      ArgumentUtility.CheckForNull<JsonSerializationCallback<T>>(serializeCallback, nameof (serializeCallback));
      this.m_deserializeCallback = deserializeCallback;
      this.m_serializeCallback = serializeCallback;
    }

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object hasExistingValue,
      JsonSerializer serializer)
    {
      return (object) this.m_deserializeCallback((IDictionary<string, object>) JObject.Load(reader).ToObject<Dictionary<string, object>>());
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => JObject.FromObject((object) this.m_serializeCallback((T) value)).WriteTo(writer);

    private static IDictionary<string, object> SerializeNotImplemented(T obj) => throw new NotImplementedException();

    public override bool CanConvert(Type objectType) => objectType == typeof (T);
  }
}
