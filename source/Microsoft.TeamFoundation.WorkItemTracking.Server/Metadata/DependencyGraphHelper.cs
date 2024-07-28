// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata.DependencyGraphHelper
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Metadata
{
  public static class DependencyGraphHelper
  {
    private const string TraceLayer = "DependencyGraphHelper";

    public static List<WorkItemDependencyGraph> GetWorkItemGraph(
      IVssRequestContext requestContext,
      int id)
    {
      List<WorkItemDependencyGraph> itemDependencyGraphList = new List<WorkItemDependencyGraph>();
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        return component.LoadDependencyGraphForWorkItem(id);
    }

    public static List<int> GetWorkItemErrorList(
      IVssRequestContext requestContext,
      List<int> workItemIds)
    {
      List<int> intList = new List<int>();
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        return component.GetWorkItemViolations(workItemIds);
    }

    public static List<WorkItemDependencyInformation> GetWorkItemDependencyInformation(
      IVssRequestContext requestContext,
      List<int> workItemIds)
    {
      List<WorkItemDependencyInformation> dependencyInformationList = new List<WorkItemDependencyInformation>();
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        return component.GetDependencyInformationForWorkItems(workItemIds);
    }

    public static List<WorkItemDependencyGraph> GetWorkItemGraph(
      IVssRequestContext requestContext,
      int[] ids)
    {
      List<WorkItemDependencyGraph> itemDependencyGraphList = new List<WorkItemDependencyGraph>();
      using (WorkItemComponent component = requestContext.CreateComponent<WorkItemComponent>())
        return component.LoadDependencyGraphForWorkItems(ids);
    }
  }
}
