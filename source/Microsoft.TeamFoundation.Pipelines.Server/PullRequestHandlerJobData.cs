// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Pipelines.Server.PullRequestHandlerJobData
// Assembly: Microsoft.TeamFoundation.Pipelines.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07451E6B-67F8-4956-AC64-CC041BD809B5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Pipelines.Server.dll

using Microsoft.VisualStudio.Services.ExternalEvent;
using Newtonsoft.Json.Linq;
using System;

namespace Microsoft.TeamFoundation.Pipelines.Server
{
  public class PullRequestHandlerJobData
  {
    public Guid HostId { get; set; }

    public JObject Authentication { get; set; }

    public string ProviderId { get; set; }

    public ExternalGitPullRequest PullRequest { get; set; }

    public ExternalPullRequestCommentEvent CommentEvent { get; set; }

    public Guid? ProjectId { get; set; }

    public int? DefinitionId { get; set; }

    public int ExecutionCount { get; set; }

    public bool HasMergeConflicts { get; set; }
  }
}
