// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments.WorkItemComments
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: B0AE48DA-B6D2-466C-91D8-D0BF0F05DE87
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Server.dll

using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Server.Common;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.WorkItemTracking.Server.WorkItems.Comments
{
  public class WorkItemComments : WorkItemSecuredObject
  {
    private WorkItemComments(int permissions, string securityToken)
      : base(permissions, securityToken)
    {
    }

    public int TotalCount { get; set; }

    public IEnumerable<WorkItemComment> Comments { get; set; }

    public string ContinuationToken { get; set; }

    public bool IsLastBatch { get; set; }

    public static WorkItemComments FromCommentsList(
      IVssRequestContext requestContext,
      CommentsList commentsList,
      Guid projectId,
      int permissions,
      string securityToken)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(projectId, nameof (projectId));
      ArgumentUtility.CheckForNull<string>(securityToken, nameof (securityToken));
      return new WorkItemComments(permissions, securityToken)
      {
        Comments = commentsList.Comments.Select<Comment, WorkItemComment>((Func<Comment, WorkItemComment>) (comment => WorkItemComment.FromComment(requestContext, comment, projectId, permissions, securityToken))),
        TotalCount = commentsList.TotalCount,
        ContinuationToken = commentsList.ContinuationToken
      };
    }
  }
}
