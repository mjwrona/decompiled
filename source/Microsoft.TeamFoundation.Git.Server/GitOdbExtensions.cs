// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitOdbExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitOdbExtensions
  {
    public static Stream GetContent(this ITfsGitRepository repo, Sha1Id objectId) => repo.Objects.GetContent(objectId, out GitObjectType _);

    public static Stream GetContent(
      this ITfsGitRepository repo,
      Sha1Id objectId,
      out GitObjectType contentType)
    {
      return repo.Objects.GetContent(objectId, out contentType);
    }

    public static IEnumerable<T> FindObjectsBetween<T>(
      this ITfsGitRepository repo,
      Sha1Id fromId,
      Sha1Id toId)
      where T : TfsGitObject
    {
      return repo.Objects.FindObjectsBetween<T>(fromId, toId);
    }

    public static TfsGitObject LookupObject(this ITfsGitRepository repo, Sha1Id objectId) => repo.Objects.LookupObject(objectId);

    public static T LookupObject<T>(this ITfsGitRepository repo, Sha1Id objectId) where T : TfsGitObject => repo.Objects.LookupObject<T>(objectId);

    public static GitObjectType LookupObjectType(this ITfsGitRepository repo, Sha1Id objectId) => repo.Objects.LookupObjectType(objectId);

    public static TfsGitObject TryLookupObject(this ITfsGitRepository repo, Sha1Id objectId) => repo.Objects.TryLookupObject(objectId);

    public static T TryLookupObject<T>(this ITfsGitRepository repo, Sha1Id objectId) where T : TfsGitObject => repo.Objects.TryLookupObject<T>(objectId);

    public static GitObjectType TryLookupObjectType(this ITfsGitRepository repo, Sha1Id objectId) => repo.Objects.TryLookupObjectType(objectId);
  }
}
