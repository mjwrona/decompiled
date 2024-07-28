// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewService
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.Server.Utils;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.CodeReview.WebApi.Events;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Common.Internal;
using Microsoft.VisualStudio.Services.Identity;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal class CodeReviewService : CodeReviewServiceBase, ICodeReviewService, IVssFrameworkService
  {
    private const long c_MaxAttachmentFileSize = 26214400;
    private const int c_MaxNumAttachments = 100;
    private readonly string[] c_AllowedAttachmentFileTypes = new string[15]
    {
      "PNG",
      "GIF",
      "JPG",
      "JPEG",
      "DOCX",
      "PPTX",
      "XLSX",
      "TXT",
      "PDF",
      "ZIP",
      "GZ",
      "LYR",
      "MOV",
      "MP4",
      "CSV"
    };

    public override void ServiceStart(IVssRequestContext systemRequestContext) => base.ServiceStart(systemRequestContext);

    public override void ServiceEnd(IVssRequestContext systemRequestContext) => base.ServiceEnd(systemRequestContext);

    public virtual Review SaveReview(
      IVssRequestContext requestContext,
      Guid projectId,
      Review review)
    {
      ICodeReviewIterationService iterationService = requestContext.GetService<ICodeReviewIterationService>();
      if (this.GetReviewContentExtension(review.SourceArtifactId) != null)
        review.CustomStorage = true;
      Review savedReview = (Review) null;
      this.ExecuteAndTrace(requestContext, 1380001, 1380002, 1380003, (Action) (() =>
      {
        Review review1 = (Review) null;
        bool hasStateChanged = false;
        ReviewNotification crEvent;
        if (review.Id == 0)
        {
          ArgumentUtility.CheckStringForNullOrWhiteSpace(review.Title, "title");
          if (review.Description == null)
            review.Description = string.Empty;
          ReviewStatus? status = review.Status;
          if (status.HasValue)
          {
            status = review.Status;
            ReviewStatus reviewStatus1 = ReviewStatus.Active;
            if (!(status.GetValueOrDefault() == reviewStatus1 & status.HasValue))
            {
              status = review.Status;
              ReviewStatus reviewStatus2 = ReviewStatus.Creating;
              if (!(status.GetValueOrDefault() == reviewStatus2 & status.HasValue))
                throw new ArgumentException("Review must start in Active, or the Creating state.", "status");
            }
          }
          else
            review.Status = new ReviewStatus?(ReviewStatus.Active);
          DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
          crEvent = (ReviewNotification) new ReviewPublishedNotification(projectId, review);
        }
        else
        {
          ArgumentUtility.CheckForOutOfRange(review.Id, "reviewId", 1);
          review1 = this.GetReviewRaw(requestContext, projectId, review.Id);
          ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, review1.Id, review1.SourceArtifactId);
          if (review.Title != null)
            ArgumentUtility.CheckStringForNullOrWhiteSpace(review.Title, "title");
          ReviewStatus? status = review.Status;
          if (status.HasValue)
            hasStateChanged = true;
          if (hasStateChanged)
          {
            status = review1.Status;
            ReviewStatus reviewStatus3 = ReviewStatus.Creating;
            if (status.GetValueOrDefault() == reviewStatus3 & status.HasValue)
            {
              status = review.Status;
              ReviewStatus reviewStatus4 = ReviewStatus.Active;
              if (status.GetValueOrDefault() == reviewStatus4 & status.HasValue)
              {
                crEvent = (ReviewNotification) new ReviewPublishedNotification(projectId, review);
                goto label_18;
              }
            }
          }
          crEvent = (ReviewNotification) new ReviewUpdatedNotification(projectId, review, review1.SourceArtifactId, review1.UpdatedDate, hasStateChanged);
        }
label_18:
        this.ValidateCommonReviewArguments(requestContext, review, iterationService.GetMaxChangeEntriesForChangeTrackingComputation(requestContext));
        if (review.Iterations != null && review.Iterations.Count == 1)
          review.Iterations[0] = ValidationHelper.SanitizeIterationInput(requestContext, review.Iterations[0], false, false);
        requestContext.Trace(1380004, TraceLevel.Verbose, this.Area, this.Layer, "Saving a code review: id: '{0}', title: '{1}', status: '{2}', iteration: '{3}', project id: '{4}'", (object) review.Id, (object) review.Title, (object) review.Status, review.Iterations == null ? (object) "{null}" : (object) review.Iterations.Count.ToString(), (object) projectId);
        EventServiceHelper.PublishDecisionPoint(requestContext, (CodeReviewEventNotification) crEvent);
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          savedReview = component.SaveReview(projectId, review);
        this.SaveReviewProperties(requestContext, projectId, review, savedReview);
        if (review.Iterations != null && review.Iterations.Count == 1 && savedReview.Iterations != null && savedReview.Iterations.Count == 1)
          savedReview.Iterations.ToList<Iteration>()[0] = this.SaveIterationProperties(requestContext, projectId, review.Iterations.First<Iteration>(), savedReview.Iterations.First<Iteration>());
        savedReview.AddReferenceLinks(requestContext, projectId, savedReview.Id);
        savedReview = IdentityHelper.FillIdentities(requestContext, (IEnumerable<Review>) new Review[1]
        {
          savedReview
        }).First<Review>();
        EventServiceHelper.PublishNotification(requestContext, !(crEvent is ReviewPublishedNotification) ? (CodeReviewEventNotification) new ReviewUpdatedNotification(projectId, savedReview.ShallowClone(), review1.SourceArtifactId, savedReview.PriorReviewUpdatedTimestamp, hasStateChanged) : (CodeReviewEventNotification) new ReviewPublishedNotification(projectId, savedReview.ShallowClone()), this.Area, this.Layer);
      }), nameof (SaveReview));
      return savedReview;
    }

    public Review GetReview(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      CodeReviewExtendedProperties extendedProperties = CodeReviewExtendedProperties.All,
      ReviewScope reviewScope = ReviewScope.All,
      int? maxChangesCount = null)
    {
      return this.ExecuteAndTrace<Review>(requestContext, 1380011, 1380012, 1380013, (Func<Review>) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        int? changesToFetch = this.EvaluateChangesToFetch(maxChangesCount);
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        int[] reviewIds = new int[1]{ reviewId };
        DateTime? modifiedSince = new DateTime?();
        int? nullable = changesToFetch;
        int num = (int) reviewScope;
        int? maxChangesCount1 = nullable;
        Review review = this.GetReviewsInternal(requestContext1, projectId1, (IEnumerable<int>) reviewIds, 1, 0, true, modifiedSince, (ReviewScope) num, maxChangesCount1).SingleOrDefault<Review>();
        if (review == null)
          throw new CodeReviewNotFoundException(reviewId);
        requestContext.Trace(1380014, TraceLevel.Verbose, this.Area, this.Layer, "Getting a code review: id: '{0}', project id: '{1}', extendedProperties: '{2}', maxChangesCount: '{3}'", (object) reviewId, (object) projectId, (object) extendedProperties, (object) maxChangesCount);
        if (extendedProperties.HasFlag((Enum) CodeReviewExtendedProperties.Review))
          ArtifactPropertyKinds.FetchReviewExtendedProperties(requestContext, projectId, (IList<Review>) new List<Review>()
          {
            review
          });
        if (extendedProperties.HasFlag((Enum) CodeReviewExtendedProperties.Iteration))
          ArtifactPropertyKinds.FetchIterationExtendedProperties(requestContext, projectId, review.Iterations);
        if (extendedProperties.HasFlag((Enum) CodeReviewExtendedProperties.Attachment))
          ArtifactPropertyKinds.FetchAttachmentExtendedProperties(requestContext, projectId, review.Attachments);
        if (review.Iterations != null && review.Iterations.Count > 0)
        {
          this.EnsurePartiallyFetchedIterationDoesNotExist(review.Iterations, changesToFetch);
          this.FilterChangeEntries(requestContext, projectId, review);
        }
        review.AddReferenceLinks(requestContext, projectId, review.Id);
        return review;
      }), nameof (GetReview));
    }

    public IEnumerable<Review> GetReviews(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> reviewIds,
      int? top = null,
      int? skip = null,
      bool includeDeleted = false,
      DateTime? modifiedSince = null)
    {
      return this.ExecuteAndTrace<IEnumerable<Review>>(requestContext, 1380021, 1380022, 1380023, (Func<IEnumerable<Review>>) (() =>
      {
        int topValue;
        int skipValue;
        ValidationHelper.EvaluateTopSkip(top, skip, out topValue, out skipValue);
        if (reviewIds != null)
        {
          foreach (int reviewId in reviewIds)
            ArgumentUtility.CheckForOutOfRange(reviewId, "reviewId", 1);
        }
        this.TraceGetReviewsInfo(requestContext, projectId, reviewIds, new int?(topValue), new int?(skipValue), includeDeleted, modifiedSince);
        return this.GetReviewsInternal(requestContext, projectId, reviewIds, topValue, skipValue, includeDeleted, modifiedSince);
      }), nameof (GetReviews));
    }

    private IEnumerable<Review> GetReviewsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> reviewIds,
      int top,
      int skip,
      bool includeDeleted,
      DateTime? modifiedSince,
      ReviewScope reviewScope = ReviewScope.All,
      int? maxChangesCount = null)
    {
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      IEnumerable<Review> reviews;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        reviews = component.GetReviews(projectId, reviewIds, top, skip, includeDeleted, modifiedSince, maxChangesCount, reviewScope);
      IEnumerable<Review> list = (IEnumerable<Review>) reviews.Where<Review>((Func<Review, bool>) (review => ReviewSecurityEvaluator.HasReviewAccess(requestContext, this.SecurityExtensions, projectId, review.Id, review.SourceArtifactId))).ToList<Review>();
      return IdentityHelper.FillIdentities(requestContext, list);
    }

    public virtual void DeleteReview(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      bool destroy = false)
    {
      this.ExecuteAndTrace(requestContext, 1380031, 1380032, 1380033, (Action) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        requestContext.Trace(1380034, TraceLevel.Verbose, this.Area, this.Layer, "Deleting a code review: id: '{0}', project id: '{1}'", (object) reviewId, (object) projectId);
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewRaw.Id, reviewRaw.SourceArtifactId);
        Guid projectId1 = projectId;
        Review review = new Review();
        review.Id = reviewId;
        string sourceArtifactId = reviewRaw.SourceArtifactId;
        int num = destroy ? 1 : 0;
        EventServiceHelper.PublishDecisionPoint(requestContext, (CodeReviewEventNotification) new ReviewDeletedNotification(projectId1, review, sourceArtifactId, num != 0));
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        {
          if (destroy)
            component.DestroyReview(projectId, reviewId);
          else
            component.DeleteReview(projectId, reviewId);
        }
        EventServiceHelper.PublishNotification(requestContext, (CodeReviewEventNotification) new ReviewDeletedNotification(projectId, reviewRaw.ShallowClone(), reviewRaw.SourceArtifactId, destroy), this.Area, this.Layer);
      }), nameof (DeleteReview));
    }

    public IEnumerable<Review> QueryReviewsBySourceArtifactId(
      IVssRequestContext requestContext,
      Guid projectId,
      string sourceArtifactId,
      bool includeDeleted = false,
      int? maxChangesCount = null)
    {
      return this.ExecuteAndTrace<IEnumerable<Review>>(requestContext, 1380051, 1380052, 1380053, (Func<IEnumerable<Review>>) (() =>
      {
        requestContext.Trace(1380054, TraceLevel.Verbose, this.Area, this.Layer, "Querying reviews by source artifact id: id: '{0}', project id: '{1}', includeDeleted: '{2}', maxChangesCount: '{3}'", (object) sourceArtifactId, (object) projectId, (object) includeDeleted, (object) maxChangesCount);
        int? changesToFetch = this.EvaluateChangesToFetch(maxChangesCount);
        return this.QueryReviewsBySourceArtifactIdsInternal(requestContext, projectId, (IEnumerable<string>) new string[1]
        {
          sourceArtifactId
        }, (includeDeleted ? 1 : 0) != 0, changesToFetch);
      }), nameof (QueryReviewsBySourceArtifactId));
    }

    public IEnumerable<Review> QueryReviewsBySourceArtifactIds(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> sourceArtifactIds,
      bool includeDeleted = false)
    {
      return this.ExecuteAndTrace<IEnumerable<Review>>(requestContext, 1380041, 1380042, 1380043, (Func<IEnumerable<Review>>) (() =>
      {
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) sourceArtifactIds.ToArray<string>(), nameof (sourceArtifactIds));
        this.TraceQueryReviewsBySourceArtifactIdsInfo(requestContext, projectId, sourceArtifactIds, includeDeleted);
        return this.QueryReviewsBySourceArtifactIdsInternal(requestContext, projectId, sourceArtifactIds, includeDeleted);
      }), nameof (QueryReviewsBySourceArtifactIds));
    }

    public IEnumerable<Review> QueryReviewsByFilters(
      IVssRequestContext requestContext,
      Guid? projectId,
      ReviewSearchCriteria searchCriteria,
      int top,
      out int totalReviewsFound)
    {
      int localTotalReviewsFound = 0;
      IEnumerable<Review> reviews1 = this.ExecuteAndTrace<IEnumerable<Review>>(requestContext, 1380161, 1380162, 1380163, (Func<IEnumerable<Review>>) (() =>
      {
        if (projectId.HasValue)
          DataspaceHelper.EnsureDataspaceExists(requestContext, projectId.Value);
        Guid? creatorId;
        Guid? reviewerId;
        CodeReviewService.ValidateAndParseSearchCriteria(projectId, searchCriteria, out creatorId, out reviewerId);
        requestContext.Trace(1380164, TraceLevel.Verbose, this.Area, this.Layer, string.Format("Querying reviews by filters: project id: '{0}', source artifact prefix: '{1}' ", (object) projectId, (object) searchCriteria.SourceArtifactPrefix) + string.Format("status: '{0}', creator id: '{1}', reviewer id: '{2}' ", (object) searchCriteria.Status, (object) creatorId, (object) reviewerId) + string.Format("min created date: '{0}', max created date: '{1}', min updated date: '{2}' ", (object) searchCriteria.MinCreatedDate, (object) searchCriteria.MaxCreatedDate, (object) searchCriteria.MinUpdatedDate) + string.Format("max updated date: '{0}', order ascending: '{1}'", (object) searchCriteria.MaxUpdatedDate, (object) searchCriteria.OrderAscending));
        IEnumerable<Review> reviews2;
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          reviews2 = component.QueryReviewsByFilters(projectId, searchCriteria, top, creatorId, reviewerId);
        localTotalReviewsFound = reviews2.Count<Review>();
        IEnumerable<Review> reviews3 = (IEnumerable<Review>) ReviewSecurityEvaluator.FetchFilteredReviews(requestContext, this.SecurityExtensions, reviews2);
        foreach (Review review in reviews3)
          review.AddReferenceLinks(requestContext, review.ProjectId, review.Id);
        return IdentityHelper.FillIdentities(requestContext, reviews3);
      }), nameof (QueryReviewsByFilters));
      totalReviewsFound = localTotalReviewsFound;
      return reviews1;
    }

    public ReviewFileContentInfo UploadFile(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      byte[] contentHash,
      Stream fileStream,
      long calculatedLength,
      long offsetFrom,
      CompressionType compressionType,
      ReviewFileType fileType = ReviewFileType.ChangeEntry,
      string fileName = null)
    {
      return this.ExecuteAndTrace<ReviewFileContentInfo>(requestContext, 1380061, 1380062, 1380063, (Func<ReviewFileContentInfo>) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) contentHash, nameof (contentHash));
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        if (fileType == ReviewFileType.Attachment)
          this.ValidateAttachment(requestContext, projectId, reviewId, fileName, calculatedLength);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
        if (this.ReviewHasCustomStorage(reviewRaw) && this.ShouldUseReviewCustomStorage(fileType))
          throw new CodeReviewContentUploadNotAllowedException(reviewId);
        List<ReviewFileContentInfo> source = this.GetContentMetadataInternal(requestContext, projectId, reviewId, (IEnumerable<byte[]>) new List<byte[]>()
        {
          contentHash
        });
        int fileServiceFileId = source.Count == 1 ? source.First<ReviewFileContentInfo>().FileServiceFileId : 0;
        if (fileServiceFileId <= 0)
        {
          ContentAccessMetadata? nullable = this.SaveFile(requestContext, projectId, reviewId, contentHash, fileServiceFileId, fileStream, calculatedLength, offsetFrom, compressionType);
          IVssRequestContext requestContext1 = requestContext;
          Guid projectId1 = projectId;
          int reviewId1 = reviewId;
          List<ContentAccessMetadata> fileInfoToSave = new List<ContentAccessMetadata>();
          fileInfoToSave.Add(nullable.Value);
          int fileType1 = (int) fileType;
          source = this.SaveContentMetadata(requestContext1, projectId1, reviewId1, fileInfoToSave, (ReviewFileType) fileType1);
        }
        requestContext.Trace(1380064, TraceLevel.Verbose, this.Area, this.Layer, "Uploading File: review id: '{0}', project id: '{1}', fileType: '{2}', file path: '{3}'", (object) reviewId, (object) projectId, (object) fileType.ToString(), source.FirstOrDefault<ReviewFileContentInfo>() == null ? (object) "{null}" : (object) source.FirstOrDefault<ReviewFileContentInfo>().Path);
        return source.FirstOrDefault<ReviewFileContentInfo>();
      }), nameof (UploadFile));
    }

    public ReviewFileContentInfo UploadFile(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      Stream fileStream,
      long calculatedLength,
      long offsetFrom,
      CompressionType compressionType,
      ReviewFileType fileType = ReviewFileType.ChangeEntry,
      string fileName = null)
    {
      byte[] hash = new SHA1Cng().ComputeHash(fileStream);
      fileStream.Seek(0L, SeekOrigin.Begin);
      return this.UploadFile(requestContext, projectId, reviewId, hash, fileStream, calculatedLength, offsetFrom, compressionType, fileType, fileName);
    }

    public ChangeEntryStream DownloadFile(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      byte[] contentHash,
      ReviewFileType fileType = ReviewFileType.ChangeEntry)
    {
      return this.ExecuteAndTrace<ChangeEntryStream>(requestContext, 1380071, 1380072, 1380073, (Func<ChangeEntryStream>) (() =>
      {
        ChangeEntryStream changeEntryStream = new ChangeEntryStream();
        IVssRequestContext requestContext1 = requestContext;
        Guid projectId1 = projectId;
        int reviewId1 = reviewId;
        List<string> contentHashes = new List<string>();
        contentHashes.Add(ReviewFileContentExtensions.ToSha1HashString(contentHash));
        int fileType1 = (int) fileType;
        Dictionary<string, ChangeEntryStream> dictionary = this.DownloadFilesStreams(requestContext1, projectId1, reviewId1, contentHashes, (ReviewFileType) fileType1);
        if (dictionary.Values.Count > 0)
          changeEntryStream = dictionary.Values.ElementAt<ChangeEntryStream>(0);
        return changeEntryStream;
      }), nameof (DownloadFile));
    }

    public Dictionary<string, ChangeEntryStream> DownloadFilesStreams(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<string> contentHashes,
      ReviewFileType fileType = ReviewFileType.ChangeEntry)
    {
      return this.ExecuteAndTrace<Dictionary<string, ChangeEntryStream>>(requestContext, 1380111, 1380112, 1380113, (Func<Dictionary<string, ChangeEntryStream>>) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) contentHashes, nameof (contentHashes));
        contentHashes.ForEach((Action<string>) (hash => ArgumentUtility.CheckStringForNullOrEmpty(hash, "contentHash")));
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
        requestContext.Trace(1380114, TraceLevel.Verbose, this.Area, this.Layer, "Downloading Files (streams): review id: '{0}', project id: '{1}'", (object) reviewId, (object) projectId);
        List<ReviewFileContentInfo> metadataInternal = this.GetContentMetadataInternal(requestContext, projectId, reviewId, (IEnumerable<byte[]>) contentHashes.Select<string, byte[]>((Func<string, byte[]>) (hash => ReviewFileContentExtensions.ToSha1HashBytes(hash))).ToList<byte[]>());
        List<ContentInfo> contentInfo = new List<ContentInfo>();
        bool onlyOneHash = contentHashes.Count == 1;
        foreach (ReviewFileContentInfo reviewFileContentInfo in metadataInternal)
        {
          if (reviewFileContentInfo != null)
            contentInfo.Add(new ContentInfo(reviewFileContentInfo.SHA1Hash, reviewFileContentInfo.Flags));
        }
        IReviewContentProvider contentExtension = this.GetReviewContentExtension(reviewRaw.SourceArtifactId);
        Dictionary<string, ChangeEntryStream> contentHashesToChangeEntryStreams = new Dictionary<string, ChangeEntryStream>();
        if (this.ShouldUseReviewCustomStorage(fileType) && contentExtension != null)
        {
          CompressionType compressionType;
          Dictionary<string, Stream> contentHashesToStreams = contentExtension.GetContentStreams(requestContext, LinkingUtilities.DecodeUri(reviewRaw.SourceArtifactId), (IEnumerable<ContentInfo>) contentInfo, out compressionType);
          List<string> hashesFound = contentHashesToStreams.Keys.ToList<string>();
          contentHashes.ForEach((Action<string>) (hash =>
          {
            if (hashesFound.Contains(hash))
              return;
            this.TraceAndThrowFileNotFound(requestContext, projectId, reviewId, hash, onlyOneHash);
          }));
          contentHashesToStreams.Keys.ToList<string>().ForEach((Action<string>) (hash => contentHashesToChangeEntryStreams.Add(hash, new ChangeEntryStream(contentHashesToStreams[hash], compressionType))));
        }
        else
        {
          List<string> hashesFound = metadataInternal.Select<ReviewFileContentInfo, string>((Func<ReviewFileContentInfo, string>) (info => info.SHA1Hash)).ToList<string>();
          contentHashes.ForEach((Action<string>) (hash =>
          {
            if (hashesFound.Contains(hash))
              return;
            this.TraceAndThrowFileNotFound(requestContext, projectId, reviewId, hash, onlyOneHash);
          }));
          metadataInternal.ForEach((Action<ReviewFileContentInfo>) (fileInfo =>
          {
            if (fileInfo.FileServiceFileId <= 0)
            {
              this.TraceAndThrowFileNotFound(requestContext, projectId, reviewId, fileInfo.SHA1Hash, onlyOneHash);
            }
            else
            {
              if (contentHashesToChangeEntryStreams.ContainsKey(fileInfo.SHA1Hash))
                return;
              contentHashesToChangeEntryStreams.Add(fileInfo.SHA1Hash, ReviewFilesUtility.GetFileContentStream(requestContext, fileInfo, fileInfo.SHA1Hash));
            }
          }));
        }
        return contentHashesToChangeEntryStreams;
      }), nameof (DownloadFilesStreams));
    }

    public PushStreamContent DownloadFilesZipped(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<string> contentHashes,
      out bool needsNextPage,
      int? top = null,
      int? skip = null,
      ReviewFileType fileType = ReviewFileType.ChangeEntry)
    {
      PushStreamContent filesContent = (PushStreamContent) null;
      bool nextPageExists = false;
      this.ExecuteAndTrace(requestContext, 1380101, 1380102, 1380103, (Action) (() =>
      {
        int topValue;
        int skipValue;
        ValidationHelper.EvaluateTopSkip(top, skip, out topValue, out skipValue);
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        this.ValidateContentHashes(contentHashes);
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
        requestContext.Trace(1380104, TraceLevel.Verbose, this.Area, this.Layer, "Downloading Files (zipped): review id: '{0}', project id: '{1}', fileType: '{2}'", (object) reviewId, (object) projectId, (object) fileType.ToString());
        List<string> filesToDownloadList = ReviewFilesUtility.GetFilesToDownloadList(contentHashes, topValue, skipValue, out nextPageExists);
        IReviewContentProvider contentExtension = this.GetReviewContentExtension(reviewRaw.SourceArtifactId);
        List<ReviewFileContentInfo> metadataInternal = this.GetContentMetadataInternal(requestContext, projectId, reviewId, filesToDownloadList.Select<string, byte[]>((Func<string, byte[]>) (contentHash => ReviewFileContentExtensions.ToSha1HashBytes(contentHash))));
        if (this.ShouldUseReviewCustomStorage(fileType) && contentExtension != null)
        {
          filesContent = ReviewFilesUtility.GetZipPushStreamContentFromExtension(requestContext, reviewRaw, metadataInternal, contentExtension);
        }
        else
        {
          this.ValidateContentMetadata((IEnumerable<ReviewFileContentInfo>) metadataInternal, (IEnumerable<string>) filesToDownloadList);
          filesContent = ReviewFilesUtility.GetZipPushStreamContent(requestContext, metadataInternal);
        }
      }), nameof (DownloadFilesZipped));
      needsNextPage = nextPageExists;
      return filesContent;
    }

    public PropertiesCollection GetProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      return this.ExecuteAndTrace<PropertiesCollection>(requestContext, 1383411, 1383412, 1383413, (Func<PropertiesCollection>) (() =>
      {
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, this.GetReviewRaw(requestContext, projectId, reviewId).SourceArtifactId);
        requestContext.Trace(1383414, TraceLevel.Verbose, this.Area, this.Layer, "Getting review properties: project id: '{0}', review id: '{1}'", (object) projectId, (object) reviewId);
        return ArtifactPropertyKinds.GetProperties(requestContext, ArtifactPropertyKinds.MakeArtifactSpec(ServerConstants.ReviewPropertyKind, ArtifactPropertyKinds.GetReviewMoniker(projectId, reviewId)));
      }), nameof (GetProperties));
    }

    public PropertiesCollection PatchProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      PropertiesCollection properties)
    {
      PropertiesCollection patchedProperties = (PropertiesCollection) null;
      this.ExecuteAndTrace(requestContext, 1383401, 1383402, 1383403, (Action) (() =>
      {
        ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
        DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
        Review reviewRaw = this.GetReviewRaw(requestContext, projectId, reviewId);
        ReviewSecurityEvaluator.CheckReviewAccess(requestContext, this.SecurityExtensions, projectId, reviewId, reviewRaw.SourceArtifactId);
        if (!reviewRaw.CanUpdate())
          throw new CodeReviewNotActiveException(reviewId);
        if (requestContext.IsTracing(1383404, TraceLevel.Verbose, this.Area, this.Layer))
          requestContext.Trace(1383404, TraceLevel.Verbose, this.Area, this.Layer, "Patching review properties: project id: '{0}', review id: '{1}', properties: '{2}'", (object) projectId, (object) reviewId, (object) ArtifactPropertyKinds.PreparePatchPropertiesInfo(properties));
        EventServiceHelper.PublishDecisionPoint(requestContext, (CodeReviewEventNotification) new PropertiesNotification(projectId, reviewId, reviewRaw.SourceArtifactId, reviewRaw.UpdatedDate, reviewRaw.UpdatedDate, ResourceType.Review, reviewId));
        patchedProperties = ArtifactPropertyKinds.PatchProperties(requestContext, ArtifactPropertyKinds.MakeArtifactSpec(ServerConstants.ReviewPropertyKind, ArtifactPropertyKinds.GetReviewMoniker(projectId, reviewId)), properties);
        UpdateTimestamps updateTimestamps;
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          updateTimestamps = component.MarkReviewAsModified(projectId, reviewId);
        EventServiceHelper.PublishNotification(requestContext, (CodeReviewEventNotification) new PropertiesNotification(projectId, reviewId, reviewRaw.SourceArtifactId, new DateTime?(updateTimestamps.Prior), new DateTime?(updateTimestamps.Current), ResourceType.Review, reviewId), this.Area, this.Layer);
      }), nameof (PatchProperties));
      return patchedProperties;
    }

    private IEnumerable<Review> QueryReviewsBySourceArtifactIdsInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> sourceArtifactIds,
      bool includeDeleted,
      int? maxChangesCount = null)
    {
      foreach (string sourceArtifactId in sourceArtifactIds)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(sourceArtifactId, "sourceArtifactId");
        CodeReviewService.CheckForWellformedSourceArtifactId(sourceArtifactId);
      }
      DataspaceHelper.EnsureDataspaceExists(requestContext, projectId);
      IEnumerable<Review> source;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
        source = component.QueryReviewsBySourceArtifactIds(projectId, sourceArtifactIds, includeDeleted, maxChangesCount);
      IEnumerable<Review> list = (IEnumerable<Review>) source.Where<Review>((Func<Review, bool>) (review => ReviewSecurityEvaluator.HasReviewAccess(requestContext, this.SecurityExtensions, projectId, review.Id, review.SourceArtifactId))).ToList<Review>();
      foreach (Review review in list)
        review.AddReferenceLinks(requestContext, projectId, review.Id);
      return IdentityHelper.FillIdentities(requestContext, list);
    }

    private void EnsurePartiallyFetchedIterationDoesNotExist(
      IList<Iteration> iterations,
      int? changeToFetch)
    {
      if (!changeToFetch.HasValue)
        return;
      IList<ChangeEntry> changeList1 = iterations[0].ChangeList;
      int? nullable1 = changeList1 != null ? changeList1.FirstOrDefault<ChangeEntry>()?.TotalChangesCount : new int?();
      if (!nullable1.HasValue)
        return;
      int num = nullable1.Value;
      int? nullable2 = changeToFetch;
      int valueOrDefault = nullable2.GetValueOrDefault();
      if (!(num > valueOrDefault & nullable2.HasValue))
        return;
      for (int index = iterations.Count - 1; index > 0; --index)
      {
        IList<ChangeEntry> changeList2 = iterations[index].ChangeList;
        IList<ChangeEntry> changeList3 = iterations[index - 1].ChangeList;
        if (changeList2 == null || changeList2.Count == 0)
        {
          iterations[index].ChangeList = (IList<ChangeEntry>) null;
          if (changeList3 != null && changeList3.Count > 0)
          {
            iterations[index - 1].ChangeList = (IList<ChangeEntry>) null;
            return;
          }
        }
      }
      iterations[iterations.Count - 1].ChangeList = (IList<ChangeEntry>) null;
    }

    private void FilterChangeEntries(
      IVssRequestContext requestContext,
      Guid projectId,
      Review review)
    {
      foreach (Iteration iteration in (IEnumerable<Iteration>) review.Iterations)
      {
        if (iteration.ChangeList != null)
        {
          List<ChangeEntry> changeEntryList = new List<ChangeEntry>();
          foreach (ChangeEntry change in (IEnumerable<ChangeEntry>) iteration.ChangeList)
          {
            if (change.Base != null || change.Modified != null)
              changeEntryList.Add(change);
          }
          iteration.ChangeList = (IList<ChangeEntry>) changeEntryList;
        }
      }
    }

    private void ValidateContentMetadata(
      IEnumerable<ReviewFileContentInfo> fileInfos,
      IEnumerable<string> contentHashes)
    {
      if (fileInfos == null || fileInfos.Count<ReviewFileContentInfo>() == 0)
        throw new FileNotFoundException(CodeReviewResources.FilesWithContentHashesNotFoundException());
      HashSet<string> stringSet = new HashSet<string>(fileInfos.Select<ReviewFileContentInfo, string>((Func<ReviewFileContentInfo, string>) (fileInfo => fileInfo.SHA1Hash)), (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase);
      foreach (string contentHash in contentHashes)
      {
        if (!stringSet.Contains(contentHash))
          throw new FileNotFoundException(CodeReviewResources.FileNotFoundException((object) contentHash));
      }
    }

    private void ValidateContentHashes(List<string> contentHashes)
    {
      ArgumentUtility.CheckEnumerableForNullOrEmpty((IEnumerable) contentHashes, nameof (contentHashes));
      foreach (string contentHash in contentHashes)
        ArgumentUtility.CheckStringForNullOrEmpty(contentHash, "contentHash");
      HashSet<string> stringSet = new HashSet<string>();
      foreach (string contentHash in contentHashes)
      {
        if (!stringSet.Add(contentHash))
          throw new ArgumentException(CodeReviewResources.DuplicateContentHashInContentHashList((object) contentHash));
      }
    }

    private void ValidateCommonReviewArguments(
      IVssRequestContext requestContext,
      Review review,
      int maxChangeEntries)
    {
      DateTime? nullable = review.Author == null ? review.CompletedDate : throw new ArgumentException(CodeReviewResources.CannotSpecifyAuthor(), "author");
      nullable = !nullable.HasValue ? review.UpdatedDate : throw new ArgumentException(CodeReviewResources.CannotSpecifyCompletedDate(), "completedDate");
      nullable = !nullable.HasValue ? review.CreatedDate : throw new ArgumentException(CodeReviewResources.CannotSpecifyUpdatedDate(), "updatedDate");
      if (nullable.HasValue)
        throw new ArgumentException(CodeReviewResources.CannotSpecifyCreatedDate(), "createdDate");
      if (review.IsDeleted)
        throw new ArgumentException(CodeReviewResources.CannotCreateOrUpdateReviewWithIsDeletedTrue(), "IsDeleted");
      if (review.Id == 0)
        review.Author = IdentityHelper.GetRequesterIdentityRef(requestContext);
      if (!string.IsNullOrEmpty(review.SourceArtifactId))
        CodeReviewService.CheckForWellformedSourceArtifactId(review.SourceArtifactId);
      if (review.Iterations != null && review.Iterations.Count > 0)
      {
        if (review.Iterations.Count > 1)
          throw new ArgumentException(CodeReviewResources.IterationCanNotBeSaved((object) review.Iterations.Count), "Iterations");
        if (review.Id > 0)
          throw new ArgumentException(CodeReviewResources.IterationsCannotBeUpdated(), "Iterations");
        ValidationHelper.ProcessIterationAndChanges(requestContext, review.Iterations.First<Iteration>(), maxChangeEntries);
      }
      if (review.Reviewers != null && review.Reviewers.Count<Reviewer>() > 0)
      {
        if (review.Id > 0)
          throw new ArgumentException(CodeReviewResources.ReviewersCannotBeUpdated(), "reviewers");
        Microsoft.VisualStudio.Services.Identity.Identity userIdentity = requestContext.GetUserIdentity();
        ValidationHelper.ValidateReviewers(requestContext, userIdentity, (IEnumerable<Reviewer>) review.Reviewers, (IList<Reviewer>) null);
      }
      if (review.Attachments != null && review.Attachments.Count<Attachment>() > 0)
      {
        if (review.Id > 0)
          throw new ArgumentException(CodeReviewResources.AttachmentsCannotBeUpdated(), "Attachments");
        throw new ArgumentException(CodeReviewResources.AttachmentsCannotBeCreated(), "Attachments");
      }
      if (review.Statuses == null || review.Statuses.Count<Status>() <= 0)
        return;
      if (review.Id > 0)
        throw new ArgumentException(CodeReviewResources.StatusesCannotBeUpdated(), "statuses");
      throw new ArgumentException(CodeReviewResources.StatusesCannotBeCreated(), "statuses");
    }

    public bool IsCurrentUserReviewAuthor(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId)
    {
      return IdentityHelper.CompareRequesterIdentity(requestContext, (this.GetReview(requestContext, projectId, reviewId, CodeReviewExtendedProperties.None, ReviewScope.All, new int?()) ?? throw new CodeReviewNotFoundException(reviewId)).Author.Id);
    }

    private static void CheckForWellformedSourceArtifactId(string sourceArtifactId)
    {
      if (!LinkingUtilities.IsUriWellFormed(sourceArtifactId))
        throw new ArgumentException(CommonResources.MalformedUri((object) sourceArtifactId), "SourceArtifactId");
    }

    private static void ValidateAndParseSearchCriteria(
      Guid? projectId,
      ReviewSearchCriteria searchCriteria,
      out Guid? creatorId,
      out Guid? reviewerId)
    {
      creatorId = new Guid?();
      reviewerId = new Guid?();
      if (searchCriteria.CreatorIdentity != (IdentityDescriptor) null)
      {
        Guid result;
        if (!Guid.TryParse(searchCriteria.CreatorIdentity.Identifier, out result))
          throw new ArgumentException(CodeReviewResources.IdentifierValidGuid(), "CreatorIdentity");
        creatorId = new Guid?(result);
      }
      if (searchCriteria.ReviewerIdentity != (IdentityDescriptor) null)
      {
        Guid result;
        if (!Guid.TryParse(searchCriteria.ReviewerIdentity.Identifier, out result))
          throw new ArgumentException(CodeReviewResources.IdentifierValidGuid(), "ReviewerIdentity");
        reviewerId = new Guid?(result);
      }
      ReviewStatus? status;
      DateTime? nullable;
      if (string.IsNullOrEmpty(searchCriteria.SourceArtifactPrefix))
      {
        status = searchCriteria.Status;
        if (!status.HasValue && searchCriteria.CreatorIdentity == (IdentityDescriptor) null && searchCriteria.ReviewerIdentity == (IdentityDescriptor) null)
        {
          nullable = searchCriteria.MinUpdatedDate;
          if (!nullable.HasValue)
          {
            nullable = searchCriteria.MaxUpdatedDate;
            if (!nullable.HasValue)
            {
              nullable = searchCriteria.MaxCreatedDate;
              if (!nullable.HasValue)
              {
                nullable = searchCriteria.MinCreatedDate;
                if (!nullable.HasValue)
                  throw new ArgumentException(CodeReviewResources.ReviewCriteriaMustBeSpecified());
              }
            }
          }
        }
      }
      status = searchCriteria.Status;
      if (!status.HasValue)
        throw new ArgumentException(CodeReviewResources.ReviewStatusMustBeSpecified());
      nullable = searchCriteria.MinUpdatedDate;
      if (nullable.HasValue)
      {
        nullable = searchCriteria.MaxUpdatedDate;
        if (nullable.HasValue)
        {
          nullable = searchCriteria.MaxCreatedDate;
          if (nullable.HasValue)
          {
            nullable = searchCriteria.MinCreatedDate;
            if (nullable.HasValue)
              throw new ArgumentException(CodeReviewResources.ReviewCriteriaCannotHaveBothCreateAndUpdateTimestamps());
          }
        }
      }
      if (!projectId.HasValue && searchCriteria.CreatorIdentity == (IdentityDescriptor) null && searchCriteria.ReviewerIdentity == (IdentityDescriptor) null)
        throw new ArgumentException(CodeReviewResources.AuthorOrReviewerMustBeSpecifiedWhileSearchingInAccount());
    }

    protected virtual int GetMaxNumAttachments() => 100;

    private void ValidateAttachment(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      string fileName,
      long fileLength)
    {
      IEnumerable<Attachment> attachments = requestContext.GetService<ICodeReviewAttachmentService>().GetAttachments(requestContext, projectId, reviewId);
      if (!string.IsNullOrEmpty(fileName))
      {
        string str = "";
        int num = fileName.LastIndexOf('.');
        if (num >= 0)
          str = fileName.Substring(num + 1);
        if (!((IEnumerable<string>) this.c_AllowedAttachmentFileTypes).Contains<string>(str, (IEqualityComparer<string>) StringComparer.OrdinalIgnoreCase))
          throw new ArgumentException(CodeReviewResources.InvalidAttachmentType((object) string.Join(", ", this.c_AllowedAttachmentFileTypes)));
        if (attachments.Where<Attachment>((Func<Attachment, bool>) (a => string.Equals(a.DisplayName, fileName))).FirstOrDefault<Attachment>() != null)
          throw new ArgumentException(CodeReviewResources.AttachmentAlreadyExists((object) fileName));
      }
      if (fileLength > 26214400L)
        throw new ArgumentException(CodeReviewResources.AttachmentSizeTooLarge((object) 26214400L));
      if (attachments.Count<Attachment>() >= this.GetMaxNumAttachments())
        throw new CodeReviewMaxAttachmentCountException(100);
    }

    private int? EvaluateChangesToFetch(int? maxChangesCount) => !maxChangesCount.HasValue ? maxChangesCount : new int?(Math.Min(2000, maxChangesCount.Value));

    private void TraceGetReviewsInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<int> reviewIds,
      int? top,
      int? skip,
      bool includeDeleted,
      DateTime? modifiedSince)
    {
      if (!requestContext.IsTracing(1380024, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (reviewIds == null)
      {
        stringBuilder.Append("all");
      }
      else
      {
        foreach (int reviewId in reviewIds)
          stringBuilder.Append(string.Format("'{0}',", (object) reviewId));
      }
      requestContext.Trace(1380024, TraceLevel.Verbose, this.Area, this.Layer, "Getting a list of code reviews: ids: '{0}', project id: '{1}', top: '{2}', skip: '{3}', includeDeleted: '{4}', modifiedSince: '{5}'", (object) stringBuilder.ToString(), (object) projectId, (object) top, (object) skip, (object) includeDeleted, (object) modifiedSince);
    }

    private void TraceQueryReviewsBySourceArtifactIdsInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      IEnumerable<string> sourceArtifactIds,
      bool includeDeleted)
    {
      if (!requestContext.IsTracing(1380044, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      foreach (string sourceArtifactId in sourceArtifactIds)
        stringBuilder.Append(string.Format("'{0}',", (object) sourceArtifactId));
      requestContext.Trace(1380044, TraceLevel.Verbose, this.Area, this.Layer, "Querying reviews by source artifact ids: ids: '{0}', project id: '{1}', includeDeleted: '{2}'", (object) stringBuilder.ToString(), (object) projectId, (object) includeDeleted);
    }

    private void TraceAndThrowFileNotFound(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      string hash,
      bool shouldThrow = false)
    {
      requestContext.Trace(1380114, TraceLevel.Verbose, this.Area, this.Layer, "File not found during downloading files (streams): review id: '{0}', project id: '{1}', hash: '{2}'", (object) reviewId, (object) projectId);
      if (shouldThrow)
        throw new FileNotFoundException(CodeReviewResources.FileNotFoundException((object) hash));
    }
  }
}
