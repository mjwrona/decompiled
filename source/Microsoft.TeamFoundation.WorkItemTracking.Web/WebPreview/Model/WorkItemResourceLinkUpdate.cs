// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model.WorkItemResourceLinkUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Extensions;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.WebPreview.Model
{
  [DataContract]
  public class WorkItemResourceLinkUpdate : LinkUpdate
  {
    [DataMember(EmitDefaultValue = false)]
    public int? ResourceId { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public ResourceLinkType? Type { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Location { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public string Name { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? CreationDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public DateTime? LastModifiedDate { get; set; }

    [DataMember(EmitDefaultValue = false)]
    public int? Length { get; set; }

    public Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdate ToWorkItemResourceLinkUpdate()
    {
      Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdate resourceLinkUpdate = new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemResourceLinkUpdate();
      resourceLinkUpdate.Comment = this.Comment;
      resourceLinkUpdate.CreationDate = this.CreationDate;
      resourceLinkUpdate.LastModifiedDate = this.LastModifiedDate;
      resourceLinkUpdate.Length = this.Length;
      resourceLinkUpdate.Location = this.Location;
      resourceLinkUpdate.Name = this.Name;
      resourceLinkUpdate.ResourceId = this.ResourceId;
      resourceLinkUpdate.SourceWorkItemId = this.SourceWorkItemId;
      resourceLinkUpdate.Type = this.Type.HasValue ? new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.ResourceLinkType?(this.Type.Value.ToResourceLinkType()) : new Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.ResourceLinkType?();
      resourceLinkUpdate.UpdateType = this.UpdateType.ToLinkUpdateType();
      return resourceLinkUpdate;
    }
  }
}
