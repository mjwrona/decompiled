// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Converters.ReleaseWorkItemConverter
// Assembly: Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: AA75D202-9F5E-426B-B40F-64BEE45B1703
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.ReleaseManagement2.Artifact.Extensions.dll

using Microsoft.Azure.DevOps.ServiceEndpoints.Sdk.Server.Jira;
using Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Artifacts;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.ExternalProviders.GitHub.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.VisualStudio.Services.ReleaseManagement.Artifact.Extensions.Converters
{
  public static class ReleaseWorkItemConverter
  {
    public static IList<WorkItemRef> ToReleaseWorkItems(IList<ResourceRef> buildWorkItems) => buildWorkItems != null ? (IList<WorkItemRef>) buildWorkItems.Select<ResourceRef, WorkItemRef>(ReleaseWorkItemConverter.\u003C\u003EO.\u003C0\u003E__ToReleaseWorkItem ?? (ReleaseWorkItemConverter.\u003C\u003EO.\u003C0\u003E__ToReleaseWorkItem = new Func<ResourceRef, WorkItemRef>(ReleaseWorkItemConverter.ToReleaseWorkItem))).ToList<WorkItemRef>() : throw new ArgumentNullException(nameof (buildWorkItems));

    public static WorkItemRef ToReleaseWorkItem(this ResourceRef buildWorkItem)
    {
      if (buildWorkItem == null)
        throw new ArgumentNullException(nameof (buildWorkItem));
      return new WorkItemRef()
      {
        Id = buildWorkItem.Id,
        Url = buildWorkItem.Url
      };
    }

    public static WorkItemRef GitHubIssueToReleaseWorkItem(GitHubData.V4.Issue issue)
    {
      if (issue == null)
        throw new ArgumentNullException(nameof (issue));
      return new WorkItemRef()
      {
        Id = issue.Number,
        Title = issue.Title,
        State = issue.State.ToString(),
        Url = issue.Url?.AbsoluteUri
      };
    }

    public static WorkItemRef AzureDevWorkItemsToReleaseWorkItem(WorkItem workItem)
    {
      if (workItem == null)
        throw new ArgumentNullException(nameof (workItem));
      return new WorkItemRef()
      {
        Id = workItem.Id.ToString(),
        Title = workItem.Fields["System.Title"].ToString(),
        State = workItem.Fields["System.State"].ToString(),
        Url = workItem.Fields["Release.WorkItemWebUrl"].ToString()
      };
    }

    public static IList<WorkItemRef> JiraIssuesToReleaseWorkItems(
      JiraData.V2.Issue[] issues,
      Uri jiraBaseUrl)
    {
      if (issues == null)
        throw new ArgumentNullException(nameof (issues));
      return (IList<WorkItemRef>) ((IEnumerable<JiraData.V2.Issue>) issues).Select<JiraData.V2.Issue, WorkItemRef>((Func<JiraData.V2.Issue, WorkItemRef>) (issue => issue.JiraIssueToReleaseWorkItem(jiraBaseUrl))).ToList<WorkItemRef>();
    }

    public static WorkItemRef JiraIssueToReleaseWorkItem(
      this JiraData.V2.Issue issue,
      Uri jiraBaseUrl)
    {
      string str = issue != null ? issue.Key : throw new ArgumentNullException(nameof (issue));
      string uriString = string.Format((IFormatProvider) CultureInfo.InvariantCulture, JiraConstants.Url.jiraIssuesRelativeHtmlUrlFormat, (object) str);
      return new WorkItemRef()
      {
        Id = str,
        Title = issue.Fields.Summary,
        State = issue.Fields.Status.StatusCategory.Name,
        Url = new Uri(jiraBaseUrl, new Uri(uriString, UriKind.Relative)).ToString(),
        Type = issue.Fields.Issuetype.Name,
        Provider = "Jira"
      };
    }
  }
}
