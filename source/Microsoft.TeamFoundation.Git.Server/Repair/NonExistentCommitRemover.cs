// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Repair.NonExistentCommitRemover
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server.Storage;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Repair
{
  internal class NonExistentCommitRemover
  {
    public static void Execute(IVssRequestContext rc, ITFLogger log, OdbId odbId)
    {
      Sha1Id sha1Id = Sha1Id.Empty;
      HashSet<Sha1Id> commits = new HashSet<Sha1Id>();
      using (Odb odb = DefaultGitDependencyRoot.Instance.CreateOdb(rc, odbId))
      {
        List<Sha1Id> allCommits;
        do
        {
          using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(odbId))
            allCommits = gitOdbComponent.GetAllCommits(new Sha1Id?(sha1Id));
          foreach (Sha1Id objectId in allCommits)
          {
            if (!odb.ContentDB.TryLookupObjectType(objectId, out GitPackObjectType _))
            {
              log.Info(string.Format("Marking SHA {0} for deletion from Odb {1}.", (object) objectId, (object) odbId));
              commits.Add(objectId);
            }
          }
          sha1Id = allCommits.LastOrDefault<Sha1Id>();
        }
        while (allCommits.Count > 0);
      }
      using (GitOdbComponent gitOdbComponent = rc.CreateGitOdbComponent(odbId))
        gitOdbComponent.DeleteCommits((IEnumerable<Sha1Id>) commits);
    }
  }
}
