// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Gallery.Server.CSR.CSRGitHubIssuePayload
// Assembly: Microsoft.VisualStudio.Services.Gallery.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B9EBBED5-135E-45CD-B0B4-F747360599CD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Gallery.Server.dll

using Newtonsoft.Json;

namespace Microsoft.VisualStudio.Services.Gallery.Server.CSR
{
  public class CSRGitHubIssuePayload
  {
    [JsonProperty("title")]
    public string Title { get; set; }

    [JsonProperty("body")]
    public string IssueContent { get; set; }

    [JsonProperty("assignees")]
    public string[] IssueAssignees { get; set; }

    [JsonProperty("labels")]
    public string[] IssueLabels { get; set; }

    public CSRGitHubIssuePayload(
      string title,
      string issueContent,
      string[] issueAssignees,
      string[] issueLabels)
    {
      this.Title = title;
      this.IssueContent = issueContent;
      this.IssueAssignees = issueAssignees;
      this.IssueLabels = issueLabels;
    }
  }
}
