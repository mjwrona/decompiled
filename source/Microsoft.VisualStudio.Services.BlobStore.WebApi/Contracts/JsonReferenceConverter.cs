// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts.JsonReferenceConverter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi.Contracts
{
  public class JsonReferenceConverter : CustomCreationConverter<Reference>
  {
    public override Reference Create(Type objectType) => throw new NotImplementedException();

    public override object ReadJson(
      JsonReader reader,
      Type objectType,
      object existingValue,
      JsonSerializer serializer)
    {
      JObject jobject = JObject.Load(reader);
      if (jobject.Property("id") != null)
        return (object) serializer.Deserialize<IdReference>(jobject.CreateReader());
      return jobject.Property("keepUntil") != null ? (object) serializer.Deserialize<KeepUntilReference>(jobject.CreateReader()) : throw new ArgumentException("Malformed blob reference JSON object");
    }
  }
}
