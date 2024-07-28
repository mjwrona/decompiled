// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.ProcessComment
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class ProcessComment : IRenderedText
  {
    public ProcessComment(string artifactId, string text, CommentFormat format)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(artifactId, nameof (artifactId));
      ArgumentUtility.CheckStringForNullOrWhiteSpace(text, nameof (text));
      ArgumentUtility.CheckForDefinedEnum<CommentFormat>(format, nameof (format));
      this.ArtifactId = artifactId;
      this.Text = text;
      this.Format = format;
      this.RenderedText = string.Empty;
    }

    public string ArtifactId { get; }

    public string Text
    {
      get => ((IRenderedText) this).Text;
      internal set => ((IRenderedText) this).Text = value;
    }

    string IRenderedText.Text { get; set; }

    public CommentFormat Format { get; }

    public string RenderedText
    {
      get => ((IRenderedText) this).RenderedText;
      internal set => ((IRenderedText) this).RenderedText = value;
    }

    string IRenderedText.RenderedText { get; set; }

    void IRenderedText.AddAttachment(Guid attachmentId)
    {
    }
  }
}
