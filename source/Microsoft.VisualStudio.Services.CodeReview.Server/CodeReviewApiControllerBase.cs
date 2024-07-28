// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.CodeReviewApiControllerBase
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.VisualStudio.Services.CodeReview.Discussion.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  public abstract class CodeReviewApiControllerBase : TfsProjectApiController
  {
    protected const string c_gzipEncoding = "gzip";
    protected const string c_fileExtensionZip = ".zip";
    public static Dictionary<Type, HttpStatusCode> s_httpExceptions = new Dictionary<Type, HttpStatusCode>();
    private const int c_filenameMaxLength = 260;
    private const string c_itemSeperator = ", ";
    private const string c_areaName = "CodeReviewService";

    static CodeReviewApiControllerBase()
    {
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (ArgumentNullException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (TeamFoundationServiceException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (ProjectException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (NotSupportedException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewIterationArgumentNullException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewChangesAlreadyExistException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewChangesWithContentHashNotFoundException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (ArgumentOutOfRangeException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewIterationMismatchedIdsException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (ReviewerCannotBeAssociatedWithUnpublishedIterationException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (ReviewerCannotVoteWithoutPublishedIterationException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewPropertiesPatchFailedException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewContinuationTokenInvalidException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewMaxAttachmentCountException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewMaxCommentCountException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewCommentThreadCreateIdException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewContentHashNotAssociatedWithReviewException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewExceededMaxAllowedZipPackageSizeException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewExceededMaxAllowedFileSizeException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewFileWithSameContentHashInPackageException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewNoFilesFoundToUploadInPackageException), HttpStatusCode.BadRequest);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (UnauthorizedAccessException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewActionRejectedByPolicyException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewActionRejectedByPullRequestException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewNotActiveException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewIterationMustBeOfUnpublishedTypeException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewIterationCannotBeUnpublishedException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewIterationCannotBePublishedException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewUnpublishedIterationAlreadyExistsException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CommentCannotBeUpdatedException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewContentUploadNotAllowedException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (ChangeEntriesWithBothSetAndUnSetChangeTrackingIdsException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewAttachmentCannotBeCreatedBeforeFileUploadException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewInvalidStatusChangeException), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewInsufficientPermissions), HttpStatusCode.Forbidden);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewNotFoundException), HttpStatusCode.NotFound);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewIterationNotFoundException), HttpStatusCode.NotFound);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (FileNotFoundException), HttpStatusCode.NotFound);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CommentThreadNotFoundException), HttpStatusCode.NotFound);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (DiscussionThreadNotFoundException), HttpStatusCode.NotFound);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CommentNotFoundException), HttpStatusCode.NotFound);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewAttachmentNotFoundException), HttpStatusCode.NotFound);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewStatusNotFoundException), HttpStatusCode.NotFound);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewIterationAlreadyExistsException), HttpStatusCode.Conflict);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewOperationFailedException), HttpStatusCode.InternalServerError);
      CodeReviewApiControllerBase.s_httpExceptions.Add(typeof (CodeReviewReviewerSaveFailedUponNullException), HttpStatusCode.InternalServerError);
    }

    public override IDictionary<Type, HttpStatusCode> HttpExceptions => (IDictionary<Type, HttpStatusCode>) CodeReviewApiControllerBase.s_httpExceptions;

    public override string TraceArea => "CodeReviewService";

    public override string ActivityLogArea => "CodeReviewService";

    protected Guid GetProjectId() => this.GetProjectId(false).Value;

    protected Guid? GetProjectId(bool allowNull)
    {
      if (!(this.ProjectId == Guid.Empty))
        return new Guid?(this.ProjectId);
      if (allowNull)
        return new Guid?();
      throw new ArgumentException(CodeReviewResources.MissingProjectId());
    }

    protected DateTime? GetModifiedSince() => !this.ControllerContext.Request.Headers.IfModifiedSince.HasValue ? new DateTime?() : new DateTime?(this.ControllerContext.Request.Headers.IfModifiedSince.Value.UtcDateTime);

    protected HttpResponseMessage CreateDownloadResponse(ChangeEntryStream entryStream)
    {
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(entryStream.FileStream);
      if (entryStream.CompressionType == CompressionType.GZip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
      string fileName = string.IsNullOrEmpty(entryStream.FileName) ? "ReviewFile" : FileChangeUtilities.GetDownloadableFileName(entryStream.FileName);
      CodeReviewApiControllerBase.AddContentDisposition(response, fileName);
      return response;
    }

    protected static ChangeEntryFileType ParseChangeEntryFileType(string fileTarget)
    {
      ChangeEntryFileType result;
      if (!System.Enum.TryParse<ChangeEntryFileType>(fileTarget, true, out result))
        throw new ArgumentOutOfRangeException(nameof (fileTarget), CodeReviewResources.InvalidFileTargetType((object) fileTarget, (object) string.Join(", ", System.Enum.GetNames(typeof (ChangeEntryFileType)))));
      return result;
    }

    protected static ReviewFileType ParseReviewFileType(string fileTarget)
    {
      ReviewFileType result;
      if (!System.Enum.TryParse<ReviewFileType>(fileTarget, true, out result))
        throw new ArgumentOutOfRangeException(nameof (fileTarget), CodeReviewResources.InvalidFileTargetType((object) fileTarget, (object) string.Join(", ", System.Enum.GetNames(typeof (ReviewFileType)))));
      return result;
    }

    protected static string TrimAndValidateFilename(string filename)
    {
      if (filename != null)
      {
        ArgumentUtility.CheckStringForNullOrEmpty(filename, nameof (filename));
        filename = filename.Trim();
        ArgumentUtility.CheckForOutOfRange(filename.Length, nameof (filename), 1, 260);
      }
      return filename;
    }

    protected static void AddContentDisposition(HttpResponseMessage response, string fileName) => response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(fileName);

    protected bool IsModified(DateTime? lastUpdatedTime, DateTime? ifModifiedSince)
    {
      if (!ifModifiedSince.HasValue || !lastUpdatedTime.HasValue)
        return true;
      long ticks = lastUpdatedTime.Value.Ticks;
      long num = ticks % 10000000L;
      return new DateTime(ticks - num, lastUpdatedTime.Value.Kind) > ifModifiedSince.Value;
    }

    protected HttpResponseMessage AddNextPageHeaders(
      HttpResponseMessage responseMessage,
      int? top,
      int? skip,
      int? totalAvailableRows = null,
      int? maxTop = null)
    {
      int? nextTop;
      int? nextSkip;
      if (maxTop.HasValue)
        ValidationHelper.EvaluateNextTopSkip(top, skip, totalAvailableRows, out nextTop, out nextSkip, maxTop.Value);
      else
        ValidationHelper.EvaluateNextTopSkip(top, skip, totalAvailableRows, out nextTop, out nextSkip);
      if (nextSkip.HasValue)
      {
        responseMessage.Headers.Add("x-CodeReview-NextTop", nextTop.Value.ToString());
        responseMessage.Headers.Add("x-CodeReview-NextSkip", nextSkip.Value.ToString());
      }
      return responseMessage;
    }

    protected void SetContinuationToken(HttpResponseMessage responseMessage, Review nextReview)
    {
      if (nextReview == null)
        return;
      string str = new ReviewsContinuationToken(nextReview).ToString();
      if (string.IsNullOrEmpty(str))
        return;
      responseMessage.Headers.Add("x-CodeReview-ContinuationToken", str);
    }
  }
}
