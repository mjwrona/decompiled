// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPageViewDeletionProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.SocialServer.Server;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public class WikiPageViewDeletionProvider : WikiPageMetaDataDeletionProviderBase
  {
    public override void Delete(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IList<WikiPageChangeInfo> deletedPages)
    {
      if (!requestContext.IsFeatureEnabled("Wiki.PageViews") || deletedPages == null)
        return;
      ISocialActivityAggregationService aggregationService = requestContext.GetService<ISocialActivityAggregationService>();
      this.GetDeletedPageIds(requestContext, wiki, deletedPages).ForEach<WikiPageChangeInfo>((Action<WikiPageChangeInfo>) (page => aggregationService.AddActivity(requestContext, "WikiPageDelete", JsonConvert.SerializeObject((object) new PageViewEvent(wiki.Id, page.PageId)), "")));
    }
  }
}
