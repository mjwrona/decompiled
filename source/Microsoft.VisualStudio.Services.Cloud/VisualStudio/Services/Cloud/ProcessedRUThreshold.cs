// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Cloud.ProcessedRUThreshold
// Assembly: Microsoft.VisualStudio.Services.Cloud, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F2D48A0E-C4C9-4233-BA34-E8461823F6F2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Cloud.dll

using System;

namespace Microsoft.VisualStudio.Services.Cloud
{
  public class ProcessedRUThreshold
  {
    public ProcessedRUThreshold(RUThreshold t, double dpSignificanceThresholdFraction)
    {
      this.Flag = t.Flag ?? -1L;
      long? nullable = t.Tarpit;
      this.Tarpit = nullable ?? -1L;
      nullable = t.Block;
      this.Block = nullable ?? -1L;
      this.DPMagnitude = t.DPMagnitude.GetValueOrDefault();
      this.CalculateDPFlag(dpSignificanceThresholdFraction);
    }

    public ProcessedRUThreshold(ProcessedRUThreshold other)
    {
      this.DPFlag = other.DPFlag;
      this.Flag = other.Flag;
      this.Tarpit = other.Tarpit;
      this.Block = other.Block;
      this.DPMagnitude = other.DPMagnitude;
    }

    public void OverrideThresholds(
      RUThreshold overrideThreshold,
      double dpSignificanceThresholdFraction)
    {
      if (overrideThreshold.Flag.HasValue)
        this.Flag = overrideThreshold.Flag.Value;
      if (overrideThreshold.Tarpit.HasValue)
        this.Tarpit = overrideThreshold.Tarpit.Value;
      if (overrideThreshold.Block.HasValue)
        this.Block = overrideThreshold.Block.Value;
      if (overrideThreshold.DPMagnitude.HasValue)
        this.DPMagnitude = overrideThreshold.DPMagnitude.Value;
      this.CalculateDPFlag(dpSignificanceThresholdFraction);
    }

    public long DPFlag { get; private set; }

    public long Flag { get; private set; }

    public long Tarpit { get; private set; }

    public long Block { get; private set; }

    public double DPMagnitude { get; private set; }

    internal ResourceState2 GetState(
      long usage,
      double dpImpact,
      out double dpFactor,
      bool ignoreBlock = false)
    {
      ResourceState2 state = ResourceState2.Normal;
      if (this.DPMagnitude > 0.0)
      {
        double num = Math.Max(60.0, (3300.0 - 3600.0 * this.DPMagnitude) / this.DPMagnitude);
        dpFactor = (num + 300.0) / (num + dpImpact);
      }
      else
        dpFactor = 1.0;
      if (this.DPFlag != -1L && usage > this.DPFlag)
        state |= ResourceState2.DPFlag;
      if (this.Flag != -1L && (double) usage > dpFactor * (double) this.Flag)
        state |= ResourceState2.Flag;
      if (this.Tarpit != -1L && (double) usage > dpFactor * (double) this.Tarpit)
        state |= ResourceState2.Tarpit;
      if (!ignoreBlock && this.Block != -1L && (double) usage > dpFactor * (double) this.Block)
        state |= ResourceState2.Block;
      return state;
    }

    internal long GetMinThrottleThreshold() => this.Tarpit != -1L && this.Block != -1L ? Math.Min(this.Block, this.Tarpit) : Math.Max(this.Block, this.Tarpit);

    private void CalculateDPFlag(double dpSignificanceThresholdFraction)
    {
      bool flag = this.Block != -1L && this.Block > 0L;
      long num = this.Tarpit == -1L || this.Tarpit <= 0L ? (flag ? this.Block : -1L) : (flag ? Math.Min(this.Tarpit, this.Block) : this.Tarpit);
      this.DPFlag = num == -1L || dpSignificanceThresholdFraction <= 0.0 ? -1L : (long) Math.Ceiling(dpSignificanceThresholdFraction * (double) num);
    }
  }
}
