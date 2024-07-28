// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.TcmAttachmentsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: CB103307-BD4A-424F-95AE-F5C3B057AC26
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
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
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "testresults", ResourceName = "attachments", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  public class TcmAttachmentsController : TcmControllerBase
  {
    private AttachmentsHelper m_attachmentsHelper;

    [HttpPost]
    [ClientLocationId("B5731898-8206-477A-A51D-3FDF116FC6BF")]
    public TestAttachmentReference CreateTestRunAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId);
    }

    [HttpPost]
    [ClientLocationId("2A632E97-E014-4275-978F-8E5C4906D4B3")]
    public TestAttachmentReference CreateTestResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId);
    }

    [HttpPost]
    [ClientLocationId("2A632E97-E014-4275-978F-8E5C4906D4B3")]
    [FeatureEnabled("TestManagement.Server.EnableHierarchicalResultAttachment")]
    public TestAttachmentReference CreateTestSubResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId,
      int testSubResultId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId, subResultId: testSubResultId);
    }

    [HttpPost]
    [ClientLocationId("2A632E97-E014-4275-978F-8E5C4906D4B3")]
    public TestAttachmentReference CreateTestIterationResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId,
      int iterationId,
      string actionPath = null)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId, iterationId, actionPath);
    }

    [HttpGet]
    [ClientLocationId("B5731898-8206-477A-A51D-3FDF116FC6BF")]
    [PublicProjectRequestRestrictions]
    public List<TestAttachment> GetTestRunAttachments(int runId) => this.AttachmentsHelper.GetTestAttachments(this.ProjectId.ToString(), runId, 0);

    [HttpGet]
    [ClientLocationId("2A632E97-E014-4275-978F-8E5C4906D4B3")]
    [PublicProjectRequestRestrictions]
    public List<TestAttachment> GetTestResultAttachments(int runId, int testCaseResultId) => this.AttachmentsHelper.GetTestAttachments(this.ProjectId.ToString(), runId, testCaseResultId);

    [HttpGet]
    [ClientLocationId("2A632E97-E014-4275-978F-8E5C4906D4B3")]
    [PublicProjectRequestRestrictions]
    [FeatureEnabled("TestManagement.Server.EnableHierarchicalResultAttachment")]
    public List<TestAttachment> GetTestSubResultAttachments(
      int runId,
      int testCaseResultId,
      int testSubResultId)
    {
      return this.AttachmentsHelper.GetTestAttachments(this.ProjectId.ToString(), runId, testCaseResultId, testSubResultId);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestRunAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestRunAttachmentContent", "application/octet-stream")]
    [ClientLocationId("B5731898-8206-477A-A51D-3FDF116FC6BF")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetTestRunAttachment(int runId, int attachmentId) => this.GetTestAttachment(runId, 0, attachmentId);

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestResultAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestResultAttachmentContent", "application/octet-stream")]
    [ClientLocationId("2A632E97-E014-4275-978F-8E5C4906D4B3")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetTestResultAttachment(
      int runId,
      int testCaseResultId,
      int attachmentId)
    {
      return this.GetTestAttachment(runId, testCaseResultId, attachmentId);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestSubResultAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestSubResultAttachmentContent", "application/octet-stream")]
    [ClientLocationId("2A632E97-E014-4275-978F-8E5C4906D4B3")]
    [PublicProjectRequestRestrictions]
    [FeatureEnabled("TestManagement.Server.EnableHierarchicalResultAttachment")]
    public HttpResponseMessage GetTestSubResultAttachment(
      int runId,
      int testCaseResultId,
      int testSubResultId,
      int attachmentId)
    {
      return this.GetTestAttachment(runId, testCaseResultId, attachmentId, testSubResultId);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestIterationAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestIterationAttachmentContent", "application/octet-stream")]
    [ClientLocationId("2A632E97-E014-4275-978F-8E5C4906D4B3")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetTestIterationAttachment(
      int runId,
      int testCaseResultId,
      int attachmentId,
      int iterationId)
    {
      return this.GetTestAttachment(runId, testCaseResultId, attachmentId, iterationId: iterationId);
    }

    [HttpDelete]
    [ClientLocationId("B5731898-8206-477A-A51D-3FDF116FC6BF")]
    public void DeleteTestRunAttachment(int runId, int attachmentId) => this.AttachmentsHelper.DeleteTestAttachment(this.ProjectId.ToString(), runId, 0, attachmentId);

    [HttpDelete]
    [ClientLocationId("2A632E97-E014-4275-978F-8E5C4906D4B3")]
    public void DeleteTestResultAttachment(int runId, int testCaseResultId, int attachmentId) => this.AttachmentsHelper.DeleteTestAttachment(this.ProjectId.ToString(), runId, testCaseResultId, attachmentId);

    internal HttpResponseMessage GetTestAttachment(
      int runId,
      int resultId,
      int attachmentId,
      int subResultId = 0,
      int iterationId = 0)
    {
      string empty = string.Empty;
      CompressionType compressionType = CompressionType.None;
      Guid projectId1;
      Stream content;
      if (iterationId == 0)
      {
        AttachmentsHelper attachmentsHelper = this.AttachmentsHelper;
        projectId1 = this.ProjectId;
        string projectId2 = projectId1.ToString();
        int runId1 = runId;
        int resultId1 = resultId;
        int attachmentId1 = attachmentId;
        ref string local1 = ref empty;
        ref CompressionType local2 = ref compressionType;
        int subResultId1 = subResultId;
        content = attachmentsHelper.GetTestAttachment(projectId2, runId1, resultId1, attachmentId1, out local1, out local2, subResultId1);
      }
      else
      {
        AttachmentsHelper attachmentsHelper = this.AttachmentsHelper;
        projectId1 = this.ProjectId;
        string projectId3 = projectId1.ToString();
        int runId2 = runId;
        int resultId2 = resultId;
        int attachmentId2 = attachmentId;
        ref string local3 = ref empty;
        ref CompressionType local4 = ref compressionType;
        int iterationId1 = iterationId;
        content = attachmentsHelper.GetTestIterationAttachment(projectId3, runId2, resultId2, attachmentId2, out local3, out local4, iterationId1);
      }
      if (content == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
      ShallowReference shallowReference = new ShallowReference();
      projectId1 = this.ProjectId;
      shallowReference.Id = projectId1.ToString();
      testCaseResult.Project = shallowReference;
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult securedObject = testCaseResult;
      response.Content = (HttpContent) new VssServerStreamContent(content, (object) securedObject);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(AttachmentDownloadHelper.GetContentType(empty));
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(empty);
      if (compressionType == CompressionType.GZip)
        response.Content.Headers.ContentEncoding.Add("gzip");
      return response;
    }

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
  }
}
