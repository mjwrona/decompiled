// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AttachmentsController
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(2.0)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Attachments", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true, SafeCrossOriginUserAgents = new string[] {".*TestRunner\\(AzureTestPlans\\).*"})]
  public class AttachmentsController : TestResultsControllerBase
  {
    private AttachmentsHelper m_attachmentsHelper;

    [HttpPost]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("4F004AF4-A507-489C-9B13-CB62060BEB11")]
    [ClientExample("POST__test_runs__newRunId__attachments.json", null, null, null)]
    public TestAttachmentReference CreateTestRunAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId);
    }

    [HttpPost]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
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
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
    public TestAttachmentReference CreateTestIterationResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId,
      int iterationId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId, iterationId);
    }

    [HttpPost]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
    public TestAttachmentReference CreateTestIterationResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId,
      int iterationId,
      string actionPath)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId, iterationId, actionPath);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestRunAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestRunAttachmentContent", "application/octet-stream")]
    [ClientLocationId("4F004AF4-A507-489C-9B13-CB62060BEB11")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetTestRunAttachment(int runId, int attachmentId) => this.GetTestAttachment(runId, 0, attachmentId);

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestResultAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestResultAttachmentContent", "application/octet-stream")]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
    [DemandFeature("2BAEE8C9-BB36-4DE1-B991-3C1B6B5CB2B5", true)]
    public HttpResponseMessage GetTestResultAttachment(
      int runId,
      int testCaseResultId,
      int attachmentId)
    {
      return this.GetTestAttachment(runId, testCaseResultId, attachmentId);
    }

    internal HttpResponseMessage GetTestAttachment(int runId, int resultId, int attachmentId)
    {
      string fileName = string.Empty;
      CompressionType compressionType = CompressionType.None;
      Stream testAttachment = this.AttachmentsHelper.GetTestAttachment(this.ProjectId.ToString(), runId, resultId, attachmentId, out fileName, out compressionType);
      if (testAttachment == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      response.Content = (HttpContent) new StreamContent(testAttachment);
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
    }
  }
}
