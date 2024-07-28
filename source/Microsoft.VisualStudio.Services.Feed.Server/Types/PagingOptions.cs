// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Types.PagingOptions
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;

namespace Microsoft.VisualStudio.Services.Feed.Server.Types
{
  public class PagingOptions
  {
    public const int MaxTopValue = 100000;
    public const int TopDefault = 1000;
    public const int SkipDefault = 0;

    public int Top { get; set; } = 1000;

    public int Skip { get; set; }

    public bool? ApplyToVersions { get; set; }

    public void Validate()
    {
      ArgumentUtility.CheckForNonnegativeInt(this.Top, "Top");
      ArgumentUtility.CheckForNonnegativeInt(this.Skip, "Skip");
      if (!this.TryValidate())
        throw PackageLimitExceededException.Create(this.Top, this.Skip);
    }

    public bool TryValidate() => this.Top >= 0 && this.Skip >= 0 && this.Top <= 100000 && this.Top <= int.MaxValue - this.Skip;

    public override bool Equals(object obj)
    {
      if (!(obj is PagingOptions pagingOptions) || this.Top != pagingOptions.Top || this.Skip != pagingOptions.Skip)
        return false;
      bool? applyToVersions1 = this.ApplyToVersions;
      bool? applyToVersions2 = pagingOptions.ApplyToVersions;
      return applyToVersions1.GetValueOrDefault() == applyToVersions2.GetValueOrDefault() & applyToVersions1.HasValue == applyToVersions2.HasValue;
    }

    public override int GetHashCode() => (((long) this.Top << 32) + (long) this.Skip + (this.ApplyToVersions.GetValueOrDefault() ? long.MinValue : 0L)).GetHashCode();
  }
}
