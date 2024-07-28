// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalGitIssueComparer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  public class ExternalGitIssueComparer : IEqualityComparer<ExternalGitIssue>
  {
    public bool Equals(ExternalGitIssue x, ExternalGitIssue y) => string.Equals(x != null ? x.NodeId<ExternalGitIssue>() : (string) null, y != null ? y.NodeId<ExternalGitIssue>() : (string) null);

    public int GetHashCode(ExternalGitIssue obj)
    {
      string str = obj != null ? obj.NodeId<ExternalGitIssue>() : (string) null;
      return str == null ? 0 : str.GetHashCode();
    }

    private ExternalGitIssueComparer()
    {
    }

    public static ExternalGitIssueComparer Instance { get; } = new ExternalGitIssueComparer();
  }
}
