// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageNameRequest`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class PackageNameRequest<TPackageName> : 
    FeedRequest,
    IPackageNameRequest<TPackageName>,
    IPackageNameRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest
    where TPackageName : IPackageName
  {
    public PackageNameRequest(IFeedRequest feedRequest, TPackageName packageName)
      : base(feedRequest)
    {
      this.PackageName = packageName;
    }

    public PackageNameRequest(FeedCore feed, TPackageName packageName)
      : base(feed, packageName.Protocol)
    {
      this.PackageName = packageName;
    }

    public TPackageName PackageName { get; }

    protected bool Equals(PackageNameRequest<TPackageName> other) => object.Equals((object) this.Feed.Id, (object) other.Feed.Id) && PackageNameComparer.NormalizedName.Equals((IPackageName) this.PackageName, (IPackageName) other.PackageName);

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((PackageNameRequest<TPackageName>) obj);
    }

    public override int GetHashCode() => (this.Feed != null ? this.Feed.Id.GetHashCode() : 0) * 397 ^ this.PackageName.NormalizedName.GetHashCode();

    IPackageName IPackageNameRequest.PackageName => (IPackageName) this.PackageName;
  }
}
