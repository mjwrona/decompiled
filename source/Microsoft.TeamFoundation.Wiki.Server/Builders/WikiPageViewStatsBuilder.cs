// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Builders.WikiPageViewStatsBuilder
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.SocialServer.Server;
using Microsoft.VisualStudio.Services.WebApi;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;

namespace Microsoft.TeamFoundation.Wiki.Server.Builders
{
  public class WikiPageViewStatsBuilder : IWikiPageMetadataBuilder
  {
    private int pageViewsForDays;

    public WikiPageViewStatsBuilder(int pageViewsForDays) => this.pageViewsForDays = pageViewsForDays;

    internal WikiPageViewStatsBuilder()
    {
    }

    public virtual void Build(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IList<WikiPageDetail> wikiPageDetails)
    {
      if (this.pageViewsForDays <= 0 || wikiPageDetails == null || wikiPageDetails.Count == 0)
        return;
      List<string> artifactIds = new List<string>();
      Guid wikiId;
      foreach (WikiPageDetail wikiPageDetail in (IEnumerable<WikiPageDetail>) wikiPageDetails)
      {
        List<string> stringList = artifactIds;
        wikiId = wiki.Id;
        string str = string.Format(string.Format("{0}/{1}", (object) wikiId.ToString(), (object) wikiPageDetail.Id));
        stringList.Add(str);
      }
      IList<SocialActivityAggregatedArtifact> aggregatedArtifactBatch = requestContext.GetService<ISocialActivityAggregationService>().GetSocialActivityAggregatedArtifactBatch(requestContext, Guid.Empty, WikiJobs.WikiPageViewDailyAggregationJobId, (byte) 1, (IList<string>) artifactIds);
      DateTime dateTime1 = DateTime.UtcNow;
      dateTime1 = dateTime1.AddDays((double) (-1 * this.pageViewsForDays));
      DateTime dateTime2 = DateTime.Parse(dateTime1.ToString("yyyy-MM-dd"));
      Dictionary<int, IList<WikiPageStat>> dictionary = new Dictionary<int, IList<WikiPageStat>>(wikiPageDetails.Count);
      ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(wiki.ProjectId, wiki.RepositoryId);
      foreach (SocialActivityAggregatedArtifact aggregatedArtifact in (IEnumerable<SocialActivityAggregatedArtifact>) aggregatedArtifactBatch)
      {
        List<WikiPageStat> wikiPageStatList = new List<WikiPageStat>();
        int pageId;
        try
        {
          WikiPageIdHelper.GetIDsfromViewsArtifactId(aggregatedArtifact.ArtifactId, out wikiId, out pageId);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(15252805, "Wiki", "Service", ex);
          continue;
        }
        AggregatedPageViews aggregatedPageViews;
        try
        {
          aggregatedPageViews = JsonConvert.DeserializeObject<AggregatedPageViews>(aggregatedArtifact.MetaData);
        }
        catch (Exception ex)
        {
          requestContext.TraceException(15252804, "Wiki", "Service", ex);
          continue;
        }
        foreach (KeyValuePair<string, int> keyValuePair in (IEnumerable<KeyValuePair<string, int>>) aggregatedPageViews.DayWiseCount)
        {
          DateTime exact = DateTime.ParseExact(keyValuePair.Key, "yyyy-MM-dd", (IFormatProvider) CultureInfo.InvariantCulture, DateTimeStyles.None);
          if (dateTime2.CompareTo(exact) < 0)
          {
            WikiPageStat wikiPageStat = new WikiPageStat(exact, keyValuePair.Value);
            wikiPageStat.SetSecuredObject(repositoryReadOnly);
            wikiPageStatList.Add(wikiPageStat);
          }
        }
        dictionary[pageId] = (IList<WikiPageStat>) wikiPageStatList;
      }
      foreach (WikiPageDetail wikiPageDetail in (IEnumerable<WikiPageDetail>) wikiPageDetails)
      {
        if (dictionary.ContainsKey(wikiPageDetail.Id))
          wikiPageDetail.ViewStats = (IEnumerable<WikiPageStat>) dictionary[wikiPageDetail.Id];
      }
    }
  }
}
