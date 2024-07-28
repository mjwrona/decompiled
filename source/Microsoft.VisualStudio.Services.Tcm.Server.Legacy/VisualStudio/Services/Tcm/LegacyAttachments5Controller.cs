// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Tcm.LegacyAttachments5Controller
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Legacy, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2DF29497-7FFC-4FD1-88DC-A3958AAA1A19
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Tcm.Server.Legacy.dll

using Microsoft.TeamFoundation.Framework.Server;
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
  [ControllerApiVersion(5.0)]
  [VersionedApiControllerCustomName(Area = "tcm", ResourceName = "attachments", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true)]
  public class LegacyAttachments5Controller : LegacyAttachmentsController
  {
    [HttpPost]
    [ClientLocationId("9A941C36-FA09-446C-B15C-6C1DC03EFCE9")]
    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetAttachmentsByQuery(
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      return this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().GetTestAttachmentsByQuery(this.TestManagementRequestContext, query);
    }

    [HttpPost]
    [ClientLocationId("99A64084-7EF3-4CD6-8BA1-1D71891FCC50")]
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
    [ClientLocationId("9A941C36-FA09-446C-B15C-6C1DC03EFCE9")]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetAttachments(
      int testRunId,
      int testResultId,
      int subResultId,
      int sessionId,
      int attachmentId)
    {
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().GetTestAttachments(this.TestManagementRequestContext, this.ProjectInfo.Name, testRunId, testResultId, sessionId, subResultId, attachmentId);
    }

    [HttpGet]
    [ClientLocationId("9A941C36-FA09-446C-B15C-6C1DC03EFCE9")]
    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetAttachments2(
      int attachmentId,
      bool getSiblingAttachments)
    {
      return (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().GetTestAttachments(this.TestManagementRequestContext, this.ProjectInfo.Name, attachmentId, getSiblingAttachments);
    }

    [HttpGet]
    [ClientResponseType(typeof (Stream), "GetTestResultAttachmentStream", "application/octet-stream")]
    [ClientLocationId("9A941C36-FA09-446C-B15C-6C1DC03EFCE9")]
    public HttpResponseMessage GetAttachmentStream(int attachmentId)
    {
      string attachmentName = string.Empty;
      Stream attachmentStream = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().GetAttachmentStream(this.TestManagementRequestContext, this.ProjectInfo.Name, attachmentId, out attachmentName);
      if (attachmentStream == null)
        return this.Request.CreateResponse(HttpStatusCode.NotFound);
      HttpResponseMessage response = this.Request.CreateResponse(HttpStatusCode.OK);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult securedObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = this.ProjectId.ToString()
        }
      };
      response.Content = (HttpContent) new VssServerStreamContent(attachmentStream, (object) securedObject);
      response.Content.Headers.ContentType = new MediaTypeHeaderValue(AttachmentDownloadHelper.GetContentType(attachmentName));
      response.Content.Headers.ContentDisposition = ContentDispositionBuilder.CreateAttachment(attachmentName);
      return response;
    }
  }
}
