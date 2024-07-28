// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.WebApi.ValidationItemJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.WebApi
{
  internal sealed class ValidationItemJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (ValidationItem).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override bool CanWrite => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      object target = (object) null;
      if (objectType == typeof (ExpressionValidationItem))
        target = (object) new ExpressionValidationItem();
      else if (objectType == typeof (InputValidationItem))
        target = (object) new InputValidationItem();
      JObject jobject = JObject.Load(reader);
      if (target == null)
      {
        if (!(serializer.ContractResolver.ResolveContract(objectType) is JsonObjectContract jsonObjectContract))
          return existingValue;
        JsonProperty closestMatchProperty = jsonObjectContract.Properties.GetClosestMatchProperty("Type");
        if (closestMatchProperty == null)
          return existingValue;
        string str = "expression";
        JToken jtoken;
        if (jobject.TryGetValue(closestMatchProperty.PropertyName, out jtoken) && jtoken.Type == JTokenType.String)
          str = (string) jtoken;
        switch (str)
        {
          case "expression":
            target = (object) new ExpressionValidationItem();
            break;
          case "input":
            target = (object) new InputValidationItem();
            break;
        }
      }
      if (jobject != null)
      {
        using (JsonReader reader1 = jobject.CreateReader())
          serializer.Populate(reader1, target);
      }
      return target;
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotSupportedException();
  }
}
