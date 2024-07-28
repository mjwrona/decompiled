// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Models.GetWorkItemTypeExpandExtensions
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Models
{
  internal static class GetWorkItemTypeExpandExtensions
  {
    public static bool IncludeStates(this Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.GetWorkItemTypeExpand expand) => (expand & Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.GetWorkItemTypeExpand.States) != 0;

    public static bool IncludeBehaviors(this Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.GetWorkItemTypeExpand expand) => (expand & Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.GetWorkItemTypeExpand.Behaviors) != 0;

    public static bool IncludeLayout(this Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.GetWorkItemTypeExpand expand) => (expand & Microsoft.TeamFoundation.WorkItemTracking.ProcessDefinitions.WebApi.Models.GetWorkItemTypeExpand.Layout) != 0;

    public static bool IncludeStates(this Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.GetWorkItemTypeExpand expand) => (expand & Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.GetWorkItemTypeExpand.States) != 0;

    public static bool IncludeBehaviors(this Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.GetWorkItemTypeExpand expand) => (expand & Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.GetWorkItemTypeExpand.Behaviors) != 0;

    public static bool IncludeLayout(this Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.GetWorkItemTypeExpand expand) => (expand & Microsoft.TeamFoundation.WorkItemTracking.Process.WebApi.Models.GetWorkItemTypeExpand.Layout) != 0;
  }
}
