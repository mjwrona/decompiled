// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.Boards.WebApi.Common.Converters.WorkItemCommentVersionRefFactory
// Assembly: Microsoft.Azure.Boards.WebApi.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: FC99C479-6852-4E74-BCA4-2660760F9D83
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.Boards.WebApi.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.DataModels;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using System;

namespace Microsoft.Azure.Boards.WebApi.Common.Converters
{
  internal class WorkItemCommentVersionRefFactory
  {
    internal static WorkItemCommentVersionRef Create(
      IVssRequestContext requestContext,
      ISecuredObject securedObject,
      Guid projectId,
      WorkItemCommentVersionRecord commentVersion,
      bool extendedVersion = false)
    {
      ArgumentUtility.CheckForNull<WorkItemCommentVersionRecord>(commentVersion, nameof (commentVersion));
      string commentVersionUrl = WitUrlHelper.GetWorkItemCommentVersionUrl(requestContext, projectId, commentVersion.WorkItemId, commentVersion.CommentId, commentVersion.Version);
      if (extendedVersion)
      {
        WorkItemCommentVersionRef commentVersionRef = new WorkItemCommentVersionRef(securedObject);
        commentVersionRef.CommentId = commentVersion.CommentId;
        commentVersionRef.Version = commentVersion.Version;
        commentVersionRef.Url = commentVersionUrl;
        commentVersionRef.Text = commentVersion.Text;
        commentVersionRef.CreatedInRevision = new int?(commentVersion.CommentOriginalRev);
        commentVersionRef.IsDeleted = commentVersion.IsDeleted ? new bool?(true) : new bool?();
        return commentVersionRef;
      }
      WorkItemCommentVersionRef commentVersionRef1 = new WorkItemCommentVersionRef(securedObject);
      commentVersionRef1.CommentId = commentVersion.CommentId;
      commentVersionRef1.Version = commentVersion.Version;
      commentVersionRef1.Url = commentVersionUrl;
      return commentVersionRef1;
    }
  }
}
