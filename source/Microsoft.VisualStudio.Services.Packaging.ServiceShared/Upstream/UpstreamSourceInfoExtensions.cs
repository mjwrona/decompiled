// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamSourceInfoExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using Microsoft.VisualStudio.Services.Packaging.Shared.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public static class UpstreamSourceInfoExtensions
  {
    public static UpstreamSource ToFeedApiSource(
      this UpstreamSourceInfo sourceInfo,
      string protocol)
    {
      UpstreamSource feedApiSource = new UpstreamSource()
      {
        Id = sourceInfo.Id,
        Name = sourceInfo.Name,
        Location = sourceInfo.Location,
        DisplayLocation = sourceInfo.DisplayLocation,
        Protocol = protocol
      };
      switch (sourceInfo.SourceType)
      {
        case PackagingSourceType.Public:
          feedApiSource.UpstreamSourceType = UpstreamSourceType.Public;
          break;
        case PackagingSourceType.Internal:
          feedApiSource.UpstreamSourceType = UpstreamSourceType.Internal;
          break;
        default:
          throw new NotSupportedException(Microsoft.VisualStudio.Services.Packaging.ServiceShared.Resources.Error_UpstreamSourceNotSupported());
      }
      return feedApiSource;
    }
  }
}
