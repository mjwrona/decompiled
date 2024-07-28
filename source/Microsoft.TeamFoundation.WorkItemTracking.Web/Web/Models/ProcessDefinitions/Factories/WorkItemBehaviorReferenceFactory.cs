// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.WorkItemBehaviorReferenceFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class WorkItemBehaviorReferenceFactory
  {
    internal static WorkItemBehaviorReference CreateReference(
      IVssRequestContext requestContext,
      Guid processId,
      Behavior behavior)
    {
      if (behavior == null)
        return (WorkItemBehaviorReference) null;
      return new WorkItemBehaviorReference()
      {
        Id = behavior.ReferenceName,
        Url = WorkItemBehaviorReferenceFactory.GetLocationUrlForWorkItemBehavior(requestContext, processId, behavior.ReferenceName)
      };
    }

    internal static string GetLocationUrlForWorkItemBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string behaviorRefName)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processes", WorkItemTrackingLocationIds.ProcessBehaviors, (object) new
      {
        processId = processId,
        behaviorRefName = behaviorRefName
      }).AbsoluteUri;
    }
  }
}
