// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.WorkItemTypeBehaviorModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class WorkItemTypeBehaviorModelFactory
  {
    internal static WorkItemTypeBehavior Create(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      BehaviorRelation behaviorRelation)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<BehaviorRelation>(behaviorRelation, nameof (behaviorRelation));
      return new WorkItemTypeBehavior()
      {
        Behavior = WorkItemBehaviorReferenceFactory.CreateReference(requestContext, processId, behaviorRelation.Behavior),
        IsDefault = behaviorRelation.IsDefault,
        Url = WorkItemTypeBehaviorModelFactory.GetLocationUrlForWorkItemTypeBehavior(requestContext, processId, witRefName, behaviorRelation.Behavior.ReferenceName)
      };
    }

    internal static IEnumerable<WorkItemTypeBehavior> Create(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      IReadOnlyCollection<BehaviorRelation> behaviors)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(witRefName, nameof (witRefName));
      ArgumentUtility.CheckForNull<IReadOnlyCollection<BehaviorRelation>>(behaviors, nameof (behaviors));
      return behaviors.Select<BehaviorRelation, WorkItemTypeBehavior>((Func<BehaviorRelation, WorkItemTypeBehavior>) (b => WorkItemTypeBehaviorModelFactory.Create(requestContext, processId, witRefName, b)));
    }

    private static string GetLocationUrlForWorkItemTypeBehavior(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName,
      string behaviorRefName)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processDefinitions", WorkItemTrackingLocationIds.ProcessDefinitionWorkItemTypeBehaviors, (object) new
      {
        processId = processId,
        witRefNameForBehaviors = witRefName,
        behaviorRefName = behaviorRefName
      }).AbsoluteUri;
    }
  }
}
