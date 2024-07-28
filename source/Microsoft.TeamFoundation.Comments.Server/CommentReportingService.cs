// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Comments.Server.CommentReportingService
// Assembly: Microsoft.TeamFoundation.Comments.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CBA40CC5-9694-4582-97B5-1660FA9D4307
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.Comments.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.TeamFoundation.Comments.Server
{
  internal class CommentReportingService : ICommentReportingService, IVssFrameworkService
  {
    public void ServiceStart(IVssRequestContext systemRequestContext)
    {
    }

    public void ServiceEnd(IVssRequestContext systemRequestContext)
    {
    }

    public IList<CommentVersion> GetCommentsVersions(
      IVssRequestContext requestContext,
      Guid projectId,
      Guid artifactKind,
      IList<GetCommentVersion> commentsVersions)
    {
      ArgumentUtility.CheckForNull<IVssRequestContext>(requestContext, nameof (requestContext));
      ArgumentUtility.CheckForEmptyGuid(artifactKind, nameof (artifactKind));
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) commentsVersions, nameof (commentsVersions));
      ArgumentUtility.CheckEnumerableForNullElement((IEnumerable) commentsVersions, nameof (commentsVersions));
      ArgumentUtility.CheckBoundsInclusive(commentsVersions.Count<GetCommentVersion>(), 1, CommentService.MaxAllowedPageSize, nameof (commentsVersions));
      using (requestContext.TraceBlock(140075, 140076, "CommentService", "Service", nameof (GetCommentsVersions)))
      {
        List<CommentVersion> commentsVersions1;
        using (CommentComponent replicaAwareComponent = requestContext.CreateReadReplicaAwareComponent<CommentComponent>("Comments"))
          commentsVersions1 = replicaAwareComponent.GetCommentsVersions(artifactKind, (IEnumerable<GetCommentVersion>) commentsVersions);
        return (IList<CommentVersion>) commentsVersions1;
      }
    }
  }
}
