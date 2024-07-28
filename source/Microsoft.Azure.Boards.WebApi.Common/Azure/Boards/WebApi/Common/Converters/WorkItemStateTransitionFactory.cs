// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemStateTransitionFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  public static class WorkItemStateTransitionFactory
  {
    public static WorkItemStateTransition Create(
      Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.WorkItemType wit,
      string from,
      string to,
      IDictionary<WorkItemTypeTransition, HashSet<string>> actions)
    {
      WorkItemTypeTransition key = new WorkItemTypeTransition()
      {
        From = from,
        To = to
      };
      HashSet<string> stringSet = (HashSet<string>) null;
      actions.TryGetValue(key, out stringSet);
      return new WorkItemStateTransition((ISecuredObject) wit)
      {
        To = to,
        Actions = (IEnumerable<string>) stringSet
      };
    }

    public static WorkItemNextStateOnTransition Create(WorkItemStateOnTransition workItemNextState) => new WorkItemNextStateOnTransition()
    {
      Id = workItemNextState.WorkItemId,
      StateOnTransition = workItemNextState.NextStateName,
      Message = workItemNextState.ErrorMessage,
      ErrorCode = workItemNextState.ErrorCode
    };
  }
}
