// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens.TemplateTokenJsonConverter
// Assembly: Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DC134C9-663D-46C7-A414-3ADCC50BB112
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Reflection;

namespace Microsoft.TeamFoundation.DistributedTask.Pipelines.Yaml.ObjectTemplating.Tokens
{
  internal sealed class TemplateTokenJsonConverter : VssSecureJsonConverter
  {
    public override bool CanWrite => false;

    public override bool CanConvert(Type objectType) => typeof (TemplateToken).GetTypeInfo().IsAssignableFrom(objectType.GetTypeInfo());

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType != JsonToken.StartObject)
        return (object) null;
      int? nullable = new int?();
      JObject jobject = JObject.Load(reader);
      JToken jtoken;
      if (!jobject.TryGetValue("type", StringComparison.OrdinalIgnoreCase, out jtoken))
      {
        nullable = new int?(0);
      }
      else
      {
        if (jtoken.Type != JTokenType.Integer)
          return existingValue;
        nullable = new int?((int) jtoken);
      }
      object target = (object) null;
      if (nullable.HasValue)
      {
        switch (nullable.GetValueOrDefault())
        {
          case 0:
            target = (object) new LiteralToken(new int?(), new int?(), new int?(), (string) null);
            break;
          case 1:
            target = (object) new SequenceToken(new int?(), new int?(), new int?());
            break;
          case 2:
            target = (object) new MappingToken(new int?(), new int?(), new int?());
            break;
          case 3:
            target = (object) new BasicExpressionToken(new int?(), new int?(), new int?(), (string) null);
            break;
          case 4:
            target = (object) new InsertExpressionToken(new int?(), new int?(), new int?());
            break;
          case 5:
            target = (object) new IfExpressionToken(new int?(), new int?(), new int?(), (string) null);
            break;
          case 6:
            target = (object) new ElseIfExpressionToken(new int?(), new int?(), new int?(), (string) null);
            break;
          case 7:
            target = (object) new ElseExpressionToken(new int?(), new int?(), new int?());
            break;
          case 8:
            target = (object) new EachExpressionToken(new int?(), new int?(), new int?(), (string) null, (string) null);
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
