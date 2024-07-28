// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.WebApi.TypeStringLookupConverter
// Assembly: Microsoft.VisualStudio.Services.Notifications.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FF217E0A-7730-437B-BE9F-877363CB7392
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Notifications.WebApi
{
  public abstract class TypeStringLookupConverter : VssSecureJsonConverter
  {
    protected abstract Dictionary<string, Type> TypeMap { get; }

    protected abstract Type BaseType { get; }

    protected abstract string TypeFieldName { get; }

    protected abstract object CreateUnsupportedTypeObject(string typeName);

    protected virtual bool LimitConvertToBaseType => false;

    public override bool CanConvert(Type objectType)
    {
      bool flag = objectType.Equals(this.BaseType);
      if (!flag && !this.LimitConvertToBaseType)
        flag = this.TypeMap.Values.Contains<Type>(objectType);
      return flag;
    }

    public override bool CanWrite => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      object obj = (object) null;
      if (reader.TokenType == JsonToken.StartObject)
      {
        string str = (string) null;
        JObject jobject = JObject.Load(reader);
        JToken jtoken = jobject[this.TypeFieldName];
        if (jtoken != null)
        {
          str = jtoken.Value<string>();
          Type objectType1;
          if (!string.IsNullOrEmpty(str) && this.TypeMap.TryGetValue(str, out objectType1))
            obj = jobject.ToObject(objectType1);
        }
        if (obj == null)
          obj = this.CreateUnsupportedTypeObject(str);
      }
      return obj;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException(nameof (WriteJson));
  }
}
