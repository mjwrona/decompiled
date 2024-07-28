// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ICodeReviewIterationService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [DefaultServiceImplementation(typeof (CodeReviewIterationService))]
  public interface ICodeReviewIterationService : IVssFrameworkService
  {
    Iteration GetIteration(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId);

    IList<Iteration> GetIterations(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      bool includeUnpublished = false);

    IList<Iteration> GetRawIterations(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      bool includeUnpublished = false);

    Iteration SaveIteration(
      IVssRequestContext requestContext,
      Guid projectId,
      Iteration iteration,
      bool updateIteration);

    void DeleteIteration(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId);

    void DeleteIterations(IVssRequestContext requestContext, Guid projectId, int reviewId);

    IterationChanges GetIterationChanges(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<int> iterationIds,
      int? top = null,
      int? skip = null);

    IEnumerable<ChangeEntry> GetChangeList(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<int> iterationIds,
      int? top = null,
      int? skip = null,
      int? baseIteration = null);

    ChangeEntryStream ReadChangeEntryFile(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId,
      int changeId,
      ChangeEntryFileType fileType);

    PushStreamContent GetIterationFilesContent(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<int> iterationIds,
      ChangeEntryFileType? fileType,
      out bool needsNextPage,
      int? top = null,
      int? skip = null);

    PropertiesCollection GetProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId);

    PropertiesCollection PatchProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      int iterationId,
      PropertiesCollection properties);

    int GetMaxChangeEntriesForChangeTrackingComputation(IVssRequestContext requestContext);
  }
}
