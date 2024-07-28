// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Notifications.Server.MatcherFilterFactory
// Assembly: Microsoft.VisualStudio.Services.Notifications.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8ED1F52D-E567-4EFA-AAED-F2D9262BE9C2
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Notifications.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.Notifications.Server
{
  public class MatcherFilterFactory
  {
    internal static IMatcherFilter GetMatcherFilter(
      IVssRequestContext requestContext,
      ISubscriptionFilter filter,
      bool throwIfNotFound = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForNull<ISubscriptionFilter>(filter, nameof (filter));
      bool flag = requestContext.IsFeatureEnabled("Notifications.SerializeFilterModel");
      switch (filter)
      {
        case ActorFilter _:
          return flag ? (IMatcherFilter) new ActorMatcherFilter() : (IMatcherFilter) new LegacyActorMatcherFilter();
        case BlockFilter _:
          return flag ? (IMatcherFilter) new BlockMatcherFilter() : (IMatcherFilter) new LegacyBlockMatcherFilter();
        case ExpressionFilter _:
          return flag ? (IMatcherFilter) new PathMatcherFilter() : (IMatcherFilter) new LegacyPathMatcherFilter("PathMatcher");
        case ArtifactFilter _:
          return (IMatcherFilter) new FollowsMatcherFilter();
        default:
          if (throwIfNotFound)
            throw new FilterMatcherNotFoundExpcetion(CoreRes.ErrorLoadFilterMatcher((object) filter.GetType().ToString()));
          return (IMatcherFilter) null;
      }
    }

    internal static IMatcherFilter GetMatcherFilter(
      IVssRequestContext requestContext,
      string matcher,
      bool throwIfNotFound = true)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(matcher, nameof (matcher));
      if (matcher.Equals("PathExpressionMatcher", StringComparison.OrdinalIgnoreCase))
        return (IMatcherFilter) new PathMatcherFilter();
      if (matcher.Equals("ActorExpressionMatcher", StringComparison.OrdinalIgnoreCase))
        return (IMatcherFilter) new ActorMatcherFilter();
      if (matcher.Equals("BlockExpressionMatcher", StringComparison.OrdinalIgnoreCase))
        return (IMatcherFilter) new BlockMatcherFilter();
      if (matcher.Equals("XPathMatcher", StringComparison.OrdinalIgnoreCase))
        return (IMatcherFilter) new LegacyPathMatcherFilter("XPathMatcher");
      if (matcher.Equals("JsonPathMatcher", StringComparison.OrdinalIgnoreCase))
        return (IMatcherFilter) new LegacyPathMatcherFilter("JsonPathMatcher");
      if (matcher.Equals("ActorMatcher", StringComparison.OrdinalIgnoreCase))
        return (IMatcherFilter) new LegacyActorMatcherFilter();
      if (matcher.Equals("BlockMatcher", StringComparison.OrdinalIgnoreCase))
        return (IMatcherFilter) new LegacyBlockMatcherFilter();
      if (matcher.Equals("FollowsMatcher", StringComparison.OrdinalIgnoreCase))
        return (IMatcherFilter) new FollowsMatcherFilter();
      if (throwIfNotFound)
        throw new FilterMatcherNotFoundExpcetion(CoreRes.ErrorLoadFilterMatcher((object) matcher));
      return (IMatcherFilter) null;
    }
  }
}
