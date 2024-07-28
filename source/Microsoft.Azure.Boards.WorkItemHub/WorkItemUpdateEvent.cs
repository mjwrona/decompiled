// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WorkItemHub.WorkItemUpdateEvent
// Assembly: Microsoft.Azure.Boards.WorkItemHub, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 749A696A-54F8-4B6F-8877-B350F1725C24
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.Azure.Boards.WorkItemHub.dll

using Microsoft.VisualStudio.Services.SignalR;
using System.Runtime.Serialization;

namespace Microsoft.Azure.Boards.WorkItemHub
{
  [DataContract]
  public class WorkItemUpdateEvent : ISignalRObject
  {
    public WorkItemUpdateEvent(
      int id,
      int revision,
      string projectId,
      ChangeType changeType,
      bool hasOnlyLinkUpdates,
      bool isCommentReactionUpdate = false,
      int commentId = 0)
    {
      this.Id = id;
      this.Revision = revision;
      this.ChangeType = changeType;
      this.ProjectId = projectId;
      this.LinkOnlyUpdate = hasOnlyLinkUpdates;
      this.IsCommentReactionUpdate = isCommentReactionUpdate;
      this.CommentId = commentId;
    }

    public WorkItemUpdateEvent(WorkItemUpdateEvent workItemData)
    {
      this.Id = workItemData.Id;
      this.Revision = workItemData.Revision;
      this.ChangeType = workItemData.ChangeType;
      this.ProjectId = workItemData.ProjectId;
      this.LinkOnlyUpdate = workItemData.LinkOnlyUpdate;
      this.IsCommentReactionUpdate = workItemData.IsCommentReactionUpdate;
      this.CommentId = workItemData.CommentId;
    }

    [DataMember(Name = "id")]
    public int Id { get; set; }

    [DataMember(Name = "revision")]
    public int Revision { get; set; }

    [DataMember(Name = "projectId")]
    public string ProjectId { get; set; }

    [DataMember(Name = "changeType")]
    public ChangeType ChangeType { get; set; }

    [DataMember(Name = "linkOnlyUpdate")]
    public bool LinkOnlyUpdate { get; set; }

    [DataMember(Name = "isCommentReactionUpdate")]
    public bool IsCommentReactionUpdate { get; set; }

    [DataMember(Name = "commentId")]
    public int CommentId { get; set; }
  }
}
