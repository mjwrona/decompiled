// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil.ConstantFutureKeepUntil
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil
{
  public class ConstantFutureKeepUntil : IKeepUntil
  {
    public const int MarkingDays = 8;
    public const int DeleteDays = -3;
    private readonly DateTimeOffset markingDate;
    private readonly DateTimeOffset sweepingDate;
    private readonly DateTimeOffset markBeforeDate;

    public ConstantFutureKeepUntil(bool useDailyDates, DateTimeOffset sweepBeforeDate)
      : this(useDailyDates, sweepBeforeDate, (ITimeProvider) new DefaultTimeProvider())
    {
    }

    public ConstantFutureKeepUntil(
      bool useDailyDates,
      DateTimeOffset sweepBeforeDate,
      ITimeProvider timeProvider)
    {
      this.markBeforeDate = KeepUntilFactory.GetDateOffset((DateTimeOffset) timeProvider.Now.ToUniversalTime(), useDailyDates);
      this.markingDate = KeepUntilFactory.GetDateOffset(this.markBeforeDate.AddDays(8.0), useDailyDates);
      DateTimeOffset date = this.markBeforeDate.AddDays(-3.0);
      this.sweepingDate = !(sweepBeforeDate > date) ? KeepUntilFactory.GetDateOffset(sweepBeforeDate, useDailyDates) : KeepUntilFactory.GetDateOffset(date, useDailyDates);
      this.AssertConstraints();
    }

    public ConstantFutureKeepUntil(
      bool useDailyDates,
      DateTimeOffset markBeforeDate,
      DateTimeOffset markDate,
      DateTimeOffset sweepBeforeDate)
    {
      this.markBeforeDate = KeepUntilFactory.GetDateOffset(markBeforeDate, useDailyDates);
      this.markingDate = KeepUntilFactory.GetDateOffset(markDate, useDailyDates);
      DateTimeOffset date = markBeforeDate.AddDays(-3.0);
      this.sweepingDate = !(sweepBeforeDate > date) ? KeepUntilFactory.GetDateOffset(sweepBeforeDate, useDailyDates) : KeepUntilFactory.GetDateOffset(date, useDailyDates);
      this.AssertConstraints();
    }

    public bool ShouldSweep(DateTimeOffset existingKeepUntil) => existingKeepUntil < this.sweepingDate;

    public bool ShouldMark(DateTimeOffset existingKeepUntil) => existingKeepUntil < this.markBeforeDate;

    public DateTime GetMarkingDate() => this.markingDate.UtcDateTime;

    public DateTime GetSweepingDate() => this.sweepingDate.UtcDateTime;

    private void AssertConstraints()
    {
      if (this.sweepingDate > this.markBeforeDate)
        throw new ArgumentOutOfRangeException(Resources.InvalidSweepingDate());
      if (this.markBeforeDate > this.markingDate)
        throw new ArgumentOutOfRangeException(Resources.InvalidMarkBeforeDate());
    }
  }
}
