// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil.KeepUntilFactory
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil
{
  public class KeepUntilFactory
  {
    private readonly bool enableBucketing;
    private readonly bool markWithFutureKeepUntil;
    private readonly bool alwaysMark;

    public KeepUntilFactory(IVssRequestContext context)
    {
      this.alwaysMark = context.IsFeatureEnabled("BlobStore.Features.AlwaysMarkKeepUntil");
      this.enableBucketing = !this.alwaysMark && context.IsFeatureEnabled("BlobStore.Features.BucketKeepUntilByDay");
      this.markWithFutureKeepUntil = !this.alwaysMark && context.IsFeatureEnabled("BlobStore.Features.MarkWithFutureKeepUntil");
    }

    public IKeepUntil Create(DateTime minKeepUntil) => this.markWithFutureKeepUntil ? (IKeepUntil) new ConstantFutureKeepUntil(this.enableBucketing, (DateTimeOffset) minKeepUntil) : this.CreateConstantKeepUntil(minKeepUntil);

    public IKeepUntil CreateConstantKeepUntil(DateTime minKeepUntil)
    {
      if (this.alwaysMark)
        return (IKeepUntil) new AlwaysMarkConstantKeepUntil((DateTimeOffset) minKeepUntil);
      return this.enableBucketing ? (IKeepUntil) new DailyConstantKeepUntil((DateTimeOffset) minKeepUntil) : (IKeepUntil) new ConstantKeepUntil((DateTimeOffset) minKeepUntil);
    }

    public static DateTimeOffset GetTicksAsUtcOffset(long ticks) => new DateTimeOffset(ticks, TimeSpan.Zero);

    public static DateTimeOffset GetDateOffset(DateTimeOffset date, bool bucket) => bucket ? new DateTimeOffset(date.Year, date.Month, date.Day, 0, 0, 0, date.Offset) : date;
  }
}
