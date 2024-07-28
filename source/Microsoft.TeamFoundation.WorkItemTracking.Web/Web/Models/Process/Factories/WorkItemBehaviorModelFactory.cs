// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.WorkItemBehaviorModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  internal static class WorkItemBehaviorModelFactory
  {
    internal static WorkItemBehavior Create(
      IVssRequestContext requestContext,
      Guid processId,
      Behavior behavior,
      bool includeFields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckForNull<Behavior>(behavior, nameof (behavior));
      return new WorkItemBehavior()
      {
        Name = behavior.Name,
        Description = behavior.Description,
        Id = behavior.ReferenceName,
        Abstract = behavior.IsAbstract,
        Overriden = behavior.Overridden,
        Color = behavior.Color,
        Url = WorkItemBehaviorModelFactory.GetLocationUrlForWorkItemBehavior(requestContext, processId, behavior.ReferenceName),
        Fields = includeFields ? WorkItemBehaviorFieldModelFactory.Create(requestContext, behavior.LegacyBehaviorFields) : (IEnumerable<WorkItemBehaviorField>) null,
        Rank = behavior.Rank,
        Inherits = WorkItemBehaviorModelFactory.CreateReference(requestContext, processId, behavior.ParentBehavior)
      };
    }

    internal static IEnumerable<WorkItemBehavior> Create(
      IVssRequestContext requestContext,
      Guid processId,
      IEnumerable<Behavior> behaviors,
      bool includeFields)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckForNull<IEnumerable<Behavior>>(behaviors, nameof (behaviors));
      return behaviors.Select<Behavior, WorkItemBehavior>((Func<Behavior, WorkItemBehavior>) (b => WorkItemBehaviorModelFactory.Create(requestContext, processId, b, includeFields)));
    }

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
        Url = WorkItemBehaviorModelFactory.GetLocationUrlForWorkItemBehavior(requestContext, processId, behavior.ReferenceName)
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
