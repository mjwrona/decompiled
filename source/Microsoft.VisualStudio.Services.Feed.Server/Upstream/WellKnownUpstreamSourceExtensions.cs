// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Feed.Server.Upstream.WellKnownUpstreamSourceExtensions
// Assembly: Microsoft.VisualStudio.Services.Feed.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 55555083-3B79-4F4F-AA85-92D66019974E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Feed.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Feed.Common;
using Microsoft.VisualStudio.Services.Feed.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;


#nullable enable
namespace Microsoft.VisualStudio.Services.Feed.Server.Upstream
{
  public static class WellKnownUpstreamSourceExtensions
  {
    public static bool IsAvailable(
      this WellKnownUpstreamSource source,
      IVssRequestContext requestContext)
    {
      if (!FeatureFlagFeedSettingProvider.IsProtocolFeatureFlagEnabled(requestContext, source.Protocol))
        return false;
      return source.FeatureFlag == null || requestContext.IsFeatureEnabled(source.FeatureFlag);
    }

    public static IEnumerable<WellKnownUpstreamSource> AvailableSources(
      this WellKnownSourceProvider wellKnownSourceProvider,
      IVssRequestContext requestContext)
    {
      return wellKnownSourceProvider.KnownSources.Where<WellKnownUpstreamSource>((Func<WellKnownUpstreamSource, bool>) (x => x.IsAvailable(requestContext)));
    }

    public static List<UpstreamSource> AvailableSourcesAsUpstreamSources(
      this WellKnownSourceProvider wellKnownSourceProvider,
      IVssRequestContext requestContext)
    {
      return wellKnownSourceProvider.AvailableSources(requestContext).Select<WellKnownUpstreamSource, UpstreamSource>((Func<WellKnownUpstreamSource, UpstreamSource>) (x => x.ToUpstreamSource())).ToList<UpstreamSource>();
    }
  }
}
