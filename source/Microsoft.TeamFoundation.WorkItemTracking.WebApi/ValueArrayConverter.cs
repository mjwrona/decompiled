// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebApi.ValueArrayConverter
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3FA6C797-B300-46B2-A8C9-CFED891348F5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebApi
{
  public class ValueArrayConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (object[]);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JArray jarray = JArray.Load(reader);
      List<object> objectList = new List<object>();
      foreach (JToken jtoken in jarray)
      {
        object obj = jtoken.Type != JTokenType.Object ? ((JValue) jtoken).Value : (object) jtoken.ToObject<IdentityRef>(serializer);
        objectList.Add(obj);
      }
      return (object) objectList.ToArray();
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
