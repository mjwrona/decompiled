// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.DeleteComment
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class DeleteComment : IChangedBy, IComment
  {
    public DeleteComment(string artifactId, int commentId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      this.ArtifactId = artifactId;
      this.CommentId = commentId;
    }

    public int CommentId { get; }

    public string ArtifactId { get; }

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
  }
}
