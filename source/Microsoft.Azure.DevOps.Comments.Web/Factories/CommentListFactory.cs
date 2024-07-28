// Decompiled with JetBrains decompiler
// Type: Microsoft.Azure.DevOps.Comments.Web.Factories.CommentListFactory
// Assembly: Microsoft.Azure.DevOps.Comments.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: A6538262-E3F2-45F5-B799-587642D68EAC
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.Azure.DevOps.Comments.Web.dll

using Microsoft.Azure.DevOps.Comments.Web.Common;
using Microsoft.Azure.DevOps.Comments.WebApi;
using Microsoft.TeamFoundation.Comments.Server;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.Azure.DevOps.Comments.Web.Factories
{
  internal static class CommentListFactory
  {
    internal static TList Create<TList, T>(
      IVssRequestContext requestContext,
      CommentsList commentsList)
      where TList : CommentList, new()
      where T : Microsoft.Azure.DevOps.Comments.WebApi.Comment, new()
    {
      if (commentsList == null)
        return default (TList);
      IEnumerable<Guid> vsids = commentsList.Comments.SelectMany<Microsoft.TeamFoundation.Comments.Server.Comment, Guid>((Func<Microsoft.TeamFoundation.Comments.Server.Comment, IEnumerable<Guid>>) (c => (IEnumerable<Guid>) CommentListFactory.GetVsids(c.CreatedBy, c.ModifiedBy)));
      IDictionary<Guid, IdentityRef> identityReferencesById = IdentityRefBuilder.Create(requestContext, vsids, true, true);
      TList list = new TList();
      list.Comments = (IEnumerable<Microsoft.Azure.DevOps.Comments.WebApi.Comment>) commentsList.Comments.Select<Microsoft.TeamFoundation.Comments.Server.Comment, T>((Func<Microsoft.TeamFoundation.Comments.Server.Comment, T>) (comment => CommentFactory.Create<TList, T>(requestContext, identityReferencesById, comment)));
      list.Count = commentsList.Comments.Count<Microsoft.TeamFoundation.Comments.Server.Comment>();
      list.TotalCount = commentsList.TotalCount;
      list.ContinuationToken = commentsList.ContinuationToken;
      list.SetSecuredObject(commentsList.SecuredObject);
      return list;
    }

    internal static IList<T> Create<T>(
      IVssRequestContext requestContext,
      IEnumerable<Microsoft.TeamFoundation.Comments.Server.CommentVersion> commentVersions)
      where T : Microsoft.Azure.DevOps.Comments.WebApi.CommentVersion, new()
    {
      IEnumerable<Guid> vsids = commentVersions.SelectMany<Microsoft.TeamFoundation.Comments.Server.CommentVersion, Guid>((Func<Microsoft.TeamFoundation.Comments.Server.CommentVersion, IEnumerable<Guid>>) (c => (IEnumerable<Guid>) CommentListFactory.GetVsids(c.CreatedBy, c.ModifiedBy)));
      IDictionary<Guid, IdentityRef> identityReferencesById = IdentityRefBuilder.Create(requestContext, vsids, true, true);
      return (IList<T>) commentVersions.Select<Microsoft.TeamFoundation.Comments.Server.CommentVersion, T>((Func<Microsoft.TeamFoundation.Comments.Server.CommentVersion, T>) (version => CommentFactory.Create<T>(requestContext, version.SecuredObject, identityReferencesById, version))).ToList<T>();
    }

    private static List<Guid> GetVsids(Guid createdBy, Guid modifiedBy) => new List<Guid>()
    {
      createdBy,
      modifiedBy
    };
  }
}
