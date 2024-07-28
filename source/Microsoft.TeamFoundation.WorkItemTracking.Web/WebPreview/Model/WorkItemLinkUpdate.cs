// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemLinkUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model
{
  [DataContract]
  public class WorkItemLinkUpdate : LinkUpdate
  {
    [DataMember]
    public int TargetWorkItemId { get; set; }

    [DataMember]
    public string LinkType { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public bool? Locked { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdate ToWorkItemLinkUpdate(
      IVssRequestContext tfsRequestContext)
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdate workItemLinkUpdate = new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemLinkUpdate();
      workItemLinkUpdate.Comment = this.Comment;
      workItemLinkUpdate.LinkType = WorkItemLink.GetWorkItemLinkTypeId(this.LinkType, tfsRequestContext);
      workItemLinkUpdate.Locked = this.Locked;
      workItemLinkUpdate.SourceWorkItemId = this.SourceWorkItemId;
      workItemLinkUpdate.TargetWorkItemId = this.TargetWorkItemId;
      workItemLinkUpdate.UpdateType = this.UpdateType.ToLinkUpdateType();
      return workItemLinkUpdate;
    }
  }
}
