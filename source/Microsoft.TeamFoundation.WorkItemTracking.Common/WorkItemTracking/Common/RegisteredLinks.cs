// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Common.RegisteredLinks
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E674B254-26A0-4D88-8FF3-86800090789C
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.WorkItemTracking.Common.dll

using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Microsoft.TeamFoundation.WorkItemTracking.Common
{
  [EditorBrowsable(EditorBrowsableState.Never)]
  [StructLayout(LayoutKind.Sequential, Size = 1)]
  public struct RegisteredLinks
  {
    public const string Related = "Related Workitem";
    public const string Hyperlink = "Workitem Hyperlink";
    public const string Changeset = "Fixed in Changeset";
    public const string SourceCode = "Source Code File";
    public const string TestResult = "Test Result";
    public const string Storyboard = "Storyboard";
    public const string GitHubCommit = "GitHub Commit";
    public const string GitHubPullRequest = "GitHub Pull Request";
    public const string GitHubIssue = "GitHub Issue";
  }
}
