// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.AnnotatedTagPeeler
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;

namespace Microsoft.TeamFoundation.Git.Server
{
  public class AnnotatedTagPeeler
  {
    private readonly ITfsGitRepository m_repository;

    public AnnotatedTagPeeler(ITfsGitRepository repository) => this.m_repository = repository;

    public bool TryPeelTagRef(TfsGitRef tagRef, out TfsGitObject peeledObject)
    {
      peeledObject = (TfsGitObject) null;
      return tagRef.Name.StartsWith("refs/tags/", StringComparison.Ordinal) && this.m_repository.LookupObject(tagRef.ObjectId) is TfsGitTag tag && tag.TryPeelToNonTag(out peeledObject);
    }
  }
}
