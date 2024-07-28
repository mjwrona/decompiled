// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.SqlGitKnownFilesProvider
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Git.Server
{
  internal sealed class SqlGitKnownFilesProvider : IGitKnownFilesProvider
  {
    private readonly IVssRequestContext m_rc;
    private readonly OdbId m_odbId;
    private const int c_resetSqlBatchSize = 10000;

    public SqlGitKnownFilesProvider(IVssRequestContext rc, OdbId odbId)
    {
      this.m_rc = rc;
      this.m_odbId = odbId;
    }

    public IReadOnlyDictionary<string, KnownFile> Read()
    {
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId))
        return gitOdbComponent.ReadKnownFiles();
    }

    public IReadOnlyDictionary<string, KnownFileIntervalData> ReadInterval(
      DateTime? minCreatedDate,
      int? minIntervals)
    {
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId))
        return gitOdbComponent.ReadKnownFilesInterval(minCreatedDate, minIntervals);
    }

    public void ResetIntervals(OdbId oidDb)
    {
      List<string> list = this.ReadInterval(new DateTime?(DateTime.UtcNow), new int?(1)).Keys.ToList<string>();
      HashSet<string> increments = new HashSet<string>();
      foreach (IList<string> source in list.Batch<string>(10000))
        this.UpdateIntervals((ISet<string>) increments, (ISet<string>) source.ToHashSet<string>());
    }

    public void Update(IReadOnlyDictionary<string, KnownFile> creates)
    {
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId))
        gitOdbComponent.WriteKnownFiles(creates);
    }

    public void UpdateIntervals(ISet<string> increments, ISet<string> resets)
    {
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId))
        gitOdbComponent.WriteKnownFilesIntervals(increments, resets);
    }

    public void Delete(IEnumerable<string> filesToDelete)
    {
      using (GitOdbComponent gitOdbComponent = this.m_rc.CreateGitOdbComponent(this.m_odbId))
        gitOdbComponent.DeleteKnownFiles(filesToDelete);
    }
  }
}
