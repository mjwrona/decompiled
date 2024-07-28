// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.VersionWithSourceChain
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public static class VersionWithSourceChain
  {
    public static VersionWithSourceChain<TPackageVersion> FromInternalSource<TPackageVersion>(
      TPackageVersion version,
      IEnumerable<UpstreamSourceInfo> sourceChain)
      where TPackageVersion : IPackageVersion
    {
      return VersionWithSourceChain<TPackageVersion>.FromInternalSource(version, sourceChain);
    }

    public static VersionWithSourceChain<TPackageVersion> FromThisFeed<TPackageVersion>(
      TPackageVersion version,
      IEnumerable<UpstreamSourceInfo> sourceChain)
      where TPackageVersion : IPackageVersion
    {
      return VersionWithSourceChain<TPackageVersion>.FromThisFeed(version, sourceChain);
    }

    public static VersionWithSourceChain<TPackageVersion> FromExternalSource<TPackageVersion>(
      TPackageVersion version)
      where TPackageVersion : IPackageVersion
    {
      return VersionWithSourceChain<TPackageVersion>.FromExternalSource(version);
    }
  }
}
