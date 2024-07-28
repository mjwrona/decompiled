// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageRequest`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class PackageRequest<TPackageId> : 
    FeedRequest,
    IPackageRequest<TPackageId>,
    IPackageRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest
    where TPackageId : IPackageIdentity
  {
    public PackageRequest(IFeedRequest feedRequest, TPackageId packageId)
      : base(feedRequest)
    {
      this.PackageId = packageId;
    }

    public PackageRequest(FeedCore feed, TPackageId packageId)
      : base(feed, packageId.Name.Protocol)
    {
      this.PackageId = packageId;
    }

    public TPackageId PackageId { get; }

    IPackageIdentity IPackageRequest.PackageId => (IPackageIdentity) this.PackageId;
  }
}
