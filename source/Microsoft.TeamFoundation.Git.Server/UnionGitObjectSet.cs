// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.UnionGitObjectSet
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Git.Server.TfsGitObjects;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class UnionGitObjectSet : ICachedGitObjectSet, IGitObjectSet, IDisposable
  {
    private readonly IReadOnlyList<ICachedGitObjectSet> m_subsets;

    public UnionGitObjectSet(List<ICachedGitObjectSet> subsets) => this.m_subsets = (IReadOnlyList<ICachedGitObjectSet>) subsets;

    public void Dispose()
    {
    }

    public IEnumerable<TGitObject> FindObjectsBetween<TGitObject>(Sha1Id fromId, Sha1Id toId) where TGitObject : TfsGitObject => throw new NotImplementedException();

    public void TryCacheObjectCore(Sha1Id objectId, IGitObjectCore value)
    {
      foreach (ICachedGitObjectSet subset in (IEnumerable<ICachedGitObjectSet>) this.m_subsets)
      {
        if (subset.TryLookupObject(objectId) != null)
        {
          subset.TryCacheObjectCore(objectId, value);
          break;
        }
      }
    }

    public bool TryGetContent(Sha1Id objectId, out Stream content, out GitObjectType objectType)
    {
      foreach (IGitObjectSet subset in (IEnumerable<ICachedGitObjectSet>) this.m_subsets)
      {
        if (subset.TryGetContent(objectId, out content, out objectType))
          return true;
      }
      content = (Stream) null;
      objectType = GitObjectType.Bad;
      return false;
    }

    public bool TryGetObjectCoreFromCache(Sha1Id objectId, out IGitObjectCore value)
    {
      foreach (ICachedGitObjectSet subset in (IEnumerable<ICachedGitObjectSet>) this.m_subsets)
      {
        if (subset.TryGetObjectCoreFromCache(objectId, out value))
          return true;
      }
      value = (IGitObjectCore) null;
      return false;
    }

    public GitObjectType TryLookupObjectType(Sha1Id objectId)
    {
      foreach (IGitObjectSet subset in (IEnumerable<ICachedGitObjectSet>) this.m_subsets)
      {
        GitObjectType gitObjectType = subset.TryLookupObjectType(objectId);
        if (gitObjectType != GitObjectType.Bad)
          return gitObjectType;
      }
      return GitObjectType.Bad;
    }
  }
}
