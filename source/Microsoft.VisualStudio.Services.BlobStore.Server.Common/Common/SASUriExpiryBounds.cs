// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.SASUriExpiryBounds
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common
{
  public struct SASUriExpiryBounds
  {
    public readonly TimeSpan MinExpiry;
    public readonly TimeSpan MaxExpiry;

    public SASUriExpiryBounds(TimeSpan minExpiry, TimeSpan maxExpiry)
    {
      this.MinExpiry = !(minExpiry > maxExpiry) ? minExpiry : throw new ArgumentOutOfRangeException(string.Format("{0} {1} exceeds {2} {3}", (object) nameof (minExpiry), (object) minExpiry, (object) nameof (maxExpiry), (object) maxExpiry));
      this.MaxExpiry = maxExpiry;
    }

    internal TimeSpan Period => this.MaxExpiry - this.MinExpiry;

    internal bool IsInBounds(DateTime baseline, DateTime target) => target >= baseline + this.MinExpiry & target <= baseline + this.MaxExpiry;

    internal bool IsInBounds(
      DateTimeOffset baseline,
      DateTimeOffset target,
      out string description)
    {
      description = string.Format("{0} in bounds", (object) target);
      int num = target >= baseline + this.MinExpiry ? 1 : 0;
      if (num == 0)
        description = string.Format("{0} below min {1}", (object) target, (object) (baseline + this.MinExpiry));
      bool flag = target <= baseline + this.MaxExpiry;
      if (!flag)
        description = string.Format("{0} above max {1}", (object) target, (object) (baseline + this.MaxExpiry));
      return (num & (flag ? 1 : 0)) != 0;
    }

    public bool MovedInBounds(
      DateTimeOffset baseline,
      DateTimeOffset target,
      out DateTimeOffset? moved,
      out string description)
    {
      description = (string) null;
      moved = new DateTimeOffset?();
      DateTimeOffset dateTimeOffset1 = baseline + this.MinExpiry;
      if (!(target >= dateTimeOffset1))
      {
        description = string.Format("moved {0:o} to {1} {2:o}", (object) target, (object) "MinExpiry", (object) dateTimeOffset1);
        moved = new DateTimeOffset?(dateTimeOffset1);
        return true;
      }
      DateTimeOffset dateTimeOffset2 = baseline + this.MaxExpiry;
      if (target <= dateTimeOffset2)
        return false;
      description = string.Format("moved {0:o} to {1} {2:o}", (object) target, (object) "MaxExpiry", (object) dateTimeOffset2);
      moved = new DateTimeOffset?(dateTimeOffset2);
      return true;
    }
  }
}
