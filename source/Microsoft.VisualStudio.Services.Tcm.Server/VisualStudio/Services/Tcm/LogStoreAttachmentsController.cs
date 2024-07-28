// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LogStoreAttachmentsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(7.1)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "testattachments", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  public class LogStoreAttachmentsController : TcmControllerBase
  {
    private AttachmentsHelper m_attachmentsHelper;

    [HttpPost]
    [ClientLocationId("1026d5de-4b0b-46ae-a31f-7c59b6af51ef")]
    public TestLogStoreAttachmentReference CreateTestRunLogStoreAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId)
    {
      return this.AttachmentsHelper.CreateTestAttachmentInLogStore(attachmentRequestModel, this.ProjectId.ToString(), runId);
    }

    [HttpPost]
    [ClientLocationId("6f747e16-18c2-435a-b4fb-fa05d6845fee")]
    public void CreateBuildAttachmentInLogStore(
      TestAttachmentRequestModel attachmentRequestModel,
      int buildId)
    {
      this.AttachmentsHelper.CreateBuildAttachmentInLogStore(attachmentRequestModel, this.ProjectId.ToString(), buildId);
    }

    [HttpGet]
    [ClientLocationId("1026d5de-4b0b-46ae-a31f-7c59b6af51ef")]
    public List<TestLogStoreAttachment> GetTestRunLogStoreAttachments(int runId) => this.AttachmentsHelper.GetTestAttachmentsFromLogStore(this.ProjectId.ToString(), runId, 0);

    [HttpGet]
    [ClientLocationId("1026d5de-4b0b-46ae-a31f-7c59b6af51ef")]
    [ClientResponseType(typeof (Stream), "GetTestRunLogStoreAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestRunLogStoreAttachmentContent", "application/octet-stream")]
    public HttpResponseMessage GetTestRunLogStoreAttachment(int runId, string filename) => this.GetTestAttachment(runId, 0, filename);

    [HttpDelete]
    [ClientLocationId("1026d5de-4b0b-46ae-a31f-7c59b6af51ef")]
    public void DeleteTestRunLogStoreAttachment(int runId, string filename) => this.AttachmentsHelper.DeleteTestAttachmentFromLogStore(this.ProjectId.ToString(), runId, 0, filename);

    internal AttachmentsHelper AttachmentsHelper
    {
      get
      {
        if (this.m_attachmentsHelper == null)
          this.m_attachmentsHelper = new AttachmentsHelper(this.TestManagementRequestContext);
        return this.m_attachmentsHelper;
      }
      set => this.m_attachmentsHelper = value;
    }

    private HttpResponseMessage GetTestAttachment(
      int runId,
      int resultId,
      string filename,
      int subResultId = 0)
    {
      CompressionType compressionType = CompressionType.None;
      this.TestManagementRequestContext.TraceInfo("RestLayer", "LogStoreAttachmentsController.GetTestAttachment projectName = {0}, runId = {1}, resultId = {2}, filename = {3}, subResultId = {4}", (object) this.ProjectId, (object) runId, (object) resultId, (object) filename, (object) subResultId);
      try
      {
        VssServerStreamContent attachmentFromLogStore = this.AttachmentsHelper.GetTestAttachmentFromLogStore(this.ProjectId.ToString(), runId, resultId, filename, out compressionType, subResultId);
        if (attachmentFromLogStore == null)
          return this.Request.CreateResponse(HttpStatusCode.NotFound);
        HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
        response.Content = (HttpContent) attachmentFromLogStore;
        response.Content.Headers.ContentType = new MediaTypeHeaderValue(AttachmentDownloadHelper.GetContentType(filename));
        response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(filename);
        if (compressionType == CompressionType.GZip)
          response.Content.Headers.ContentEncoding.Add("gzip");
        return response;
      }
      catch (Exception ex)
      {
        this.TestManagementRequestContext.TraceError("RestLayer", "LogStoreAttachmentsController.GetTestAttachment failed to get the attachment from logstore for: projectName = {0}, runId = {1}, resultId = {2}, filename = {3}, subResultId = {4}. Exception message: {5}", (object) this.ProjectId, (object) runId, (object) resultId, (object) filename, (object) subResultId, (object) ex.Message);
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      }
    }
  }
}
