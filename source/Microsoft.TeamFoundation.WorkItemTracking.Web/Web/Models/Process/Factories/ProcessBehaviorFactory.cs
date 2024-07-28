// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.ProcessBehaviorFactory
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
  internal static class ProcessBehaviorFactory
  {
    internal static ProcessBehavior Create(
      IVssRequestContext requestContext,
      Guid processId,
      Behavior behavior,
      GetBehaviorsExpand expand)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckForNull<Behavior>(behavior, nameof (behavior));
      IEnumerable<ProcessBehaviorField> processBehaviorFields = (IEnumerable<ProcessBehaviorField>) null;
      switch (expand)
      {
        case GetBehaviorsExpand.Fields:
          processBehaviorFields = ProcessBehaviorFieldFactory.Create(requestContext, behavior.LegacyBehaviorFields);
          break;
        case GetBehaviorsExpand.CombinedFields:
          IReadOnlyDictionary<string, ProcessFieldDefinition> legacyCombinedFields = behavior.GetLegacyCombinedFields(requestContext) as IReadOnlyDictionary<string, ProcessFieldDefinition>;
          processBehaviorFields = ProcessBehaviorFieldFactory.Create(requestContext, legacyCombinedFields);
          break;
      }
      return new ProcessBehavior()
      {
        Name = behavior.Name,
        Description = behavior.Description,
        ReferenceName = behavior.ReferenceName,
        Color = behavior.Color,
        Url = ProcessBehaviorFactory.GetLocationUrlForWorkItemBehavior(requestContext, processId, behavior.ReferenceName),
        Fields = processBehaviorFields,
        Inherits = ProcessBehaviorFactory.CreateReference(requestContext, processId, behavior.ParentBehavior),
        Customization = (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType) behavior.Customization,
        Rank = behavior.Rank
      };
    }

    internal static IEnumerable<ProcessBehavior> Create(
      IVssRequestContext requestContext,
      Guid processId,
      IEnumerable<Behavior> behaviors,
      GetBehaviorsExpand expand)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(processId, nameof (processId));
      ArgumentUtility.CheckForNull<IEnumerable<Behavior>>(behaviors, nameof (behaviors));
      return behaviors.Select<Behavior, ProcessBehavior>((Func<Behavior, ProcessBehavior>) (b => ProcessBehaviorFactory.Create(requestContext, processId, b, expand)));
    }

    internal static ProcessBehaviorReference CreateReference(
      IVssRequestContext requestContext,
      Guid processId,
      Behavior behavior)
    {
      if (behavior == null)
        return (ProcessBehaviorReference) null;
      return new ProcessBehaviorReference()
      {
        BehaviorRefName = behavior.ReferenceName,
        Url = ProcessBehaviorFactory.GetLocationUrlForWorkItemBehavior(requestContext, processId, behavior.ReferenceName)
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
