// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.CodeReview.Server.ChangesContentController
// Assembly: Microsoft.VisualStudio.Services.CodeReview.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2BCB2866-BDCB-4FDE-9EA3-48DFA660C131
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.CodeReview.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.CodeReview.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.CodeReview.Server
{
  [VersionedApiControllerCustomName(Area = "CodeReview", ResourceName = "contents")]
  [RequestContentTypeRestriction(AllowStream = true)]
  public class ChangesContentController : CodeReviewApiControllerBase
  {
    private const string c_tfsFileLengthHeader = "x-tfs-filelength";

    [HttpGet]
    [ClientResponseType(typeof (Stream), "DownloadContent", "application/octet-stream")]
    public HttpResponseMessage DownloadContent(
      int reviewId,
      [FromUri(Name = "contentHash")] string contentHash,
      [FromUri(Name = "downloadFileName")] string downloadFileName = null,
      [FromUri(Name = "fileType")] ReviewFileType fileType = ReviewFileType.ChangeEntry)
    {
      ArgumentUtility.CheckForNull<string>(contentHash, nameof (contentHash));
      downloadFileName = CodeReviewApiControllerBase.TrimAndValidateFilename(downloadFileName);
      byte[] sha1HashBytes = ReviewFileContentExtensions.ToSha1HashBytes(contentHash);
      ChangeEntryStream entryStream = this.TfsRequestContext.GetService<ICodeReviewService>().DownloadFile(this.TfsRequestContext, this.GetProjectId(), reviewId, sha1HashBytes, fileType);
      entryStream.FileName = downloadFileName ?? entryStream.FileName;
      return this.CreateDownloadResponse(entryStream);
    }

    [HttpGet]
    [ClientResponseType(typeof (ReviewFilesZipContent), "DownloadContentZip", "application/zip")]
    public HttpResponseMessage DownloadContents(
      int reviewId,
      [FromUri(Name = "iterationId")] int iterationId,
      [FromUri(Name = "filterBy")] string filterBy = null,
      [FromUri(Name = "top")] int? top = null,
      [FromUri(Name = "skip")] int? skip = null,
      [FromUri(Name = "downloadFileName")] string downloadFileName = null)
    {
      if (MediaTypeFormatUtility.GetPrioritizedAcceptHeaders(this.Request, new List<RequestMediaType>()
      {
        RequestMediaType.Zip,
        RequestMediaType.None
      }).FirstOrDefault<RequestMediaType>() != RequestMediaType.Zip)
        return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, CodeReviewResources.MustAcceptZip());
      ChangeEntryFileType? nullable = new ChangeEntryFileType?();
      if (!string.IsNullOrEmpty(filterBy))
        nullable = new ChangeEntryFileType?(CodeReviewApiControllerBase.ParseChangeEntryFileType(filterBy));
      downloadFileName = CodeReviewApiControllerBase.TrimAndValidateFilename(downloadFileName);
      this.TfsRequestContext.RequestTimeout = TimeSpan.FromMinutes(30.0);
      HttpResponseMessage httpResponseMessage1 = this.Request.CreateResponse(HttpStatusCode.OK);
      ICodeReviewIterationService service = this.TfsRequestContext.GetService<ICodeReviewIterationService>();
      HttpResponseMessage httpResponseMessage2 = httpResponseMessage1;
      ICodeReviewIterationService iterationService = service;
      IVssRequestContext tfsRequestContext = this.TfsRequestContext;
      Guid projectId = this.GetProjectId();
      int reviewId1 = reviewId;
      List<int> iterationIds = new List<int>();
      iterationIds.Add(iterationId);
      ChangeEntryFileType? fileType = nullable;
      bool flag;
      ref bool local = ref flag;
      int? top1 = top;
      int? skip1 = skip;
      PushStreamContent iterationFilesContent = iterationService.GetIterationFilesContent(tfsRequestContext, projectId, reviewId1, iterationIds, fileType, out local, top1, skip1);
      httpResponseMessage2.Content = (HttpContent) iterationFilesContent;
      if (flag)
        httpResponseMessage1 = this.AddNextPageHeaders(httpResponseMessage1, top, skip);
      downloadFileName = downloadFileName ?? string.Format("logs_{0}.{1}", (object) reviewId, (object) ".zip");
      CodeReviewApiControllerBase.AddContentDisposition(httpResponseMessage1, downloadFileName);
      return httpResponseMessage1;
    }

    [HttpPost]
    [ClientRequestBodyIsStream]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage UploadContent(int reviewId, [FromUri(Name = "contentHash")] string contentHash, [FromUri(Name = "fileType")] string fileType = null)
    {
      ArgumentUtility.CheckForNull<string>(contentHash, nameof (contentHash));
      byte[] sha1HashBytes = ReviewFileContentExtensions.ToSha1HashBytes(contentHash);
      HttpContent content = this.Request.Content;
      ArgumentUtility.CheckForNull<HttpContent>(content, "request.Content");
      long offsetFrom;
      CompressionType compressionType;
      long calculatedLength;
      Stream contentStream;
      this.DetermineUploadParameters(content, out offsetFrom, out compressionType, out calculatedLength, out contentStream);
      ReviewFileType fileType1 = ReviewFileType.ChangeEntry;
      if (!string.IsNullOrEmpty(fileType))
        fileType1 = CodeReviewApiControllerBase.ParseReviewFileType(fileType);
      return this.TfsRequestContext.GetService<ICodeReviewService>().UploadFile(this.TfsRequestContext, this.GetProjectId(), reviewId, sha1HashBytes, contentStream, calculatedLength, offsetFrom, compressionType, fileType1).FileServiceFileId < 0 ? this.Request.CreateResponse(HttpStatusCode.Created) : this.Request.CreateResponse(HttpStatusCode.Accepted);
    }

    [HttpPost]
    [ClientResponseType(typeof (IEnumerable<ReviewFileContentInfo>), null, null)]
    [ClientResponseCode(HttpStatusCode.Created, "All file(s) in package got uploaded.", false)]
    [ClientResponseCode(HttpStatusCode.Accepted, null, false)]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientRequestContentMediaType("application/octet-stream")]
    public HttpResponseMessage UploadContentsBatch(int reviewId)
    {
      HttpContent content = this.Request.Content;
      ArgumentUtility.CheckForNull<HttpContent>(content, "request.Content");
      IEnumerable<ReviewFileContentInfo> source = (IEnumerable<ReviewFileContentInfo>) new List<ReviewFileContentInfo>();
      using (Stream result = content.ReadAsStreamAsync().Result)
      {
        if (result == null)
          return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, CodeReviewResources.BatchContentInvalid());
        source = (IEnumerable<ReviewFileContentInfo>) this.TfsRequestContext.GetService<ICodeReviewContentService>().UploadFiles(this.TfsRequestContext, this.GetProjectId(), reviewId, result).ToList<ReviewFileContentInfo>();
      }
      return this.Request.CreateResponse(source.Any<ReviewFileContentInfo>() ? HttpStatusCode.Created : HttpStatusCode.Accepted);
    }

    private void DetermineUploadParameters(
      HttpContent content,
      out long offsetFrom,
      out CompressionType compressionType,
      out long calculatedLength,
      out Stream contentStream)
    {
      offsetFrom = 0L;
      long num1 = 0;
      if (content.Headers.ContentRange != null && content.Headers.ContentRange.From.HasValue && content.Headers.ContentRange.Length.HasValue)
      {
        ref long local = ref offsetFrom;
        long? nullable = content.Headers.ContentRange.From;
        long num2 = nullable.Value;
        local = num2;
        nullable = content.Headers.ContentRange.Length;
        num1 = nullable.Value;
      }
      else
      {
        long? contentLength = content.Headers.ContentLength;
        long num3 = 0;
        if (contentLength.GetValueOrDefault() > num3 & contentLength.HasValue)
          throw new ArgumentException(CodeReviewResources.InvalidContentRange());
      }
      if (content.Headers.ContentEncoding.Contains("gzip"))
      {
        IEnumerable<string> values;
        if (!this.Request.Headers.TryGetValues("x-tfs-filelength", out values) || !long.TryParse(values.FirstOrDefault<string>(), out calculatedLength) || calculatedLength <= 0L)
          throw new ArgumentException(CodeReviewResources.GzipUnsupportedWithoutLength());
        compressionType = CompressionType.GZip;
      }
      else
      {
        compressionType = CompressionType.None;
        calculatedLength = num1;
      }
      if (calculatedLength == 0L)
        contentStream = (Stream) new MemoryStream();
      else
        contentStream = content.ReadAsStreamAsync().Result;
    }
  }
}
