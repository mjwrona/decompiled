// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Providers.WikiPageCommentDeletionProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Wiki.Server.Providers
{
  public class WikiPageCommentDeletionProvider : WikiPageMetaDataDeletionProviderBase
  {
    public override void Delete(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IList<WikiPageChangeInfo> deletedPages)
    {
      if (!requestContext.IsFeatureEnabled("Wiki.WikiPageComments") || deletedPages == null)
        return;
      ISet<string> artifactIds = (ISet<string>) new HashSet<string>();
      deletedPages = this.GetDeletedPageIds(requestContext, wiki, deletedPages);
      deletedPages.ForEach<WikiPageChangeInfo>((Action<WikiPageChangeInfo>) (deletedPage => artifactIds.Add(WikiPageIdHelper.GetArtifactId(wiki.ProjectId, wiki.Id, deletedPage.PageId))));
      if (artifactIds.Count == 0)
        return;
      requestContext.GetService<ICommentService>().DestroyCommentsForArtifacts(requestContext, wiki.ProjectId, WikiArtifactKinds.WikiPage, artifactIds);
    }
  }
}
