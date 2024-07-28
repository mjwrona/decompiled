// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.Events.WorkItemTypeCategoryUpdateEventData
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.Events
{
  internal class WorkItemTypeCategoryUpdateEventData : AdminUpdateEventData
  {
    public WorkItemTypeCategoryUpdateType ChangeType { get; set; }

    public int ProjectId { get; set; }

    public string ReferenceName { get; set; }

    public string Name { get; set; }

    public int CategoryId { get; set; }

    public int WorkItemTypeId { get; set; }

    public int TempCategoryID { get; set; }

    public int TempWorkItemTypeId { get; set; }

    public int CategoryMemberId { get; set; }

    public int DefaultWorkItemTypeId { get; set; }

    public int TempDefaultWorkItemTypeId { get; set; }
  }
}
