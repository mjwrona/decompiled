// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPageViewStatsProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi;
using System;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiPageViewStatsProvider : IWikiPageViewStatsProvider
  {
    public void CreateOrUpdatePageViewStats(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid wikiId,
      GitVersionDescriptor wikiVersion,
      string pagePath,
      string oldPagePath,
      out WikiPageViewStats pageViewStats)
    {
      if (requestContext == null)
        throw new ArgumentNullException(nameof (requestContext));
      if (wikiVersion == null)
        throw new ArgumentNullException(nameof (wikiVersion));
      if (string.IsNullOrEmpty(pagePath))
        throw new ArgumentException("Null or empty argument: ", nameof (pagePath));
      string wikiId1 = wikiId.ToString();
      string urlFriendlyString = WikiPageViewStatsProvider.TranslateToUrlFriendlyString(wikiVersion);
      PageViewKey currentKey = new PageViewKey(projectId, wikiId1, urlFriendlyString, pagePath);
      PageViewKey prevKey = (PageViewKey) null;
      if ((oldPagePath == null ? 0 : (!string.Equals(pagePath, oldPagePath, StringComparison.InvariantCultureIgnoreCase) ? 1 : 0)) != 0)
        prevKey = new PageViewKey(projectId, wikiId1, urlFriendlyString, oldPagePath);
      PageViewValue pageViewValue = requestContext.GetService<PageViewCacheService>().AddOrUpdate(requestContext, currentKey, prevKey);
      pageViewStats = new WikiPageViewStats()
      {
        Path = pageViewValue.Key.Path,
        Count = pageViewValue.ViewCountBase + pageViewValue.ViewCountDelta,
        LastViewedTime = pageViewValue.LastViewedAt
      };
    }

    private static string TranslateToUrlFriendlyString(GitVersionDescriptor versionDescriptor)
    {
      string empty = string.Empty;
      string str;
      switch (versionDescriptor.VersionType)
      {
        case GitVersionType.Branch:
          str = "B";
          break;
        case GitVersionType.Tag:
          str = "T";
          break;
        case GitVersionType.Commit:
          str = "C";
          break;
        default:
          str = string.Empty;
          break;
      }
      return "G" + str + versionDescriptor.Version;
    }
  }
}
