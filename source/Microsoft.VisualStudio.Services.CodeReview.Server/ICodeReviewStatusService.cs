// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ICodeReviewStatusService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [DefaultServiceImplementation(typeof (CodeReviewStatusService))]
  public interface ICodeReviewStatusService : IVssFrameworkService
  {
    Status GetStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int statusId,
      int? iterationId = null);

    IEnumerable<Status> GetStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int? iterationId = null,
      bool includeProperties = false);

    IEnumerable<StatusContext> GetLatestStatusContextsBySourceArtifactPrefix(
      IVssRequestContext requestContext,
      Guid projectId,
      string reviewArtifactPrefix);

    IEnumerable<Status> GetLatestStatusesByReviewId(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int top = 500);

    ILookup<int, Status> GetStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> reviewIds);

    Status SaveStatus(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Status status,
      int? iterationId = null);

    void DeleteStatuses(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<int> statusIds,
      int? iterationId = null);
  }
}
