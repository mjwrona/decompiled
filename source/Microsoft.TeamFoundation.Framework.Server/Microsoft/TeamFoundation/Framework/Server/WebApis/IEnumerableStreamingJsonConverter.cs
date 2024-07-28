// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Framework.Server.WebApis.IEnumerableStreamingJsonConverter
// Assembly: Microsoft.TeamFoundation.Framework.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07453D77-2935-488F-B470-3D01F1673D9B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Framework.Server.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections;

namespace Microsoft.TeamFoundation.Framework.Server.WebApis
{
  public sealed class IEnumerableStreamingJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => typeof (IEnumerable).IsAssignableFrom(objectType);

    public override bool CanRead => false;

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override bool CanWrite => true;

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      if (!(value is IEnumerable enumerable))
        throw new ArgumentOutOfRangeException(nameof (value), "The parameter is not an IEnumerable");
      writer.WriteStartArray();
      foreach (object obj in enumerable)
      {
        serializer.Serialize(writer, obj);
        writer.Flush();
      }
      writer.WriteEndArray();
    }
  }
}
