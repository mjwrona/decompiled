// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.IterationCommitsData
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public struct IterationCommitsData
  {
    public IterationCommitsData(IList<GitCommitRef> commits, bool hasMore, IterationReason reason)
    {
      this.Commits = commits;
      this.HasMore = hasMore;
      this.Reason = reason;
    }

    public IList<GitCommitRef> Commits { get; }

    public bool HasMore { get; }

    public IterationReason Reason { get; }
  }
}
