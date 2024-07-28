// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.SocialAuthorBinder
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  internal class SocialAuthorBinder : ObjectBinder<SocialAuthor>
  {
    internal SqlColumnBinder DiscussionIdColumn = new SqlColumnBinder("DiscussionId");
    internal SqlColumnBinder CommentIdColumn = new SqlColumnBinder("CommentId");
    internal SqlColumnBinder AuthorColumn = new SqlColumnBinder("Author");

    protected override SocialAuthor Bind() => new SocialAuthor(this.DiscussionIdColumn.GetInt32((IDataReader) this.Reader, int.MinValue, int.MinValue), this.CommentIdColumn.GetInt16((IDataReader) this.Reader, short.MinValue, short.MinValue), this.AuthorColumn.GetGuid((IDataReader) this.Reader, false, Guid.Empty));
  }
}
