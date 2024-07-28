// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.PackageRangeRequest`2
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class PackageRangeRequest<TName, TVer> : FeedRequest
    where TName : IPackageName
    where TVer : IPackageVersion
  {
    public PackageRangeRequest(
      IFeedRequest feedRequest,
      TName packageName,
      TVer packageVersionLower,
      TVer packageVersionUpper)
      : base(feedRequest)
    {
      this.PackageName = packageName;
      this.PackageVersionLower = packageVersionLower;
      this.PackageVersionUpper = packageVersionUpper;
    }

    public TName PackageName { get; set; }

    public TVer PackageVersionLower { get; set; }

    public TVer PackageVersionUpper { get; set; }
  }
}
