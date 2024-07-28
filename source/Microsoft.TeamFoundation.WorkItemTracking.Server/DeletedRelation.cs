// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.DeletedRelation
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

namespace Microsoft.TeamFoundation.WorkItemTracking.Server
{
  public class DeletedRelation : IWorkItemChangedRelation
  {
    private string linkNameField;
    private string workItemIdField;
    private string workItemTitleField;
    private string workItemTypeName;
    private string displayUrl;
    private bool isBacklogParentLink;
    private bool isBacklogChildLink;

    public bool IsBacklogParentLink
    {
      get => this.isBacklogParentLink;
      set => this.isBacklogParentLink = value;
    }

    public bool IsBacklogChildLink
    {
      get => this.isBacklogChildLink;
      set => this.isBacklogChildLink = value;
    }

    public string LinkName
    {
      get => this.linkNameField;
      set => this.linkNameField = value;
    }

    public string WorkItemId
    {
      get => this.workItemIdField;
      set => this.workItemIdField = value;
    }

    public string Title
    {
      get => this.workItemTitleField;
      set => this.workItemTitleField = value;
    }

    public string Type
    {
      get => this.workItemTypeName;
      set => this.workItemTypeName = value;
    }

    public string DisplayUrl
    {
      get => this.displayUrl;
      set => this.displayUrl = value;
    }
  }
}
