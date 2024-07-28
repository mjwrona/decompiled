// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Web.Models.VSTabData
// Assembly: Microsoft.VisualStudio.Services.Gallery.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 17D36576-2EF3-4ABC-94BA-AF7891D15A3A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Web.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Gallery.Web.Models
{
  public class VSTabData
  {
    public VSTabData(
      List<VSSearchResult> featuredExtensions,
      List<VSSearchResult> mostPopularExtensions,
      List<VSSearchResult> topRatedExtensions,
      List<VSSearchResult> recentlyAddedExtensions,
      VSCategory[] categories)
    {
      this.FeaturedExtensions = featuredExtensions;
      this.MostPopularExtensions = mostPopularExtensions;
      this.TopRatedExtensions = topRatedExtensions;
      this.RecentlyAddedExtensions = recentlyAddedExtensions;
      this.Categories = categories;
    }

    public List<VSSearchResult> FeaturedExtensions { get; private set; }

    public List<VSSearchResult> MostPopularExtensions { get; private set; }

    public List<VSSearchResult> TopRatedExtensions { get; private set; }

    public List<VSSearchResult> RecentlyAddedExtensions { get; private set; }

    public VSCategory[] Categories { get; private set; }
  }
}
