// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupStoreClientContextConverter
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  public class DedupStoreClientContextConverter : JsonConverter<DedupStoreClientContext>
  {
    public override bool CanWrite => false;

    public override void WriteJson(
      JsonWriter writer,
      DedupStoreClientContext value,
      JsonSerializer serializer)
    {
      throw new NotImplementedException();
    }

    public override DedupStoreClientContext ReadJson(
      JsonReader reader,
      Type objectType,
      DedupStoreClientContext existingValue,
      bool hasExistingValue,
      JsonSerializer serializer)
    {
      JObject jobject = JObject.Load(reader);
      JToken jtoken1 = jobject["MaxParallelismCount"];
      int? maxParallelism = jtoken1 != null ? new int?(jtoken1.Value<int>()) : new int?();
      JToken jtoken2 = jobject["DownloadPageSize"];
      int? downloadPageSize = jtoken2 != null ? new int?(jtoken2.Value<int>()) : new int?();
      JToken jtoken3 = jobject["WriterCapacity"];
      int? writerCapacity = jtoken3 != null ? new int?(jtoken3.Value<int>()) : new int?();
      JToken jtoken4 = jobject["DownloadCapacity"];
      int? downloadCapacity = jtoken4 != null ? new int?(jtoken4.Value<int>()) : new int?();
      JToken jtoken5 = jobject["EnsureUriOrdered"];
      bool? ensureUriOrdered = jtoken5 != null ? jtoken5.Value<bool?>() : new bool?();
      JToken jtoken6 = jobject["ChunkValidationLevel"];
      ChunkValidationLevel? chunkValidationLevel = new ChunkValidationLevel?((ChunkValidationLevel) (jtoken6 != null ? new int?(jtoken6.Value<int>()) : new int?()).Value);
      JToken jtoken7 = jobject["ChunkCacheSizeInMegabytes"];
      int? chunkCacheSizeInMegabytes = jtoken7 != null ? new int?(jtoken7.Value<int>()) : new int?();
      bool? disableHardLinks = new bool?();
      return new DedupStoreClientContext(maxParallelism, downloadPageSize, writerCapacity, downloadCapacity, ensureUriOrdered, chunkValidationLevel, chunkCacheSizeInMegabytes, disableHardLinks);
    }
  }
}
