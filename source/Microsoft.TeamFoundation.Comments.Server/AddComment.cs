// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.AddComment
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class AddComment : IRenderedText, IChangedBy
  {
    private readonly HashSet<Guid> attachmentIds = new HashSet<Guid>();

    public AddComment(string artifactId, string text, int? parentId = null, string properties = null)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(text, nameof (text));
      this.ArtifactId = artifactId;
      this.Text = text;
      this.RenderedText = string.Empty;
      this.ParentId = parentId;
      this.Properties = properties;
    }

    public string ArtifactId { get; }

    public string Text
    {
      get => ((IRenderedText) this).Text;
      internal set => ((IRenderedText) this).Text = value;
    }

    public int? ParentId { get; set; }

    public string Properties { get; set; }

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

    internal string CreatedOnBehalfOf { get; set; }

    internal DateTime? CreatedOnBehalfDate { get; set; }

    Guid IChangedBy.ChangedBy { get; set; }

    DateTime IChangedBy.ChangedDate { get; set; }

    internal IReadOnlyCollection<Guid> Attachments => (IReadOnlyCollection<Guid>) this.attachmentIds;

    void IRenderedText.AddAttachment(Guid attachmentId)
    {
      ArgumentUtility.CheckForEmptyGuid(attachmentId, nameof (attachmentId));
      this.attachmentIds.Add(attachmentId);
    }
  }
}
