// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.Web.Factories.CommentReactionFactory
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.Azure.DevOps.Comments.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.Azure.DevOps.Comments.Web.Factories
{
  internal static class CommentReactionFactory
  {
    internal static Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction CreateReaction(
      Microsoft.TeamFoundation.Comments.Server.CommentReaction commentReaction)
    {
      return CommentReactionFactory.CreateReaction(commentReaction, commentReaction.SecuredObject);
    }

    internal static Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction CreateReaction(
      Microsoft.TeamFoundation.Comments.Server.CommentReaction commentReaction,
      ISecuredObject securedObject)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.Comments.Server.CommentReaction>(commentReaction, "CommentReaction");
      return new Microsoft.Azure.DevOps.Comments.WebApi.CommentReaction(securedObject)
      {
        Type = (CommentReactionType) commentReaction.Type,
        Count = commentReaction.Count,
        IsCurrentUserEngaged = commentReaction.IsCurrentUserEngaged,
        CommentId = commentReaction.CommentId
      };
    }
  }
}
