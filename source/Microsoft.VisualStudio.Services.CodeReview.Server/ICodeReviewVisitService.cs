// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ICodeReviewVisitService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [DefaultServiceImplementation(typeof (CodeReviewVisitService))]
  public interface ICodeReviewVisitService : IVssFrameworkService
  {
    IEnumerable<ArtifactVisit> QueryVisits(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactVisit> visits);

    ArtifactVisit QueryVisit(IVssRequestContext requestContext, ArtifactVisit visit);

    ArtifactVisit UpdateLastVisit(IVssRequestContext requestContext, ArtifactVisit visit);

    ArtifactVisit UpdateViewedState(
      IVssRequestContext requestContext,
      ArtifactVisit visitIn,
      string[] hashes,
      ViewedStatus newViewedStatus);

    IEnumerable<ArtifactStats> QueryStats(
      IVssRequestContext requestContext,
      IEnumerable<ArtifactStats> stats,
      bool includeUpdatesSinceLastVisit);

    ArtifactStats QueryStats(
      IVssRequestContext requestContext,
      ArtifactStats stats,
      bool includeUpdatesSinceLastVisit);

    bool VerifyArtifactId(IVssRequestContext requestContext, string artifactId);
  }
}
