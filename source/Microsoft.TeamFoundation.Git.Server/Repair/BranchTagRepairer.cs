// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Repair.BranchTagRepairer
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Repair
{
  public class BranchTagRepairer
  {
    public static void Execute(IVssRequestContext rc, ITFLogger log, Guid repoId, string badRef)
    {
      ITeamFoundationGitRepositoryService service1 = rc.GetService<ITeamFoundationGitRepositoryService>();
      IInternalGitRefService service2 = rc.GetService<IInternalGitRefService>();
      IVssRequestContext requestContext = rc;
      Guid repositoryId = repoId;
      using (ITfsGitRepository repositoryById = service1.FindRepositoryById(requestContext, repositoryId))
      {
        TfsGitRef tfsGitRef = repositoryById.Refs.MatchingName(badRef);
        TfsGitCommit commit = repositoryById.LookupObject<TfsGitTag>(tfsGitRef.ObjectId).ResolveToCommit();
        string name = "refs/tags" + badRef.Substring("refs/heads".Length);
        TfsGitRefUpdateResultSet refUpdateResultSet = service2.UpdateRefs(rc, repoId, new List<TfsGitRefUpdateRequest>()
        {
          new TfsGitRefUpdateRequest(name, Sha1Id.Empty, tfsGitRef.ObjectId),
          new TfsGitRefUpdateRequest(badRef, tfsGitRef.ObjectId, commit.ObjectId)
        }, updateMode: GitRefUpdateMode.AllOrNone);
        if (refUpdateResultSet.CountSucceeded != 2)
          throw new Exception(string.Format("Failed to create the new ref. create:{0}, update:{1}", (object) refUpdateResultSet.Results[0].Status, (object) refUpdateResultSet.Results[1].Status));
      }
    }
  }
}
