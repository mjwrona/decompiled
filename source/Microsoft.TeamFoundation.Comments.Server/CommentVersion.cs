// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentVersion
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentVersion
  {
    internal CommentVersion(
      Guid artifactKind,
      string artifactId,
      int commentId,
      int version,
      string text)
      : this(artifactKind, artifactId, commentId, version, text, (ISecuredObject) null)
    {
    }

    internal CommentVersion(
      Guid artifactKind,
      string artifactId,
      int commentId,
      int version,
      string text,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckForNull<string>(text, nameof (text));
      this.ArtifactKind = artifactKind;
      this.ArtifactId = artifactId;
      this.CommentId = commentId;
      this.Version = version;
      this.Text = text;
      this.SetSecuredObject(securedObject);
    }

    public int CommentId { get; }

    public string ArtifactId { get; }

    public Guid ArtifactKind { get; }

    public int Version { get; }

    public string Text { get; }

    public string RenderedText { get; internal set; }

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

    public ISecuredObject SecuredObject { get; private set; }

    internal void SetSecuredObject(ISecuredObject securedObject) => this.SecuredObject = securedObject;
  }
}
