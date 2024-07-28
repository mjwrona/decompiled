// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels.WorkItemMentionRecord
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Mention.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Comments;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels
{
  internal class WorkItemMentionRecord
  {
    internal WorkItemMentionRecord()
    {
    }

    internal WorkItemMentionRecord(
      Microsoft.TeamFoundation.Mention.Server.Mention mention,
      int tempId,
      WorkItemMentionUpdateState updateState = WorkItemMentionUpdateState.New)
    {
      this.SourceId = mention.Source.Identifier;
      this.SourceType = mention.Source.Type;
      this.SourceProjectGuid = mention.Source.ProjectGuid;
      this.RawText = mention.RawText;
      this.ArtifactId = mention.ArtifactId;
      this.ArtifactType = mention.ArtifactType;
      this.CommentId = mention.CommentId.GetValueOrDefault();
      this.TempId = tempId;
      this.MentionAction = mention.MentionAction;
      this.TargetId = mention.TargetId;
      Guid result;
      if (Guid.TryParse(mention.TargetId, out result))
        this.StorageKey = new Guid?(result);
      this.ProjectGuid = mention.ProjectGuid;
      this.NormalizedSourceId = mention.Source.NormalizedId;
      this.UpdateState = updateState;
    }

    public string SourceId { get; internal set; }

    public string SourceType { get; internal set; }

    public Guid SourceProjectGuid { get; internal set; }

    public string RawText { get; internal set; }

    public string ArtifactId { get; internal set; }

    public string ArtifactType { get; internal set; }

    public int CommentId { get; internal set; }

    public int TempId { get; internal set; }

    public string MentionAction { get; internal set; }

    public string TargetId { get; internal set; }

    public Guid ProjectGuid { get; internal set; }

    public Guid? StorageKey { get; internal set; }

    public string NormalizedSourceId { get; internal set; }

    public WorkItemMentionUpdateState UpdateState { get; internal set; }

    public Microsoft.TeamFoundation.Mention.Server.Mention ToMention(
      WorkItemCommentSourceContext sourceContext)
    {
      return new Microsoft.TeamFoundation.Mention.Server.Mention()
      {
        Source = (IMentionSourceContext) sourceContext,
        ArtifactId = this.ArtifactId,
        ArtifactType = this.ArtifactType,
        CommentId = new int?(this.CommentId),
        MentionAction = this.MentionAction,
        RawText = this.RawText,
        StorageKey = this.StorageKey,
        TargetId = this.TargetId,
        IsNew = this.UpdateState == WorkItemMentionUpdateState.New,
        ProjectGuid = this.ProjectGuid
      };
    }

    public static WorkItemMentionRecord FromMention(Microsoft.TeamFoundation.Mention.Server.Mention mention) => new WorkItemMentionRecord()
    {
      SourceId = mention.Source.Identifier,
      SourceType = mention.Source.Type,
      SourceProjectGuid = mention.Source.ProjectGuid,
      ArtifactId = mention.ArtifactId,
      ArtifactType = mention.ArtifactType,
      CommentId = mention.CommentId.Value,
      TempId = mention.CommentId.Value,
      MentionAction = mention.MentionAction,
      RawText = mention.RawText,
      StorageKey = mention.StorageKey,
      TargetId = mention.TargetId,
      UpdateState = mention.IsNew ? WorkItemMentionUpdateState.New : WorkItemMentionUpdateState.Existing,
      ProjectGuid = mention.ProjectGuid,
      NormalizedSourceId = mention.Source.NormalizedId
    };
  }
}
