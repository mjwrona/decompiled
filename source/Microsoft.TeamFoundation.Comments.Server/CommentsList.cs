// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentsList
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentsList
  {
    public CommentsList()
    {
    }

    public CommentsList(ICollection<Comment> comments)
    {
      this.Comments = (IReadOnlyCollection<Comment>) comments.ToList<Comment>().AsReadOnly();
      this.TotalCount = comments.Count;
    }

    public CommentsList(ICollection<Comment> comments, ISecuredObject securedObject)
      : this(comments)
    {
      this.SetSecuredObject(securedObject);
    }

    public int TotalCount { get; internal set; }

    public IReadOnlyCollection<Comment> Comments { get; internal set; }

    public string ContinuationToken { get; internal set; }

    public ISecuredObject SecuredObject { get; private set; }

    internal void SetSecuredObject(ISecuredObject securedObject)
    {
      this.SecuredObject = securedObject;
      if (this.Comments == null || securedObject == null)
        return;
      this.Comments.ForEach<Comment>((Action<Comment>) (c => c.SetSecuredObject(securedObject)));
    }
  }
}
