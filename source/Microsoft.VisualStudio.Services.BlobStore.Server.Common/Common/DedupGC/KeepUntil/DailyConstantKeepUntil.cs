// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil.DailyConstantKeepUntil
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil
{
  public class DailyConstantKeepUntil : IKeepUntil
  {
    private readonly DateTimeOffset dailyKeepUntil;

    public DailyConstantKeepUntil(DateTimeOffset keepUntil) => this.dailyKeepUntil = KeepUntilFactory.GetDateOffset(keepUntil, true);

    public DateTimeOffset GetAsDateOffset() => this.dailyKeepUntil;

    public bool ShouldSweep(DateTimeOffset existingKeepUntil) => existingKeepUntil < this.dailyKeepUntil;

    public bool ShouldMark(DateTimeOffset existingKeepUntil) => existingKeepUntil < this.dailyKeepUntil;

    public DateTime GetMarkingDate() => this.dailyKeepUntil.UtcDateTime;

    public DateTime GetSweepingDate() => this.dailyKeepUntil.UtcDateTime;
  }
}
