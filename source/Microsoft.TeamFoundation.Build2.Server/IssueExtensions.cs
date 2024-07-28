// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Build2.Server.IssueExtensions
// Assembly: Microsoft.TeamFoundation.Build2.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 680FF5F5-CB5D-4078-8EFA-56C292BFDBB7
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Build2.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Build2.Server
{
  public static class IssueExtensions
  {
    public static Microsoft.TeamFoundation.Build.WebApi.Issue ToBuildIssue(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.Issue taskIssue,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<ISecuredObject>(securedObject, nameof (securedObject));
      if (taskIssue == null)
        return (Microsoft.TeamFoundation.Build.WebApi.Issue) null;
      Microsoft.TeamFoundation.Build.WebApi.Issue buildIssue = new Microsoft.TeamFoundation.Build.WebApi.Issue(securedObject)
      {
        Category = taskIssue.Category,
        Message = taskIssue.Message,
        Type = taskIssue.Type.ToBuildIssueType()
      };
      foreach (KeyValuePair<string, string> keyValuePair in (IEnumerable<KeyValuePair<string, string>>) taskIssue.Data)
        buildIssue.Data.Add(keyValuePair.Key, keyValuePair.Value);
      return buildIssue;
    }

    public static Microsoft.TeamFoundation.Build.WebApi.IssueType ToBuildIssueType(
      this Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType issueType)
    {
      return issueType != Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType.Error && issueType == Microsoft.TeamFoundation.DistributedTask.WebApi.IssueType.Warning ? Microsoft.TeamFoundation.Build.WebApi.IssueType.Warning : Microsoft.TeamFoundation.Build.WebApi.IssueType.Error;
    }
  }
}
