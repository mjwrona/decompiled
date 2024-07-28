// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Web.Controllers.WikiPagesBatchController
// Assembly: Microsoft.TeamFoundation.Wiki.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D56D2437-BFF5-4193-B3F8-AC19F31B2530
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Wiki.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.SourceControl.WebServer;
using Microsoft.TeamFoundation.Wiki.Server;
using Microsoft.TeamFoundation.Wiki.Server.Builders;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.Web.Helpers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.TeamFoundation.Wiki.WebApi.Contracts;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.ModelBinding;

namespace Microsoft.TeamFoundation.Wiki.Web.Controllers
{
  [ApiTelemetry(true, false)]
  [VersionedApiControllerCustomName(Area = "wiki", ResourceName = "pagesBatch")]
  public class WikiPagesBatchController : WikiApiController
  {
    [HttpPost]
    [ClientResponseType(typeof (IPagedList<WikiPageDetail>), null, null)]
    [ClientResourceOperation(ClientResourceOperationName.Get)]
    [ClientLocationId("71323C46-2592-4398-8771-CED73DD87207")]
    [TraceFilter(15251300, 15251399)]
    [FeatureEnabled("Wiki.PageViews")]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET_wiki_pageBatch_WithContinuationToken.json", "Get page details in batch after sending continuation token", null, null)]
    [ClientExample("GET_wiki_pageBatch_WithoutContinuationToken.json", "Get page details in batch without sending continuation token", null, null)]
    [ClientExample("GET_wiki_pageBatch_WithPageViews.json", "Get page details in batch with page views", null, null)]
    public HttpResponseMessage GetPagesBatch(
      [FromUri, ClientParameterType(typeof (Guid), true)] string wikiIdentifier,
      [FromBody] WikiPagesBatchRequest pagesBatchRequest,
      [ModelBinder(typeof (GitVersionDescriptorModelBinder))] GitVersionDescriptor versionDescriptor = null)
    {
      int? afterPageId1;
      WikiPagesBatchValidationHelper.Validate(pagesBatchRequest, out afterPageId1);
      WikiV2 wikiV2 = ValidationUtils.ValidateWiki(this.TfsRequestContext, this.ProjectId, wikiIdentifier);
      if (versionDescriptor == null || string.IsNullOrEmpty(versionDescriptor.Version))
        versionDescriptor = wikiV2.Versions.ToList<GitVersionDescriptor>()[0];
      List<IWikiPageMetadataBuilder> wikiPageMetadataBuilders = new List<IWikiPageMetadataBuilder>();
      int? nullable = pagesBatchRequest.PageViewsForDays;
      if (nullable.HasValue)
      {
        List<IWikiPageMetadataBuilder> pageMetadataBuilderList = wikiPageMetadataBuilders;
        nullable = pagesBatchRequest.PageViewsForDays;
        WikiPageViewStatsBuilder viewStatsBuilder = new WikiPageViewStatsBuilder(nullable.Value);
        pageMetadataBuilderList.Add((IWikiPageMetadataBuilder) viewStatsBuilder);
      }
      WikiPageMetadataProvider metadataProvider = new WikiPageMetadataProvider((IList<IWikiPageMetadataBuilder>) wikiPageMetadataBuilders);
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      WikiV2 wiki = wikiV2;
      GitVersionDescriptor wikiVersion = versionDescriptor;
      int? afterPageId2 = afterPageId1;
      nullable = pagesBatchRequest.Top;
      int maxTopCount;
      if (!nullable.HasValue)
      {
        maxTopCount = WikiPagesBatchConstants.MaxTopCount;
      }
      else
      {
        nullable = pagesBatchRequest.Top;
        maxTopCount = nullable.Value;
      }
      IList<WikiPageDetail> pagesData = metadataProvider.GetPagesData(tfsRequestContext, wiki, wikiVersion, afterPageId2, maxTopCount);
      HttpResponseMessage response = this.Request.CreateResponse<IList<WikiPageDetail>>(HttpStatusCode.OK, pagesData);
      if (pagesData.Count != 0)
        response.Headers.Add("X-MS-ContinuationToken", pagesData.Last<WikiPageDetail>().Id.ToString());
      return response;
    }
  }
}
