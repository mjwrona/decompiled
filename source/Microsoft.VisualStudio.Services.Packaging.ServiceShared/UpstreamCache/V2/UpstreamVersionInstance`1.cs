// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamVersionInstance`1
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public class UpstreamVersionInstance<TPackageVersion> : IUpstreamVersionInstance where TPackageVersion : IPackageVersion
  {
    IPackageVersion IUpstreamVersionInstance.Version => (IPackageVersion) this.Version;

    public TPackageVersion Version { get; }

    public UpstreamSource ImmediateSource { get; }

    public IReadOnlyList<UpstreamSourceInfo> SourceChain { get; }

    public PackageOrigin Origin { get; }

    public bool IsLocal { get; }

    public bool IsDeleted { get; }

    private UpstreamVersionInstance(
      TPackageVersion version,
      UpstreamSource immediateSource,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      PackageOrigin origin,
      bool isLocal,
      bool isDeleted)
    {
      this.Version = version;
      this.ImmediateSource = immediateSource;
      this.SourceChain = sourceChain != null ? (IReadOnlyList<UpstreamSourceInfo>) sourceChain.ToImmutableList<UpstreamSourceInfo>() : (IReadOnlyList<UpstreamSourceInfo>) null;
      this.Origin = origin;
      this.IsLocal = isLocal;
      this.IsDeleted = isDeleted;
    }

    public static UpstreamVersionInstance<TPackageVersion> FromUpstreamVersion(
      TPackageVersion version,
      UpstreamSource immediateSource,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      PackageOrigin origin)
    {
      if ((object) version == null)
        throw new ArgumentNullException(nameof (version));
      if (immediateSource == null)
        throw new ArgumentNullException(nameof (immediateSource));
      return new UpstreamVersionInstance<TPackageVersion>(version, immediateSource, sourceChain, origin, false, false);
    }

    public static UpstreamVersionInstance<TPackageVersion> FromLocalVersion(
      TPackageVersion version,
      UpstreamSource immediateSource,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      PackageOrigin origin,
      bool isDeleted)
    {
      if ((object) version == null)
        throw new ArgumentNullException(nameof (version));
      if (sourceChain == null)
        throw new ArgumentNullException(nameof (sourceChain));
      return new UpstreamVersionInstance<TPackageVersion>(version, immediateSource, sourceChain, origin, true, isDeleted);
    }
  }
}
