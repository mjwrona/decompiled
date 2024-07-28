// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.GitVersionSpec
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using System;

namespace Microsoft.TeamFoundation.SourceControl.WebServer
{
  public abstract class GitVersionSpec
  {
    protected readonly string m_BranchTagOrCommitId;
    protected readonly char m_Identifier;

    public virtual string formatPath(string path) => throw new NotImplementedException();

    public virtual string ToDisplayText() => throw new NotImplementedException();

    public GitVersionSpec(string branchTagOrCommitId, char identifier)
    {
      this.m_BranchTagOrCommitId = string.IsNullOrEmpty(branchTagOrCommitId) ? "" : branchTagOrCommitId;
      this.m_Identifier = identifier;
    }

    public string ToVersionString() => GitVersionSpecCommon.GitIdentifierPrefix.ToString() + (object) this.m_Identifier + this.m_BranchTagOrCommitId;

    public string ToFriendlyName() => this.m_BranchTagOrCommitId;
  }
}
