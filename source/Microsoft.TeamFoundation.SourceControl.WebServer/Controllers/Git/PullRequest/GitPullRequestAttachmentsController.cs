// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.PullRequest.GitPullRequestAttachmentsController
// Assembly: Microsoft.TeamFoundation.SourceControl.WebServer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 47C05AF5-4412-4EF0-BF63-D475D8EECD03
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.SourceControl.WebServer.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Git.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.SourceControl.WebApi;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.SourceControl.WebServer.Controllers.Git.PullRequest
{
  [RequestContentTypeRestriction(AllowStream = true)]
  public class GitPullRequestAttachmentsController : GitApiController
  {
    [HttpGet]
    [ClientLocationId("965D9361-878B-413B-A494-45D5B5FD8AB7")]
    [ClientResponseType(typeof (IList<Attachment>), null, null)]
    [PublicProjectRequestRestrictions]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage GetAttachments([ClientParameterType(typeof (Guid), true)] string repositoryId, int pullRequestId)
    {
      TfsGitPullRequest pullRequest;
      RepoKey repoScope;
      this.GetPullRequest(repositoryId, pullRequestId, out pullRequest, out repoScope);
      return this.Request.CreateResponse<IEnumerable<Attachment>>(HttpStatusCode.OK, this.TfsRequestContext.GetService<ITeamFoundationGitAttachmentService>().GetAttachments(this.TfsRequestContext, repoScope, pullRequest));
    }

    [HttpGet]
    [ClientLocationId("965D9361-878B-413B-A494-45D5B5FD8AB7")]
    [ClientResponseType(typeof (Stream), "GetAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetAttachmentContent", "application/octet-stream")]
    [PublicProjectRequestRestrictions]
    [MethodInformation(IsLongRunning = true)]
    public HttpResponseMessage GetAttachment(
      string fileName,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId)
    {
      TfsGitPullRequest pullRequest;
      RepoKey repoScope;
      this.GetPullRequest(repositoryId, pullRequestId, out pullRequest, out repoScope);
      Attachment pullRequestAttachment;
      CompressionType compressionType;
      Stream attachmentContent = this.TfsRequestContext.GetService<ITeamFoundationGitAttachmentService>().GetAttachmentContent(this.TfsRequestContext, repoScope, pullRequest, fileName, out pullRequestAttachment, out compressionType);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new VssServerStreamContent(attachmentContent, (object) pullRequestAttachment);
      string mimeMapping = System.Web.MimeMapping.GetMimeMapping(fileName);
      if (mimeMapping.StartsWith("image/", StringComparison.OrdinalIgnoreCase) && !mimeMapping.StartsWith("image/svg", StringComparison.OrdinalIgnoreCase) || mimeMapping.StartsWith("audio/", StringComparison.OrdinalIgnoreCase) || mimeMapping.StartsWith("video/", StringComparison.OrdinalIgnoreCase) || string.Equals(mimeMapping, "text/plain", StringComparison.OrdinalIgnoreCase))
      {
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(mimeMapping);
      }
      else
      {
        response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/octet-stream");
        response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(pullRequestAttachment.DisplayName);
      }
      if (compressionType == CompressionType.GZip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      return response;
    }

    [HttpPost]
    [ClientResponseType(typeof (Attachment), null, null)]
    [ClientLocationId("965D9361-878B-413B-A494-45D5B5FD8AB7")]
    [ClientRequestContentMediaType("application/octet-stream")]
    [ClientRequestContentIsRawData]
    [ClientRequestBodyIsStream]
    [ClientResponseCode(HttpStatusCode.Created, null, false)]
    [ClientResponseCode(HttpStatusCode.Accepted, null, false)]
    public HttpResponseMessage CreateAttachment(
      [FromUri(Name = "fileName")] string fileName,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId)
    {
      TfsGitPullRequest pullRequest;
      RepoKey repoScope;
      this.GetPullRequest(repositoryId, pullRequestId, out pullRequest, out repoScope);
      long offsetFrom;
      CompressionType compressionType;
      long calculatedLength;
      this.DetermineUploadParameters(this.Request.Content, out offsetFrom, out compressionType, out calculatedLength);
      HttpContent content1 = this.Request.Content;
      ArgumentUtility.CheckForNull<HttpContent>(content1, "request.Content");
      Stream content2 = (Stream) null;
      Attachment attachment = (Attachment) null;
      bool completedUpload;
      try
      {
        content2 = content1.ReadAsStreamAsync().Result;
        attachment = this.TfsRequestContext.GetService<ITeamFoundationGitAttachmentService>().CreateAttachment(content2, offsetFrom, compressionType, calculatedLength, this.TfsRequestContext, repoScope, pullRequest, fileName, out completedUpload);
      }
      finally
      {
        content2?.Dispose();
      }
      return completedUpload ? this.Request.CreateResponse<Attachment>(HttpStatusCode.Created, attachment) : this.Request.CreateResponse<Attachment>(HttpStatusCode.Accepted, attachment);
    }

    [HttpDelete]
    [ClientResponseType(typeof (void), null, null)]
    public HttpResponseMessage DeleteAttachment(
      [FromUri(Name = "fileName")] string fileName,
      [ClientParameterType(typeof (Guid), true)] string repositoryId,
      int pullRequestId)
    {
      TfsGitPullRequest pullRequest;
      RepoKey repoScope;
      this.GetPullRequest(repositoryId, pullRequestId, out pullRequest, out repoScope);
      this.TfsRequestContext.GetService<ITeamFoundationGitAttachmentService>().DeleteAttachment(this.TfsRequestContext, repoScope, pullRequest, fileName);
      return this.Request.CreateResponse(HttpStatusCode.OK);
    }

    private void GetPullRequest(
      string repositoryId,
      int pullRequestId,
      out TfsGitPullRequest pullRequest,
      out RepoKey repoScope)
    {
      ITeamFoundationGitPullRequestService service = this.TfsRequestContext.GetService<ITeamFoundationGitPullRequestService>();
      using (ITfsGitRepository tfsGitRepository = this.GetTfsGitRepository(repositoryId))
      {
        repoScope = tfsGitRepository.Key;
        pullRequest = service.GetPullRequestDetails(this.TfsRequestContext, tfsGitRepository, pullRequestId);
        if (pullRequest == null)
          throw new GitPullRequestNotFoundException();
      }
    }

    private void DetermineUploadParameters(
      HttpContent content,
      out long offsetFrom,
      out CompressionType compressionType,
      out long calculatedLength)
    {
      offsetFrom = 0L;
      offsetFrom = 0L;
      long num = content.Headers.ContentLength.Value;
      compressionType = CompressionType.None;
      calculatedLength = num;
    }
  }
}
