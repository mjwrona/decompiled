// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package.PackageFeed
// Assembly: Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EE1DF96-C85D-457F-AAA1-93619829BFD4
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.dll

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Microsoft.VisualStudio.Services.Search.Platforms.SearchEngine.Definitions.Entities.Package
{
  public class PackageFeed
  {
    public string CollectionId { get; private set; }

    public string CollectionName { get; private set; }

    public string CollectionUrl { get; private set; }

    public string FeedId { get; private set; }

    public string FeedName { get; private set; }

    public string PackageId { get; private set; }

    public string LatestVersion { get; private set; }

    public IEnumerable<PackageViewInfo> Views { get; private set; }

    [SuppressMessage("Microsoft.Design", "CA1054:UriParametersShouldNotBeStrings", MessageId = "2#", Justification = "This is a string")]
    public PackageFeed(
      string collectionId,
      string collectionName,
      string collectionUrl,
      string feedId,
      string feedName,
      string packageId,
      string latestVersion,
      IEnumerable<PackageViewInfo> views)
    {
      this.CollectionId = collectionId;
      this.CollectionName = collectionName;
      this.CollectionUrl = collectionUrl;
      this.FeedId = feedId;
      this.FeedName = feedName;
      this.PackageId = packageId;
      this.LatestVersion = latestVersion;
      this.Views = views;
    }
  }
}
