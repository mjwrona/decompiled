// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.ITeamFoundationTestManagementAttachmentsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using System;
using System.Collections.Generic;
using System.IO;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  [DefaultServiceImplementation(typeof (TeamFoundationTestManagementAttachmentsService))]
  public interface ITeamFoundationTestManagementAttachmentsService : IVssFrameworkService
  {
    bool DeleteTestAttachmentFromLogStore(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int runId,
      string filename,
      TestLogType testLogType = TestLogType.GeneralAttachment,
      int resultId = 0,
      int iterationId = 0,
      int subResultId = 0);

    bool CreateTestAttachmentInLogStore(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int runId,
      int resultId = 0,
      int iterationId = 0,
      string actionPath = null,
      int subResultId = 0);

    int CreateTestAttachment(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int runId,
      int resultId = 0,
      int iterationId = 0,
      string actionPath = null,
      int subResultId = 0);

    long[] CreateTestSubResultJsonAttachment(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int runId,
      int[] resultIds,
      bool shouldUploadToLogStore = false);

    Stream GetTestAttachment(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int testRunId,
      int testResultId,
      int attachmentId,
      out string attachmentFileName,
      out CompressionType attachmentCompressionType,
      int subResultId = 0);

    List<TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int runId,
      int resultId,
      int subResultId = 0,
      int attachmentId = 0);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int sessionId,
      int subResultId,
      int attachmentId);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      string projectName,
      int runId,
      int resultId,
      int sessionId,
      int subResultId,
      int attachmentId);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      string projectName,
      int attachmentId,
      bool getSiblingAttachments);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int attachmentId,
      bool getSiblingAttachments);

    List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachmentsByQuery(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query);

    List<bool> CheckIfActionRecordingExists(
      TestManagementRequestContext context,
      Guid projectId,
      IList<int> testCaseIds);

    List<bool> CheckIfActionRecordingExists(
      TestManagementRequestContext context,
      string projectName,
      IList<int> testCaseIds);

    Stream GetAttachmentStream(
      TestManagementRequestContext context,
      string projectName,
      int attachmentId,
      out string attachmentName);

    void DeleteTestAttachment(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int runId,
      int resultId,
      int attachmentId);

    Stream GetTestIterationAttachment(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int testRunId,
      int testResultId,
      int attachmentId,
      int iterationId,
      out string attachmentFileName,
      out CompressionType attachmentCompressionType);

    bool UploadAttachmentToLogStore(
      TestManagementRequestContext context,
      HttpPostedTcmAttachment attachment,
      Guid projectId,
      int runId,
      int resultId,
      int sessionId,
      int attachmentId,
      long uncompressedLength);

    bool CreateBuildAttachmentInLogStore(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int buildId);

    List<TestResultAttachment> CreateAttachmentIdMappingsForLogStore(
      TestManagementRequestContext context,
      string projectId,
      List<TestResultAttachment> attachments);

    TestResultAttachment GetAttachmentIdMappingForLogStore(
      TestManagementRequestContext context,
      string projectId,
      TestResultAttachment attachment);
  }
}
