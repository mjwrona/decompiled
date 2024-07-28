// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.ChunkDedupGCCheckpoint
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using BuildXL.Cache.ContentStore.Hashing;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.BlobStore.Common;
using Microsoft.VisualStudio.Services.Content.Server.Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace Microsoft.VisualStudio.Services.BlobStore.Server
{
  public class ChunkDedupGCCheckpoint
  {
    private Stopwatch CheckpointStopwatch;

    public string CurrentDedupIdentifier { get; set; }

    public int DeleteIfBeforeDays { get; internal set; } = -7;

    public DateTimeOffset DeleteIfBeforeTime => this.MarkTime.AddDays((double) this.DeleteIfBeforeDays);

    public DateTimeOffset MarkTime { get; set; }

    public ChunkDedupGCPhase CurrentPhase { get; internal set; }

    public HashSet<DedupIdentifier> MissingDedups { get; set; }

    public int InvalidKeepUntilDedupsCount { get; set; }

    [JsonIgnore]
    private long MillisecondsBetweenCheckpoints { get; set; }

    [JsonConverter(typeof (DomainIdJsonConverter))]
    public IDomainId DomainId { get; set; }

    public bool IsValidationOnly { get; set; }

    public ChunkDedupGCCheckpoint(
      IDomainId domainId,
      string currentDedupIdentifier,
      DateTimeOffset jobStartTime,
      ChunkDedupGCPhase currentPhase,
      long millisecondsBetweenCheckpoints,
      bool isValidationOnly)
    {
      this.CurrentDedupIdentifier = currentDedupIdentifier;
      this.MarkTime = jobStartTime;
      this.CurrentPhase = currentPhase;
      this.MissingDedups = new HashSet<DedupIdentifier>();
      this.InvalidKeepUntilDedupsCount = 0;
      this.DomainId = domainId;
      this.IsValidationOnly = isValidationOnly;
      this.CheckpointStopwatch = new Stopwatch();
      this.MillisecondsBetweenCheckpoints = millisecondsBetweenCheckpoints;
      this.CheckpointStopwatch.Start();
    }

    public static bool TryGetCheckpoint(
      IVssRequestContext requestContext,
      IDomainId domainId,
      long millisecondsBetweenCheckpoints,
      out ChunkDedupGCCheckpoint data)
    {
      data = (ChunkDedupGCCheckpoint) null;
      if (requestContext.IsFeatureEnabled("BlobStore.Features.EnableChunkDedupGCCheckpoints"))
      {
        string json = requestContext.GetService<ISqlRegistryService>().GetValue(requestContext, (RegistryQuery) string.Format(ServiceRegistryConstants.ChunkDedupGarbageCollectionJobCheckpointFormat, (object) domainId.Serialize()), false, (string) null);
        if (json == null)
        {
          data = (ChunkDedupGCCheckpoint) null;
          return false;
        }
        data = Microsoft.VisualStudio.Services.Content.Common.JsonSerializer.Deserialize<ChunkDedupGCCheckpoint>(json);
        data.MillisecondsBetweenCheckpoints = millisecondsBetweenCheckpoints;
        data.CheckpointStopwatch.Start();
      }
      return data != null;
    }

    public void SetCheckpointDataInRegistry(IVssRequestContext requestContext)
    {
      if (!requestContext.IsFeatureEnabled("BlobStore.Features.EnableChunkDedupGCCheckpoints"))
        return;
      requestContext.GetService<ISqlRegistryService>().SetValue<string>(requestContext, string.Format(ServiceRegistryConstants.ChunkDedupGarbageCollectionJobCheckpointFormat, (object) this.DomainId.Serialize()), Microsoft.VisualStudio.Services.Content.Common.JsonSerializer.Serialize<ChunkDedupGCCheckpoint>(this));
    }

    public void ClearCheckpointDataFromRegistry(IVssRequestContext requestContext)
    {
      requestContext.GetService<ISqlRegistryService>().DeleteEntries(requestContext, string.Format(ServiceRegistryConstants.ChunkDedupGarbageCollectionJobCheckpointFormat, (object) this.DomainId.Serialize()));
      this.CheckpointStopwatch.Stop();
    }

    public bool TrySetNextPhase()
    {
      switch (this.CurrentPhase)
      {
        case ChunkDedupGCPhase.Mark:
          this.SetChunkDedupGCPhase(ChunkDedupGCPhase.Delete);
          return true;
        case ChunkDedupGCPhase.Validate:
          return false;
        case ChunkDedupGCPhase.Delete:
          this.SetChunkDedupGCPhase(ChunkDedupGCPhase.Validate);
          return true;
        default:
          return false;
      }
    }

    public void SetChunkDedupGCPhase(ChunkDedupGCPhase phase)
    {
      this.CurrentPhase = phase;
      this.CurrentDedupIdentifier = (string) null;
    }

    private bool ShouldCheckpoint() => this.MillisecondsBetweenCheckpoints >= 0L && this.CheckpointStopwatch.ElapsedMilliseconds >= this.MillisecondsBetweenCheckpoints;

    public void SaveCheckpoint(IVssRequestContext requestContext, string currentDedupIdentifier)
    {
      using (requestContext.Enter(ContentTracePoints.DedupStoreService.SaveCheckpointCall, nameof (SaveCheckpoint)))
      {
        if (!this.ShouldCheckpoint())
          return;
        this.CurrentDedupIdentifier = currentDedupIdentifier;
        this.SetCheckpointDataInRegistry(requestContext);
        this.CheckpointStopwatch.Restart();
      }
    }
  }
}
