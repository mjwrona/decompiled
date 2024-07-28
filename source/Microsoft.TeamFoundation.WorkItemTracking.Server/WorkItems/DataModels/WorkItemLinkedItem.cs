// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemLinkedItem
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Common.Metadata;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Runtime.Serialization;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  [DataContract]
  public class WorkItemLinkedItem : BaseSecuredObject
  {
    public WorkItemLinkedItem(
      IVssRequestContext requestContext,
      WorkItemLinkInfo link,
      ISecuredObject securedObject)
      : base(securedObject)
    {
      WorkItemTrackingRequestContext trackingRequestContext = requestContext.WitContext();
      WorkItemLinkTypeEnd linkType;
      if (trackingRequestContext.LinkService.TryGetLinkTypeEndById(trackingRequestContext.RequestContext, link.LinkType, out linkType))
      {
        this.Name = linkType.Name;
        this.ReferenceName = linkType.ReferenceName;
      }
      this.TargetWorkItemId = link.TargetId;
      this.TargetProjectId = link.TargetProjectId;
      this.IsLocked = link.IsLocked;
      this.Comment = link.Comment;
    }

    [DataMember(Name = "targetWorkItemId")]
    public int TargetWorkItemId { get; set; }

    [DataMember(Name = "name")]
    public string Name { get; set; }

    [DataMember(Name = "referenceName")]
    public string ReferenceName { get; set; }

    [DataMember(Name = "targetProjectId")]
    public Guid TargetProjectId { get; set; }

    [DataMember(Name = "isLocked")]
    public bool IsLocked { get; set; }

    [DataMember(Name = "comment")]
    public string Comment { get; set; }
  }
}
