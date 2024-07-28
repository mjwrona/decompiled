// Decompiled with JetBrains decompiler
// Type: Nest.MergePolicySettingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MergePolicySettingsDescriptor : 
    DescriptorBase<MergePolicySettingsDescriptor, IMergePolicySettings>,
    IMergePolicySettings
  {
    int? IMergePolicySettings.ExpungeDeletesAllowed { get; set; }

    string IMergePolicySettings.FloorSegment { get; set; }

    int? IMergePolicySettings.MaxMergeAtOnce { get; set; }

    int? IMergePolicySettings.MaxMergeAtOnceExplicit { get; set; }

    string IMergePolicySettings.MaxMergedSegment { get; set; }

    double? IMergePolicySettings.ReclaimDeletesWeight { get; set; }

    int? IMergePolicySettings.SegmentsPerTier { get; set; }

    public MergePolicySettingsDescriptor ExpungeDeletesAllowed(int? allowed) => this.Assign<int?>(allowed, (Action<IMergePolicySettings, int?>) ((a, v) => a.ExpungeDeletesAllowed = v));

    public MergePolicySettingsDescriptor FloorSegment(string floorSegment) => this.Assign<string>(floorSegment, (Action<IMergePolicySettings, string>) ((a, v) => a.FloorSegment = v));

    public MergePolicySettingsDescriptor MaxMergeAtOnce(int? maxMergeAtOnce) => this.Assign<int?>(maxMergeAtOnce, (Action<IMergePolicySettings, int?>) ((a, v) => a.MaxMergeAtOnce = v));

    public MergePolicySettingsDescriptor MaxMergeAtOnceExplicit(int? maxMergeOnceAtOnceExplicit) => this.Assign<int?>(maxMergeOnceAtOnceExplicit, (Action<IMergePolicySettings, int?>) ((a, v) => a.MaxMergeAtOnceExplicit = v));

    public MergePolicySettingsDescriptor MaxMergedSegement(string maxMergedSegment) => this.Assign<string>(maxMergedSegment, (Action<IMergePolicySettings, string>) ((a, v) => a.MaxMergedSegment = v));

    public MergePolicySettingsDescriptor ReclaimDeletesWeight(double? weight) => this.Assign<double?>(weight, (Action<IMergePolicySettings, double?>) ((a, v) => a.ReclaimDeletesWeight = v));

    public MergePolicySettingsDescriptor SegmentsPerTier(int? segmentsPerTier) => this.Assign<int?>(segmentsPerTier, (Action<IMergePolicySettings, int?>) ((a, v) => a.SegmentsPerTier = v));
  }
}
