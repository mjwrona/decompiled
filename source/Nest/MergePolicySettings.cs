// Decompiled with JetBrains decompiler
// Type: Nest.MergePolicySettings
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class MergePolicySettings : IMergePolicySettings
  {
    public int? ExpungeDeletesAllowed { get; set; }

    public string FloorSegment { get; set; }

    public int? MaxMergeAtOnce { get; set; }

    public int? MaxMergeAtOnceExplicit { get; set; }

    public string MaxMergedSegment { get; set; }

    public double? ReclaimDeletesWeight { get; set; }

    public int? SegmentsPerTier { get; set; }
  }
}
