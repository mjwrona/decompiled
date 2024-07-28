// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Common.DomainIdJsonConverter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FAFB0281-5CF2-4D3F-992C-49FBB9BEC906
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.BlobStore.Common.dll

using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Common
{
  public class DomainIdJsonConverter : VssSecureJsonConverter
  {
    public override bool CanConvert(Type objectType) => objectType == typeof (IDomainId);

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      return (object) DomainIdFactory.Create((string) reader.Value);
    }

    public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
    {
      base.WriteJson(writer, value, serializer);
      writer.WriteValue(((IDomainId) value).Serialize());
    }
  }
}
