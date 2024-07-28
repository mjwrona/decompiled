// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter.GitObjectFilter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server.BusinessIntelligence;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server.Protocol.UploadPack.Filter
{
  internal class GitObjectFilter
  {
    public GitObjectFilter()
    {
      this.AllowBlobs = true;
      this.AllowTrees = true;
    }

    public bool Include(GitObjectType objType) => (this.AllowTrees || objType != GitObjectType.Tree) && (this.AllowBlobs && this.AllowTrees || objType != GitObjectType.Blob);

    public bool Include(Sha1Id objectId, GitObjectType objType, ISet<Sha1Id> wants) => this.FilterIsNoop() || wants.Contains(objectId) || this.Include(objType);

    public ISet<Sha1Id> Filter(ISet<Sha1Id> objects, ISet<Sha1Id> wants, ITfsGitRepository repo)
    {
      if (this.FilterIsNoop())
        return objects;
      if (objects == null)
        return (ISet<Sha1Id>) null;
      HashSet<Sha1Id> sha1IdSet = new HashSet<Sha1Id>(objects.Count);
      foreach (Sha1Id objectId in (IEnumerable<Sha1Id>) objects)
      {
        if (wants.Contains(objectId))
          sha1IdSet.Add(objectId);
        else if (this.Include(repo.TryLookupObjectType(objectId)))
          sha1IdSet.Add(objectId);
      }
      return (ISet<Sha1Id>) sha1IdSet;
    }

    public void ReportClientTraceData(ClientTraceData ctData)
    {
      ctData?.Add("AllowTrees", (object) this.AllowTrees);
      ctData?.Add("AllowBlobs", (object) this.AllowBlobs);
    }

    public bool FilterIsNoop() => this.AllowTrees && this.AllowBlobs;

    internal bool AllowTrees { get; set; }

    internal bool AllowBlobs { get; set; }
  }
}
