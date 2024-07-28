// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Providers.WikiPageMetadataProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.TeamFoundation.Wiki.Server.Builders;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server.Providers
{
  public class WikiPageMetadataProvider : IWikiPageMetadataProvider
  {
    protected IWikiPageIdDetailsProvider wikiPageIdDetailsProvider = (IWikiPageIdDetailsProvider) new WikiPageIdDetailsProvider();
    private IList<IWikiPageMetadataBuilder> wikiPageMetadataBuilders;

    public WikiPageMetadataProvider(
      IList<IWikiPageMetadataBuilder> wikiPageMetadataBuilders)
    {
      this.wikiPageMetadataBuilders = wikiPageMetadataBuilders ?? throw new Exception("Builder List should not be null");
    }

    internal WikiPageMetadataProvider(
      IWikiPageIdDetailsProvider wikiPageIdDetailsProvider,
      IList<IWikiPageMetadataBuilder> wikiPageMetadataBuilders)
    {
      this.wikiPageIdDetailsProvider = wikiPageIdDetailsProvider;
      this.wikiPageMetadataBuilders = wikiPageMetadataBuilders;
    }

    internal WikiPageMetadataProvider()
    {
    }

    public IList<WikiPageDetail> GetPagesData(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      GitVersionDescriptor wikiVersion,
      int? afterPageId,
      int batchSize)
    {
      IList<WikiPageIdDetails> pageIdDetails = this.wikiPageIdDetailsProvider.GetPageIdDetails(requestContext, wiki.ProjectId, wiki.Id, wikiVersion, afterPageId, batchSize);
      IList<WikiPageDetail> wikiPageDetails;
      this.BuildWikiPageDetails(requestContext, wiki, pageIdDetails, out wikiPageDetails);
      return wikiPageDetails;
    }

    public WikiPageDetail GetPageData(IVssRequestContext requestContext, WikiV2 wiki, int pageId)
    {
      WikiPageIdDetails pageIdDetails = this.wikiPageIdDetailsProvider.GetPageIdDetails(requestContext, wiki.ProjectId, wiki, pageId);
      IVssRequestContext requestContext1 = requestContext;
      WikiV2 wiki1 = wiki;
      List<WikiPageIdDetails> wikiPageIdDetails = new List<WikiPageIdDetails>();
      wikiPageIdDetails.Add(pageIdDetails);
      IList<WikiPageDetail> source;
      ref IList<WikiPageDetail> local = ref source;
      this.BuildWikiPageDetails(requestContext1, wiki1, (IList<WikiPageIdDetails>) wikiPageIdDetails, out local);
      return source.FirstOrDefault<WikiPageDetail>();
    }

    private void BuildWikiPageDetails(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IList<WikiPageIdDetails> wikiPageIdDetails,
      out IList<WikiPageDetail> wikiPageDetails)
    {
      wikiPageDetails = (IList<WikiPageDetail>) new List<WikiPageDetail>();
      ISecuredObject repositoryReadOnly = GitSecuredObjectFactory.CreateRepositoryReadOnly(wiki.ProjectId, wiki.RepositoryId);
      foreach (WikiPageIdDetails wikiPageIdDetail in (IEnumerable<WikiPageIdDetails>) wikiPageIdDetails)
      {
        WikiPageDetail wikiPageDetail = new WikiPageDetail(wikiPageIdDetail.PageId, Microsoft.TeamFoundation.Wiki.Server.PathHelper.GetPageReadablePathFromUnReadablePath(Path.ChangeExtension(wikiPageIdDetail.GitFriendlyPagePath, (string) null)));
        wikiPageDetail.SetSecuredObject(repositoryReadOnly);
        wikiPageDetails.Add(wikiPageDetail);
      }
      foreach (IWikiPageMetadataBuilder pageMetadataBuilder in (IEnumerable<IWikiPageMetadataBuilder>) this.wikiPageMetadataBuilders)
        pageMetadataBuilder.Build(requestContext, wiki, wikiPageDetails);
    }
  }
}
