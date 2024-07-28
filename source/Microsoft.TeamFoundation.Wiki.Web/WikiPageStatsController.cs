// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiPageStatsController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.Server.Builders;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Web.Http;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "pageStats")]
  public class WikiPageStatsController : WikiApiController
  {
    [HttpGet]
    [ClientResourceOperation(ClientResourceOperationName.Get)]
    [ClientLocationId("81C4E0FE-7663-4D62-AD46-6AB78459F274")]
    [TraceFilter(15251400, 15251499)]
    [FeatureEnabled("Wiki.PageViews")]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET_wiki_pageStats.json", "Get page stats by id", null, null)]
    public WikiPageDetail GetPageData([FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier, [FromUri] int pageId, [ClientQueryParameter] int pageViewsForDays = 0)
    {
      WikiV2 wiki = ValidationUtils.ValidateWiki(this.TfsRequestContext, this.ProjectId, wikiIdentifier);
      if (pageViewsForDays != 0)
        ArgumentUtility.CheckBoundsInclusive(pageViewsForDays, WikiPageViewsConstants.MinPageViewsDays, WikiPageViewsConstants.MaxPageViewsDays, nameof (pageViewsForDays));
      List<IWikiPageMetadataBuilder> wikiPageMetadataBuilders = new List<IWikiPageMetadataBuilder>();
      if (pageViewsForDays != 0)
        wikiPageMetadataBuilders.Add((IWikiPageMetadataBuilder) new WikiPageViewStatsBuilder(pageViewsForDays));
      return new WikiPageMetadataProvider((IList<IWikiPageMetadataBuilder>) wikiPageMetadataBuilders).GetPageData(this.TfsRequestContext, wiki, pageId);
    }
  }
}
