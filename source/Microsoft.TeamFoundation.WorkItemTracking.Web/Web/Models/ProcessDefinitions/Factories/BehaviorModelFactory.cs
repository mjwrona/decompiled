// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.BehaviorModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class BehaviorModelFactory
  {
    internal static BehaviorModel Create(
      IVssRequestContext requestContext,
      Guid processId,
      Behavior serverBehavior)
    {
      BehaviorModel behaviorModel = new BehaviorModel();
      behaviorModel.Id = serverBehavior.ReferenceName;
      WorkItemBehaviorReference behaviorReference;
      if (serverBehavior.ParentBehavior != null)
        behaviorReference = new WorkItemBehaviorReference()
        {
          Id = serverBehavior.ParentBehavior.ReferenceName,
          Url = BehaviorModelFactory.GetLocationUrlForType(requestContext, serverBehavior.ParentBehavior.ProcessId, (BaseWorkItemType) serverBehavior.ParentBehavior)
        };
      else
        behaviorReference = (WorkItemBehaviorReference) null;
      behaviorModel.Inherits = behaviorReference;
      behaviorModel.Color = serverBehavior.Color;
      behaviorModel.Rank = serverBehavior.Rank;
      behaviorModel.Abstract = serverBehavior.IsAbstract;
      behaviorModel.Description = serverBehavior.Description;
      behaviorModel.Overridden = serverBehavior.Overridden;
      behaviorModel.Name = serverBehavior.Name;
      behaviorModel.Url = BehaviorModelFactory.GetLocationUrlForType(requestContext, serverBehavior.ProcessId, (BaseWorkItemType) serverBehavior);
      return behaviorModel;
    }

    private static string GetLocationUrlForType(
      IVssRequestContext requestContext,
      Guid processId,
      BaseWorkItemType wit)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processDefinitions", WorkItemTrackingLocationIds.ProcessDefinitionBehavior, (object) new
      {
        processId = processId,
        behaviorReferenceName = wit.ReferenceName
      }).ToString();
    }
  }
}
