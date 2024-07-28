// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models.BacklogConfigurationExtensions
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8CB058E6-FA40-46B0-B867-53112DFAAB81
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Server.WebAccess.WorkItemTracking.Common.Models
{
  public static class BacklogConfigurationExtensions
  {
    public static IReadOnlyDictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>> GetTaskWorkItemTypeStateMap(
      this BacklogConfiguration backlogConfiguration)
    {
      HashSet<string> taskWits = backlogConfiguration.TaskBacklog.WorkItemTypes.ToHashSet<string>((IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
      return (IReadOnlyDictionary<string, IReadOnlyDictionary<string, WorkItemStateCategory>>) backlogConfiguration.WorkItemTypeMappedStates.Where<WorkItemTypeStateInfo>((Func<WorkItemTypeStateInfo, bool>) (t => taskWits.Contains(t.WorkItemTypeName))).ToDictionary<WorkItemTypeStateInfo, string, IReadOnlyDictionary<string, WorkItemStateCategory>>((Func<WorkItemTypeStateInfo, string>) (t => t.WorkItemTypeName), (Func<WorkItemTypeStateInfo, IReadOnlyDictionary<string, WorkItemStateCategory>>) (t => t.States), (IEqualityComparer<string>) TFStringComparer.WorkItemTypeName);
    }
  }
}
