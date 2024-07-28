// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ICodeReviewService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [DefaultServiceImplementation(typeof (CodeReviewService))]
  public interface ICodeReviewService : IVssFrameworkService
  {
    Review GetReview(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      CodeReviewExtendedProperties extendedProperties = CodeReviewExtendedProperties.All,
      ReviewScope reviewScope = ReviewScope.All,
      int? maxChangesCount = null);

    IEnumerable<Review> GetReviews(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> reviewIds,
      int? top = null,
      int? skip = null,
      bool includeDeleted = false,
      DateTime? modifiedSince = null);

    IEnumerable<Review> QueryReviewsBySourceArtifactId(
      IVssRequestContext requestContext,
      Guid projectId,
      string sourceArtifactId,
      bool includeDeleted = false,
      int? maxChangesCount = null);

    IEnumerable<Review> QueryReviewsBySourceArtifactIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> sourceArtifactIds,
      bool includeDeleted = false);

    IEnumerable<Review> QueryReviewsByFilters(
      IVssRequestContext requestContext,
      Guid? projectId,
      ReviewSearchCriteria searchCriteria,
      int top,
      out int totalReviewsFound);

    Review SaveReview(IVssRequestContext requestContext, Guid projectId, Review review);

    void DeleteReview(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      bool destroy = false);

    bool IsCurrentUserReviewAuthor(IVssRequestContext requestContext, Guid projectId, int reviewId);

    ReviewFileContentInfo UploadFile(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      byte[] contentHash,
      Stream fileStream,
      long calculatedLength,
      long offsetFrom,
      CompressionType compressionType,
      ReviewFileType fileType = ReviewFileType.ChangeEntry,
      string fileName = null);

    ReviewFileContentInfo UploadFile(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Stream fileStream,
      long calculatedLength,
      long offsetFrom,
      CompressionType compressionType,
      ReviewFileType fileType = ReviewFileType.ChangeEntry,
      string fileName = null);

    ChangeEntryStream DownloadFile(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      byte[] contentHash,
      ReviewFileType fileType = ReviewFileType.ChangeEntry);

    Dictionary<string, ChangeEntryStream> DownloadFilesStreams(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<string> contentHashes,
      ReviewFileType fileType = ReviewFileType.ChangeEntry);

    PushStreamContent DownloadFilesZipped(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<string> contentHashes,
      out bool needsNextPage,
      int? top = null,
      int? skip = null,
      ReviewFileType fileType = ReviewFileType.ChangeEntry);

    PropertiesCollection GetProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId);

    PropertiesCollection PatchProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      PropertiesCollection properties);
  }
}
