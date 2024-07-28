// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.ProcessBacklogs
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Work.WebApi;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public class ProcessBacklogs
  {
    public ProcessBacklogDefinition RequirementBacklog { get; set; }

    public ProcessBacklogDefinition TaskBacklog { get; set; }

    public IReadOnlyCollection<ProcessBacklogDefinition> PortfolioBacklogs { get; set; }

    public IReadOnlyCollection<string> HiddenBacklogs { get; set; }

    public BugsBehavior BugsBehavior { get; set; }

    public IReadOnlyCollection<string> BugWorkItems { get; set; }
  }
}
