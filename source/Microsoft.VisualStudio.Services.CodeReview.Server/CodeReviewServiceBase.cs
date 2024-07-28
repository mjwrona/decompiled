// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewServiceBase
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  internal abstract class CodeReviewServiceBase : IVssFrameworkService
  {
    private IDisposableReadOnlyList<ICodeReviewSecurityExtension> m_securityExtensions;
    private IDisposableReadOnlyList<IReviewContentProvider> m_contentProviderExtensions;
    private IDisposableReadOnlyList<IReviewNotificationContentProvider> m_notificationContentProviderExtensions;
    private IDisposableReadOnlyList<IArtifactProvider> m_artifactProviderExtensions;

    public virtual void ServiceStart(IVssRequestContext systemRequestContext)
    {
      this.m_contentProviderExtensions = systemRequestContext.GetExtensions<IReviewContentProvider>();
      this.m_securityExtensions = systemRequestContext.GetExtensions<ICodeReviewSecurityExtension>();
      this.m_notificationContentProviderExtensions = systemRequestContext.GetExtensions<IReviewNotificationContentProvider>();
      this.m_artifactProviderExtensions = systemRequestContext.GetExtensions<IArtifactProvider>();
    }

    public virtual void ServiceEnd(IVssRequestContext systemRequestContext)
    {
      if (this.m_contentProviderExtensions != null)
      {
        this.m_contentProviderExtensions.Dispose();
        this.m_contentProviderExtensions = (IDisposableReadOnlyList<IReviewContentProvider>) null;
      }
      if (this.m_securityExtensions != null)
      {
        this.m_securityExtensions.Dispose();
        this.m_securityExtensions = (IDisposableReadOnlyList<ICodeReviewSecurityExtension>) null;
      }
      if (this.m_notificationContentProviderExtensions != null)
      {
        this.m_notificationContentProviderExtensions.Dispose();
        this.m_notificationContentProviderExtensions = (IDisposableReadOnlyList<IReviewNotificationContentProvider>) null;
      }
      if (this.m_artifactProviderExtensions == null)
        return;
      this.m_artifactProviderExtensions.Dispose();
      this.m_artifactProviderExtensions = (IDisposableReadOnlyList<IArtifactProvider>) null;
    }

    protected void ExecuteAndTrace(
      IVssRequestContext requestContext,
      string methodName,
      int enterTracePoint,
      int exceptionTracePoint,
      int leaveTracePoint,
      Action action)
    {
      this.ExecuteAndTrace(requestContext, enterTracePoint, exceptionTracePoint, leaveTracePoint, action, methodName);
    }

    protected void ExecuteAndTrace(
      IVssRequestContext requestContext,
      int enterTracePoint,
      int exceptionTracePoint,
      int leaveTracePoint,
      Action action,
      [CallerMemberName] string methodName = null)
    {
      requestContext.TraceBlock(enterTracePoint, leaveTracePoint, exceptionTracePoint, this.Area, this.Layer, action, methodName);
    }

    protected T ExecuteAndTrace<T>(
      IVssRequestContext requestContext,
      int enterTracePoint,
      int exceptionTracePoint,
      int leaveTracePoint,
      Func<T> func,
      [CallerMemberName] string methodName = null)
    {
      return requestContext.TraceBlock<T>(enterTracePoint, leaveTracePoint, exceptionTracePoint, this.Area, this.Layer, func, methodName);
    }

    public static string DataspaceCategory => "Git";

    public string Area => this.GetType().Namespace;

    public string Layer => this.GetType().Name;

    protected void SaveReviewProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      Review reviewToSave,
      Review savedReview)
    {
      try
      {
        ArtifactPropertyKinds.SaveReviewProperties(requestContext, reviewToSave.Properties, projectId, savedReview.Id);
        savedReview.Properties = reviewToSave.Properties;
      }
      catch (Exception ex)
      {
        requestContext.Trace(1384006, TraceLevel.Error, this.Area, this.Layer, "There was an issue saving the review properties. ProjectId: {0}, ReviewId: {1}, Property count: {3}", (object) projectId, (object) reviewToSave.Id, (object) reviewToSave.Properties.Count);
        throw;
      }
    }

    protected Iteration SaveIterationProperties(
      IVssRequestContext requestContext,
      Guid projectId,
      Iteration iterationToSave,
      Iteration savedIteration)
    {
      try
      {
        ArtifactPropertyKinds.SaveIterationProperties(requestContext, iterationToSave.Properties, projectId, savedIteration.ReviewId, savedIteration.Id.Value);
        savedIteration.Properties = iterationToSave.Properties;
      }
      catch (Exception ex)
      {
        requestContext.Trace(1384001, TraceLevel.Error, this.Area, this.Layer, "There was an issue saving the iteration properties. ProjectId: {0}, ReviewId: {1}, IterationId: {2}, Property count: {3}", (object) projectId, (object) iterationToSave.ReviewId, (object) iterationToSave.Id, (object) iterationToSave.Properties.Count);
        throw;
      }
      savedIteration.AddReferenceLinks(requestContext, projectId, savedIteration.ReviewId, savedIteration.Id.Value);
      return savedIteration;
    }

    protected IReviewContentProvider GetReviewContentExtension(string sourceArtifactId)
    {
      if (string.IsNullOrEmpty(sourceArtifactId))
        return (IReviewContentProvider) null;
      ArtifactId artifact = LinkingUtilities.DecodeUri(sourceArtifactId);
      foreach (IReviewContentProvider providerExtension in (IEnumerable<IReviewContentProvider>) this.m_contentProviderExtensions)
      {
        if (providerExtension.CanResolveArtifactId(artifact))
          return providerExtension;
      }
      return (IReviewContentProvider) null;
    }

    protected IReviewNotificationContentProvider GetReviewNotificationContentExtension(
      string sourceArtifactId)
    {
      if (string.IsNullOrEmpty(sourceArtifactId))
        return (IReviewNotificationContentProvider) null;
      ArtifactId artifact = LinkingUtilities.DecodeUri(sourceArtifactId);
      foreach (IReviewNotificationContentProvider providerExtension in (IEnumerable<IReviewNotificationContentProvider>) this.m_notificationContentProviderExtensions)
      {
        if (providerExtension.CanResolveArtifactId(artifact))
          return providerExtension;
      }
      return (IReviewNotificationContentProvider) null;
    }

    protected List<ReviewFileContentInfo> GetContentMetadataInternal(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<byte[]> contentHashes)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      return this.ExecuteAndTrace<List<ReviewFileContentInfo>>(requestContext, 1380081, 1380082, 1380083, (Func<List<ReviewFileContentInfo>>) (() =>
      {
        this.TraceGetContentMetadataInfo(requestContext, projectId, reviewId, contentHashes, 1380084, "Getting content metadata");
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          return component.ReadContentMetadata(projectId, reviewId, contentHashes);
      }), nameof (GetContentMetadataInternal));
    }

    protected bool ReviewHasCustomStorage(Review review) => this.GetReviewContentExtension(review.SourceArtifactId) != null;

    protected bool ShouldUseReviewCustomStorage(ReviewFileType fileType) => fileType != ReviewFileType.Attachment;

    public Review GetReviewRaw(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      ReviewScope reviewScope = ReviewScope.ReviewLevelOnly)
    {
      Review cachedCodeReview = CodeReviewRequestContextCacheUtil.GetCachedCodeReview(requestContext, projectId, reviewId, reviewScope);
      if (cachedCodeReview != null)
        return cachedCodeReview;
      IEnumerable<Review> reviews;
      using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
      {
        CodeReviewComponent codeReviewComponent = component;
        Guid projectId1 = projectId;
        int[] reviewIds = new int[1]{ reviewId };
        DateTime? modifiedSince = new DateTime?();
        ReviewScope reviewScope1 = reviewScope;
        int? maxChangesCount = new int?(0);
        int num = (int) reviewScope1;
        reviews = codeReviewComponent.GetReviews(projectId1, (IEnumerable<int>) reviewIds, 1, 0, true, modifiedSince, maxChangesCount, (ReviewScope) num);
      }
      Review review = reviews.SingleOrDefault<Review>();
      if (review == null)
        throw new CodeReviewNotFoundException(reviewId);
      CodeReviewRequestContextCacheUtil.CacheCodeReview(requestContext, review, reviewScope);
      return review;
    }

    protected ContentAccessMetadata? SaveFile(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      byte[] contentHash,
      int existingFileId,
      Stream fileStream,
      long calculatedLength,
      long offsetFrom,
      CompressionType compressionType)
    {
      ITeamFoundationFileService service = requestContext.GetService<ITeamFoundationFileService>();
      if (existingFileId > 0)
        return new ContentAccessMetadata?();
      int fileId = existingFileId;
      if (fileId < 0)
        fileId = -fileId;
      if (!service.UploadFile(requestContext, ref fileId, fileStream, (byte[]) null, calculatedLength, calculatedLength, offsetFrom, compressionType, OwnerId.Generic, new Guid(), (string) null))
        fileId = -fileId;
      return new ContentAccessMetadata?(new ContentAccessMetadata()
      {
        ContentHash = contentHash,
        FileId = fileId
      });
    }

    protected List<ReviewFileContentInfo> SaveContentMetadata(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<ContentAccessMetadata> fileInfoToSave,
      ReviewFileType fileType)
    {
      ArgumentUtility.CheckForOutOfRange(reviewId, nameof (reviewId), 1);
      return this.ExecuteAndTrace<List<ReviewFileContentInfo>>(requestContext, 1380091, 1380092, 1380093, (Func<List<ReviewFileContentInfo>>) (() =>
      {
        this.TraceSaveContentMetadataInfo(requestContext, projectId, reviewId, fileInfoToSave, 1380094, "Saving content metadata");
        using (CodeReviewComponent component = requestContext.CreateComponent<CodeReviewComponent>())
          return component.SaveContentMetadata(projectId, reviewId, fileInfoToSave, fileType);
      }), nameof (SaveContentMetadata));
    }

    private void TraceSaveContentMetadataInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      List<ContentAccessMetadata> metadata,
      int tracePoint,
      string description)
    {
      if (!requestContext.IsTracing(tracePoint, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (metadata != null)
      {
        foreach (ContentAccessMetadata contentAccessMetadata in metadata)
          stringBuilder.Append(string.Format("'{0}',", (object) contentAccessMetadata.FilePath));
      }
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "{0}: review id: '{1}', project id: '{2}', file paths: '{3}'", (object) description, (object) reviewId, (object) projectId, (object) stringBuilder.ToString());
    }

    private void TraceGetContentMetadataInfo(
      IVssRequestContext requestContext,
      Guid projectId,
      int reviewId,
      IEnumerable<byte[]> contentHashes,
      int tracePoint,
      string description)
    {
      if (!requestContext.IsTracing(tracePoint, TraceLevel.Verbose, this.Area, this.Layer))
        return;
      StringBuilder stringBuilder = new StringBuilder();
      if (contentHashes != null)
      {
        foreach (byte[] contentHash in contentHashes)
          stringBuilder.Append(string.Format("'{0}',", (object) HexConverter.ToString(contentHash)));
      }
      requestContext.Trace(tracePoint, TraceLevel.Verbose, this.Area, this.Layer, "{0}: review id: '{1}', project id: '{2}', content hashes: '{3}'", (object) description, (object) reviewId, (object) projectId, (object) stringBuilder.ToString());
    }

    protected virtual bool IsUserProjectAdmin(IVssRequestContext requestContext, Guid projectId) => IdentityHelper.IsUserProjectAdmin(requestContext, projectId);

    public IDisposableReadOnlyList<ICodeReviewSecurityExtension> SecurityExtensions => this.m_securityExtensions;

    public virtual IDisposableReadOnlyList<IArtifactProvider> ArtifactProviderExtensions => this.m_artifactProviderExtensions;
  }
}
