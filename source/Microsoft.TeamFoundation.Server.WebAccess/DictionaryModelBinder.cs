// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.DictionaryModelBinder
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Newtonsoft.Json;
using System.Collections.Generic;
using System.Web.Script.Serialization;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class DictionaryModelBinder : JsonModelBinder
  {
    public override JsonConverter[] GetConverters() => (JsonConverter[]) new JsonConverterWithCallbacks<IDictionary<string, object>>[1]
    {
      new JsonConverterWithCallbacks<IDictionary<string, object>>(DictionaryModelBinder.\u003C\u003EO.\u003C0\u003E__DeserializeJsonDictionary ?? (DictionaryModelBinder.\u003C\u003EO.\u003C0\u003E__DeserializeJsonDictionary = new JsonDeserializationCallback<IDictionary<string, object>>(DictionaryModelBinder.DeserializeJsonDictionary)))
    };

    public static IDictionary<string, object> DeserializeJsonDictionary(
      IDictionary<string, object> dictionary)
    {
      return dictionary;
    }

    public override JavaScriptConverter[] GetJsConverters() => (JavaScriptConverter[]) new JsonConverterJsSerializerWithCallbacks<IDictionary<string, object>>[1]
    {
      new JsonConverterJsSerializerWithCallbacks<IDictionary<string, object>>(DictionaryModelBinder.\u003C\u003EO.\u003C1\u003E__DeserializeJsonDictionaryJsSerializer ?? (DictionaryModelBinder.\u003C\u003EO.\u003C1\u003E__DeserializeJsonDictionaryJsSerializer = new JsonDeserializationJSCallback<IDictionary<string, object>>(DictionaryModelBinder.DeserializeJsonDictionaryJsSerializer)))
    };

    public static IDictionary<string, object> DeserializeJsonDictionaryJsSerializer(
      IDictionary<string, object> dictionary,
      JavaScriptSerializer serializer)
    {
      return dictionary;
    }
  }
}
