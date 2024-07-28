// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.Comment
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class Comment : IComment, IRenderedText
  {
    private readonly List<CommentReaction> reactionsList = new List<CommentReaction>();

    internal Comment(Guid artifactKind, string artifactId, int commentId, string text)
      : this(artifactKind, artifactId, commentId, text, (string) null)
    {
    }

    internal Comment(
      Guid artifactKind,
      string artifactId,
      int commentId,
      string text,
      string renderedText)
      : this(artifactKind, artifactId, commentId, text, renderedText, (ISecuredObject) null)
    {
    }

    internal Comment(
      Guid artifactKind,
      string artifactId,
      int commentId,
      string text,
      string renderedText,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      this.ArtifactKind = artifactKind;
      this.ArtifactId = artifactId;
      this.CommentId = commentId;
      this.Text = text ?? string.Empty;
      this.RenderedText = renderedText ?? string.Empty;
      this.SetSecuredObject(securedObject);
    }

    internal void SetSecuredObject(ISecuredObject securedObject)
    {
      this.SecuredObject = securedObject;
      if (this.Children == null)
        return;
      this.Children.SetSecuredObject(securedObject);
    }

    public int CommentId { get; }

    public Guid ArtifactKind { get; }

    public string ArtifactId { get; }

    public int Version { get; internal set; }

    public string Text
    {
      get => ((IRenderedText) this).Text;
      internal set => ((IRenderedText) this).Text = value;
    }

    string IRenderedText.Text { get; set; }

    public string RenderedText
    {
      get => ((IRenderedText) this).RenderedText;
      internal set => ((IRenderedText) this).RenderedText = value;
    }

    string IRenderedText.RenderedText { get; set; }

    public IReadOnlyCollection<CommentReaction> Reactions => (IReadOnlyCollection<CommentReaction>) this.reactionsList;

    public CommentFormat Format { get; internal set; }

    public Guid CreatedBy { get; internal set; }

    public DateTime CreatedDate { get; internal set; }

    public string CreatedOnBehalfOf { get; internal set; }

    public DateTime CreatedOnBehalfDate { get; internal set; }

    public Guid ModifiedBy { get; internal set; }

    public DateTime ModifiedDate { get; internal set; }

    public bool IsDeleted { get; internal set; }

    public int? ParentId { get; internal set; }

    public CommentState State { get; internal set; }

    public string Properties { get; internal set; }

    public CommentsList Children { get; internal set; }

    public ISecuredObject SecuredObject { get; private set; }

    public override bool Equals(object o) => o is Comment comment && this.CommentId == comment.CommentId && !(this.ArtifactId != comment.ArtifactId) && !(this.ArtifactKind != comment.ArtifactKind);

    public override int GetHashCode() => this.CommentId.GetHashCode();

    internal void AddReaction(CommentReaction reaction)
    {
      ArgumentUtility.CheckForNull<CommentReaction>(reaction, nameof (reaction));
      reaction.CommentId = this.CommentId;
      this.reactionsList.Add(reaction);
    }

    void IRenderedText.AddAttachment(Guid attachmentId)
    {
    }
  }
}
