// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.ExpressionValueJsonConverter`1
// Assembly: Microsoft.TeamFoundation.DistributedTask.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9201F3B5-DEAF-44A3-860C-DB7B277BB5C6
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines
{
  internal class ExpressionValueJsonConverter<T> : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType.GetTypeInfo().Equals((Type) typeof (string).GetTypeInfo()) || typeof (T).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.String)
        return (object) new ExpressionValue<T>(serializer.Deserialize<T>(reader));
      string str = (string) reader.Value;
      return ExpressionValue.IsExpression(str) ? (object) ExpressionValue.FromExpression<T>(str) : (object) new ExpressionValue<string>(str);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      ExpressionValue<T> expressionValue = value as ExpressionValue<T>;
      if ((object) expressionValue == null)
        return;
      if (!string.IsNullOrEmpty(expressionValue.Expression))
        serializer.Serialize(writer, (object) ("$[ " + expressionValue.Expression + " ]"));
      else
        serializer.Serialize(writer, (object) expressionValue.Literal);
    }
  }
}
