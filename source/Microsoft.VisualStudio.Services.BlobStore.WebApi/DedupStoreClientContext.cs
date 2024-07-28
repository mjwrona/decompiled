// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.WebApi.DedupStoreClientContext
// Assembly: Microsoft.VisualStudio.Services.BlobStore.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4500AC57-FBCC-4F18-B11F-F661A75E4A46
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.WebApi.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.WebApi
{
  [JsonConverter(typeof (DedupStoreClientContextConverter))]
  public class DedupStoreClientContext
  {
    public const int DefaultMaxParallelism = 64;
    public const int DefaultChunkCacheSizeInMegabytes = 1024;

    public int ChunkCacheSizeInMegabytes { get; set; }

    public int MaxParallelismCount { get; }

    public int DownloadPageSize { get; }

    public int WriterCapacity { get; }

    public int DownloadCapacity { get; }

    public bool? EnsureUriOrdered { get; }

    public bool DisableHardLinks { get; }

    public ChunkValidationLevel ChunkValidationLevel { get; }

    public DedupStoreClientContext(
      int? maxParallelism = null,
      int? downloadPageSize = null,
      int? writerCapacity = null,
      int? downloadCapacity = null,
      bool? ensureUriOrdered = null,
      ChunkValidationLevel? chunkValidationLevel = null,
      int? chunkCacheSizeInMegabytes = null,
      bool? disableHardLinks = null)
    {
      this.MaxParallelismCount = maxParallelism ?? 64;
      int? nullable = downloadPageSize;
      this.DownloadPageSize = nullable ?? 1000;
      nullable = writerCapacity;
      this.WriterCapacity = nullable ?? 1;
      nullable = downloadCapacity;
      this.DownloadCapacity = nullable ?? 1;
      this.EnsureUriOrdered = ensureUriOrdered;
      this.ChunkValidationLevel = (ChunkValidationLevel) ((int) chunkValidationLevel ?? (int) DedupStoreClientContext.GetDefaultVerificationLevel());
      nullable = chunkCacheSizeInMegabytes;
      this.ChunkCacheSizeInMegabytes = nullable ?? DedupStoreClientContext.GetDefaultChunkCacheSize();
      this.DisableHardLinks = ((int) disableHardLinks ?? (DedupStoreClientContext.GetDefaultDisableHardLinks() ? 1 : 0)) != 0;
    }

    public static ChunkValidationLevel GetDefaultVerificationLevel()
    {
      ChunkValidationLevel result = ChunkValidationLevel.Default;
      if (Enum.TryParse<ChunkValidationLevel>(Environment.GetEnvironmentVariable("VSO_DROP_CHUNK_VALIDATION_LEVEL"), out result) && !Enum.IsDefined(typeof (ChunkValidationLevel), (object) result))
        result = ChunkValidationLevel.Default;
      return result;
    }

    public static int GetDefaultChunkCacheSize()
    {
      string environmentVariable = Environment.GetEnvironmentVariable("VSO_DROP_CHUNK_CACHE_SIZE_IN_MB");
      int result;
      return !string.IsNullOrWhiteSpace(environmentVariable) && int.TryParse(environmentVariable, out result) ? result : 1024;
    }

    public static bool GetDefaultDisableHardLinks()
    {
      string environmentVariable = Environment.GetEnvironmentVariable("VSO_DROP_DISABLE_HARDLINKS");
      bool result;
      return !string.IsNullOrWhiteSpace(environmentVariable) && bool.TryParse(environmentVariable, out result) && result;
    }
  }
}
