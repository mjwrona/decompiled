// Decompiled with JetBrains decompiler
// Type: Microsoft.AspNet.OData.Query.Expressions.DynamicTypeWrapperConverter
// Assembly: Microsoft.TeamFoundation.AspNet.OData, Version=7.3.1.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 08758328-0988-4E6C-88EC-9BA90EA42587
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.AspNet.OData.dll

using Microsoft.AspNet.OData.Common;
using Newtonsoft.Json;
using System;

namespace Microsoft.AspNet.OData.Query.Expressions
{
  internal class DynamicTypeWrapperConverter : JsonConverter
  {
    public override bool CanConvert(Type objectType) => !(objectType == (Type) null) ? objectType.IsAssignableFrom(typeof (DynamicTypeWrapper)) : throw Error.ArgumentNull(nameof (objectType));

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      if (!(value is DynamicTypeWrapper dynamicTypeWrapper))
        return;
      serializer.Serialize(writer, (object) dynamicTypeWrapper.Values);
    }
  }
}
