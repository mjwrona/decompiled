// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemComponent20
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  internal class WorkItemComponent20 : WorkItemComponent19
  {
    internal override WorkItemComponent.UpdateWorkItemsResultsReader GetUpdateWorkItemsResultsReader(
      bool bypassRules,
      bool isAdmin,
      WorkItemUpdateDataset updateDataset)
    {
      return (WorkItemComponent.UpdateWorkItemsResultsReader) new WorkItemComponent20.UpdateWorkItemsResultsReader2(bypassRules, isAdmin, updateDataset);
    }

    internal class UpdateWorkItemsResultsReader2 : WorkItemComponent.UpdateWorkItemsResultsReader
    {
      internal UpdateWorkItemsResultsReader2(
        bool bypassRules,
        bool isAdmin,
        WorkItemUpdateDataset updateDataset)
        : base(bypassRules, isAdmin, updateDataset)
      {
        this.tieCoreFieldUpdatesWithResourceLinkUpdates = false;
      }
    }
  }
}
