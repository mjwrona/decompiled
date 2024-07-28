// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.UpstreamVersionInstance
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.PackageMetadata;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public static class UpstreamVersionInstance
  {
    public static UpstreamVersionInstance<TPackageVersion> FromUpstreamVersion<TPackageVersion>(
      TPackageVersion version,
      UpstreamSource immediateSource,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      PackageOrigin origin)
      where TPackageVersion : IPackageVersion
    {
      return UpstreamVersionInstance<TPackageVersion>.FromUpstreamVersion(version, immediateSource, sourceChain, origin);
    }

    public static UpstreamVersionInstance<TPackageVersion> FromLocalVersion<TPackageVersion>(
      TPackageVersion version,
      UpstreamSource immediateSource,
      IEnumerable<UpstreamSourceInfo> sourceChain,
      PackageOrigin origin,
      bool isDeleted)
      where TPackageVersion : IPackageVersion
    {
      return UpstreamVersionInstance<TPackageVersion>.FromLocalVersion(version, immediateSource, sourceChain, origin, isDeleted);
    }
  }
}
