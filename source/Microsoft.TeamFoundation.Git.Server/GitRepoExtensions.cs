// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.GitRepoExtensions
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class GitRepoExtensions
  {
    public static List<TfsGitPushMetadata> QueryPushHistory(
      this ITfsGitRepository repo,
      bool includeRefUpdates,
      DateTime? fromDate,
      DateTime? toDate,
      Guid? pusherId,
      int? skip,
      int? take,
      string refName)
    {
      List<Guid> pusherIds = new List<Guid>();
      if (pusherId.HasValue)
        pusherIds.Add(pusherId.Value);
      return repo.QueryPushHistory(includeRefUpdates, fromDate, toDate, (IEnumerable<Guid>) pusherIds, false, skip, take, refName);
    }
  }
}
