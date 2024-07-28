// Decompiled with JetBrains decompiler
// Type: Nest.IMergePolicySettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public interface IMergePolicySettings
  {
    int? ExpungeDeletesAllowed { get; set; }

    string FloorSegment { get; set; }

    int? MaxMergeAtOnce { get; set; }

    int? MaxMergeAtOnceExplicit { get; set; }

    string MaxMergedSegment { get; set; }

    double? ReclaimDeletesWeight { get; set; }

    int? SegmentsPerTier { get; set; }
  }
}
