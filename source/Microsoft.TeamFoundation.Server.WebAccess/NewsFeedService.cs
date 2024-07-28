// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.NewsFeedService
// Assembly: Microsoft.TeamFoundation.Server.WebAccess, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A2CCA8C5-6910-48A5-82D9-BDC1350B5B4D
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.WebAccess.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess
{
  public class NewsFeedService : VssMemoryCacheService<string, SyndicationFeed>
  {
    private const int WebAccessExceptionEaten = 599999;
    private const string c_tfsNewsCacheKey = "TfsNewsCache";
    private const string c_tfsNewsFeedUrl = "https://go.microsoft.com/fwlink/?LinkId=322031";
    private static readonly TimeSpan s_maxCacheLife = TimeSpan.FromHours(1.0);
    private static readonly TimeSpan s_cleanupInterval = new TimeSpan(0, 5, 0);

    public NewsFeedService()
      : base((IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase, NewsFeedService.s_cleanupInterval)
    {
      this.ExpiryInterval.Value = NewsFeedService.s_maxCacheLife;
    }

    protected override void ServiceStart(IVssRequestContext systemRequestContext)
    {
      base.ServiceStart(systemRequestContext);
      systemRequestContext.CheckDeploymentRequestContext();
    }

    public NewsResult GetNewsResult(IVssRequestContext requestContext, int maxCount)
    {
      NewsResult newsResult = (NewsResult) null;
      SyndicationFeed feedContent1;
      if (this.TryGetValue(requestContext, "TfsNewsCache", out feedContent1))
      {
        newsResult = NewsFeedService.GetNewsResult(feedContent1, maxCount);
      }
      else
      {
        try
        {
          using (XmlReader reader = XmlReader.Create("https://go.microsoft.com/fwlink/?LinkId=322031", new XmlReaderSettings()
          {
            DtdProcessing = DtdProcessing.Prohibit,
            XmlResolver = (XmlResolver) null
          }))
          {
            SyndicationFeed feedContent2 = SyndicationFeed.Load(reader);
            newsResult = NewsFeedService.GetNewsResult(feedContent2, maxCount);
            this.Set(requestContext, "TfsNewsCache", feedContent2);
          }
        }
        catch (Exception ex)
        {
          TeamFoundationTracingService.TraceExceptionRaw(599999, TraceLevel.Error, "WebAccess", TfsTraceLayers.Controller, nameof (NewsFeedService), ex);
          newsResult = (NewsResult) null;
        }
      }
      return newsResult;
    }

    private static NewsResult GetNewsResult(SyndicationFeed feedContent, int maxCount)
    {
      NewsResult newsResult = new NewsResult();
      if (feedContent != null)
      {
        if (feedContent.Links.Count > 0)
          newsResult.Link = feedContent.Links[0].Uri.AbsoluteUri;
        if (feedContent.Items.Any<SyndicationItem>())
          newsResult.Items = feedContent.Items.OrderByDescending<SyndicationItem, DateTimeOffset>((Func<SyndicationItem, DateTimeOffset>) (x => x.PublishDate)).Select<SyndicationItem, NewsItem>((Func<SyndicationItem, NewsItem>) (x => new NewsItem()
          {
            Title = x.Title.Text,
            Summary = x.ElementExtensions.Any<SyndicationElementExtension>((Func<SyndicationElementExtension, bool>) (y => string.Compare(y.OuterName, "summary", StringComparison.OrdinalIgnoreCase) == 0)) ? x.ElementExtensions.First<SyndicationElementExtension>((Func<SyndicationElementExtension, bool>) (y => string.Compare(y.OuterName, "summary", StringComparison.OrdinalIgnoreCase) == 0)).GetObject<XElement>().Value : string.Empty,
            Url = x.Links[0].Uri.AbsoluteUri
          })).Take<NewsItem>(maxCount);
      }
      return newsResult;
    }
  }
}
