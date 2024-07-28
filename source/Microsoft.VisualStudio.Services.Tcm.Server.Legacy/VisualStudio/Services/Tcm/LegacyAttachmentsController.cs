// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyAttachmentsController
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace Microsoft.VisualStudio.Services.Tcm
{
  [ControllerApiVersion(4.1)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "attachments", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  public class LegacyAttachmentsController : TcmControllerBase
  {
    private AttachmentsHelper m_attachmentsHelper;

    [HttpPost]
    [ClientLocationId("E78951D9-D80E-483D-BA86-9FE5E53A750E")]
    public TestAttachmentReference CreateTestRunAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId);
    }

    [HttpPost]
    [ClientLocationId("99A64084-7EF3-4CD6-8BA1-1D71891FCC50")]
    public TestAttachmentReference CreateTestResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId);
    }

    [HttpPost]
    [ClientLocationId("99A64084-7EF3-4CD6-8BA1-1D71891FCC50")]
    [FeatureEnabled("TestManagement.Server.EnableHierarchicalResultAttachment")]
    public TestAttachmentReference CreateTestSubResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId,
      int testSubResultId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId, subResultId: testSubResultId);
    }

    [HttpGet]
    [ClientLocationId("E78951D9-D80E-483D-BA86-9FE5E53A750E")]
    [PublicProjectRequestRestrictions]
    public List<TestAttachment> GetTestRunAttachments(int runId) => this.AttachmentsHelper.GetTestAttachments(this.ProjectId.ToString(), runId, 0);

    [HttpGet]
    [ClientLocationId("99A64084-7EF3-4CD6-8BA1-1D71891FCC50")]
    [PublicProjectRequestRestrictions]
    public List<TestAttachment> GetTestResultAttachments(int runId, int testCaseResultId) => this.AttachmentsHelper.GetTestAttachments(this.ProjectId.ToString(), runId, testCaseResultId);

    [HttpGet]
    [ClientLocationId("99A64084-7EF3-4CD6-8BA1-1D71891FCC50")]
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
    [ClientLocationId("E78951D9-D80E-483D-BA86-9FE5E53A750E")]
    [PublicProjectRequestRestrictions]
    public HttpResponseMessage GetTestRunAttachment(int runId, int attachmentId) => this.GetTestAttachment(runId, 0, attachmentId);

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestResultAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestResultAttachmentContent", "application/octet-stream")]
    [ClientLocationId("99A64084-7EF3-4CD6-8BA1-1D71891FCC50")]
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
    [ClientLocationId("99A64084-7EF3-4CD6-8BA1-1D71891FCC50")]
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
    }
  }
}
