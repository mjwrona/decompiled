// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil.ConstantKeepUntil
// Assembly: Microsoft.VisualStudio.Services.BlobStore.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB48D0BF-32A2-483C-A1D4-2F10DEBB3D56
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.BlobStore.Server.Common.dll

using System;

namespace Microsoft.VisualStudio.Services.BlobStore.Server.Common.DedupGC.KeepUntil
{
  public class ConstantKeepUntil : IKeepUntil
  {
    private readonly DateTimeOffset keepUntil;

    public ConstantKeepUntil(DateTimeOffset keepUntil) => this.keepUntil = keepUntil;

    public bool ShouldSweep(DateTimeOffset existingKeepUntil) => existingKeepUntil < this.keepUntil;

    public virtual bool ShouldMark(DateTimeOffset existingKeepUntil) => existingKeepUntil < this.keepUntil;

    public DateTime GetMarkingDate() => this.keepUntil.UtcDateTime;

    public DateTime GetSweepingDate() => this.keepUntil.UtcDateTime;
  }
}
