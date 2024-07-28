// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Git.Server.CommentDiscussionConverter
// Assembly: Microsoft.TeamFoundation.Git.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: E1F0714E-7EF5-4D28-9AF2-C8D8620EA079
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Git.Server.dll

using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Git.Server
{
  public static class CommentDiscussionConverter
  {
    public static CommentThreadContext ToThreadContext(this DiscussionThread discussionThread)
    {
      if (discussionThread == null)
        return (CommentThreadContext) null;
      CommentThreadContext threadContext = new CommentThreadContext()
      {
        FilePath = discussionThread.ItemPath
      };
      PropertiesCollection properties = discussionThread.Properties;
      if (properties != null && properties.ContainsKey("Microsoft.TeamFoundation.Discussion.Position.PositionContext"))
      {
        string str = (string) properties["Microsoft.TeamFoundation.Discussion.Position.PositionContext"];
        if (properties.ContainsKey("Microsoft.TeamFoundation.Discussion.Position.StartLine") && properties.ContainsKey("Microsoft.TeamFoundation.Discussion.Position.StartColumn"))
        {
          CommentPosition commentPosition = new CommentPosition()
          {
            Line = (int) properties["Microsoft.TeamFoundation.Discussion.Position.StartLine"],
            Offset = (int) properties["Microsoft.TeamFoundation.Discussion.Position.StartColumn"] - 1
          };
          switch (str)
          {
            case "LeftBuffer":
              threadContext.LeftFileStart = commentPosition;
              break;
            case "RightBuffer":
              threadContext.RightFileStart = commentPosition;
              break;
          }
        }
        if (properties.ContainsKey("Microsoft.TeamFoundation.Discussion.Position.EndLine") && properties.ContainsKey("Microsoft.TeamFoundation.Discussion.Position.EndColumn"))
        {
          CommentPosition commentPosition = new CommentPosition()
          {
            Line = (int) properties["Microsoft.TeamFoundation.Discussion.Position.EndLine"],
            Offset = (int) properties["Microsoft.TeamFoundation.Discussion.Position.EndColumn"] - 1
          };
          switch (str)
          {
            case "LeftBuffer":
              threadContext.LeftFileEnd = commentPosition;
              break;
            case "RightBuffer":
              threadContext.RightFileEnd = commentPosition;
              break;
          }
        }
      }
      return threadContext;
    }
  }
}
