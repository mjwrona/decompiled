// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2.PackageOriginClassifier
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi.Types;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.UpstreamCache.V2
{
  public static class PackageOriginClassifier
  {
    public static PackageOrigin ClassifyLocalPackageOrigin(
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      PackagingSourceType? nullable = sourceChain != null ? sourceChain.LastOrDefault<UpstreamSourceInfo>()?.SourceType : new PackagingSourceType?();
      return !nullable.HasValue ? PackageOrigin.Internal : PackageOriginClassifier.ConvertPackagingSourceTypeToPackageOrigin(nullable.Value);
    }

    private static PackageOrigin ConvertPackagingSourceTypeToPackageOrigin(
      PackagingSourceType sourceType)
    {
      if (sourceType == PackagingSourceType.Public)
        return PackageOrigin.External;
      if (sourceType == PackagingSourceType.Internal)
        return PackageOrigin.Internal;
      throw new ArgumentOutOfRangeException();
    }

    public static PackageOrigin ClassifyUpstreamPackageOrigin(
      UpstreamSource currentSource,
      IEnumerable<UpstreamSourceInfo> sourceChain)
    {
      return sourceChain == null ? PackageOrigin.Unknown : PackageOriginClassifier.ConvertPackagingSourceTypeToPackageOrigin((PackagingSourceType) ((int) sourceChain.LastOrDefault<UpstreamSourceInfo>()?.SourceType ?? (int) currentSource.ToUpstreamSourceInfo().SourceType));
    }
  }
}
