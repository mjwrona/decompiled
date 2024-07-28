// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers.TaskHubTimelineRecordAttachmentController
// Assembly: Microsoft.TeamFoundation.DistributedTask.Orchestration.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 07FD5059-3D25-415E-AA3A-5372051D7E71
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.dll

using Microsoft.TeamFoundation.DistributedTask.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using System.Web.Http;

namespace Microsoft.TeamFoundation.DistributedTask.Orchestration.Server.Controllers
{
  [ControllerApiVersion(2.0)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  [ClientTemporarySwaggerExclusion]
  [VersionedApiControllerCustomName(Area = "distributedtask", ResourceName = "attachments")]
  public sealed class TaskHubTimelineRecordAttachmentController : TaskHubApiController
  {
    [HttpPut]
    [ClientRequestBodyIsStream]
    [ClientLocationId("7898F959-9CDF-4096-B29E-7F293031629E")]
    public async Task<TaskAttachment> CreateAttachment(
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      TaskHubTimelineRecordAttachmentController attachmentController = this;
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      HttpContent content1 = attachmentController.Request.Content;
      ArgumentUtility.CheckForNull<HttpContent>(content1, "content");
      if (!content1.Headers.ContentLength.HasValue)
        throw new ArgumentException("Attachment content is empty.", "content");
      Stream content2 = await content1.ReadAsStreamAsync();
      return await attachmentController.Hub.CreateAttachmentAsync(attachmentController.TfsRequestContext, attachmentController.ScopeIdentifier, planId, timelineId, recordId, type, name, content2);
    }

    [HttpPut]
    [ClientLocationId("7898F959-9CDF-4096-B29E-7F293031629E")]
    public async Task<TaskAttachment> CreateAttachmentFromArtifact(
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name,
      string artifactHash,
      long length)
    {
      TaskHubTimelineRecordAttachmentController attachmentController = this;
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      ArgumentUtility.CheckStringForNullOrEmpty(artifactHash, nameof (artifactHash));
      return await attachmentController.Hub.AssociateAttachmentAsync(attachmentController.TfsRequestContext, attachmentController.ScopeIdentifier, planId, timelineId, recordId, type, name, artifactHash, length);
    }

    [HttpGet]
    [ClientLocationId("EB55E5D6-2F30-4295-B5ED-38DA50B1FC52")]
    public Task<IList<TaskAttachment>> GetPlanAttachments(Guid planId, string type)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      return this.Hub.GetAttachmentsAsync(this.TfsRequestContext, this.ScopeIdentifier, planId, type);
    }

    [HttpGet]
    [ClientLocationId("7898F959-9CDF-4096-B29E-7F293031629E")]
    public Task<IList<TaskAttachment>> GetAttachments(
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type)
    {
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      return this.Hub.GetAttachmentsAsync(this.TfsRequestContext, this.ScopeIdentifier, planId, type, new Guid?(timelineId), new Guid?(recordId));
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), null, null, MethodName = "GetAttachmentContent", MediaType = "application/octet-stream")]
    [ClientResponseType(typeof (TaskAttachment), null, null, MethodName = "GetAttachment")]
    [ClientLocationId("7898F959-9CDF-4096-B29E-7F293031629E")]
    public async Task<HttpResponseMessage> GetAttachment(
      Guid planId,
      Guid timelineId,
      Guid recordId,
      string type,
      string name)
    {
      TaskHubTimelineRecordAttachmentController attachmentController = this;
      ArgumentUtility.CheckForEmptyGuid(planId, nameof (planId));
      ArgumentUtility.CheckForEmptyGuid(timelineId, nameof (timelineId));
      ArgumentUtility.CheckForEmptyGuid(recordId, nameof (recordId));
      ArgumentUtility.CheckStringForNullOrEmpty(type, nameof (type));
      ArgumentUtility.CheckStringForNullOrEmpty(name, nameof (name));
      if (attachmentController.OctetStreamRequested())
      {
        Stream contentStream = await attachmentController.Hub.GetAttachmentAsync(attachmentController.TfsRequestContext, attachmentController.ScopeIdentifier, planId, timelineId, recordId, type, name);
        if (contentStream == null)
          return attachmentController.Request.CreateResponse(HttpStatusCode.NotFound);
        HttpResponseMessage response = attachmentController.Request.CreateResponse(HttpStatusCode.OK);
        response.Content = (HttpContent) new PushStreamContent((Func<Stream, HttpContent, TransportContext, Task>) (async (stream, httpContent, transportContext) =>
        {
          try
          {
            this.TfsRequestContext.UpdateTimeToFirstPage();
            using (contentStream)
              await contentStream.CopyToAsync(stream);
          }
          finally
          {
            stream?.Dispose();
          }
        }), new MediaTypeHeaderValue("application/octet-stream"));
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_TotalDownloads").Increment();
        VssPerformanceCounterManager.GetPerformanceCounter("Microsoft.TeamFoundation.Framework.Server.Perf_FCS_DownloadSec").Increment();
        return response;
      }
      TaskAttachment attachmentMetadataAsync = await attachmentController.Hub.GetAttachmentMetadataAsync(attachmentController.TfsRequestContext, attachmentController.ScopeIdentifier, planId, timelineId, recordId, type, name);
      return attachmentController.Request.CreateResponse<TaskAttachment>(HttpStatusCode.OK, attachmentMetadataAsync);
    }
  }
}
