// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitConflictResolverInternalException
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using LibGit2Sharp;
using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Git.Server
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [Serializable]
  internal class GitConflictResolverInternalException : TeamFoundationServiceException
  {
    internal GitConflictResolverInternalException(Conflict nativeConflict)
      : base(GitConflictResolverInternalException.GetExceptionMessage(nativeConflict))
    {
    }

    private static string GetExceptionMessage(Conflict nativeConflict)
    {
      if (nativeConflict == (Conflict) null)
        return "nativeConflict: null";
      return "ours: " + GitConflictResolverInternalException.GetIndexMessage(nativeConflict.Ours) + ", theirs: " + GitConflictResolverInternalException.GetIndexMessage(nativeConflict.Theirs) + ", ancestor: " + GitConflictResolverInternalException.GetIndexMessage(nativeConflict.Ancestor);
    }

    private static string GetIndexMessage(LibGit2Sharp.IndexEntry indexEntry) => indexEntry == (LibGit2Sharp.IndexEntry) null ? "null" : string.Format("<{0}> <{1}>", (object) indexEntry.Id, (object) indexEntry.Path);
  }
}
