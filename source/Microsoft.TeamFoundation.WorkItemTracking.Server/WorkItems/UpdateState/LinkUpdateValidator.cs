// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState.LinkUpdateValidator
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.UpdateState
{
  public class LinkUpdateValidator
  {
    internal static void Validate(
      WorkItemTrackingRequestContext witRequestContext,
      IEnumerable<WorkItemUpdateState> updateStates)
    {
      Lazy<bool> lazy = new Lazy<bool>((Func<bool>) (() => CommonWITUtils.IsRemoteLinkingEnabled(witRequestContext.RequestContext)));
      foreach (WorkItemUpdateState workItemUpdateState in updateStates.Where<WorkItemUpdateState>((Func<WorkItemUpdateState, bool>) (us => us.HasLinkUpdates)))
      {
        foreach (WorkItemLinkUpdate linkUpdate in workItemUpdateState.Update.LinkUpdates)
        {
          MDWorkItemLinkType linkType;
          if (!witRequestContext.LinkService.TryGetLinkTypeById(witRequestContext.RequestContext, linkUpdate.LinkType, out linkType))
          {
            workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemTrackingLinkTypeNotFoundException(linkUpdate.LinkType));
            break;
          }
          string linkName = linkUpdate.LinkType == linkType.ForwardId ? linkType.ForwardEndName : linkType.ReverseEndName;
          if (!linkType.Enabled)
          {
            workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkTypeDisabledException(workItemUpdateState.Id, linkUpdate.LinkType, linkUpdate.SourceWorkItemId, linkUpdate.TargetWorkItemId, linkName));
            break;
          }
          if (linkType.IsRemote)
          {
            if (!lazy.Value)
              workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkTypeDisabledException(linkUpdate.SourceWorkItemId, linkUpdate.LinkType, linkUpdate.SourceWorkItemId, linkUpdate.TargetWorkItemId, linkName));
            if (linkUpdate.Locked.GetValueOrDefault(false))
            {
              workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkAddRemoteLinkLocked(linkUpdate.SourceWorkItemId, linkUpdate.LinkType, linkName, linkUpdate.SourceWorkItemId, linkUpdate.TargetWorkItemId));
              break;
            }
            if (object.Equals((object) linkUpdate.RemoteHostId, (object) witRequestContext.RequestContext.ServiceHost.InstanceId))
            {
              workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkAddRemoteLinkToSameAccount(linkUpdate.SourceWorkItemId, linkUpdate.LinkType, linkName));
              break;
            }
            if (!linkUpdate.RemoteHostId.HasValue || object.Equals((object) linkUpdate.RemoteHostId, (object) Guid.Empty))
            {
              workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkAddRemoteLinkMissingAccount(linkUpdate.SourceWorkItemId, linkUpdate.LinkType, linkName));
              break;
            }
          }
          else
          {
            if (linkUpdate.SourceWorkItemId == linkUpdate.TargetWorkItemId)
            {
              workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkAddLinkToSelf(linkUpdate.TargetWorkItemId, linkUpdate.LinkType, linkName));
              break;
            }
            if (linkUpdate.SourceWorkItemId != workItemUpdateState.Id && linkUpdate.TargetWorkItemId != workItemUpdateState.Id)
              workItemUpdateState.UpdateResult.AddException((TeamFoundationServiceException) new WorkItemLinkInvalidEndsException(workItemUpdateState.Id, linkUpdate.LinkType, linkName, linkUpdate.SourceWorkItemId, linkUpdate.TargetWorkItemId));
          }
          if (linkUpdate.Comment != null && linkUpdate.Comment.Length > (int) byte.MaxValue)
            linkUpdate.Comment = linkUpdate.Comment.Substring(0, (int) byte.MaxValue);
        }
      }
    }
  }
}
