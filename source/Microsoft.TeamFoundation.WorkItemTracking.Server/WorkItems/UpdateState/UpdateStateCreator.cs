// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState.UpdateStateCreator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState
{
  public class UpdateStateCreator
  {
    internal static IEnumerable<WorkItemUpdateState> Create(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdate> workItemUpdates,
      WorkItemUpdateRuleExecutionMode ruleExecutionMode,
      WorkItemUpdateMode updateMode,
      bool isNoHistoryEnabledFieldsSupported)
    {
      ArgumentUtility.CheckForNull<WorkItemTrackingRequestContext>(witRequestContext, nameof (witRequestContext));
      ArgumentUtility.CheckForNull<IEnumerable<WorkItemUpdate>>(workItemUpdates, nameof (workItemUpdates));
      List<WorkItemUpdateState> workItemUpdateStateList = new List<WorkItemUpdateState>();
      foreach (WorkItemUpdate workItemUpdate in workItemUpdates)
      {
        if (workItemUpdate == null)
          throw new ArgumentException("Null work item update object.", nameof (workItemUpdates));
        if (workItemUpdate.Id == 0)
          throw new ArgumentException("Work item id of update cannot be 0.", nameof (workItemUpdates));
        if (workItemUpdate.IsNew && (workItemUpdate.Fields == null || !workItemUpdate.Fields.Any<KeyValuePair<string, object>>()))
          throw new ArgumentException("New work item updates must specify Area and Iteration node ids.", nameof (workItemUpdates));
        if (workItemUpdate.ResourceLinkUpdates != null)
        {
          foreach (WorkItemResourceLinkUpdate resourceLinkUpdate in workItemUpdate.ResourceLinkUpdates)
          {
            ResourceLinkType? type = resourceLinkUpdate.Type;
            ResourceLinkType resourceLinkType = ResourceLinkType.ArtifactLink;
            if (type.GetValueOrDefault() == resourceLinkType & type.HasValue && string.IsNullOrEmpty(resourceLinkUpdate.Name) && resourceLinkUpdate.UpdateType != LinkUpdateType.Delete)
              throw new ArgumentException("Artifact links must have a valid name specified.", nameof (workItemUpdates));
          }
        }
        workItemUpdateStateList.Add(new WorkItemUpdateState(witRequestContext, workItemUpdate, isNoHistoryEnabledFieldsSupported));
      }
      if (workItemUpdateStateList.Count > 200)
        throw new WorkItemUpdateBatchLimitExceeded(workItemUpdateStateList.Count);
      IGrouping<int, WorkItemUpdateState> grouping = workItemUpdateStateList.ToLookup<WorkItemUpdateState, int>((Func<WorkItemUpdateState, int>) (us => us.Id)).FirstOrDefault<IGrouping<int, WorkItemUpdateState>>((Func<IGrouping<int, WorkItemUpdateState>, bool>) (g => g.Take<WorkItemUpdateState>(2).Count<WorkItemUpdateState>() > 1));
      if (grouping != null)
      {
        if (grouping.Key < 0)
          throw new ArgumentException(ServerResources.UpdateDuplicateTempIdsInUpdateXmlException(), nameof (workItemUpdates));
        throw new ArgumentException(ServerResources.UpdateWorkItemMultipleTimes((object) grouping.Key.ToString((IFormatProvider) CultureInfo.InvariantCulture)), nameof (workItemUpdates));
      }
      FieldUpdateValidator.Validate(witRequestContext, (IEnumerable<WorkItemUpdateState>) workItemUpdateStateList, ruleExecutionMode, updateMode);
      LinkUpdateValidator.Validate(witRequestContext, workItemUpdateStateList.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)));
      ResourceLinkUpdateValidator.Validate(witRequestContext, workItemUpdateStateList.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.Success)));
      return (IEnumerable<WorkItemUpdateState>) workItemUpdateStateList;
    }
  }
}
