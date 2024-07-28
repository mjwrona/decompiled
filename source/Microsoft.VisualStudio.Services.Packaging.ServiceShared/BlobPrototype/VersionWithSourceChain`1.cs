// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype.VersionWithSourceChain`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.BlobPrototype
{
  public class VersionWithSourceChain<TPackageVersion> : IVersionWithSourceChain<TPackageVersion> where TPackageVersion : IPackageVersion
  {
    public TPackageVersion Version { get; }

    public IReadOnlyList<UpstreamSourceInfo> SourceChain { get; }

    private VersionWithSourceChain(
      TPackageVersion version,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      if (sourceChain == null)
        throw new ArgumentNullException(nameof (sourceChain));
      this.Version = version;
      this.SourceChain = (IReadOnlyList<UpstreamSourceInfo>) sourceChain.ToImmutableList<UpstreamSourceInfo>();
    }

    public static VersionWithSourceChain<TPackageVersion> FromInternalSource(
      TPackageVersion version,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      return new VersionWithSourceChain<TPackageVersion>(version, sourceChain);
    }

    public static VersionWithSourceChain<TPackageVersion> FromThisFeed(
      TPackageVersion version,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      return new VersionWithSourceChain<TPackageVersion>(version, sourceChain);
    }

    public static VersionWithSourceChain<TPackageVersion> FromExternalSource(TPackageVersion version) => new VersionWithSourceChain<TPackageVersion>(version, (IEnumerable<UpstreamSourceInfo>) ImmutableList<UpstreamSourceInfo>.Empty);
  }
}
