// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Core.WebApi.IdentityRefOrJValueConverter
// Assembly: Microsoft.TeamFoundation.Core.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 3A82A796-05AB-42F0-97D0-CB8516E08665
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Core.WebApi.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Core.WebApi
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  public class IdentityRefOrJValueConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (object);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JToken jtoken = JToken.Load(reader);
      return jtoken.Type == JTokenType.Object ? (object) jtoken.ToObject<IdentityRef>(serializer) : ((JValue) jtoken).Value;
    }

    public override bool CanWrite => false;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer) => throw new NotImplementedException();
  }
}
