// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Events.TeamSettingsChangeType
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using System;
using System.ComponentModel;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Events
{
  [Flags]
  [EditorBrowsable(EditorBrowsableState.Never)]
  public enum TeamSettingsChangeType
  {
    UpdateTeamFields = 1,
    UpdateBugsBehavior = 2,
    UpdateDefaultIteration = 4,
    UpdateCumulativeFlowDiagram = 8,
    UpdateBacklogIterations = 16, // 0x00000010
    UpdateTeamIterationCapacity = 32, // 0x00000020
    UpdateTeamIterationDaysOff = 64, // 0x00000040
    UpdateBacklogVisibilities = 128, // 0x00000080
    UpdateTeamWeekends = 256, // 0x00000100
    Delete = 512, // 0x00000200
    Update = UpdateTeamWeekends | UpdateBacklogVisibilities | UpdateTeamIterationDaysOff | UpdateTeamIterationCapacity | UpdateBacklogIterations | UpdateCumulativeFlowDiagram | UpdateDefaultIteration | UpdateBugsBehavior | UpdateTeamFields, // 0x000001FF
  }
}
