// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream.UpstreamSourceExtensions
// Assembly: Microsoft.VisualStudio.Services.Packaging.ServiceShared, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 4EF1F7D3-C7DF-4C8F-8AAB-58F76976F85D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Packaging.ServiceShared.dll

using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Packaging.ServiceShared.Upstream
{
  public static class UpstreamSourceExtensions
  {
    public static string GetFullyQualifiedFeedId(this UpstreamSource upstreamSource) => string.Format("{0}@{1}", (object) upstreamSource.InternalUpstreamFeedId, (object) upstreamSource.InternalUpstreamViewId);

    public static Guid? GetProjectId(this UpstreamSource upstreamSource) => upstreamSource.InternalUpstreamProjectId;

    public static bool IsWellKnownSource(this UpstreamSource upstreamSource) => (object) WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstreamSource.Location) != null;

    public static bool IsWellKnownSource(
      this UpstreamSource upstreamSource,
      WellKnownUpstreamSource wellKnownSource)
    {
      return WellKnownSourceProvider.Instance.GetWellKnownSourceOrDefault(upstreamSource.Location) == wellKnownSource;
    }
  }
}
