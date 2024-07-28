// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts.PackageFileRequest`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using System;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.DataContracts
{
  public class PackageFileRequest<TPackageId> : 
    PackageRequest<TPackageId>,
    IPackageFileRequest<TPackageId>,
    IPackageRequest<TPackageId>,
    IPackageRequest,
    IFeedRequest,
    IProtocolAgnosticFeedRequest,
    IPackageFileRequest,
    IEquatable<PackageFileRequest<TPackageId>>
    where TPackageId : IPackageIdentity
  {
    public PackageFileRequest(IFeedRequest feedRequest, TPackageId packageId, string filePath)
      : base(feedRequest, packageId)
    {
      this.FilePath = filePath;
    }

    public PackageFileRequest(IPackageRequest<TPackageId> request, string filePath)
      : this((IFeedRequest) request, request.PackageId, filePath)
    {
    }

    public string FilePath { get; }

    public bool Equals(PackageFileRequest<TPackageId> other)
    {
      if (other == null)
        return false;
      if (this == other)
        return true;
      return object.Equals((object) this.Feed?.FullyQualifiedId, (object) other.Feed?.FullyQualifiedId) && EqualityComparer<TPackageId>.Default.Equals(this.PackageId, other.PackageId) && string.Equals(this.FilePath, other.FilePath);
    }

    public override bool Equals(object obj)
    {
      if (obj == null)
        return false;
      if (this == obj)
        return true;
      return !(obj.GetType() != this.GetType()) && this.Equals((PackageFileRequest<TPackageId>) obj);
    }

    public override int GetHashCode() => ((this.Feed?.FullyQualifiedId != null ? this.Feed.FullyQualifiedId.GetHashCode() : 0) * 397 ^ EqualityComparer<TPackageId>.Default.GetHashCode(this.PackageId)) * 397 ^ (this.FilePath != null ? this.FilePath.GetHashCode() : 0);
  }
}
