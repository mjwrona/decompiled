// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.ExtensionMethods
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Discussion.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FFC5EC6C-1B94-4299-8BA9-787264C21330
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.CodeReview.Discussion.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using System;

namespace Microsoft.VisualStudio.Services.CodeReview.Discussion.Server
{
  public static class ExtensionMethods
  {
    internal static CommentId ToCommentId(this DiscussionComment comment)
    {
      if (comment == null)
        return (CommentId) null;
      return new CommentId()
      {
        Id = comment.CommentId,
        DiscussionId = comment.DiscussionId
      };
    }

    internal static int GetPropertyId(this ArtifactPropertyValue artifactPropertyValue)
    {
      byte[] destinationArray = new byte[artifactPropertyValue.Spec.Id.Length];
      Array.Copy((Array) artifactPropertyValue.Spec.Id, (Array) destinationArray, destinationArray.Length);
      if (BitConverter.IsLittleEndian)
        Array.Reverse((Array) destinationArray);
      return BitConverter.ToInt32(destinationArray, 0);
    }
  }
}
