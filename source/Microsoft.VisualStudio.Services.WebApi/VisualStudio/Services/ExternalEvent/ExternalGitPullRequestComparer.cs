// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ExternalEvent.ExternalGitPullRequestComparer
// Assembly: Microsoft.VisualStudio.Services.WebApi, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7B264323-C592-4F23-AB6B-55AEDC85864F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.WebApi.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ExternalEvent
{
  public class ExternalGitPullRequestComparer : IEqualityComparer<ExternalGitPullRequest>
  {
    public bool Equals(ExternalGitPullRequest x, ExternalGitPullRequest y) => string.Equals(x != null ? x.NodeId() : (string) null, y != null ? y.NodeId() : (string) null);

    public int GetHashCode(ExternalGitPullRequest obj)
    {
      string str = obj != null ? obj.NodeId() : (string) null;
      return str == null ? 0 : str.GetHashCode();
    }

    private ExternalGitPullRequestComparer()
    {
    }

    public static ExternalGitPullRequestComparer Instance { get; } = new ExternalGitPullRequestComparer();
  }
}
