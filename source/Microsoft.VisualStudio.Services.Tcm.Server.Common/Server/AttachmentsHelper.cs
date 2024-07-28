// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.AttachmentsHelper
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.Tcm.WebApi;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class AttachmentsHelper : RestApiHelper
  {
    private MergeTcmDataHelper m_mergeDataHelper;
    private const int c_maximumFileSize = 104857600;

    public AttachmentsHelper(TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public TestAttachmentReference CreateTestAttachment(
      TestAttachmentRequestModel attachmentRequestModel,
      string projectId,
      int runId,
      int resultId = 0,
      int iterationId = 0,
      string actionPath = null,
      int subResultId = 0)
    {
      ArgumentUtility.CheckForNull<TestAttachmentRequestModel>(attachmentRequestModel, nameof (attachmentRequestModel), "Test Results");
      this.RequestContext.RequestTimer.SetTimeToFirstPageBegin();
      this.RequestContext.TraceInfo("RestLayer", "AttachmentsHelper.CreateTestAttachment projectName = {0}, runId = {1}", (object) projectId, (object) runId);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      this.PublishTelemetryDataForCreateAttachment(projectId, runId, resultId, iterationId, attachmentRequestModel.FileName, nameof (CreateTestAttachment));
      return this.ExecuteAction<TestAttachmentReference>("AttachmentsHelper.CreateTestAttachment", (Func<TestAttachmentReference>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        if (resultId == 0)
        {
          TestAttachmentReference attachment;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryCreateTestRunAttachment(this.RequestContext, projectReference.Id, attachmentRequestModel, runId, out attachment))
            return attachment;
        }
        else if (subResultId == 0)
        {
          if (iterationId == 0)
          {
            TestAttachmentReference attachment;
            if (this.TestManagementRequestContext.TcmServiceHelper.TryCreateTestResultAttachment(this.RequestContext, projectReference.Id, attachmentRequestModel, runId, resultId, out attachment))
              return attachment;
          }
          else
          {
            TestAttachmentReference attachment;
            if (this.TestManagementRequestContext.TcmServiceHelper.TryCreateTestIterationResultAttachment(this.RequestContext, projectReference.Id, attachmentRequestModel, runId, resultId, iterationId, actionPath, out attachment))
              return attachment;
          }
        }
        else
        {
          TestAttachmentReference attachment;
          if (this.TestManagementRequestContext.TcmServiceHelper.TryCreateTestSubResultAttachment(this.RequestContext, projectReference.Id, attachmentRequestModel, runId, resultId, subResultId, out attachment))
            return attachment;
        }
        if (attachmentRequestModel.AttachmentType == TestResultsConstants.TestSubResultJsonAttachmentType)
          throw new InvalidPropertyException("AttachmentType", ServerResources.InvalidAttachmentType);
        int testAttachment = this.TestManagementAttachmentsService.CreateTestAttachment(this.TestManagementRequestContext, attachmentRequestModel, projectReference, runId, resultId, iterationId, actionPath, subResultId);
        string attachmentUrl = this.GetAttachmentUrl(projectReference.Name, runId, resultId, testAttachment);
        return new TestAttachmentReference(testAttachment, attachmentUrl);
      }), 1015054, "TestManagement");
    }

    public TestLogStoreAttachmentReference CreateTestAttachmentInLogStore(
      TestAttachmentRequestModel attachmentRequestModel,
      string projectId,
      int runId,
      int resultId = 0,
      int iterationId = 0,
      string actionPath = null,
      int subResultId = 0)
    {
      this.RequestContext.RequestTimer.SetTimeToFirstPageBegin();
      this.RequestContext.TraceInfo("RestLayer", "AttachmentsHelper.CreateTestAttachmentInLogStore projectName = {0}, runId = {1}, resultId = {2}, iterationId = {3}, filename = {4}", (object) projectId, (object) runId, (object) resultId, (object) iterationId, (object) attachmentRequestModel.FileName);
      ArgumentUtility.CheckForNull<TestAttachmentRequestModel>(attachmentRequestModel, nameof (attachmentRequestModel), "Test Results");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      this.PublishTelemetryDataForCreateAttachment(projectId, runId, resultId, iterationId, attachmentRequestModel.FileName, "CreateTestAttachmentInLogStore_NewApi");
      return this.ExecuteAction<TestLogStoreAttachmentReference>("AttachmentsHelper.CreateTestAttachmentInLogStore", (Func<TestLogStoreAttachmentReference>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        if (attachmentRequestModel.AttachmentType == TestResultsConstants.TestSubResultJsonAttachmentType)
          throw new InvalidPropertyException("AttachmentType", ServerResources.InvalidAttachmentType);
        if (!this.TestManagementAttachmentsService.CreateTestAttachmentInLogStore(this.TestManagementRequestContext, attachmentRequestModel, projectReference, runId, resultId, iterationId, actionPath, subResultId))
          throw new Exception(string.Format("Attachment not created in the LogStore. Filename: {0}, ProjectId: {1}, RunId: {2}, ResultId: {3}, SubResultId: {4}, IterationId: {5}", (object) attachmentRequestModel.FileName, (object) projectId, (object) runId, (object) resultId, (object) subResultId, (object) iterationId));
        return new TestLogStoreAttachmentReference(this.GetLogStoreAttachmentUrl(projectReference.Name, runId, resultId, attachmentRequestModel.FileName));
      }), 1015054, "TestManagement");
    }

    public void CreateBuildAttachmentInLogStore(
      TestAttachmentRequestModel attachmentRequestModel,
      string projectId,
      int buildId)
    {
      ArgumentUtility.CheckForNull<string>(projectId, nameof (projectId), "Test Results");
      ArgumentUtility.CheckForNull<TestAttachmentRequestModel>(attachmentRequestModel, nameof (attachmentRequestModel), "Test Results");
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      this.TestManagementAttachmentsService.CreateBuildAttachmentInLogStore(this.TestManagementRequestContext, attachmentRequestModel, projectReference, buildId);
    }

    public virtual Stream GetTestIterationAttachment(
      string projectId,
      int runId,
      int resultId,
      int attachmentId,
      out string fileName,
      out CompressionType compressionType,
      int iterationId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      if (resultId < 0)
        throw new InvalidPropertyException(nameof (resultId), ServerResources.InvalidPropertyMessage);
      if (iterationId < 0)
        throw new InvalidPropertyException(nameof (iterationId), ServerResources.InvalidPropertyMessage);
      string attachmentName = string.Empty;
      CompressionType attachmentCompressionType = CompressionType.None;
      Stream iterationAttachment1 = this.ExecuteAction<Stream>("AttachmentsHelper.GetTestIterationAttachment", (Func<Stream>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        this.CheckForViewTestResultPermission(projectReference.Name);
        Attachment attachment = (Attachment) null;
        bool flag = false;
        if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestIterationAttachment(this.RequestContext, projectReference.Id, runId, resultId, attachmentId, iterationId, out attachment))
          flag = true;
        if (flag)
        {
          attachmentName = attachment?.FileName;
          attachmentCompressionType = attachment == null || !string.Equals("gzip", attachment.CompressionType, StringComparison.OrdinalIgnoreCase) ? CompressionType.None : CompressionType.GZip;
          return attachment?.Stream;
        }
        Stream iterationAttachment2 = this.TestManagementAttachmentsService.GetTestIterationAttachment(this.TestManagementRequestContext, projectReference, runId, resultId, attachmentId, iterationId, out attachmentName, out attachmentCompressionType);
        if (iterationAttachment2 != null && iterationAttachment2.Length > 0L && !string.IsNullOrEmpty(attachmentName) || !this.TestManagementRequestContext.IsTcmService || !this.CheckIfQueryingFromLogStoreIsAllowed() || !this.TryGetTestAttachmentFromLogStore(projectId, runId, attachmentId, resultId, 0, out attachment))
          return iterationAttachment2;
        attachmentName = attachment?.FileName;
        attachmentCompressionType = attachment == null || !string.Equals("gzip", attachment.CompressionType, StringComparison.OrdinalIgnoreCase) ? CompressionType.None : CompressionType.GZip;
        iterationAttachment2 = attachment?.Stream;
        return iterationAttachment2;
      }), 1015054, "TestManagement");
      fileName = attachmentName;
      compressionType = attachmentCompressionType;
      this.PublishTelemetryData(resultId, fileName, iterationId);
      return iterationAttachment1;
    }

    public virtual Stream GetTestAttachment(
      string projectId,
      int runId,
      int resultId,
      int attachmentId,
      out string fileName,
      out CompressionType compressionType,
      int subResultId = 0)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      if (runId <= 0)
        throw new InvalidPropertyException(nameof (runId), ServerResources.InvalidPropertyMessage);
      if (resultId < 0)
        throw new InvalidPropertyException(nameof (resultId), ServerResources.InvalidPropertyMessage);
      if (subResultId < 0)
        throw new InvalidPropertyException(nameof (subResultId), ServerResources.InvalidPropertyMessage);
      string attachmentName = string.Empty;
      CompressionType attachmentCompressionType = CompressionType.None;
      Stream testAttachment1 = this.ExecuteAction<Stream>("AttachmentsHelper.GetTestAttachment", (Func<Stream>) (() =>
      {
        TeamProjectReference projectReference = this.GetProjectReference(projectId);
        this.CheckForViewTestResultPermission(projectReference.Name);
        Attachment attachment = (Attachment) null;
        bool flag = false;
        try
        {
          if (resultId == 0)
          {
            if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestRunAttachment(this.RequestContext, projectReference.Id, runId, attachmentId, out attachment))
              flag = true;
          }
          else if (subResultId == 0)
          {
            if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultAttachment(this.RequestContext, projectReference.Id, runId, resultId, attachmentId, out attachment))
              flag = true;
          }
          else if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestSubResultAttachment(this.RequestContext, projectReference.Id, runId, resultId, subResultId, attachmentId, out attachment))
            flag = true;
        }
        catch (Exception ex) when (ex.Message == "NotFound")
        {
          this.RequestContext.TraceError("RestLayer", "AttachmentsHelper.GetTestAttachment: Attachment not found. projectName = {0}, runId = {1}, resultId = {2}, subResultId = {3}, attachmentId = {4}", (object) projectId, (object) runId, (object) resultId, (object) subResultId, (object) attachmentId);
          return (Stream) null;
        }
        if (flag)
        {
          attachmentName = attachment?.FileName;
          attachmentCompressionType = attachment == null || !string.Equals("gzip", attachment.CompressionType, StringComparison.OrdinalIgnoreCase) ? CompressionType.None : CompressionType.GZip;
          return attachment?.Stream;
        }
        Stream testAttachment2 = this.TestManagementAttachmentsService.GetTestAttachment(this.TestManagementRequestContext, projectReference, runId, resultId, attachmentId, out attachmentName, out attachmentCompressionType, subResultId);
        if (testAttachment2 != null && testAttachment2.Length > 0L && !string.IsNullOrEmpty(attachmentName) || !this.TestManagementRequestContext.IsTcmService || !this.CheckIfQueryingFromLogStoreIsAllowed() || !this.TryGetTestAttachmentFromLogStore(projectId, runId, attachmentId, resultId, subResultId, out attachment))
          return testAttachment2;
        attachmentName = attachment?.FileName;
        attachmentCompressionType = attachment == null || !string.Equals("gzip", attachment.CompressionType, StringComparison.OrdinalIgnoreCase) ? CompressionType.None : CompressionType.GZip;
        testAttachment2 = attachment?.Stream;
        return testAttachment2;
      }), 1015054, "TestManagement");
      fileName = attachmentName;
      compressionType = attachmentCompressionType;
      this.PublishTelemetryData(resultId, fileName);
      return testAttachment1;
    }

    public virtual VssServerStreamContent GetTestAttachmentFromLogStore(
      string projectId,
      int runId,
      int resultId,
      string fileName,
      out CompressionType compressionType,
      int subResultId = 0)
    {
      this.RequestContext.TraceInfo("RestLayer", "AttachmentsHelper.GetTestAttachmentFromLogStore projectName = {0}, runId = {1}, resultId = {2}, subResultId = {3}, fileName = {4}", (object) projectId, (object) runId, (object) resultId, (object) subResultId, (object) fileName);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      if (resultId < 0)
        throw new InvalidPropertyException(nameof (resultId), ServerResources.InvalidPropertyMessage);
      if (subResultId < 0)
        throw new InvalidPropertyException(nameof (subResultId), ServerResources.InvalidPropertyMessage);
      string attachmentName = string.Empty;
      CompressionType attachmentCompressionType = CompressionType.None;
      Stream content = this.ExecuteAction<Stream>("AttachmentsHelper.GetTestAttachmentFromLogStore", (Func<Stream>) (() =>
      {
        this.CheckForViewTestResultPermission(this.GetProjectReference(projectId).Name);
        Attachment attachment = (Attachment) null;
        Stream attachmentFromLogStore = (Stream) new MemoryStream();
        if (this.TestManagementRequestContext.IsTcmService && this.CheckIfQueryingFromLogStoreIsAllowed() && this.TryGetTestAttachmentFromLogStoreV2(projectId, runId, fileName, resultId, subResultId, out attachment))
        {
          attachmentName = attachment?.FileName;
          attachmentCompressionType = attachment == null || !string.Equals("gzip", attachment.CompressionType, StringComparison.OrdinalIgnoreCase) ? CompressionType.None : CompressionType.GZip;
          attachmentFromLogStore = attachment?.Stream;
        }
        return attachmentFromLogStore;
      }), 1015054, "TestManagement");
      fileName = attachmentName;
      compressionType = attachmentCompressionType;
      this.PublishTelemetryDataForGetTestAttachmentFromLogStore(projectId, runId, resultId, subResultId, fileName);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult securedObject = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      };
      return new VssServerStreamContent(content, (object) securedObject);
    }

    public List<TestAttachment> GetTestAttachments(
      string projectId,
      int runId,
      int resultId,
      int subResultId = 0)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      AttachmentsHelper.ValidateInputParameters(runId, resultId, subResultId);
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      this.CheckForViewTestResultPermission(projectReference.Name);
      List<TestAttachment> attachments1 = (List<TestAttachment>) null;
      List<TestAttachment> testAttachmentList = this.GetAttachmentsFromFileService(runId, resultId, subResultId, projectReference, attachments1);
      List<TestAttachment> attachmentsFromLogStore = this.GetAttachmentsFromLogStore(projectId, runId, resultId, subResultId);
      if (testAttachmentList == null)
        testAttachmentList = this.ExecuteAction<List<TestAttachment>>("AttachmentsHelper.GetTestAttachments", (Func<List<TestAttachment>>) (() =>
        {
          List<TestResultAttachment> list = this.TestManagementAttachmentsService.GetTestAttachments(this.TestManagementRequestContext, projectReference, runId, resultId, subResultId).Where<TestResultAttachment>((Func<TestResultAttachment, bool>) (t => !string.Equals(t.AttachmentType, TestResultsConstants.TestSubResultJsonAttachmentType, StringComparison.OrdinalIgnoreCase))).ToList<TestResultAttachment>();
          List<TestAttachment> testAttachments = new List<TestAttachment>();
          if (list != null)
          {
            foreach (TestResultAttachment attachment in list)
            {
              if (attachment != null)
                testAttachments.Add(this.ConvertTestResultAttachmentToDataContract(projectReference, attachment));
            }
          }
          return testAttachments;
        }), 1015054, "TestManagement");
      List<TestAttachment> attachments2 = testAttachmentList ?? new List<TestAttachment>();
      attachments2.AddRange((IEnumerable<TestAttachment>) attachmentsFromLogStore);
      this.SecureTestAttachment(attachments2, projectReference.Id);
      return attachments2;
    }

    public List<TestLogStoreAttachment> GetTestAttachmentsFromLogStore(
      string projectId,
      int runId,
      int resultId,
      int subResultId = 0)
    {
      this.RequestContext.TraceInfo("RestLayer", "AttachmentsHelper.GetTestAttachmentsFromLogStore projectName = {0}, runId = {1}, resultId = {2}, subResultId = {3}", (object) projectId, (object) runId, (object) resultId, (object) subResultId);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      AttachmentsHelper.ValidateInputParameters(runId, resultId, subResultId);
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      this.CheckForViewTestResultPermission(projectReference.Name);
      List<TestLogStoreAttachment> logStoreAttachments = new List<TestLogStoreAttachment>();
      if (this.TestManagementRequestContext.IsTcmService && this.CheckIfQueryingFromLogStoreIsAllowed())
        this.TryGetTestAttachmentsFromLogStoreV2(projectId, runId, resultId, subResultId, out logStoreAttachments);
      this.SecureTestLogStoreAttachment(logStoreAttachments, projectReference.Id);
      return logStoreAttachments;
    }

    public async Task<List<TestAttachment>> GetTestAttachmentsAsync(
      string projectId,
      int runId,
      int resultId,
      int subResultId = 0)
    {
      AttachmentsHelper attachmentsHelper = this;
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      AttachmentsHelper.ValidateInputParameters(runId, resultId, subResultId);
      TeamProjectReference projectReference = attachmentsHelper.GetProjectReference(projectId);
      attachmentsHelper.CheckForViewTestResultPermission(projectReference.Name);
      List<TestAttachment> attachments = (List<TestAttachment>) null;
      attachments = attachmentsHelper.GetAttachmentsFromFileService(runId, resultId, subResultId, projectReference, attachments) ?? attachmentsHelper.ExecuteAction<List<TestAttachment>>("AttachmentsHelper.GetTestAttachmentsAsync", (Func<List<TestAttachment>>) (() =>
      {
        List<TestResultAttachment> list = this.TestManagementAttachmentsService.GetTestAttachments(this.TestManagementRequestContext, projectReference, runId, resultId, subResultId).Where<TestResultAttachment>((Func<TestResultAttachment, bool>) (t => !string.Equals(t.AttachmentType, TestResultsConstants.TestSubResultJsonAttachmentType, StringComparison.OrdinalIgnoreCase))).ToList<TestResultAttachment>();
        List<TestAttachment> attachmentsAsync = new List<TestAttachment>();
        if (list != null)
        {
          foreach (TestResultAttachment attachment in list)
          {
            if (attachment != null)
              attachmentsAsync.Add(this.ConvertTestResultAttachmentToDataContract(projectReference, attachment));
          }
        }
        return attachmentsAsync;
      }), 1015850, "TestManagement");
      attachments = attachments ?? new List<TestAttachment>();
      attachments.AddRange((IEnumerable<TestAttachment>) await attachmentsHelper.GetAttachmentsFromLogStoreAsync(projectId, runId, resultId, subResultId));
      attachmentsHelper.SecureTestAttachment(attachments, projectReference.Id);
      List<TestAttachment> attachmentsAsync1 = attachments;
      attachments = (List<TestAttachment>) null;
      return attachmentsAsync1;
    }

    public static string GetFilePathForAttachmentType(TestAttachmentType testAttachmentType)
    {
      switch (testAttachmentType)
      {
        case TestAttachmentType.TestRun:
          return TestResultsConstants.TestRunLogStoreFilePath;
        case TestAttachmentType.TestResult:
          return TestResultsConstants.TestResultLogStoreFilePath;
        case TestAttachmentType.TestSubResult:
          return TestResultsConstants.TestSubResultV2LogStoreFilePath;
        default:
          return TestResultsConstants.TestRunLogStoreFilePath;
      }
    }

    private List<TestAttachment> GetAttachmentsFromLogStore(
      string projectId,
      int runId,
      int resultId,
      int subResultId)
    {
      List<TestAttachment> logStoreAttachments = new List<TestAttachment>();
      if (this.TestManagementRequestContext.IsTcmService && this.CheckIfQueryingFromLogStoreIsAllowed())
        this.TryGetTestAttachmentsFromLogStore(projectId, runId, resultId, subResultId, out logStoreAttachments);
      return logStoreAttachments;
    }

    private async Task<List<TestAttachment>> GetAttachmentsFromLogStoreAsync(
      string projectId,
      int runId,
      int resultId,
      int subResultId)
    {
      AttachmentsHelper attachmentsHelper = this;
      List<TestAttachment> fromLogStoreAsync = new List<TestAttachment>();
      if (attachmentsHelper.TestManagementRequestContext.IsTcmService && attachmentsHelper.CheckIfQueryingFromLogStoreIsAllowed())
        fromLogStoreAsync = await attachmentsHelper.TryGetTestAttachmentsFromLogStoreAsync(projectId, runId, resultId, subResultId);
      return fromLogStoreAsync;
    }

    private List<TestAttachment> GetAttachmentsFromFileService(
      int runId,
      int resultId,
      int subResultId,
      TeamProjectReference projectReference,
      List<TestAttachment> attachments)
    {
      if (resultId == 0)
      {
        List<TestAttachment> attachments1;
        if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestRunAttachments(this.RequestContext, projectReference.Id, runId, out attachments1))
          attachments = attachments1;
      }
      else if (subResultId == 0)
      {
        List<TestAttachment> attachments2;
        if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultAttachments(this.RequestContext, projectReference.Id, runId, resultId, out attachments2))
          attachments = attachments2;
      }
      else
      {
        List<TestAttachment> attachments3;
        if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestSubResultAttachments(this.RequestContext, projectReference.Id, runId, resultId, subResultId, out attachments3))
          attachments = attachments3;
      }
      return attachments;
    }

    private static void ValidateInputParameters(int runId, int resultId, int subResultId)
    {
      if (runId <= 0)
        throw new InvalidPropertyException(nameof (runId), ServerResources.InvalidPropertyMessage);
      if (resultId < 0)
        throw new InvalidPropertyException(nameof (resultId), ServerResources.InvalidPropertyMessage);
      if (subResultId < 0)
        throw new InvalidPropertyException(nameof (subResultId), ServerResources.InvalidPropertyMessage);
    }

    public List<bool> CheckIfActionRecordingExists(
      TestManagementRequestContext context,
      string projectName,
      IList<int> testCaseIds)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      Dictionary<int, bool> existenceMapping;
      context.TcmServiceHelper.TryGetAfnStripsExistenceMapping(context.RequestContext, projectName, testCaseIds, out existenceMapping);
      List<bool> boolList = context.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().CheckIfActionRecordingExists(context, projectName, testCaseIds);
      Dictionary<int, bool> mapping1 = new Dictionary<int, bool>();
      Dictionary<int, int> dictionary1 = new Dictionary<int, int>();
      for (int index = 0; index < testCaseIds.Count; ++index)
      {
        mapping1.Add(testCaseIds[index], boolList[index]);
        dictionary1.Add(testCaseIds[index], index);
      }
      Dictionary<int, bool> dictionary2 = this.MergeDataHelper.MergeExistenceMapping(mapping1, existenceMapping);
      bool[] source = new bool[testCaseIds.Count];
      foreach (KeyValuePair<int, bool> keyValuePair in dictionary2)
        source[dictionary1[keyValuePair.Key]] = keyValuePair.Value;
      return ((IEnumerable<bool>) source).ToList<bool>();
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachmentsByQuery(
      TestManagementRequestContext context,
      string projectName,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery>(query, nameof (query), "Test Results");
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments;
      context.TcmServiceHelper.TryGetAttachmentsByQuery(context.RequestContext, projectName, query, out attachments);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachmentsByQuery = context.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().GetTestAttachmentsByQuery(context, query);
      return context.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableUnionMergeLogic") ? this.MergeDataHelper.MergeAttachments(attachments, attachmentsByQuery, true) : this.MergeDataHelper.MergeAttachments(attachments, attachmentsByQuery);
    }

    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      string projectName,
      int attachmentId,
      bool getSiblingAttachments)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments;
      return context.TcmServiceHelper.TryGetTestAttachments(context.RequestContext, projectName, attachmentId, getSiblingAttachments, out attachments) ? attachments : (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) context.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().GetTestAttachments(context, projectName, attachmentId, getSiblingAttachments);
    }

    public IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      string projectName,
      int runId,
      int resultId,
      int sessionId,
      int subResultId,
      int attachmentId)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> attachments;
      return context.TcmServiceHelper.TryGetTestAttachments(context.RequestContext, projectName, runId, resultId, sessionId, subResultId, attachmentId, out attachments) ? attachments : (IList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) context.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().GetTestAttachments(context, projectName, runId, resultId, sessionId, subResultId, attachmentId);
    }

    public void DeleteTestAttachment(string projectId, int runId, int resultId, int attachmentId)
    {
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      this.RequestContext.Trace(1015851, TraceLevel.Info, "TestManagement", "DeleteAttachment", "AttachmentsHelper.DeleteTestAttachment projectName = {0}, runId = {1}, resultId: {2}, attachmentId: {3}", (object) projectId, (object) runId, (object) resultId, (object) attachmentId);
      if (runId <= 0)
        throw new InvalidPropertyException(nameof (runId), ServerResources.InvalidPropertyMessage);
      if (resultId < 0)
        throw new InvalidPropertyException(nameof (resultId), ServerResources.InvalidPropertyMessage);
      this.TestManagementAttachmentsService.DeleteTestAttachment(this.TestManagementRequestContext, this.GetProjectReference(projectId), runId, resultId, attachmentId);
      this.PublishTelemetryDataForDeleteTestAttachment(projectId, runId, resultId, 0, attachmentId);
    }

    public void DeleteTestAttachmentFromLogStore(
      string projectId,
      int runId,
      int resultId,
      string filename)
    {
      this.RequestContext.TraceInfo("RestLayer", "AttachmentsHelper.DeleteTestAttachmentFromLogStore projectName = {0}, runId = {1}, resultId: {2}, filename: {3}", (object) projectId, (object) runId, (object) resultId, (object) filename);
      this.PublishTelemetryDataForDeleteTestAttachmentFromLogStore(projectId, runId, resultId, 0, filename);
      ArgumentUtility.CheckStringForNullOrWhiteSpace(projectId, "teamProjectId", "Test Results");
      TeamProjectReference projectReference = this.GetProjectReference(projectId);
      if (!this.TestManagementAttachmentsService.DeleteTestAttachmentFromLogStore(this.TestManagementRequestContext, projectReference, runId, filename, resultId: resultId))
      {
        this.TestManagementRequestContext.TraceError("DeleteAttachment", "Delete Attachment failed for ProjectName {0} with RunId {1}, ResultId {2}, Attachment filename {3}", (object) projectReference.Id, (object) runId, (object) resultId, (object) filename);
        throw new InvalidPropertyException("AttachmentFileName", ServerResources.InvalidPropertyMessage);
      }
    }

    public Stream GetAttachmentStream(
      string projectName,
      int attachmentId,
      out string attachmentName)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      ITeamFoundationTestManagementAttachmentsService service = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>();
      try
      {
        TcmAttachment tcmAttachment;
        if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultAttachment(this.TestManagementRequestContext, projectName, attachmentId, out tcmAttachment))
        {
          attachmentName = tcmAttachment.FileName;
          return tcmAttachment.Stream;
        }
      }
      catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException ex)
      {
        this.TestManagementRequestContext.TraceError("DownloadAttachment", "Download Attachment failed for ProjectName {0} with AttachmentId {1}", (object) projectName, (object) attachmentId);
      }
      return service.GetAttachmentStream(this.TestManagementRequestContext, projectName, attachmentId, out attachmentName);
    }

    public string GetAttachmentName(string projectName, int attachmentId)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      try
      {
        TcmAttachment tcmAttachment;
        if (this.TestManagementRequestContext.TcmServiceHelper.TryGetTestResultAttachment(this.TestManagementRequestContext, projectName, attachmentId, out tcmAttachment))
          return tcmAttachment.FileName;
      }
      catch (Microsoft.TeamFoundation.TestManagement.WebApi.TestObjectNotFoundException ex)
      {
        this.TestManagementRequestContext.TraceError(nameof (GetAttachmentName), "Get Attachment from TcmServiceHelper failed for ProjectName {0} with AttachmentId {1}", (object) projectName, (object) attachmentId);
      }
      try
      {
        List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> testAttachments = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>().GetTestAttachments(this.TestManagementRequestContext, projectName, attachmentId, false);
        if (testAttachments != null)
        {
          if (testAttachments.Count > 0)
            return testAttachments[0].FileName;
        }
      }
      catch (TestObjectNotFoundException ex)
      {
        this.TestManagementRequestContext.TraceError(nameof (GetAttachmentName), "Get Attachment from ITeamFoundationTestManagementAttachmentsService failed for ProjectName {0} with AttachmentId {1}", (object) projectName, (object) attachmentId);
      }
      return string.Empty;
    }

    public static TestLogType GetTestLogType(string attachmentType)
    {
      TestLogType result = TestLogType.GeneralAttachment;
      if ("TestImpactDetails".Equals(attachmentType))
        return TestLogType.TestImpact;
      return !string.IsNullOrEmpty(attachmentType) && !EnumUtility.TryParse<TestLogType>(attachmentType, true, out result) ? TestLogType.System : result;
    }

    private bool CheckIfQueryingFromLogStoreIsAllowed() => this.TestManagementRequestContext.IsFeatureEnabled("TestManagement.Server.TestLogStoreOnTCMService") && this.TestManagementRequestContext.IsFeatureEnabled("TestManagement.Server.QueryTestLogStoreFromAttachmentsAPI");

    private bool IsLogStoreAttachmentIdMappingInDBEnabled() => this.TestManagementRequestContext.IsFeatureEnabled("TestManagement.Server.EnableLogStoreAttachmentIdMappingInDB");

    private bool TryGetTestAttachmentFromLogStore(
      string projectId,
      int runId,
      int attachmentId,
      int resultId,
      int subResultId,
      out Attachment attachment)
    {
      attachment = new Attachment();
      Stream targetStream = (Stream) new MemoryStream();
      try
      {
        ITestLogStoreService service = this.TestManagementRequestContext.RequestContext.GetService<ITestLogStoreService>();
        AttachmentTestLogReference testLogReference1 = new AttachmentTestLogReference();
        testLogReference1.RunId = runId;
        testLogReference1.ResultId = resultId;
        testLogReference1.SubResultId = subResultId;
        testLogReference1.Scope = TestLogScope.Run;
        testLogReference1.AttachmentId = attachmentId;
        List<AttachmentTestLog> attachmentTestLogs = new List<AttachmentTestLog>();
        if (this.IsLogStoreAttachmentIdMappingInDBEnabled())
        {
          AttachmentTestLog attachmentTestLog = this.GetEntryFromTableForAttachmentTestLog(projectId, runId, resultId, subResultId, attachmentId);
          if (attachmentTestLog != null)
            attachmentTestLogs.Add(attachmentTestLog);
        }
        else
        {
          ITestLogStoreService testLogStoreService = service;
          TestManagementRequestContext managementRequestContext = this.TestManagementRequestContext;
          ProjectInfo projectFromGuid = this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(new Guid(projectId));
          TestLogQueryParameters logQueryParameters = new TestLogQueryParameters();
          logQueryParameters.FetchMetaData = true;
          AttachmentTestLogReference logReference = testLogReference1;
          attachmentTestLogs = testLogStoreService.GetAllAttachmentTestLogs(managementRequestContext, projectFromGuid, logQueryParameters, logReference);
        }
        if (attachmentTestLogs == null || attachmentTestLogs.Count == 0)
          return false;
        AttachmentTestLog attachmentTestLog1 = attachmentTestLogs.Count == 1 ? attachmentTestLogs[0] : this.GetRecentAttachmentTestLog(attachmentTestLogs);
        this.ValidateAttachmentSize(attachmentTestLog1.Size);
        AttachmentTestLogReference testLogReference2 = attachmentTestLog1.AttachmentTestLogReference;
        if (testLogReference2 == null)
          return false;
        service.DownloadToStream(this.TestManagementRequestContext, this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(new Guid(projectId)), (TestLogReference) testLogReference2, targetStream);
        attachment.FileName = testLogReference2.FilePath;
        targetStream.Seek(0L, SeekOrigin.Begin);
        attachment.Stream = targetStream;
        bool isAttachmentFound = true;
        this.PublishTelemetryDataForGetAttachmentFromLogStore((TestLogReference) testLogReference2, attachmentId, attachmentTestLogs.Count, isAttachmentFound);
        return isAttachmentFound;
      }
      catch (Exception ex)
      {
        this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Error, "TestManagement", "AttachmentHandler", "GetTestAttachment - Failed while getting attachments from Log Store, ExceptionHit: {0}", (object) ex.Message);
        targetStream?.Dispose();
        throw;
      }
    }

    private AttachmentTestLog GetEntryFromTableForAttachmentTestLog(
      string projectId,
      int runId,
      int resultId,
      int subResultId,
      int attachmentId)
    {
      ITeamFoundationTestManagementAttachmentsService service = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>();
      TestResultAttachment resultAttachment = new TestResultAttachment()
      {
        Id = attachmentId,
        TestRunId = runId,
        TestResultId = resultId,
        SubResultId = subResultId
      };
      TestManagementRequestContext managementRequestContext = this.TestManagementRequestContext;
      string projectId1 = projectId;
      TestResultAttachment attachment = resultAttachment;
      TestResultAttachment mappingForLogStore = service.GetAttachmentIdMappingForLogStore(managementRequestContext, projectId1, attachment);
      AttachmentTestLog attachmentTestLog = (AttachmentTestLog) null;
      if (mappingForLogStore != null && mappingForLogStore.FileName != null)
      {
        attachmentTestLog = new AttachmentTestLog();
        AttachmentTestLogReference testLogReference = new AttachmentTestLogReference();
        testLogReference.RunId = runId;
        testLogReference.ResultId = resultId;
        testLogReference.SubResultId = subResultId;
        testLogReference.AttachmentId = attachmentId;
        testLogReference.FilePath = mappingForLogStore.FileName;
        testLogReference.Type = AttachmentsHelper.GetTestLogType(mappingForLogStore.AttachmentType);
        attachmentTestLog.AttachmentTestLogReference = testLogReference;
      }
      return attachmentTestLog;
    }

    private bool TryGetTestAttachmentFromLogStoreV2(
      string projectId,
      int runId,
      string filename,
      int resultId,
      int subResultId,
      out Attachment attachment,
      TestLogType testLogType = TestLogType.GeneralAttachment)
    {
      attachment = new Attachment();
      Stream targetStream = (Stream) new MemoryStream();
      TestLogReference testLogReference = new TestLogReference();
      List<TestLog> testLogList = new List<TestLog>();
      bool isAttachmentFound = false;
      try
      {
        this.RequestContext.TraceInfo("RestLayer", "AttachmentsHelper.TryGetTestAttachmentFromLogStoreV2 projectName = {0}, runId = {1}, resultId = {2}, subResultId = {3}, fileName = {4}, TestLogType = {5}", (object) projectId, (object) runId, (object) resultId, (object) subResultId, (object) filename, (object) testLogType);
        int num = resultId == 0 ? 0 : (subResultId == 0 ? 1 : 2);
        ITestLogStoreService service = this.TestManagementRequestContext.RequestContext.GetService<ITestLogStoreService>();
        string str = AttachmentsHelper.GetFilePathForAttachmentType((TestAttachmentType) num) + filename;
        testLogReference.RunId = runId;
        testLogReference.ResultId = resultId;
        testLogReference.SubResultId = subResultId;
        testLogReference.Scope = TestLogScope.Run;
        testLogReference.FilePath = str;
        testLogReference.Type = testLogType;
        testLogList = service.GetAllTestLogs(this.TestManagementRequestContext, this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(new Guid(projectId)), new TestLogQueryParameters()
        {
          Type = testLogType,
          DirectoryPath = str,
          FetchMetaData = true
        }, testLogReference, true);
        if (testLogList == null || testLogList.Count == 0)
          return false;
        this.ValidateAttachmentSize(testLogList[0].Size);
        service.DownloadToStream(this.TestManagementRequestContext, this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(new Guid(projectId)), testLogReference, targetStream);
        attachment.FileName = testLogReference.FilePath;
        targetStream.Seek(0L, SeekOrigin.Begin);
        attachment.Stream = targetStream;
        isAttachmentFound = true;
        return true;
      }
      catch (Exception ex)
      {
        this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Error, "TestManagement", "AttachmentHandler", "GetTestAttachment - Failed while getting attachments from Log Store, ExceptionHit: {0}", (object) ex.Message);
        targetStream?.Dispose();
        throw;
      }
      finally
      {
        this.PublishTelemetryDataForGetAttachmentFromLogStore(testLogReference, 0, testLogList.Count, isAttachmentFound, false);
      }
    }

    private AttachmentTestLog GetRecentAttachmentTestLog(List<AttachmentTestLog> attachmentTestLogs)
    {
      attachmentTestLogs.Sort((Comparison<AttachmentTestLog>) ((x, y) => DateTime.Compare(x.ModifiedOn, y.ModifiedOn)));
      return attachmentTestLogs.LastOrDefault<AttachmentTestLog>();
    }

    private void ValidateAttachmentSize(long size)
    {
      if (size > 104857600L)
        throw new TestManagementValidationException(string.Format(ServerResources.FileDownloadExceedMaxSize, (object) 104857600));
    }

    private bool TryGetTestAttachmentsFromLogStore(
      string projectId,
      int runId,
      int resultId,
      int subResultId,
      out List<TestAttachment> logStoreAttachments)
    {
      logStoreAttachments = new List<TestAttachment>();
      try
      {
        List<TestLog> testLogs = resultId == 0 ? this.GetTestLogsAtRunLevelFromLogStore(projectId, runId) : this.GetTestLogsFromLogStore(projectId, runId, resultId, subResultId);
        if (testLogs != null && testLogs.Count == 0)
          return false;
        if (this.IsLogStoreAttachmentIdMappingInDBEnabled())
        {
          List<TestAttachment> attachmentIdMappings = this.CreateLogStoreAttachmentIdMappings(projectId, runId, resultId, subResultId, testLogs);
          logStoreAttachments.AddRange((IEnumerable<TestAttachment>) attachmentIdMappings);
        }
        else
        {
          List<AttachmentTestLog> logsFromLogStore = this.GetAllAttachmentTestLogsFromLogStore(projectId, runId, resultId, subResultId);
          if (testLogs != null && logsFromLogStore != null && testLogs.Count == logsFromLogStore.Count)
          {
            logStoreAttachments = this.ConvertAttachmentTestLogsToTestAttachmentDataContract(logsFromLogStore, projectId, runId, resultId, subResultId);
            this.PublishTelemetryDataForGetAttachmentsFromLogStore(runId, resultId, subResultId, testLogs.Count, false);
            this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Info, "TestManagement", "AttachmentHandler", "GetTestAttachments - Retrieved {0} TestLogs", (object) testLogs.Count);
          }
          else if (testLogs != null && testLogs.Count > 0)
          {
            int attachmentId = 1;
            foreach (TestLog testLog in testLogs)
            {
              this.CreateEmptyBlobForAttachmentTestLog(projectId, attachmentId, testLog);
              logStoreAttachments.Add(this.ConvertTestLogToTestAttachment(testLog, testLog.LogReference, projectId, attachmentId));
              attachmentId += 2;
            }
            this.PublishTelemetryDataForGetAttachmentsFromLogStore(runId, resultId, subResultId, testLogs.Count, true);
            this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Info, "TestManagement", "AttachmentHandler", "GetTestAttachments NewlyCreated - Retrieved {0} TestLogs", (object) testLogs.Count);
          }
        }
        return true;
      }
      catch (Exception ex)
      {
        this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Error, "TestManagement", "AttachmentHandler", "GetTestAttachments - Failed while getting attachments from Log Store, ExceptionHit: {0}", (object) ex);
        throw;
      }
    }

    private List<TestAttachment> CreateLogStoreAttachmentIdMappings(
      string projectId,
      int runId,
      int resultId,
      int subResultId,
      List<TestLog> testLogs)
    {
      List<TestAttachment> attachmentIdMappings = new List<TestAttachment>();
      if (testLogs != null && testLogs.Count > 0)
      {
        int num = 0;
        int batchSize = this.TestManagementRequestContext.RequestContext.GetService<IVssRegistryService>().GetValue<int>(this.TestManagementRequestContext.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TcmLogStoreAttachmentIdMappingTableInsertBatchSize", 1000);
        foreach (List<TestLog> testLogs1 in testLogs.Select((item, index) => new
        {
          item = item,
          index = index
        }).GroupBy(x => x.index / batchSize).Select<IGrouping<int, \u003C\u003Ef__AnonymousType5<TestLog, int>>, List<TestLog>>(g => g.Select(x => x.item).ToList<TestLog>()).ToList<List<TestLog>>())
        {
          List<TestResultAttachment> tableForTestLogs = this.CreateAttachmentIdMappingInTableForTestLogs(projectId, testLogs1, runId, resultId, subResultId);
          num += tableForTestLogs.Count;
          foreach (TestResultAttachment testAttachment in tableForTestLogs)
            attachmentIdMappings.Add(this.ConvertTestResultAttachmentToTestAttachment(projectId, testAttachment));
        }
        this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Info, "TestManagement", "AttachmentHandler", "GetTestAttachments - Retrieved {0} TestLogs", (object) testLogs.Count);
        this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Info, "TestManagement", "AttachmentHandler", "GetTestAttachments - Retrieved (Inserted or already existing) {0} testAttachments from database", (object) num);
      }
      return attachmentIdMappings;
    }

    private TestAttachment ConvertTestResultAttachmentToTestAttachment(
      string projectId,
      TestResultAttachment testAttachment)
    {
      AttachmentType attachmentType = (AttachmentType) Enum.Parse(typeof (AttachmentType), testAttachment.AttachmentType);
      string attachmentUrl = this.GetAttachmentUrl(this.TestManagementRequestContext.ProjectServiceHelper.GetProjectName(new Guid(projectId)), testAttachment.TestRunId, testAttachment.TestResultId, testAttachment.Id, testAttachment.SubResultId);
      return new TestAttachment()
      {
        AttachmentType = attachmentType,
        Id = testAttachment.Id,
        FileName = testAttachment.FileName,
        Url = attachmentUrl,
        Size = testAttachment.Length,
        CreatedDate = testAttachment.CreationDate
      };
    }

    private List<TestResultAttachment> CreateAttachmentIdMappingInTableForTestLogs(
      string projectId,
      List<TestLog> testLogs,
      int runId,
      int resultId,
      int subResultId)
    {
      ITeamFoundationTestManagementAttachmentsService service = this.TestManagementRequestContext.RequestContext.GetService<ITeamFoundationTestManagementAttachmentsService>();
      List<TestResultAttachment> attachments = new List<TestResultAttachment>();
      foreach (TestLog testLog in testLogs)
      {
        TestLogReference logReference = testLog.LogReference;
        string name = Enum.GetName(typeof (AttachmentType), (object) this.GetAttachmentTypeFromLogType(logReference.Type));
        TestResultAttachment resultAttachment = new TestResultAttachment()
        {
          FileName = logReference.FilePath,
          AttachmentType = name,
          TestRunId = runId,
          TestResultId = resultId,
          SubResultId = subResultId,
          CreationDate = testLog.ModifiedOn,
          Length = testLog.Size,
          IsComplete = true
        };
        if (logReference.FilePath.Length > 1000)
          this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Error, "TestManagement", "AttachmentHandler", "GetTestAttachments - testLog with FileName {0} is greater than 1000 length. Path is too long to store in Db table.", (object) logReference.FilePath);
        else
          attachments.Add(resultAttachment);
      }
      List<TestResultAttachment> mappingsForLogStore = service.CreateAttachmentIdMappingsForLogStore(this.TestManagementRequestContext, projectId, attachments);
      return mappingsForLogStore != null && mappingsForLogStore.Count > 0 ? mappingsForLogStore : new List<TestResultAttachment>();
    }

    private bool TryGetTestAttachmentsFromLogStoreV2(
      string projectId,
      int runId,
      int resultId,
      int subResultId,
      out List<TestLogStoreAttachment> logStoreAttachments)
    {
      logStoreAttachments = new List<TestLogStoreAttachment>();
      try
      {
        List<TestLog> testLogs = resultId == 0 ? this.GetTestLogsByAttachmentTypeFromLogStore(projectId, runId, TestAttachmentType.TestRun) : this.GetTestLogsFromLogStore(projectId, runId, resultId, subResultId);
        if (testLogs == null || testLogs.Count == 0)
          return false;
        logStoreAttachments = this.ConvertAttachmentTestLogsToTestLogStoreAttachmentDataContract(testLogs, projectId, runId, resultId, subResultId);
        this.PublishTelemetryDataForGetAttachmentsFromLogStoreV2(runId, resultId, subResultId, testLogs.Count);
        this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Info, "TestManagement", "AttachmentHandler", "TryGetTestAttachmentsFromLogStoreV2 - Retrieved {0} TestLogs", (object) testLogs.Count);
        return true;
      }
      catch (Exception ex)
      {
        this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Error, "TestManagement", "AttachmentHandler", "TryGetTestAttachmentsFromLogStoreV2 - Failed while getting attachments from Log Store, ExceptionHit: {0}", (object) ex);
        throw;
      }
    }

    private async Task<List<TestAttachment>> TryGetTestAttachmentsFromLogStoreAsync(
      string projectId,
      int runId,
      int resultId,
      int subResultId)
    {
      AttachmentsHelper attachmentsHelper = this;
      List<TestAttachment> logStoreAttachments = new List<TestAttachment>();
      try
      {
        List<TestLog> testLogs = resultId == 0 ? attachmentsHelper.GetTestLogsAtRunLevelFromLogStore(projectId, runId) : attachmentsHelper.GetTestLogsFromLogStore(projectId, runId, resultId, subResultId);
        if (testLogs != null && testLogs.Count == 0)
          return logStoreAttachments;
        if (attachmentsHelper.IsLogStoreAttachmentIdMappingInDBEnabled())
        {
          logStoreAttachments.AddRange((IEnumerable<TestAttachment>) attachmentsHelper.CreateLogStoreAttachmentIdMappings(projectId, runId, resultId, subResultId, testLogs));
        }
        else
        {
          List<AttachmentTestLog> logsFromLogStore = attachmentsHelper.GetAllAttachmentTestLogsFromLogStore(projectId, runId, resultId, subResultId);
          if (testLogs != null && logsFromLogStore != null && testLogs.Count == logsFromLogStore.Count)
          {
            logStoreAttachments = attachmentsHelper.ConvertAttachmentTestLogsToTestAttachmentDataContract(logsFromLogStore, projectId, runId, resultId, subResultId);
            attachmentsHelper.PublishTelemetryDataForGetAttachmentsFromLogStore(runId, resultId, subResultId, testLogs.Count, false, true);
          }
          else if (testLogs != null && testLogs.Count > 0)
          {
            int attachmentId = 1;
            foreach (TestLog testLog in testLogs)
            {
              await attachmentsHelper.CreateEmptyBlobForAttachmentTestLogAsync(projectId, attachmentId, testLog);
              logStoreAttachments.Add(attachmentsHelper.ConvertTestLogToTestAttachment(testLog, testLog.LogReference, projectId, attachmentId));
              attachmentId += 2;
            }
            attachmentsHelper.PublishTelemetryDataForGetAttachmentsFromLogStore(runId, resultId, subResultId, testLogs.Count, true, true);
          }
        }
        return logStoreAttachments;
      }
      catch (Exception ex)
      {
        attachmentsHelper.TestManagementRequestContext.RequestContext.Trace(1015850, TraceLevel.Error, "TestManagement", "AttachmentHandler", "GetTestAttachmentsAsync - Failed while getting attachments from Log Store, ExceptionHit: {0}", (object) ex.Message);
        throw;
      }
    }

    private List<AttachmentTestLog> GetAllAttachmentTestLogsFromLogStore(
      string projectId,
      int runId,
      int resultId = 0,
      int subResultId = 0)
    {
      ITestLogStoreService service = this.TestManagementRequestContext.RequestContext.GetService<ITestLogStoreService>();
      TestLogQueryParameters logQueryParameters1 = new TestLogQueryParameters();
      logQueryParameters1.FetchMetaData = true;
      AttachmentTestLogReference testLogReference = new AttachmentTestLogReference();
      testLogReference.RunId = runId;
      testLogReference.ResultId = resultId;
      testLogReference.SubResultId = subResultId;
      TestManagementRequestContext managementRequestContext = this.TestManagementRequestContext;
      ProjectInfo projectFromGuid = this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(new Guid(projectId));
      TestLogQueryParameters logQueryParameters2 = logQueryParameters1;
      AttachmentTestLogReference logReference = testLogReference;
      return service.GetAllAttachmentTestLogs(managementRequestContext, projectFromGuid, logQueryParameters2, logReference);
    }

    private List<TestAttachment> ConvertAttachmentTestLogsToTestAttachmentDataContract(
      List<AttachmentTestLog> attachmentTestLogs,
      string projectId,
      int runId,
      int resultId = 0,
      int subResultId = 0)
    {
      List<TestAttachment> attachmentDataContract = new List<TestAttachment>();
      if (attachmentTestLogs == null)
        return attachmentDataContract;
      foreach (AttachmentTestLog attachmentTestLog in attachmentTestLogs)
      {
        TestAttachment testAttachment = this.ConvertTestLogToTestAttachment((TestLog) attachmentTestLog, (TestLogReference) attachmentTestLog.AttachmentTestLogReference, projectId, attachmentTestLog.AttachmentTestLogReference.AttachmentId);
        attachmentDataContract.Add(testAttachment);
      }
      return attachmentDataContract;
    }

    private List<TestLogStoreAttachment> ConvertAttachmentTestLogsToTestLogStoreAttachmentDataContract(
      List<TestLog> testLogs,
      string projectId,
      int runId,
      int resultId = 0,
      int subResultId = 0)
    {
      List<TestLogStoreAttachment> attachmentDataContract = new List<TestLogStoreAttachment>();
      foreach (TestLog testLog in testLogs)
      {
        TestLogStoreAttachment logStoreAttachment = this.ConvertTestLogToTestLogStoreAttachment(testLog, testLog.LogReference, projectId);
        attachmentDataContract.Add(logStoreAttachment);
      }
      return attachmentDataContract;
    }

    private TestLogStoreAttachment ConvertTestLogToTestLogStoreAttachment(
      TestLog testLog,
      TestLogReference testLogReference,
      string projectId)
    {
      string filename = testLogReference.FilePath.Substring(testLogReference.FilePath.LastIndexOf('/') + 1);
      return new TestLogStoreAttachment()
      {
        AttachmentType = this.GetAttachmentTypeFromLogType(testLogReference.Type),
        FileName = filename,
        Url = this.GetLogStoreAttachmentUrl(this.TestManagementRequestContext.ProjectServiceHelper.GetProjectName(new Guid(projectId)), testLogReference.RunId, testLogReference.ResultId, filename, testLogReference.SubResultId),
        Size = testLog.Size,
        CreatedDate = testLog.ModifiedOn
      };
    }

    private TestAttachment ConvertTestLogToTestAttachment(
      TestLog testLog,
      TestLogReference testLogReference,
      string projectId,
      int attachmentId)
    {
      return new TestAttachment()
      {
        AttachmentType = this.GetAttachmentTypeFromLogType(testLogReference.Type),
        Id = attachmentId,
        FileName = testLogReference.FilePath,
        Url = this.GetAttachmentUrl(this.TestManagementRequestContext.ProjectServiceHelper.GetProjectName(new Guid(projectId)), testLogReference.RunId, testLogReference.ResultId, attachmentId, testLogReference.SubResultId),
        Size = testLog.Size,
        CreatedDate = testLog.ModifiedOn
      };
    }

    private List<TestLog> GetTestLogsFromLogStore(
      string projectId,
      int runId,
      int resultId = 0,
      int subResultId = 0,
      TestLogType logType = TestLogType.GeneralAttachment)
    {
      List<TestLog> testLogList = new List<TestLog>();
      return this.TestManagementRequestContext.RequestContext.GetService<ITestLogStoreService>().GetAllTestLogs(this.TestManagementRequestContext, this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(new Guid(projectId)), new TestLogQueryParameters()
      {
        Type = logType
      }, new TestLogReference()
      {
        RunId = runId,
        ResultId = resultId,
        SubResultId = subResultId,
        Type = logType
      });
    }

    private List<TestLog> GetTestLogsByAttachmentTypeFromLogStore(
      string projectId,
      int runId,
      TestAttachmentType testAttachmentType,
      int resultId = 0,
      int subResultId = 0,
      TestLogType logType = TestLogType.GeneralAttachment)
    {
      this.RequestContext.TraceInfo("RestLayer", "AttachmentsHelper.GetTestLogsByAttachmentTypeFromLogStore projectName = {0}, runId = {1}, resultId = {2}, subResultId = {3}, TestAttachmentType = {4}, TestLogType = {5}", (object) projectId, (object) runId, (object) resultId, (object) subResultId, (object) testAttachmentType, (object) logType);
      List<TestLog> testLogList = new List<TestLog>();
      return this.TestManagementRequestContext.RequestContext.GetService<ITestLogStoreService>().GetAllTestLogs(this.TestManagementRequestContext, this.TestManagementRequestContext.ProjectServiceHelper.GetProjectFromGuid(new Guid(projectId)), new TestLogQueryParameters()
      {
        Type = logType,
        DirectoryPath = AttachmentsHelper.GetFilePathForAttachmentType(testAttachmentType)
      }, new TestLogReference()
      {
        RunId = runId,
        ResultId = resultId,
        SubResultId = subResultId,
        Type = logType
      });
    }

    private async Task CreateEmptyBlobForAttachmentTestLogAsync(
      string projectId,
      int attachmentId,
      TestLog testLog)
    {
      AttachmentsHelper attachmentsHelper = this;
      ITestLogStoreService service = attachmentsHelper.TestManagementRequestContext.RequestContext.GetService<ITestLogStoreService>();
      AttachmentTestLogReference testLogReference = new AttachmentTestLogReference();
      testLogReference.RunId = testLog.LogReference.RunId;
      testLogReference.ResultId = testLog.LogReference.ResultId;
      testLogReference.SubResultId = testLog.LogReference.SubResultId;
      testLogReference.AttachmentId = attachmentId;
      testLogReference.Scope = TestLogScope.Run;
      testLogReference.FilePath = testLog.LogReference.FilePath;
      TestManagementRequestContext managementRequestContext = attachmentsHelper.TestManagementRequestContext;
      Guid projectId1 = new Guid(projectId);
      AttachmentTestLogReference attachmentTestLogReference = testLogReference;
      if (await service.CreateEmptyBlobForAttachmentTestLogAsync(managementRequestContext, projectId1, attachmentTestLogReference, new Dictionary<string, string>()
      {
        {
          "type",
          testLog.LogReference.Type.ToString()
        },
        {
          "size",
          testLog.Size.ToString()
        }
      }))
        return;
      attachmentsHelper.TestManagementRequestContext.RequestContext.Trace(1015850, TraceLevel.Warning, "TestManagement", "AttachmentHandler", "GetTestAttachmentsAsync - CreateBlobAsync failed or the Blob already created While creating empty blob to track the log store blob.");
    }

    private void CreateEmptyBlobForAttachmentTestLog(
      string projectId,
      int attachmentId,
      TestLog testLog)
    {
      ITestLogStoreService service = this.TestManagementRequestContext.RequestContext.GetService<ITestLogStoreService>();
      AttachmentTestLogReference testLogReference = new AttachmentTestLogReference();
      testLogReference.RunId = testLog.LogReference.RunId;
      testLogReference.ResultId = testLog.LogReference.ResultId;
      testLogReference.SubResultId = testLog.LogReference.SubResultId;
      testLogReference.AttachmentId = attachmentId;
      testLogReference.Scope = TestLogScope.Run;
      testLogReference.FilePath = testLog.LogReference.FilePath;
      TestManagementRequestContext managementRequestContext = this.TestManagementRequestContext;
      Guid projectId1 = new Guid(projectId);
      AttachmentTestLogReference attachmentTestLogReference = testLogReference;
      if (service.CreateEmptyBlobForAttachmentTestLog(managementRequestContext, projectId1, attachmentTestLogReference, new Dictionary<string, string>()
      {
        {
          "type",
          testLog.LogReference.Type.ToString()
        },
        {
          "size",
          testLog.Size.ToString()
        }
      }))
        return;
      this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Warning, "TestManagement", "AttachmentHandler", "GetTestAttachments - CreateBlobAsync failed or the Blob already created While creating empty blob to track the log store blob.");
    }

    private List<TestLog> GetTestLogsAtRunLevelFromLogStore(string projectId, int runId)
    {
      List<TestLog> logsFromLogStore1 = this.GetTestLogsFromLogStore(projectId, runId);
      List<TestLog> logsFromLogStore2 = this.GetTestLogsFromLogStore(projectId, runId, logType: TestLogType.CodeCoverage);
      List<TestLog> logsFromLogStore3 = this.GetTestLogsFromLogStore(projectId, runId, logType: TestLogType.TestImpact);
      List<TestLog> levelFromLogStore = logsFromLogStore1 ?? new List<TestLog>();
      if (logsFromLogStore2 != null && logsFromLogStore2.Count > 0)
        levelFromLogStore.AddRange((IEnumerable<TestLog>) logsFromLogStore2);
      if (logsFromLogStore3 != null && logsFromLogStore3.Count > 0)
        levelFromLogStore.AddRange((IEnumerable<TestLog>) logsFromLogStore3);
      return levelFromLogStore;
    }

    private void PublishTelemetryDataForGetAttachmentsFromLogStore(
      int runId,
      int resultId,
      int subResultId,
      int logStoreCount,
      bool newlyCreated,
      bool isCalledFromCoverageAsyncApi = false)
    {
      try
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData();
        cid.Add("TestRunId", (double) runId);
        cid.Add("TestResultId", (double) resultId);
        cid.Add("TestSubResultId", (double) subResultId);
        cid.Add("LogStoreAttachmentsFetched", (double) logStoreCount);
        cid.Add("EmptyTestLogAndTestLogComparison", newlyCreated);
        cid.Add("CoverageAsyncApi", isCalledFromCoverageAsyncApi);
        if (newlyCreated)
          cid.Add("EmptyLogStoreAttachmentsCreated", (double) logStoreCount);
        else
          cid.Add("EmptyLogStoreAttachmentsCreated", 0.0);
        this.TelemetryLogger.PublishData(this.TestManagementRequestContext.RequestContext, "GetTestAttachmentsFromLogStore", cid);
      }
      catch (Exception ex)
      {
        this.TestManagementRequestContext.RequestContext.Trace(1015003, TraceLevel.Error, "TestManagement", "AttachmentHandler", "GetTestAttachments - Failed while publishing telemetry, ExceptionHit: {0}", (object) ex);
      }
    }

    private void PublishTelemetryDataForGetAttachmentFromLogStore(
      TestLogReference testLogReference,
      int attachmentId,
      int attachmentsFound,
      bool isAttachmentFound,
      bool publishAttachmentId = true)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("TestRunId", (double) testLogReference.RunId);
      cid.Add("TestResultId", (double) testLogReference.ResultId);
      cid.Add("TestSubResultId", (double) testLogReference.SubResultId);
      cid.Add("LogStoreAttachmentsFetched", (double) attachmentsFound);
      cid.Add("LogStoreAttachmentDownloaded", isAttachmentFound);
      if (publishAttachmentId)
        cid.Add("AttachmentId", (double) attachmentId);
      this.TelemetryLogger.PublishData(this.TestManagementRequestContext.RequestContext, "GetTestAttachmentFromLogStore", cid);
    }

    private void PublishTelemetryDataForGetAttachmentsFromLogStoreV2(
      int runId,
      int resultId,
      int subResultId,
      int logStoreCount)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("TestRunId", (double) runId);
      cid.Add("TestResultId", (double) resultId);
      cid.Add("TestSubResultId", (double) subResultId);
      cid.Add("LogStoreAttachmentsFetched", (double) logStoreCount);
      this.TelemetryLogger.PublishData(this.TestManagementRequestContext.RequestContext, "GetTestAttachmentsFromLogStore_NewApi", cid);
    }

    private AttachmentType GetAttachmentTypeFromLogType(TestLogType type)
    {
      if (type == TestLogType.CodeCoverage)
        return AttachmentType.CodeCoverage;
      return type == TestLogType.TestImpact ? AttachmentType.TestImpactDetails : AttachmentType.GeneralAttachment;
    }

    private TestAttachment ConvertTestResultAttachmentToDataContract(
      TeamProjectReference projectRef,
      TestResultAttachment attachment)
    {
      string str = string.Empty;
      if (attachment.TestRunId > 0 && attachment.IterationId == 0 && attachment.SessionId == 0)
        str = this.GetAttachmentUrl(projectRef.Name, attachment.TestRunId, attachment.TestResultId, attachment.Id, attachment.SubResultId);
      return new TestAttachment()
      {
        Id = attachment.Id,
        FileName = attachment.FileName,
        Comment = attachment.Comment,
        CreatedDate = attachment.CreationDate,
        AttachmentType = this.GetAttachmentType(attachment.AttachmentType),
        Url = str,
        Size = attachment.Length
      };
    }

    private string GetAttachmentUrl(
      string projectName,
      int runId,
      int resultId,
      int attachmentId,
      int subresultId = 0)
    {
      if (resultId < 1)
      {
        RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestRunAttachments];
        return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
        {
          runId = runId,
          project = projectName,
          attachmentId = attachmentId
        });
      }
      if (subresultId <= 0)
      {
        RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestResultAttachmentsV2];
        return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
        {
          runId = runId,
          project = projectName,
          testCaseResultId = resultId,
          attachmentId = attachmentId
        });
      }
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          "testSubResultId",
          subresultId.ToString()
        }
      };
      RestApiResourceDetails resourceMapping1 = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestResultAttachmentsV2];
      return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping1.ServiceInstanceType, resourceMapping1.Area, resourceMapping1.ResourceId, (object) new
      {
        runId = runId,
        project = projectName,
        testCaseResultId = resultId,
        attachmentId = attachmentId,
        testSubResultId = subresultId
      }, parameters);
    }

    private string GetLogStoreAttachmentUrl(
      string projectName,
      int runId,
      int resultId,
      string filename,
      int subresultId = 0)
    {
      Dictionary<string, string> parameters = new Dictionary<string, string>()
      {
        {
          nameof (filename),
          filename
        }
      };
      if (resultId == 0)
      {
        RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestRunLogStoreAttachments];
        return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
        {
          runId = runId,
          project = projectName
        }, parameters);
      }
      if (subresultId == 0)
      {
        RestApiResourceDetails resourceMapping = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestResultAttachmentsV2];
        return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping.ServiceInstanceType, resourceMapping.Area, resourceMapping.ResourceId, (object) new
        {
          runId = runId,
          project = projectName,
          testCaseResultId = resultId
        }, parameters);
      }
      parameters.Add("testSubResultId", subresultId.ToString());
      RestApiResourceDetails resourceMapping1 = this.TestManagementRequestContext.ResourceMappings[ResourceMappingConstants.TestResultAttachmentsV2];
      return UrlBuildHelper.GetResourceUrl(this.RequestContext, resourceMapping1.ServiceInstanceType, resourceMapping1.Area, resourceMapping1.ResourceId, (object) new
      {
        runId = runId,
        project = projectName,
        testCaseResultId = resultId,
        testSubResultId = subresultId
      }, parameters);
    }

    private AttachmentType GetAttachmentType(string attachmentType)
    {
      AttachmentType result = AttachmentType.GeneralAttachment;
      if (string.IsNullOrEmpty(attachmentType))
        return result;
      Enum.TryParse<AttachmentType>(attachmentType, true, out result);
      return result;
    }

    private void PublishTelemetryData(int resultId, string fileName, int iterationId = 0)
    {
      string extension = Path.GetExtension(fileName);
      string featureName = iterationId != 0 ? "DownloadTestIterationAttachment_REST" : (resultId != 0 ? "DownloadTestResultAttachment_REST" : "DownloadTestRunAttachment_REST");
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("FileExtensionType", extension);
      this.TelemetryLogger.PublishData(this.RequestContext, featureName, cid);
    }

    private void PublishTelemetryDataForGetTestAttachmentFromLogStore(
      string projectId,
      int runId,
      int resultId,
      int subResultId,
      string fileName)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("ProjectId", projectId);
      cid.Add("TestRunId", (double) runId);
      cid.Add("TestResultId", (double) resultId);
      cid.Add("TestSubResultId", (double) subResultId);
      cid.Add("FileName", fileName);
      this.TelemetryLogger.PublishData(this.RequestContext, "DownloadTestAttachmentFromLogStore", cid);
    }

    private void PublishTelemetryDataForDeleteTestAttachment(
      string projectId,
      int runId,
      int resultId,
      int subResultId,
      int attachmentId)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("ProjectId", projectId);
      cid.Add("TestRunId", (double) runId);
      cid.Add("TestResultId", (double) resultId);
      cid.Add("TestSubResultId", (double) subResultId);
      cid.Add("AttachmentId", (double) attachmentId);
      this.TelemetryLogger.PublishData(this.RequestContext, "DeleteTestAttachment", cid);
    }

    private void PublishTelemetryDataForDeleteTestAttachmentFromLogStore(
      string projectId,
      int runId,
      int resultId,
      int subResultId,
      string fileName)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("ProjectId", projectId);
      cid.Add("TestRunId", (double) runId);
      cid.Add("TestResultId", (double) resultId);
      cid.Add("TestSubResultId", (double) subResultId);
      cid.Add("FileName", fileName);
      this.TelemetryLogger.PublishData(this.RequestContext, "DeleteTestAttachmentFromLogStore", cid);
    }

    private void PublishTelemetryDataForCreateAttachment(
      string projectId,
      int runId,
      int resultId,
      int iterationId,
      string fileName,
      string featureName)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData();
      cid.Add("ProjectId", projectId);
      cid.Add("TestRunId", (double) runId);
      cid.Add("TestResultId", (double) resultId);
      cid.Add("Iteration", (double) iterationId);
      cid.Add("FileName", fileName);
      this.TelemetryLogger.PublishData(this.RequestContext, featureName, cid);
    }

    private void SecureTestAttachment(List<TestAttachment> attachments, Guid projectId)
    {
      ISecuredObject securedObject = (ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      };
      foreach (TestManagementBaseSecuredObject attachment in attachments)
        attachment.InitializeSecureObject(securedObject);
      this.RequestContext.CheckPermissionToReadPublicIdentityInfo();
    }

    private void SecureTestLogStoreAttachment(
      List<TestLogStoreAttachment> attachments,
      Guid projectId)
    {
      ISecuredObject securedObject = (ISecuredObject) new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        Project = new ShallowReference()
        {
          Id = projectId.ToString()
        }
      };
      foreach (TestManagementBaseSecuredObject attachment in attachments)
        attachment.InitializeSecureObject(securedObject);
      this.RequestContext.CheckPermissionToReadPublicIdentityInfo();
    }

    public virtual IMergeDataHelper MergeDataHelper
    {
      get
      {
        if (this.m_mergeDataHelper == null)
          this.m_mergeDataHelper = new MergeTcmDataHelper();
        return (IMergeDataHelper) this.m_mergeDataHelper;
      }
    }
  }
}
