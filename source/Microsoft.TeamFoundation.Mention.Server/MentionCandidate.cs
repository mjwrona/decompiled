// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Mention.Server.MentionCandidate
// Assembly: Microsoft.TeamFoundation.Mention.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: C680EDB7-9FDC-4722-A198-4B5BA1B43B52
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Mention.Server.dll

using Microsoft.VisualStudio.Services.Common;
using System.Collections.Generic;

namespace Microsoft.TeamFoundation.Mention.Server
{
  public class MentionCandidate
  {
    public MentionCandidate(
      IMentionSourceContext sourceContext,
      string text,
      MentionContentType contentType,
      int? commentId = null,
      CommentState commentState = CommentState.Added)
    {
      ArgumentUtility.CheckForNull<IMentionSourceContext>(sourceContext, "context");
      this.SourceContext = sourceContext;
      this.Text = text;
      this.ContentType = contentType;
      this.CommentId = commentId;
      this.CommentState = commentState;
    }

    public int? CommentId { get; private set; }

    public CommentState CommentState { get; private set; }

    public string Text { get; private set; }

    public MentionContentType ContentType { get; private set; }

    public IMentionSourceContext SourceContext { get; private set; }

    public IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention> Mentions { get; set; } = (IEnumerable<Microsoft.TeamFoundation.Mention.Server.Mention>) new List<Microsoft.TeamFoundation.Mention.Server.Mention>();
  }
}
