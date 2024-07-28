// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentReaction
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Social.WebApi;
using Microsoft.VisualStudio.Services.WebApi;

namespace Microsoft.TeamFoundation.Comments.Server
{
  public class CommentReaction
  {
    internal CommentReaction()
    {
    }

    public CommentReaction(int commentId, SocialEngagementRecord record)
    {
      ArgumentUtility.CheckForNonnegativeInt(commentId, nameof (commentId));
      ArgumentUtility.CheckForNull<SocialEngagementRecord>(record, nameof (record));
      ArgumentUtility.CheckStringForNullOrEmpty(record.ArtifactId, "ArtifactId");
      this.CommentId = commentId;
      this.Type = record.EngagementType;
      this.Count = record.SocialEngagementStatistics.UserCount;
      this.IsCurrentUserEngaged = record.CreationDate.HasValue;
      this.SetSecuredObject((ISecuredObject) record);
    }

    internal void SetSecuredObject(ISecuredObject securedObject) => this.SecuredObject = securedObject;

    public SocialEngagementType Type { get; internal set; }

    public int Count { get; internal set; }

    public bool IsCurrentUserEngaged { get; internal set; }

    public int CommentId { get; internal set; }

    public ISecuredObject SecuredObject { get; private set; }
  }
}
