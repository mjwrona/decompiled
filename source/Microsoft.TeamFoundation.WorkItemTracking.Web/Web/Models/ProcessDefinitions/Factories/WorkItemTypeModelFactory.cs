// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories.WorkItemTypeModelFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.ProcessDefinitions.Factories
{
  internal static class WorkItemTypeModelFactory
  {
    internal static WorkItemTypeModel Create(
      IVssRequestContext requestContext,
      ProcessWorkItemType wit,
      IReadOnlyCollection<WorkItemStateDefinition> states = null,
      IReadOnlyCollection<BehaviorRelation> behaviors = null,
      Layout layout = null)
    {
      return new WorkItemTypeModel()
      {
        Id = wit.ReferenceName,
        Name = wit.Name,
        Description = wit.Description,
        Inherits = wit.IsDerived ? wit.ParentTypeRefName : (string) null,
        Class = WorkItemTypeModelFactory.GetWorkItemTypeClass(wit.IsDerived, wit.IsCustomType),
        Color = wit.Color,
        Icon = wit.Icon,
        States = states != null ? states.Select<WorkItemStateDefinition, WorkItemStateResultModel>((Func<WorkItemStateDefinition, WorkItemStateResultModel>) (s => WorkItemStateResultModelFactory.Create(requestContext, wit.ProcessId, wit.ReferenceName, s))) : (IEnumerable<WorkItemStateResultModel>) null,
        Behaviors = behaviors != null ? (IReadOnlyCollection<WorkItemTypeBehavior>) behaviors.Select<BehaviorRelation, WorkItemTypeBehavior>((Func<BehaviorRelation, WorkItemTypeBehavior>) (b => WorkItemTypeBehaviorModelFactory.Create(requestContext, wit.ProcessId, wit.ReferenceName, b))).ToList<WorkItemTypeBehavior>() : (IReadOnlyCollection<WorkItemTypeBehavior>) null,
        IsDisabled = new bool?(wit.IsDisabled),
        Url = WorkItemTypeModelFactory.GetLocationUrlForType(requestContext, wit),
        Layout = layout != null ? layout.GetWebApiModel() : (Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.FormLayout) null
      };
    }

    internal static WorkItemTypeModel Create(
      IVssRequestContext requestContext,
      Guid processId,
      ComposedWorkItemType wit,
      IReadOnlyCollection<WorkItemStateDefinition> states,
      IReadOnlyCollection<BehaviorRelation> behaviors,
      Layout layout)
    {
      return new WorkItemTypeModel()
      {
        Id = wit.ReferenceName,
        Name = wit.Name,
        Description = wit.Description,
        Color = wit.Color,
        Icon = wit.Icon,
        IsDisabled = new bool?(wit.IsDisabled),
        States = states != null ? states.Select<WorkItemStateDefinition, WorkItemStateResultModel>((Func<WorkItemStateDefinition, WorkItemStateResultModel>) (s => WorkItemStateResultModelFactory.Create(requestContext, processId, wit.ReferenceName, s))) : (IEnumerable<WorkItemStateResultModel>) null,
        Behaviors = behaviors != null ? (IReadOnlyCollection<WorkItemTypeBehavior>) behaviors.Select<BehaviorRelation, WorkItemTypeBehavior>((Func<BehaviorRelation, WorkItemTypeBehavior>) (b => WorkItemTypeBehaviorModelFactory.Create(requestContext, wit.ProcessId, wit.ReferenceName, b))).ToList<WorkItemTypeBehavior>() : (IReadOnlyCollection<WorkItemTypeBehavior>) null,
        Inherits = wit.IsDerived ? wit.ParentTypeRefName : (string) null,
        Class = WorkItemTypeModelFactory.GetWorkItemTypeClass(wit.IsDerived, wit.IsCustomType),
        Url = WorkItemTypeModelFactory.GetLocationUrlForType(requestContext, processId, (BaseWorkItemType) wit),
        Layout = layout != null ? layout.GetWebApiModel() : (Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.FormLayout) null
      };
    }

    private static WorkItemTypeClass GetWorkItemTypeClass(
      bool isWorkItemTypeDerived,
      bool isWorkItemTypeCustom)
    {
      if (isWorkItemTypeDerived)
        return WorkItemTypeClass.Derived;
      return isWorkItemTypeCustom ? WorkItemTypeClass.Custom : WorkItemTypeClass.System;
    }

    private static string GetLocationUrlForType(
      IVssRequestContext requestContext,
      ProcessWorkItemType wit)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processDefinitions", WorkItemTrackingLocationIds.ProcessDefinitionWorkItemTypes, (object) new
      {
        processId = wit.ProcessId,
        witRefName = wit.ReferenceName
      }).ToString();
    }

    private static string GetLocationUrlForType(
      IVssRequestContext requestContext,
      Guid processId,
      BaseWorkItemType wit)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processDefinitions", WorkItemTrackingLocationIds.ProcessDefinitionWorkItemTypes, (object) new
      {
        processId = processId,
        witRefName = wit.ReferenceName
      }).ToString();
    }
  }
}
