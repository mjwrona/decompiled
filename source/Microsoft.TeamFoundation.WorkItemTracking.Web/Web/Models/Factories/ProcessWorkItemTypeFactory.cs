// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories.ProcessWorkItemTypeFactory
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.Azure.Boards.WebApi.Common;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Extensions;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Process.Factories;
using Microsoft.VisualStudio.Services.Location.Server;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories
{
  public class ProcessWorkItemTypeFactory
  {
    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType Create(
      IVssRequestContext requestContext,
      Guid processId,
      Microsoft.TeamFoundation.WorkItemTracking.Server.ProcessWorkItemType workItemType)
    {
      if (workItemType == null)
        throw new ArgumentNullException(nameof (workItemType));
      return new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType()
      {
        ReferenceName = workItemType.ReferenceName,
        Name = workItemType.Name,
        Color = workItemType.Color,
        Description = workItemType.Description,
        Icon = workItemType.Icon,
        IsDisabled = workItemType.IsDisabled,
        Inherits = workItemType.IsDerived ? workItemType.ParentTypeRefName : (string) null,
        Url = this.GetUrl(requestContext, processId, workItemType.ReferenceName)?.ToString(),
        Customization = ProcessWorkItemTypeFactory.GetCustomizationType(workItemType.IsDerived, workItemType.IsCustomType)
      };
    }

    public Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType Create(
      IVssRequestContext requestContext,
      Guid processId,
      ComposedWorkItemType workItemType,
      GetWorkItemTypeExpand expand = GetWorkItemTypeExpand.None)
    {
      if (workItemType == null)
        throw new ArgumentNullException(nameof (workItemType));
      return new Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.ProcessWorkItemType()
      {
        ReferenceName = workItemType.ReferenceName,
        Name = workItemType.Name,
        Color = workItemType.Color,
        Description = workItemType.Description,
        Icon = workItemType.Icon,
        Inherits = workItemType.IsDerived ? workItemType.ParentTypeRefName : (string) null,
        Url = this.GetUrl(requestContext, processId, workItemType.ReferenceName)?.ToString(),
        IsDisabled = workItemType.IsDisabled,
        Customization = ProcessWorkItemTypeFactory.GetCustomizationType(workItemType.IsDerived, workItemType.IsCustomType),
        Layout = expand.IncludeLayout() ? this.GetLayout(requestContext, workItemType) : (FormLayout) null,
        Behaviors = expand.IncludeBehaviors() ? this.GetBehaviors(requestContext, processId, workItemType.ReferenceName) : (IReadOnlyCollection<WorkItemTypeBehavior>) null,
        States = expand.IncludeStates() ? this.GetStates(requestContext, processId, workItemType.ReferenceName) : (IEnumerable<WorkItemStateResultModel>) null
      };
    }

    private static Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType GetCustomizationType(
      bool isDerived,
      bool isCustomType)
    {
      if (isDerived)
        return Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.Inherited;
      return isCustomType ? Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.Custom : Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.CustomizationType.System;
    }

    protected virtual FormLayout GetLayout(
      IVssRequestContext requestContext,
      ComposedWorkItemType workItemType)
    {
      return workItemType.GetFormLayout(requestContext).GetProcessWebApiModel();
    }

    protected virtual IReadOnlyCollection<WorkItemTypeBehavior> GetBehaviors(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName)
    {
      IReadOnlyCollection<BehaviorRelation> behaviorsForWorkItemType = requestContext.GetService<IProcessWorkItemTypeService>().GetBehaviorsForWorkItemType(requestContext, processId, witRefName, true);
      return behaviorsForWorkItemType == null ? (IReadOnlyCollection<WorkItemTypeBehavior>) null : (IReadOnlyCollection<WorkItemTypeBehavior>) behaviorsForWorkItemType.Select<BehaviorRelation, WorkItemTypeBehavior>((Func<BehaviorRelation, WorkItemTypeBehavior>) (b => WorkItemTypeBehaviorModelFactory.Create(requestContext, processId, witRefName, b))).ToList<WorkItemTypeBehavior>();
    }

    protected virtual IEnumerable<WorkItemStateResultModel> GetStates(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName)
    {
      IReadOnlyCollection<WorkItemStateDefinition> stateDefinitions = requestContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinitions(requestContext, processId, witRefName, true);
      return stateDefinitions == null ? (IEnumerable<WorkItemStateResultModel>) null : stateDefinitions.Select<WorkItemStateDefinition, WorkItemStateResultModel>((Func<WorkItemStateDefinition, WorkItemStateResultModel>) (s => WorkItemStateResultModelFactory.Create(requestContext, processId, witRefName, s)));
    }

    protected virtual Uri GetUrl(
      IVssRequestContext requestContext,
      Guid processId,
      string witRefName)
    {
      return requestContext.GetService<ILocationService>().GetResourceUri(requestContext, "processes", WorkItemTrackingLocationIds.ProcessWorkItemTypes, (object) new
      {
        processId = processId,
        witRefName = witRefName
      });
    }
  }
}
