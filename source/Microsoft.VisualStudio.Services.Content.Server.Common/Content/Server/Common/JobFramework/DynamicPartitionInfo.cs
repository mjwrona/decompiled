// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework.DynamicPartitionInfo
// Assembly: Microsoft.VisualStudio.Services.Content.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 203E0171-FB50-4FDE-9B1F-EFC6366423BC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Content.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.Content.Server.Common.JobFramework
{
  [Serializable]
  public class DynamicPartitionInfo
  {
    public Guid HostId { get; set; }

    public Guid JobId { get; set; }

    public int CurrentPartitionNumber { get; set; }

    public TimeSpan CurrentAvgExecutionTime { get; set; }

    public int NumberofSamples { get; set; }

    public int NumberOfSamplesExceedingAvgTime { get; set; }

    public TimeSpan ExecutionThreshold { get; set; }

    public long SuggestedPartitionNumber { get; set; }

    public TimeSpan EstimatedReducedExecutionTime { get; set; }

    public bool ActionSuggested { get; set; }
  }
}
