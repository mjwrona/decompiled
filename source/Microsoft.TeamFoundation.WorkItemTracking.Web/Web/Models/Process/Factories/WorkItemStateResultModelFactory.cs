// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories.WorkItemStateResultModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.VisualStudio.Services.Location.Server;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories
{
  internal static class WorkItemStateResultModelFactory
  {
    private static string GetLocationUrlForState(
      IVssRequestContext requestContext,
      Guid processId,
      string witName,
      WorkItemStateDefinition stateDefinition)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processes", WorkItemTrackingLocationIds.ProcessesStatesGuid, (object) new
      {
        processId = processId,
        witRefName = witName,
        stateId = stateDefinition.Id
      }).ToString();
    }

    internal static WorkItemStateResultModel Create(
      IVssRequestContext tfsRequestContext,
      Guid processId,
      string witRefName,
      WorkItemStateDefinition stateDefinition)
    {
      return new WorkItemStateResultModel()
      {
        Id = stateDefinition.Id,
        Name = stateDefinition.Name,
        Color = stateDefinition.Color,
        StateCategory = stateDefinition.StateCategory.ToString(),
        Order = stateDefinition.Order,
        Url = WorkItemStateResultModelFactory.GetLocationUrlForState(tfsRequestContext, processId, witRefName, stateDefinition),
        Hidden = stateDefinition.Hidden,
        CustomizationType = (Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType) stateDefinition.Customization
      };
    }

    internal static WorkItemStateResultModel ToProcessModel(this WorkItemStateResultModel state) => new WorkItemStateResultModel()
    {
      Id = state.Id,
      Name = state.Name,
      Color = state.Color,
      StateCategory = state.StateCategory,
      Order = state.Order,
      Url = state.Url,
      Hidden = state.Hidden,
      CustomizationType = state.CustomizationType
    };
  }
}
