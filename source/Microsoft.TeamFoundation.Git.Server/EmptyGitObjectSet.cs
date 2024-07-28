// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.EmptyGitObjectSet
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal class EmptyGitObjectSet : IGitObjectSet, IDisposable
  {
    public static readonly EmptyGitObjectSet Instance = new EmptyGitObjectSet();

    private EmptyGitObjectSet()
    {
    }

    public void Dispose()
    {
    }

    public bool TryGetContent(Sha1Id objectId, out Stream content, out GitObjectType objectType)
    {
      content = (Stream) null;
      objectType = GitObjectType.Bad;
      return false;
    }

    public GitObjectType TryLookupObjectType(Sha1Id objectId) => GitObjectType.Bad;

    public TfsGitObject TryLookupObject(Sha1Id objectId) => (TfsGitObject) null;

    public IEnumerable<TGitObject> FindObjectsBetween<TGitObject>(Sha1Id fromId, Sha1Id toId) where TGitObject : TfsGitObject => Enumerable.Empty<TGitObject>();
  }
}
