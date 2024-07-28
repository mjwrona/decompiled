// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Attachments3Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(3.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Attachments", ResourceVersion = 1)]
  [DemandFeature("402E4502-9389-420C-BA11-796CDA2E4867", true)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  public class Attachments3Controller : TestResultsControllerBase
  {
    private AttachmentsHelper m_attachmentsHelper;

    [HttpGet]
    [ClientLocationId("4F004AF4-A507-489C-9B13-CB62060BEB11")]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET__test_runs__newRunId__attachments.json", null, null, null)]
    public List<TestAttachment> GetTestRunAttachments(int runId) => this.AttachmentsHelper.GetTestAttachments(this.ProjectId.ToString(), runId, 0);

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestRunAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestRunAttachmentContent", "application/octet-stream")]
    [ClientLocationId("4F004AF4-A507-489C-9B13-CB62060BEB11")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetTestRunAttachment(int runId, int attachmentId) => this.GetTestAttachment(runId, 0, attachmentId);

    [HttpGet]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
    [PublicProjectRequestRestrictions]
    [ClientExample("GET__test_runs__newRunId__results__result1__attachments.json", null, null, null)]
    public List<TestAttachment> GetTestResultAttachments(int runId, int testCaseResultId) => this.AttachmentsHelper.GetTestAttachments(this.ProjectId.ToString(), runId, testCaseResultId);

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestResultAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestResultAttachmentContent", "application/octet-stream")]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetTestResultAttachment(
      int runId,
      int testCaseResultId,
      int attachmentId)
    {
      return this.GetTestAttachment(runId, testCaseResultId, attachmentId);
    }

    [HttpPost]
    [ClientLocationId("4F004AF4-A507-489C-9B13-CB62060BEB11")]
    [ClientExample("POST__test_runs__newRunId__attachments.json", null, null, null)]
    public TestAttachmentReference CreateTestRunAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId);
    }

    [HttpPost]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
    [ClientExample("POST__test_runs__newRunId__results__result1__attachments.json", null, null, null)]
    public TestAttachmentReference CreateTestResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId);
    }

    [HttpPost]
    [ClientInternalUseOnly(false)]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
    public TestAttachmentReference CreateTestIterationResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId,
      int iterationId,
      string actionPath = null)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId, iterationId, actionPath);
    }

    internal HttpResponseMessage GetTestAttachment(
      int runId,
      int resultId,
      int attachmentId,
      int subResultId = 0)
    {
      string fileName = string.Empty;
      CompressionType compressionType = CompressionType.None;
      Stream testAttachment = this.AttachmentsHelper.GetTestAttachment(this.ProjectId.ToString(), runId, resultId, attachmentId, out fileName, out compressionType, subResultId);
      if (testAttachment == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult securedObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectId.ToString()
        }
      };
      response.Content = (HttpContent) new VssServerStreamContent(testAttachment, (object) securedObject);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(AttachmentDownloadHelper.GetContentType(fileName));
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(fileName);
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
