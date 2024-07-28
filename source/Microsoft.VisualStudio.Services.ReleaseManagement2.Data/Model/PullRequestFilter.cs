// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model.PullRequestFilter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Data, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A69B0FB7-3028-4162-BA38-794D80D7A49A
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.ReleaseManagement2.Data.dll

using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Data.Model
{
  public class PullRequestFilter
  {
    public string TargetBranch { get; set; }

    public IList<string> Tags { get; private set; }

    public PullRequestFilter() => this.Tags = (IList<string>) new List<string>();

    public PullRequestFilter(string targetBranch, IList<string> tags)
    {
      this.TargetBranch = targetBranch;
      this.Tags = tags;
    }
  }
}
