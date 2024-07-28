// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.JsonConverterJsSerializerWithCallbacks`1
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class JsonConverterJsSerializerWithCallbacks<T> : JavaScriptConverter
  {
    private JsonDeserializationJSCallback<T> m_deserializeCallback;
    private JsonSerializationJSCallback<T> m_serializeCallback;

    public JsonConverterJsSerializerWithCallbacks(
      JsonDeserializationJSCallback<T> deserializeCallback)
      : this(deserializeCallback, JsonConverterJsSerializerWithCallbacks<T>.\u003C\u003EO.\u003C0\u003E__SerializeNotImplemented ?? (JsonConverterJsSerializerWithCallbacks<T>.\u003C\u003EO.\u003C0\u003E__SerializeNotImplemented = new JsonSerializationJSCallback<T>(JsonConverterJsSerializerWithCallbacks<T>.SerializeNotImplemented)))
    {
      // ISSUE: reference to a compiler-generated field (out of statement scope)
      // ISSUE: reference to a compiler-generated field (out of statement scope)
    }

    public JsonConverterJsSerializerWithCallbacks(
      JsonDeserializationJSCallback<T> deserializeCallback,
      JsonSerializationJSCallback<T> serializeCallback)
    {
      ArgumentUtility.CheckForNull<JsonDeserializationJSCallback<T>>(deserializeCallback, nameof (deserializeCallback));
      ArgumentUtility.CheckForNull<JsonSerializationJSCallback<T>>(serializeCallback, nameof (serializeCallback));
      this.m_deserializeCallback = deserializeCallback;
      this.m_serializeCallback = serializeCallback;
    }

    public override object Deserialize(
      IDictionary<string, object> dictionary,
      Type type,
      JavaScriptSerializer serializer)
    {
      return (object) this.m_deserializeCallback(dictionary, serializer);
    }

    public override IDictionary<string, object> Serialize(
      object obj,
      JavaScriptSerializer serializer)
    {
      return this.m_serializeCallback((T) obj, serializer);
    }

    private static IDictionary<string, object> SerializeNotImplemented(
      T obj,
      JavaScriptSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override IEnumerable<Type> SupportedTypes => (IEnumerable<Type>) new Type[1]
    {
      typeof (T)
    };
  }
}
