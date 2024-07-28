// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.DedupCleanupJobInfo
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D3AB5C9B-EB54-4477-A304-63BB297414A3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.dll

using Newtonsoft.Json;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  [JsonObject]
  [Serializable]
  public struct DedupCleanupJobInfo
  {
    public string DeletionStageRequested;
    public string ExpiryTimeMark;
    public StorageRetentionInfo RetentionPolicy;
    public long RootsFound;
    public long TotalVisited;
    public long BloomBitsSet;
    public ulong BloomBitCount;
    public TimeSpan? MarkCompleteTime;
    public long BloomHits;
    public long BloomMisses;
    public TimeSpan? SweepCompleteTime;
    public long TotalScanned;
    public long TotalRequiringDelete;
    public long TotalSuccessfullyDeleted;
    public bool JobComplete;
    public string Exception;
  }
}
