// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentMentionContext
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Mention.Server;
using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentMentionContext : IMentionSourceContext
  {
    private const MentionActionType SupportedActions = MentionActionType.Save | MentionActionType.Email;

    public CommentMentionContext(
      Guid projectGuid,
      Guid artifactKind,
      string artifactfriendlyName,
      string artifactId,
      string artifactTitle,
      string artifactUri,
      int? commentId,
      string commentText,
      MentionContentType mentionContentType,
      Guid mentioner,
      bool suppessNotifications)
    {
      ArgumentUtility.CheckForNull<string>(artifactId, nameof (artifactId));
      this.ProjectGuid = projectGuid;
      this.ArtifactKind = artifactKind;
      this.ArtifactFriendlyName = artifactfriendlyName;
      this.ArtifactId = artifactId;
      this.ArtifactTitle = artifactTitle;
      this.ArtifactUri = artifactUri;
      this.CommentId = commentId;
      this.CommentText = commentText;
      this.ContentType = mentionContentType;
      this.Mentioner = mentioner;
      this.SupportedActionTypes = MentionActionType.Save | MentionActionType.Email;
      if (!suppessNotifications)
        return;
      this.SupportedActionTypes = this.SupportedActionTypes & ~MentionActionType.Email;
    }

    public string Type => MentionConstants.PlatformCommentType;

    public string DisplayText => string.Format(MentionResources.CommentSourceText, (object) this.ArtifactKind, (object) this.ArtifactId, (object) this.CommentId);

    public string Identifier => string.Format("{0}|{1}|{2}|{3}", (object) this.ArtifactKind, (object) this.ArtifactId, (object) this.CommentId, (object) MentionConstants.PlatformCommentType);

    public Guid ProjectGuid { get; }

    public Guid ArtifactKind { get; }

    public string ArtifactFriendlyName { get; }

    public string ArtifactId { get; }

    public string ArtifactTitle { get; }

    public string ArtifactUri { get; }

    public int? CommentId { get; }

    public string CommentText { get; }

    public MentionContentType ContentType { get; }

    public Guid Mentioner { get; }

    public MentionActionType SupportedActionTypes { get; }

    public string NormalizedId => string.Empty;
  }
}
