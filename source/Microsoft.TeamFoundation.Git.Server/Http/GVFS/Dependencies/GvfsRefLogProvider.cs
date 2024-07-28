// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.Http.GVFS.Dependencies.GvfsRefLogProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server.Http.GVFS.Dependencies
{
  internal class GvfsRefLogProvider : IGvfsRefLogProvider
  {
    private readonly ITfsGitRepository m_repo;

    public GvfsRefLogProvider(ITfsGitRepository repo) => this.m_repo = repo;

    public IEnumerable<TfsGitRefLogEntry> ReadRefLogSince(long lastTimestamp) => this.m_repo.QueryPushHistory(true, new DateTime?(GitServerConstants.UtcEpoch.AddSeconds((double) lastTimestamp)), new DateTime?(), new Guid?(), new int?(0), new int?(int.MaxValue), (string) null).SelectMany<TfsGitPushMetadata, TfsGitRefLogEntry>((Func<TfsGitPushMetadata, IEnumerable<TfsGitRefLogEntry>>) (x => (IEnumerable<TfsGitRefLogEntry>) x.RefLog));
  }
}
