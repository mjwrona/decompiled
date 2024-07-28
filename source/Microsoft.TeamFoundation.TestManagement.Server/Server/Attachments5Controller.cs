// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.Attachments5Controller
// Assembly: Microsoft.TeamFoundation.TestManagement.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: F9B71993-88CC-4B0D-89B6-4ADDEEAB3DE1
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.TestManagement.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.Server.Core;
using Microsoft.TeamFoundation.Server.Types;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Http;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [ControllerApiVersion(5.0)]
  [DemandFeature("402E4502-9389-420C-BA11-796CDA2E4867", true)]
  [VersionedApiControllerCustomName(Area = "Test", ResourceName = "Attachments", ResourceVersion = 1)]
  [RequestContentTypeRestriction(AllowStream = true, AllowEmptyOrPlainTextContentTypeForCompatServiceCalls = true, SafeCrossOriginUserAgents = new string[] {".*TestRunner\\(AzureTestPlans\\).*"})]
  public class Attachments5Controller : Attachments3Controller
  {
    [HttpGet]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
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
    [ClientResponseType(typeof (Stream), "GetTestSubResultAttachmentZip", "application/zip")]
    [ClientResponseType(typeof (Stream), "GetTestSubResultAttachmentContent", "application/octet-stream")]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
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

    [HttpPost]
    [ClientLocationId("2BFFEBE9-2F0F-4639-9AF8-56129E9FED2D")]
    [FeatureEnabled("TestManagement.Server.EnableHierarchicalResultAttachment")]
    public TestAttachmentReference CreateTestSubResultAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      int runId,
      int testCaseResultId,
      int testSubResultId)
    {
      return this.AttachmentsHelper.CreateTestAttachment(attachmentRequestModel, this.ProjectId.ToString(), runId, testCaseResultId, subResultId: testSubResultId);
    }
  }
}
