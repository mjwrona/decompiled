// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemDependencyGraph
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  public class WorkItemDependencyGraph : WorkItemSecuredObject
  {
    public int SourceId { get; set; }

    public int TargetId { get; set; }

    public Guid SourceProjectId { get; set; }

    public Guid TargetProjectId { get; set; }

    public Guid SourceIterationId { get; set; }

    public Guid TargetIterationId { get; set; }

    public int LinkType { get; set; }

    public string SourceWorkItemType { get; set; }

    public string TargetWorkItemType { get; set; }

    public DateTime? SourceTargetDate { get; set; }

    public DateTime? TargetTargetDate { get; set; }

    public bool IterationError { get; set; }

    public bool TargetDateError { get; set; }

    public string SourceName { get; set; }

    public string TargetName { get; set; }

    public int RootWorkItemId { get; set; }
  }
}
