// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.UpdateComment
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class UpdateComment : IChangedBy, IComment, IRenderedText
  {
    private readonly ISet<Guid> attachmentIds = (ISet<Guid>) new HashSet<Guid>();

    public UpdateComment(
      string artifactId,
      int commentId,
      string text,
      CommentState? state = null,
      string properties = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(text, nameof (text));
      this.ArtifactId = artifactId;
      this.CommentId = commentId;
      this.Text = text;
      this.RenderedText = string.Empty;
      this.State = state;
      this.Properties = properties;
    }

    public int CommentId { get; }

    public string ArtifactId { get; }

    public string Text
    {
      get => ((IRenderedText) this).Text;
      internal set => ((IRenderedText) this).Text = value;
    }

    public string Properties { get; internal set; }

    string IRenderedText.Text { get; set; }

    internal string RenderedText
    {
      get => ((IRenderedText) this).RenderedText;
      set => ((IRenderedText) this).RenderedText = value;
    }

    string IRenderedText.RenderedText { get; set; }

    internal Guid ChangedBy
    {
      get => ((IChangedBy) this).ChangedBy;
      set => ((IChangedBy) this).ChangedBy = value;
    }

    internal DateTime ChangedDate
    {
      get => ((IChangedBy) this).ChangedDate;
      set => ((IChangedBy) this).ChangedDate = value;
    }

    Guid IChangedBy.ChangedBy { get; set; }

    DateTime IChangedBy.ChangedDate { get; set; }

    public CommentState? State { get; internal set; }

    internal IReadOnlyCollection<Guid> Attachments => (IReadOnlyCollection<Guid>) this.attachmentIds.ToList<Guid>();

    void IRenderedText.AddAttachment(Guid attachmentId)
    {
      ArgumentUtility.CheckForEmptyGuid(attachmentId, nameof (attachmentId));
      this.attachmentIds.Add(attachmentId);
    }
  }
}
