// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamSourceInfoUtils
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public static class UpstreamSourceInfoUtils
  {
    public static UpstreamSourceInfo ToUpstreamSourceInfo(this UpstreamSource sourceInput) => UpstreamSourceInfoUtils.CreateUpstreamSourceInfo(sourceInput);

    public static UpstreamSourceInfo CreateUpstreamSourceInfo(UpstreamSource sourceInput)
    {
      UpstreamSourceInfo upstreamSourceInfo = new UpstreamSourceInfo()
      {
        Id = sourceInput.Id,
        Name = sourceInput.Name,
        Location = sourceInput.Location,
        DisplayLocation = sourceInput.DisplayLocation
      };
      switch (sourceInput.UpstreamSourceType)
      {
        case UpstreamSourceType.Public:
          upstreamSourceInfo.SourceType = PackagingSourceType.Public;
          break;
        case UpstreamSourceType.Internal:
          upstreamSourceInfo.SourceType = PackagingSourceType.Internal;
          break;
        default:
          throw new NotSupportedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamSourceNotSupported());
      }
      return upstreamSourceInfo;
    }

    public static string GetSourceChainKey(IEnumerable<UpstreamSourceInfo> sourceChain) => string.Join<Guid>(",", (sourceChain ?? Enumerable.Empty<UpstreamSourceInfo>()).Select<UpstreamSourceInfo, Guid>((Func<UpstreamSourceInfo, Guid>) (s => s.Id)));
  }
}
