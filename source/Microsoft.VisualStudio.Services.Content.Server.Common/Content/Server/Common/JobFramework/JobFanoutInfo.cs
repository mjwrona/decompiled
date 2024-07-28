// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework.JobFanoutInfo
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework
{
  [Serializable]
  public class JobFanoutInfo
  {
    public Guid HostId { get; set; }

    public Guid JobId { get; set; }

    public TimeSpan TargetThreshold { get; set; }

    public int NumberOfSamples { get; set; }

    public int SuccessfulCountExceedingTargetThresholdTime { get; set; }

    public int CurrentTotalPartitions { get; set; }

    public int RecommendedTotalPartitions { get; set; } = -1;

    public bool IsScaleOutRecommended => this.CurrentTotalPartitions < this.RecommendedTotalPartitions && this.RecommendedTotalPartitions != -1;
  }
}
