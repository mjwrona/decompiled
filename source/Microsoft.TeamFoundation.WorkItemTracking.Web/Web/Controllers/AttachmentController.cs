// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers.AttachmentController
// Assembly: Microsoft.TeamFoundation.WorkItemTracking.Web, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CCDEBCB9-DB6B-41A2-8797-78C2455AE321
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.TeamFoundation.WorkItemTracking.Web.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.Web.Models.Factories;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.WebApi;
using Microsoft.VisualStudio.Services.WebApi.Internal;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.WorkItemTracking.Web.Controllers
{
  [VersionedApiControllerCustomName(Area = "wit", ResourceName = "attachments", ResourceVersion = 2)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  [ControllerApiVersion(1.0)]
  [ClientIgnoreRouteScopes(ClientRouteScopes.Project)]
  public class AttachmentController : WorkItemTrackingApiController
  {
    private const int WitRestControllerStart = 5900000;
    private const int TraceRange = 5900000;

    public override string TraceArea => "attachments";

    [TraceFilter(5900000, 5900010)]
    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetAttachmentContent", "application/octet-stream")]
    [ClientExample("GET__wit_attachments_get_attachment_content.json", "Get Attachment Content", null, null)]
    [ClientExample("GET__wit_attachments_get_project_attachment_content.json", "Get Project Attachment Content", null, null)]
    [ClientExample("GET__wit_attachments_download_attachment.json", "Download Attachment", null, null)]
    [PublicProjectRequestRestrictions(false, false, "5.0")]
    [ClientTSProjectParameterPosition(2)]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage GetAttachment(Guid id, string fileName = null, bool download = false)
    {
      if (Guid.Empty.Equals(id))
        throw new VssPropertyValidationException(nameof (id), ResourceStrings.NullOrEmptyParameter((object) nameof (id)));
      ITeamFoundationWorkItemAttachmentService service = this.TfsRequestContext.GetService<ITeamFoundationWorkItemAttachmentService>();
      Stream file = (Stream) null;
      ISecuredObject securedObject;
      CompressionType compressionType;
      using (this.TfsRequestContext.CreateTimeToFirstPageExclusionBlock())
        file = service.RetrieveAttachment(this.TfsRequestContext, new Guid?(this.ProjectId), id, out long _, out securedObject, out compressionType);
      return this.CreateGetAttachmentResponse(file, id, fileName, compressionType, download, securedObject);
    }

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientIgnore]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage GetAttachmentBackCompat(Guid fileNameGuid, string fileName = null) => this.GetAttachment(fileNameGuid, fileName);

    [HttpGet]
    [PublicProjectRequestRestrictions]
    [ClientIgnore]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage GetAttachmentBackCompat2(int fileID, string fileName = null)
    {
      ITeamFoundationWorkItemAttachmentService service = this.TfsRequestContext.GetService<ITeamFoundationWorkItemAttachmentService>();
      Stream file = (Stream) null;
      Guid attachmentGuid = Guid.Empty;
      ISecuredObject securedObject;
      CompressionType compressionType;
      using (this.TfsRequestContext.CreateTimeToFirstPageExclusionBlock())
        file = service.RetrieveAttachment(this.TfsRequestContext, new Guid?(this.ProjectId), fileID, out long _, out securedObject, out compressionType, out attachmentGuid);
      return this.CreateGetAttachmentResponse(file, attachmentGuid, fileName, compressionType, false, securedObject);
    }

    [TraceFilter(5900010, 5900020)]
    [HttpPost]
    [ClientResponseType(typeof (AttachmentReference), null, null)]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientExample("POST__wit_attachments_fileName-_textFile_.json", "Upload a text file", null, "https://github.com/Microsoft/vsts-dotnet-samples/blob/master/ClientLibrary/Snippets/Microsoft.TeamServices.Samples.Client/WorkItemTracking/AttachmentsSample.cs#L23")]
    [ClientExample("POST__wit_attachments_fileName-_binaryFile_.json", "Upload a binary file", null, "https://github.com/Microsoft/vsts-dotnet-samples/blob/master/ClientLibrary/Snippets/Microsoft.TeamServices.Samples.Client/WorkItemTracking/AttachmentsSample.cs#L49")]
    [ClientExample("POST_wit_attachments_fileName_chunked_register.json", "Start a Chunked Upload", null, null)]
    [ClientTSProjectParameterPosition(3)]
    public HttpResponseMessage CreateAttachment(
      string fileName = null,
      string uploadType = "Simple",
      string areaPath = null)
    {
      if (string.IsNullOrWhiteSpace(uploadType))
        throw new VssPropertyValidationException(nameof (uploadType), ResourceStrings.NullOrEmptyParameter((object) nameof (uploadType)));
      ITeamFoundationWorkItemAttachmentService service = this.TfsRequestContext.GetService<ITeamFoundationWorkItemAttachmentService>();
      Guid attachmentId = Guid.NewGuid();
      bool returnProjectScopedUrl = this.ShouldReturnProjectScopedUrls(this.TfsRequestContext);
      if (uploadType.Equals("Chunked", StringComparison.OrdinalIgnoreCase))
      {
        ISecuredObject securedObject;
        service.RegisterAttachmentUpload(this.TfsRequestContext, new Guid?(this.ProjectId), attachmentId, out securedObject, areaPath);
        AttachmentReference attachmentReference = AttachmentFactory.Create(this.TfsRequestContext.WitContext(), this.ProjectId, attachmentId, securedObject, fileName, returnProjectScopedUrl);
        HttpResponseMessage response = this.Request.CreateResponse<AttachmentReference>(HttpStatusCode.Accepted, attachmentReference);
        response.Headers.Location = new Uri(attachmentReference.Url);
        return response;
      }
      if (!uploadType.Equals("Simple", StringComparison.OrdinalIgnoreCase))
        throw new VssPropertyValidationException(nameof (uploadType), ResourceStrings.UnsupportedAttachmentUploadType((object) uploadType, (object) "POST"));
      Stream inputStream = (Stream) null;
      try
      {
        if (this.Request.Content.IsMimeMultipartContent())
        {
          IEnumerable<HttpContent> parts = (IEnumerable<HttpContent>) null;
          try
          {
            using (this.TfsRequestContext.CreateTimeToFirstPageExclusionBlock())
              Task.Factory.StartNew<IEnumerable<HttpContent>>((Func<IEnumerable<HttpContent>>) (() => parts = (IEnumerable<HttpContent>) this.Request.Content.ReadAsMultipartAsync().Result.Contents), CancellationToken.None, TaskCreationOptions.LongRunning, TaskScheduler.Default).Wait();
          }
          catch (AggregateException ex)
          {
            return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ex.Message);
          }
          if (parts != null && parts.Count<HttpContent>() == 1)
          {
            HttpContent httpContent = parts.First<HttpContent>();
            if (httpContent.Headers != null && httpContent.Headers.ContentDisposition != null && string.IsNullOrEmpty(fileName) && !string.IsNullOrEmpty(httpContent.Headers.ContentDisposition.FileName))
              fileName = httpContent.Headers.ContentDisposition.FileName.Trim('"');
            inputStream = httpContent.ReadAsStreamAsync().Result;
          }
        }
        else
          inputStream = (this.Request.Content ?? throw new VssPropertyValidationException("attachment", ResourceStrings.NullOrEmptyParameter((object) "attachment"))).ReadAsStreamAsync().Result;
        if (inputStream == null)
          return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResourceStrings.AttachmentContentInvalid());
        ISecuredObject securedObject;
        using (this.TfsRequestContext.CreateTimeToFirstPageExclusionBlock())
          service.UploadAttachment(this.TfsRequestContext, new Guid?(this.ProjectId), attachmentId, inputStream, out securedObject, areaPath);
        return this.Request.CreateResponse<AttachmentReference>(HttpStatusCode.Created, AttachmentFactory.Create(this.TfsRequestContext.WitContext(), this.ProjectId, attachmentId, securedObject, fileName, returnProjectScopedUrl));
      }
      finally
      {
        inputStream?.Dispose();
      }
    }

    [TraceFilter(5900020, 5900030)]
    [HttpPut]
    [ClientResponseType(typeof (AttachmentReference), null, null)]
    [ClientHeaderParameter("Content-Range", typeof (string), "contentRangeHeader", "starting and ending byte positions for chunked file upload, format is \"Content-Range\": \"bytes 0-10000/50000\"", false, false)]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientSwaggerOperationId("Upload Chunk")]
    [ClientInclude(RestClientLanguages.TypeScript | RestClientLanguages.Swagger2 | RestClientLanguages.TypeScriptWebPlatform)]
    [ClientTSProjectParameterPosition(3)]
    [ClientExample("PUT_wit_attachments_fileName_chunked_content1.json", "Upload a Chunk", null, null)]
    public HttpResponseMessage UploadAttachment(Guid id, string fileName = null)
    {
      Stream inputStream = (Stream) null;
      try
      {
        HttpContent content = this.Request.Content;
        if (content == null || content.Headers == null)
          throw new VssPropertyValidationException("attachment", ResourceStrings.NullOrEmptyParameter((object) "attachment"));
        if (this.Request.Content.IsMimeMultipartContent() || this.Request.Content.IsFormData())
          return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResourceStrings.UnsupportedContentTypeForAttachmentUpload());
        ContentRangeHeaderValue contentRange = content.Headers.ContentRange;
        if (contentRange == null || !contentRange.Length.HasValue || !contentRange.From.HasValue || !contentRange.To.HasValue)
          return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResourceStrings.IncorrectContentRangeForAttachmentUpload());
        long? nullable = content.Headers.ContentRange.Length;
        long fileSize = nullable.Value;
        nullable = content.Headers.ContentRange.From;
        long offsetFrom = nullable.Value;
        inputStream = content.ReadAsStreamAsync().Result;
        if (inputStream == null)
          return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResourceStrings.AttachmentContentInvalid());
        long? to = contentRange.To;
        long? from = contentRange.From;
        nullable = to.HasValue & from.HasValue ? new long?(to.GetValueOrDefault() - from.GetValueOrDefault()) : new long?();
        long num = inputStream.Length - 1L;
        if (!(nullable.GetValueOrDefault() == num & nullable.HasValue))
          return this.Request.CreateErrorResponse(HttpStatusCode.BadRequest, ResourceStrings.IncorrectAttachmentContentLength());
        inputStream.Seek(0L, SeekOrigin.Begin);
        ITeamFoundationWorkItemAttachmentService service = this.TfsRequestContext.GetService<ITeamFoundationWorkItemAttachmentService>();
        bool flag = false;
        ISecuredObject securedObject;
        using (this.TfsRequestContext.CreateTimeToFirstPageExclusionBlock())
          flag = service.UploadAttachmentChunk(this.TfsRequestContext, new Guid?(this.ProjectId), id, inputStream, fileSize, offsetFrom, out securedObject);
        AttachmentReference attachmentReference = AttachmentFactory.Create(this.TfsRequestContext.WitContext(), this.ProjectId, id, securedObject, fileName, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext));
        HttpResponseMessage httpResponseMessage = !flag ? this.Request.CreateResponse<AttachmentReference>(HttpStatusCode.Accepted, attachmentReference) : this.Request.CreateResponse<AttachmentReference>(HttpStatusCode.Created, attachmentReference);
        httpResponseMessage.Headers.Location = new Uri(attachmentReference.Url);
        return httpResponseMessage;
      }
      finally
      {
        inputStream?.Dispose();
      }
    }

    private HttpResponseMessage SetResponseContentHeaders(
      Stream fileStream,
      string fileName,
      AttachmentReference secureAttachmentObject,
      CompressionType compressionType,
      bool download)
    {
      bool flag1 = false;
      bool flag2 = false;
      MediaTypeHeaderValue mediaType = new MediaTypeHeaderValue("application/octet-stream");
      if (!string.IsNullOrEmpty(fileName))
      {
        string mimeMapping = System.Web.MimeMapping.GetMimeMapping(fileName);
        flag2 = mimeMapping.StartsWith("video/", StringComparison.OrdinalIgnoreCase);
        MediaTypeHeaderValue parsedValue;
        if (((!mimeMapping.StartsWith("image/", StringComparison.OrdinalIgnoreCase) || mimeMapping.Equals("image/svg+xml", StringComparison.OrdinalIgnoreCase) ? (mimeMapping.StartsWith("application/pdf", StringComparison.OrdinalIgnoreCase) ? 1 : 0) : 1) | (flag2 ? 1 : 0)) != 0 && MediaTypeHeaderValue.TryParse(mimeMapping, out parsedValue))
        {
          mediaType = parsedValue;
          flag1 = true;
        }
      }
      HttpResponseMessage response;
      if (flag2 && this.Request.Headers.Range != null)
      {
        response = this.Request.CreateResponse(HttpStatusCode.PartialContent);
        response.Content = (HttpContent) new VssByteRangeStreamContent(fileStream, this.Request.Headers.Range, mediaType, (object) secureAttachmentObject);
      }
      else
      {
        response = this.Request.CreateResponse(HttpStatusCode.OK);
        response.Content = (HttpContent) new VssServerStreamContent(fileStream, (object) secureAttachmentObject);
        response.Content.Headers.ContentType = mediaType;
      }
      if (download || !flag1)
        response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(fileName);
      if (compressionType == CompressionType.GZip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      return response;
    }

    private HttpResponseMessage CreateGetAttachmentResponse(
      Stream file,
      Guid attachmentGuid,
      string fileName,
      CompressionType compressionType,
      bool download,
      ISecuredObject securedObject)
    {
      AttachmentReference secureAttachmentObject = AttachmentFactory.Create(this.TfsRequestContext.WitContext(), this.ProjectId, attachmentGuid, securedObject, fileName, this.ShouldReturnProjectScopedUrls(this.TfsRequestContext));
      HttpResponseMessage attachmentResponse = this.SetResponseContentHeaders(file, fileName, secureAttachmentObject, compressionType, download);
      this.TfsRequestContext.RequestTimer.SetTimeToFirstPageEnd();
      return attachmentResponse;
    }
  }
}
