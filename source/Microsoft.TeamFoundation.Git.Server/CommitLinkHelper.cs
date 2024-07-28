// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CommitLinkHelper
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class CommitLinkHelper
  {
    private static readonly LinkFilter[] s_queryLinkFilter = new LinkFilter[1]
    {
      new LinkFilter()
      {
        FilterType = FilterType.ToolType,
        FilterValues = new string[2]
        {
          "WorkItemTracking",
          "Git"
        }
      }
    };

    public static Dictionary<Sha1Id, List<int>> GetWorkItemIdsLinkedToCommits(
      IVssRequestContext rc,
      RepoKey repoKey,
      IList<Sha1Id> commitIds)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(rc, nameof (rc));
      ArgumentUtility.CheckForNull<RepoKey>(repoKey, nameof (repoKey));
      ArgumentUtility.CheckForNull<IList<Sha1Id>>(commitIds, nameof (commitIds));
      string[] strArray = new string[commitIds.Count];
      for (int index = 0; index < commitIds.Count; ++index)
        strArray[index] = GitCommitArtifactId.GetArtifactUriForCommit(repoKey, commitIds[index]);
      IWorkItemArtifactUriQueryRemotableService service = rc.GetService<IWorkItemArtifactUriQueryRemotableService>();
      ArtifactUriQuery artifactUriQuery1 = new ArtifactUriQuery()
      {
        ArtifactUris = (IEnumerable<string>) strArray
      };
      IVssRequestContext requestContext = rc;
      ArtifactUriQuery artifactUriQuery2 = artifactUriQuery1;
      IDictionary<string, IEnumerable<WorkItemReference>> artifactUrisQueryResult = service.QueryWorkItemsForArtifactUris(requestContext, artifactUriQuery2).ArtifactUrisQueryResult;
      Dictionary<Sha1Id, List<int>> idsLinkedToCommits = new Dictionary<Sha1Id, List<int>>();
      foreach (KeyValuePair<string, IEnumerable<WorkItemReference>> keyValuePair in (IEnumerable<KeyValuePair<string, IEnumerable<WorkItemReference>>>) artifactUrisQueryResult)
      {
        Sha1Id idFromArtifactUri = GitCommitArtifactId.GetCommitIdFromArtifactUri(keyValuePair.Key);
        List<int> list = keyValuePair.Value.Select<WorkItemReference, int>((Func<WorkItemReference, int>) (workItemRef => workItemRef.Id)).ToList<int>();
        idsLinkedToCommits[idFromArtifactUri] = list;
      }
      return idsLinkedToCommits;
    }
  }
}
