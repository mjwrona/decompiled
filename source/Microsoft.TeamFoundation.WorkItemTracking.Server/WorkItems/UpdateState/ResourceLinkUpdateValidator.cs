// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState.ResourceLinkUpdateValidator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState
{
  public class ResourceLinkUpdateValidator
  {
    internal static void Validate(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.HasResourceLinkUpdates)))
      {
        foreach (WorkItemResourceLinkUpdate resourceLinkUpdate in workItemUpdateState.Update.ResourceLinkUpdates)
        {
          if (ResourceLinkUpdateValidator.IsLinkMissing(resourceLinkUpdate))
            workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new ResourceLinkNotFoundException(workItemUpdateState.Id));
          else if (ResourceLinkUpdateValidator.IsLinkTypeUnspecified(resourceLinkUpdate))
            workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new ResourceLinkTypeUnspecifiedException(workItemUpdateState.Id));
          else if (ResourceLinkUpdateValidator.IsLinkTargetUnspecified(resourceLinkUpdate))
            workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new ResourceLinkTargetUnspecifiedException(workItemUpdateState.Id));
          else if (ResourceLinkUpdateValidator.IsLinkLengthValid(resourceLinkUpdate))
            workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new ResourceLinkLengthInvalidException(workItemUpdateState.Id));
        }
      }
    }

    private static bool IsLinkMissing(WorkItemResourceLinkUpdate resourceLinkUpdate) => (resourceLinkUpdate.UpdateType == LinkUpdateType.Update || resourceLinkUpdate.UpdateType == LinkUpdateType.Delete) && !resourceLinkUpdate.ResourceId.HasValue;

    private static bool IsLinkTypeUnspecified(WorkItemResourceLinkUpdate resourceLinkUpdate) => resourceLinkUpdate.UpdateType == LinkUpdateType.Add && !resourceLinkUpdate.Type.HasValue;

    private static bool IsLinkTargetUnspecified(WorkItemResourceLinkUpdate resourceLinkUpdate) => resourceLinkUpdate.UpdateType == LinkUpdateType.Add && resourceLinkUpdate.Location == null;

    public static bool IsLinkLengthValid(WorkItemResourceLinkUpdate resourceLinkUpdate) => resourceLinkUpdate.Length.HasValue && resourceLinkUpdate.Length.Value < 0;
  }
}
