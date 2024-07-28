// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.WorkItemCommentUpdate
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using System;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems
{
  public class WorkItemCommentUpdate
  {
    public WorkItemCommentUpdate(string text)
    {
      this.Text = text;
      this.Format = CommentFormat.Html;
    }

    public WorkItemCommentUpdate(bool isMarkdown, string text)
    {
      this.Format = isMarkdown ? CommentFormat.Markdown : CommentFormat.Html;
      this.Text = text;
    }

    public WorkItemCommentUpdate(string format, string commentText)
      : this(commentText)
    {
      CommentFormat result;
      if (!Enum.TryParse<CommentFormat>(format, out result))
        return;
      this.Format = result;
    }

    public CommentFormat Format { get; set; }

    public string Text { get; set; }

    public bool IsMarkdown => this.Format == CommentFormat.Markdown;
  }
}
