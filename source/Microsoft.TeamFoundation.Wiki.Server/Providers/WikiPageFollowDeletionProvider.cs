// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Wiki.Server.Providers.WikiPageFollowDeletionProvider
// Assembly: Microsoft.TeamFoundation.Wiki.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B3E52AF1-8928-4A06-8693-F7E4A258A64E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Wiki.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Wiki.Server.Contracts;
using Microsoft.TeamFoundation.Wiki.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Notifications.Server.Follows;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Wiki.Server.Providers
{
  public class WikiPageFollowDeletionProvider : WikiPageMetaDataDeletionProviderBase
  {
    public override void Delete(
      IVssRequestContext requestContext,
      WikiV2 wiki,
      IList<WikiPageChangeInfo> deletedPages)
    {
      if (deletedPages == null)
        return;
      ISet<string> artifactIds = (ISet<string>) new HashSet<string>();
      deletedPages = this.GetDeletedPageIds(requestContext, wiki, deletedPages);
      deletedPages.ForEach<WikiPageChangeInfo>((Action<WikiPageChangeInfo>) (deletedPage => artifactIds.Add(WikiPageIdHelper.GetArtifactId(wiki.ProjectId, wiki.Id, deletedPage.PageId))));
      if (artifactIds.Count == 0)
        return;
      IVssFollowsService service = requestContext.GetService<IVssFollowsService>();
      IEnumerable<ArtifactSubscription> artifactSubscriptions = service.GetArtifactSubscriptions(requestContext, "WikiPageId", artifactIds.ToArray<string>());
      int[] array = artifactSubscriptions.Select<ArtifactSubscription, int>((Func<ArtifactSubscription, int>) (s => s.SubscriptionId)).ToArray<int>();
      if (artifactSubscriptions == null || !artifactSubscriptions.Any<ArtifactSubscription>())
        return;
      service.Unfollow(requestContext, (IEnumerable<int>) array);
    }
  }
}
