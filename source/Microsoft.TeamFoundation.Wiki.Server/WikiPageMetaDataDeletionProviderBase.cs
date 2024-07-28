// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.WikiPageMetaDataDeletionProviderBase
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.Server.Providers;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server
{
  public abstract class WikiPageMetaDataDeletionProviderBase : IWikiPageMetaDataDeletionProvider
  {
    public abstract void Delete(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IList<WikiPageChangeInfo> deletedPages);

    public IList<WikiPageChangeInfo> GetDeletedPageIds(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IList<WikiPageChangeInfo> deletedPages)
    {
      ArgumentUtility.CheckForNull<WikiV2>(wiki, nameof (wiki));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) deletedPages, nameof (deletedPages));
      List<KeyValuePair<Guid, int>> wikiIdWithPageIds = new List<KeyValuePair<Guid, int>>();
      deletedPages.ForEach<WikiPageChangeInfo>((Action<WikiPageChangeInfo>) (deletedPage => wikiIdWithPageIds.Add(new KeyValuePair<Guid, int>(wiki.Id, deletedPage.PageId))));
      IEnumerable<KeyValuePair<Guid, int>> collection = WikiPageIdDetailsProvider.ValidatePageIds(requestContext, wiki.ProjectId, (IEnumerable<KeyValuePair<Guid, int>>) wikiIdWithPageIds);
      HashSet<int> nonDeletedPageIds = new HashSet<int>();
      Action<KeyValuePair<Guid, int>> action = (Action<KeyValuePair<Guid, int>>) (page => nonDeletedPageIds.Add(page.Value));
      collection.ForEach<KeyValuePair<Guid, int>>(action);
      return (IList<WikiPageChangeInfo>) deletedPages.Where<WikiPageChangeInfo>((Func<WikiPageChangeInfo, bool>) (page => !nonDeletedPageIds.Contains(page.PageId))).ToList<WikiPageChangeInfo>();
    }
  }
}
