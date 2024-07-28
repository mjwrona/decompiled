// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.JsonCreationConverter`1
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 443E2621-CB19-4319-96B1-AE621A0F5B5B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi
{
  public abstract class JsonCreationConverter<T> : VssSecureCustomCreationConverter<T>
  {
    public override T Create(Type objectType) => throw new InvalidOperationException();

    public abstract T Create(Type objectType, JObject jObject);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      if (reader.TokenType == JsonToken.Null)
        return (object) null;
      JObject jObject = JObject.Load(reader);
      T target = this.Create(objectType, jObject);
      if ((object) target == null)
        throw new JsonSerializationException("No object created.");
      serializer.Populate(jObject.CreateReader(), (object) target);
      return (object) target;
    }
  }
}
