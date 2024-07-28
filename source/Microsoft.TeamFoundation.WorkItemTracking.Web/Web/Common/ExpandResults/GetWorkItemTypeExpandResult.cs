// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ExpandResults.GetWorkItemTypeExpandResult
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.FormLayout;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Common.ExpandResults
{
  internal class GetWorkItemTypeExpandResult
  {
    public GetWorkItemTypeExpandResult(
      Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.GetWorkItemTypeExpand expand,
      IVssRequestContext tfsContext,
      ComposedWorkItemType wit)
    {
      if (expand.IncludeStates())
        this.States = tfsContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinitions(tfsContext, wit.ProcessId, wit.ReferenceName);
      if (expand.IncludeBehaviors())
        this.Behaviors = tfsContext.GetService<IProcessWorkItemTypeService>().GetBehaviorsForWorkItemType(tfsContext, wit.ProcessId, wit.ReferenceName, true);
      this.Layout = expand.IncludeLayout() ? wit.GetFormLayout(tfsContext) : (Layout) null;
    }

    public GetWorkItemTypeExpandResult(
      Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.GetWorkItemTypeExpand expand,
      IVssRequestContext tfsContext,
      ComposedWorkItemType wit)
    {
      if (expand.IncludeStates())
        this.States = tfsContext.GetService<IWorkItemStateDefinitionService>().GetStateDefinitions(tfsContext, wit.ProcessId, wit.ReferenceName);
      if (expand.IncludeBehaviors())
        this.Behaviors = tfsContext.GetService<IProcessWorkItemTypeService>().GetBehaviorsForWorkItemType(tfsContext, wit.ProcessId, wit.ReferenceName, true);
      this.Layout = expand.IncludeLayout() ? wit.GetFormLayout(tfsContext) : (Layout) null;
    }

    public IReadOnlyCollection<BehaviorRelation> Behaviors { get; private set; }

    public Layout Layout { get; private set; }

    public IReadOnlyCollection<WorkItemStateDefinition> States { get; private set; }
  }
}
