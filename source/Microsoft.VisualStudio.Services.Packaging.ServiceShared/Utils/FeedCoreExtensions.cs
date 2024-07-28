// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils.FeedCoreExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Utils
{
  public static class FeedCoreExtensions
  {
    public static IEnumerable<UpstreamSource> GetSourcesForProtocol(
      this FeedCore feed,
      IProtocol protocol)
    {
      return feed.UpstreamSources == null ? Enumerable.Empty<UpstreamSource>() : feed.UpstreamSources.Where<UpstreamSource>((Func<UpstreamSource, bool>) (source => source.Status != UpstreamStatus.Disabled && string.Equals(source.Protocol, protocol.CorrectlyCasedName, StringComparison.OrdinalIgnoreCase)));
    }
  }
}
