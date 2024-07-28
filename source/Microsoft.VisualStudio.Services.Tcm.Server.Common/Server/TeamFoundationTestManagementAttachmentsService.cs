// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TeamFoundationTestManagementAttachmentsService
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Common;
using Microsoft.TeamFoundation.Common.Internal;
using Microsoft.TeamFoundation.Core.WebApi;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Server.Legacy;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.TestManagement.WebApi.Legacy;
using Microsoft.VisualStudio.Services.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  internal class TeamFoundationTestManagementAttachmentsService : 
    TeamFoundationTestManagementService,
    ITeamFoundationTestManagementAttachmentsService,
    IVssFrameworkService
  {
    private const int c_maximumFileSize = 104857600;
    private const string c_trmxExtension = ".trmx";
    private const string c_filesElementName = "Files";
    private const string c_fileElementName = "File";
    private const string c_fileNameAttribute = "name";

    public TeamFoundationTestManagementAttachmentsService()
    {
    }

    public TeamFoundationTestManagementAttachmentsService(
      TestManagementRequestContext requestContext)
      : base(requestContext)
    {
    }

    public int CreateTestAttachment(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int runId,
      int resultId = 0,
      int iterationId = 0,
      string actionPath = null,
      int subResultId = 0)
    {
      return this.ExecuteAction<int>(context.RequestContext, "TeamFoundationTestAttachmentsService.CreateTestAttachment", (Func<int>) (() =>
      {
        this.CreateAttachmentInputValidation(context, attachmentRequestModel, projectRef, runId);
        ArgumentUtility.CheckGreaterThanOrEqualToZero((float) subResultId, nameof (subResultId), "Test Results");
        IVssRegistryService service1 = context.RequestContext.GetService<IVssRegistryService>();
        this.ValidateTestRunParameters(context, projectRef, runId, resultId, iterationId);
        context.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementAttachmentsService.CreateTestAttachment invoked with runId: {0}, resultId: {1}, iterationId: {2}", (object) runId, (object) resultId, (object) iterationId);
        TestResultAttachment resultAttachment = new TestResultAttachment();
        resultAttachment.TestRunId = runId;
        resultAttachment.TestResultId = resultId;
        resultAttachment.SessionId = 0;
        resultAttachment.FileName = attachmentRequestModel.FileName;
        resultAttachment.ActionPath = actionPath;
        resultAttachment.IterationId = iterationId;
        resultAttachment.CreationDate = DateTime.UtcNow;
        resultAttachment.SubResultId = subResultId;
        resultAttachment.AttachmentType = this.GetAttachmentType(attachmentRequestModel.AttachmentType);
        resultAttachment.Comment = attachmentRequestModel.Comment;
        TelemetryLogger.Instance.PublishData(context.RequestContext, nameof (CreateTestAttachment), new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          ["RunId"] = (object) resultAttachment.TestRunId,
          ["ResultId"] = (object) resultAttachment.TestResultId,
          ["SubResultId"] = (object) resultAttachment.SubResultId,
          ["IterationId"] = (object) resultAttachment.IterationId,
          ["SessionId"] = (object) resultAttachment.TestResultId,
          ["FileName"] = (object) resultAttachment.FileName,
          ["AttachmentType"] = (object) resultAttachment.AttachmentType,
          ["projectId"] = (object) projectRef.Id
        }));
        bool coverageChanged = false;
        int[] attachmentMapping;
        if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.LogStoreAttachmentTableForCreateAttachment") && context.RequestContext.ServiceHost.DeploymentServiceHost.IsHosted)
        {
          ITeamFoundationAttachmentService service2 = context.RequestContext.GetService<ITeamFoundationAttachmentService>();
          resultAttachment.Length = this.getStreamSize(attachmentRequestModel.Stream);
          if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.AttachmentsUniformPathEnabled"))
          {
            resultAttachment.FileName = this.UploadAttachmentToLogStoreHelper(context, attachmentRequestModel, projectRef, runId, resultId, subResultId);
            try
            {
              attachmentMapping = service2.CreateAttachmentMapping(context, projectRef, new TestResultAttachment[1]
              {
                resultAttachment
              });
              context.RequestContext.Trace(1015920, TraceLevel.Info, "TestManagement", "AttachmentHandler", "AttachmentId generation via new flow has been triggered for {0} , {1} , {2}", (object) runId, (object) resultId, (object) subResultId);
            }
            catch (Exception ex)
            {
              context.Logger.Error(1015940, string.Format("CreateAttachmentFlow: Write to db failed {0}", (object) ex));
              throw;
            }
            try
            {
              using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
                coverageChanged = managementDatabase.GetAppendAttachmentStatusForUploadedFileInLogStore(context, projectRef.Id, runId, resultId, 0, attachmentMapping[0], true);
            }
            catch (Exception ex)
            {
              context.Logger.Error(1015940, string.Format("CreateAttachmentFlow: Update file upload status in attachment table failed {0}", (object) ex));
              throw;
            }
          }
          else
          {
            try
            {
              attachmentMapping = service2.CreateAttachmentMapping(context, projectRef, new TestResultAttachment[1]
              {
                resultAttachment
              });
              context.RequestContext.Trace(1015920, TraceLevel.Info, "TestManagement", "AttachmentHandler", "AttachmentId generation via new flow has been triggered for {0} , {1} , {2}", (object) runId, (object) resultId, (object) subResultId);
            }
            catch (Exception ex)
            {
              context.Logger.Error(1015940, string.Format("CreateAttachmentFlow: Write to db failed {0}", (object) ex));
              throw;
            }
            this.UploadAttachmentToLogStoreHelper(context, attachmentRequestModel, projectRef, runId, resultId, subResultId, attachmentMapping[0]);
            try
            {
              using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
                coverageChanged = managementDatabase.GetAppendAttachmentStatusForUploadedFileInLogStore(context, projectRef.Id, runId, resultId, 0, attachmentMapping[0], true);
            }
            catch (Exception ex)
            {
              context.Logger.Error(1015940, string.Format("CreateAttachmentFlow: Update file upload status in attachment table failed {0}", (object) ex));
              throw;
            }
          }
        }
        else
        {
          attachmentMapping = TestResultAttachment.Create(context, new TestResultAttachment[1]
          {
            resultAttachment
          }, projectRef.Name, false);
          if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.AllowNegativeAttachmentIds"))
          {
            if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.AttachmentIdLogging") && attachmentMapping.Length != 0)
            {
              int num1 = service1.GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TracePointForThirtyPercentPositiveRange", 0);
              int num2 = service1.GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TracePointForFiftyPercentPositiveRange", 0);
              if (((IEnumerable<int>) attachmentMapping).Any<int>((Func<int, bool>) (x => x > 1503238553)) && num1 == 0)
              {
                context.RequestContext.TraceAlways(1015916, TraceLevel.Info, "TestManagement", "AttachmentHandler", "AttachmentIds have reached 30% of max limit");
                service1.SetValue<int>(context.RequestContext, "/Service/TestManagement/Settings/TracePointForThirtyPercentPositiveRange", 1503238553);
              }
              else if (((IEnumerable<int>) attachmentMapping).Any<int>((Func<int, bool>) (x => x > 1073741823)) && num2 == 0)
              {
                context.RequestContext.TraceAlways(1015918, TraceLevel.Info, "TestManagement", "AttachmentHandler", "AttachmentIds have reached 50% of max limit");
                service1.SetValue<int>(context.RequestContext, "/Service/TestManagement/Settings/TracePointForFiftyPercentPositiveRange", 1073741823);
              }
            }
            if (attachmentMapping.Length < 1 || attachmentMapping[0] < 1)
              throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestAttachmentIdNotObtained));
          }
          if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.AllowNegativeAttachmentIds"))
          {
            if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.AttachmentIdLogging") && attachmentMapping.Length != 0)
            {
              int num = service1.GetValue<int>(context.RequestContext, (RegistryQuery) "/Service/TestManagement/Settings/TracePointForFiftyPercentPositiveRange", 0);
              if (((IEnumerable<int>) attachmentMapping).Any<int>((Func<int, bool>) (x => x > -644245094)) && num == 0)
                context.RequestContext.TraceAlways(1015917, TraceLevel.Info, "TestManagement", "AttachmentHandler", "AttachmentIds have reached 30% of max limit(Negative Range)");
              service1.SetValue<int>(context.RequestContext, "/Service/TestManagement/Settings/TracePointForThirtyPercentNegativeRange", -644245094);
            }
            if (attachmentMapping.Length < 1)
              throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestAttachmentIdNotObtained));
          }
          byte[] buffer;
          try
          {
            buffer = Convert.FromBase64String(attachmentRequestModel.Stream);
          }
          catch (Exception ex)
          {
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.StreamNotObtained), ex);
          }
          using (Stream stream = (Stream) new MemoryStream(buffer, 0, buffer.Length, false))
          {
            if (stream == null)
              throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentContent));
            stream.Seek(0L, SeekOrigin.Begin);
            context.RequestContext.UpdateTimeToFirstPage();
            byte[] md5 = MD5Util.CalculateMD5(stream, true);
            coverageChanged = this.SaveFileData(context, projectRef.Id, runId, 0, resultId, attachmentMapping[0], stream.Length, stream.Length, md5, CompressionType.None, 0L, stream.Length, 0, attachmentRequestModel.FileName, stream);
          }
        }
        AttachmentUploadHelper uploadHelperInstance = this.GetAttachmentUploadHelperInstance();
        uploadHelperInstance?.WakeUpCoverAnJobIfRequired(context, coverageChanged);
        uploadHelperInstance?.WakeUpNewCoverageJobIfRequired(context, projectRef, runId, coverageChanged, attachmentRequestModel.AttachmentType);
        return attachmentMapping[0];
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    private string UploadAttachmentToLogStoreHelper(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int runId,
      int resultId = 0,
      int subResultId = 0,
      int attachmentId = 0)
    {
      TestLogType testLogType = AttachmentsHelper.GetTestLogType(this.GetAttachmentType(attachmentRequestModel.AttachmentType));
      if (resultId == 0)
        return this.UploadAttachmentToLogStoreThroughCreateAttachment(context, attachmentRequestModel.Stream, projectRef, runId, attachmentRequestModel.FileName, TestAttachmentType.TestRun, attachmentId: attachmentId, testLogType: testLogType);
      return subResultId == 0 ? this.UploadAttachmentToLogStoreThroughCreateAttachment(context, attachmentRequestModel.Stream, projectRef, runId, attachmentRequestModel.FileName, TestAttachmentType.TestResult, resultId, attachmentId: attachmentId, testLogType: testLogType) : this.UploadAttachmentToLogStoreThroughCreateAttachment(context, attachmentRequestModel.Stream, projectRef, runId, attachmentRequestModel.FileName, TestAttachmentType.TestSubResult, resultId, subResultId, attachmentId, testLogType);
    }

    public bool CreateBuildAttachmentInLogStore(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int buildId)
    {
      return this.ExecuteAction<bool>(context.RequestContext, "TeamFoundationTestAttachmentsService.CreateBuildAttachmentInLogStore", (Func<bool>) (() =>
      {
        Dictionary<string, object> data = new Dictionary<string, object>()
        {
          {
            "ResultMessage",
            (object) ""
          }
        };
        data[nameof (buildId)] = (object) buildId;
        data["FileName"] = (object) attachmentRequestModel.FileName;
        data["AttachmentType"] = (object) attachmentRequestModel.AttachmentType;
        data["projectId"] = (object) projectRef.Id;
        Microsoft.TeamFoundation.Build.WebApi.Build build;
        if (!LogStoreHelper.TryGetBuild(context, buildId, projectRef.Id, out build))
        {
          context.RequestContext.Trace(1015160, TraceLevel.Error, "TestManagement", "LogStorage", "BuildId is not present");
          data["ResultMessage"] = (object) "buildIdNotPresent";
          TelemetryLogger.Instance.PublishData(context.RequestContext, "UploadBuildAttachmentsToLogStore", new CustomerIntelligenceData((IDictionary<string, object>) data));
          throw new TestObjectNotFoundException(context.RequestContext, buildId, ObjectTypes.Build);
        }
        ITestLogStoreService service = context.RequestContext.GetService<ITestLogStoreService>();
        string fileName = attachmentRequestModel.FileName;
        TestLogType testLogType = !string.Equals(attachmentRequestModel.AttachmentType, "CodeCoverage", StringComparison.OrdinalIgnoreCase) ? TestLogType.Intermediate : TestLogType.CodeCoverage;
        TestLogReference attachmentTestLogReference = new TestLogReference()
        {
          BuildId = buildId,
          FilePath = fileName,
          Type = testLogType,
          Scope = TestLogScope.Build
        };
        bool attachmentInLogStore = false;
        try
        {
          byte[] buffer = Convert.FromBase64String(attachmentRequestModel.Stream);
          using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length, false))
          {
            attachmentInLogStore = service.CreateBlob(context, projectRef.Id, attachmentTestLogReference, (Stream) memoryStream, (Dictionary<string, string>) null);
            data["ResultMessage"] = !attachmentInLogStore ? (object) "UploadAttachmentFailed" : (object) "UploadAttachmentSucceeded";
          }
        }
        catch (Exception ex)
        {
          context.RequestContext.Trace(1015920, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlob - CreateBlob failed, ExceptionHit: {0} ", (object) ex.Message);
          data["ResultMessage"] = (object) "UploadAttachmentFailed";
        }
        finally
        {
          TelemetryLogger.Instance.PublishData(context.RequestContext, "UploadBuildAttachmentsToLogStore", new CustomerIntelligenceData((IDictionary<string, object>) data));
        }
        if (!attachmentInLogStore)
        {
          context.RequestContext.Trace(1015112, TraceLevel.Error, "TestManagement", nameof (TeamFoundationTestManagementAttachmentsService), "TeamFoundationTestManagementAttachmentsService.CreateBuildAttachmentInLogStore failed for builId: {0} and FileName: {1}", (object) build, (object) attachmentRequestModel.FileName);
          throw new Exception("CreateBlob returned false for uploading the attachment");
        }
        return attachmentInLogStore;
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public bool CreateTestAttachmentInLogStore(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int runId,
      int resultId = 0,
      int iterationId = 0,
      string actionPath = null,
      int subResultId = 0)
    {
      return this.ExecuteAction<bool>(context.RequestContext, "TeamFoundationTestAttachmentsService.CreateTestAttachmentInLogStore", (Func<bool>) (() =>
      {
        this.CreateAttachmentInputValidation(context, attachmentRequestModel, projectRef, runId);
        ArgumentUtility.CheckGreaterThanOrEqualToZero((float) subResultId, nameof (subResultId), "Test Results");
        context.RequestContext.GetService<IVssRegistryService>();
        this.ValidateTestRunParameters(context, projectRef, runId, resultId, iterationId);
        context.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementAttachmentsService.CreateTestAttachmentInLogStore invoked with runId: {0}, resultId: {1}, iterationId: {2}", (object) runId, (object) resultId, (object) iterationId);
        return resultId != 0 ? (subResultId != 0 ? this.UploadAttachmentToLogStoreThroughCreateAttachmentV2(context, attachmentRequestModel.Stream, projectRef, runId, attachmentRequestModel.FileName, TestAttachmentType.TestSubResult, TestLogType.GeneralAttachment) : this.UploadAttachmentToLogStoreThroughCreateAttachmentV2(context, attachmentRequestModel.Stream, projectRef, runId, attachmentRequestModel.FileName, TestAttachmentType.TestResult, TestLogType.GeneralAttachment)) : this.UploadAttachmentToLogStoreThroughCreateAttachmentV2(context, attachmentRequestModel.Stream, projectRef, runId, attachmentRequestModel.FileName, TestAttachmentType.TestRun, TestLogType.GeneralAttachment);
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public bool UploadAttachmentToLogStore(
      TestManagementRequestContext context,
      HttpPostedTcmAttachment attachment,
      Guid projectId,
      int runId,
      int resultId,
      int sessionId,
      int attachmentId,
      long uncompressedLength)
    {
      ArgumentUtility.CheckForNull<HttpPostedTcmAttachment>(attachment, nameof (attachment), "Test Results");
      TeamProjectReference projectReference = this.GetProjectReference(context.RequestContext, projectId.ToString());
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectReference, "projectRef", "Test Results");
      context.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementAttachmentsService.UploadAttachmentToLogStore invoked with runId: {0}, resultId: {1}, attachmentId: {2}, projectId: {3}", (object) runId, (object) resultId, (object) attachmentId, (object) projectId);
      TestResultAttachment resultAttachment = new TestResultAttachment()
      {
        TestRunId = runId,
        TestResultId = resultId,
        SessionId = sessionId,
        FileName = attachment.FileName,
        CreationDate = DateTime.UtcNow
      };
      resultAttachment.FileName = this.UploadAttachmentToLogStoreThroughCreateAttachment(context, attachment.AttachmentContent, projectReference, runId, attachment.FileName, TestAttachmentType.TestResult, resultId);
      try
      {
        using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
          managementDatabase.UpdateAttachmentInLogStoreAttachmentMapper(projectId, runId, resultId, attachmentId, uncompressedLength, resultAttachment.FileName);
      }
      catch (Exception ex)
      {
        context.RequestContext.TraceError("BusinessLayer", "UpdateAttachmentInLogStoreAttachmentMapper operation failed for ProjectName {0} with RunId {1}, ResultId {2}, AttachmentId {3}, Exception Message: {4}", (object) projectReference.Id, (object) runId, (object) resultId, (object) attachmentId, (object) ex.Message);
        throw;
      }
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(context))
        return managementDatabase.GetAppendAttachmentStatusForUploadedFileV2(context, projectReference.Id, runId, resultId, 0, attachmentId, true);
    }

    public bool DeleteTestAttachmentFromLogStore(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int runId,
      string filename,
      TestLogType testLogType = TestLogType.GeneralAttachment,
      int resultId = 0,
      int iterationId = 0,
      int subResultId = 0)
    {
      return this.ExecuteAction<bool>(context.RequestContext, "TeamFoundationTestAttachmentsService.DeleteTestAttachmentFromLogStore", (Func<bool>) (() =>
      {
        ArgumentUtility.CheckGreaterThanOrEqualToZero((float) subResultId, nameof (subResultId), "Test Results");
        context.RequestContext.GetService<IVssRegistryService>();
        this.ValidateTestRunParameters(context, projectRef, runId, resultId, iterationId);
        context.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementAttachmentsService.DeleteTestAttachmentFromLogStore invoked with runId: {0}, resultId: {1}, iterationId: {2}, filename: {3}", (object) runId, (object) resultId, (object) iterationId, (object) filename);
        string str = AttachmentsHelper.GetFilePathForAttachmentType(resultId == 0 ? TestAttachmentType.TestRun : (subResultId == 0 ? TestAttachmentType.TestResult : TestAttachmentType.TestSubResult)) + filename;
        return context.RequestContext.GetService<ITestLogStoreService>().DeleteBlob(context, projectRef.Id, new TestLogReference()
        {
          RunId = runId,
          ResultId = resultId,
          SubResultId = subResultId,
          Scope = TestLogScope.Run,
          FilePath = str,
          Type = testLogType
        });
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public long[] CreateTestSubResultJsonAttachment(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int runId,
      int[] resultIds,
      bool shouldUploadToLogStore = false)
    {
      return this.ExecuteAction<long[]>(context.RequestContext, "TeamFoundationTestAttachmentsService.CreateTestSubResultJsonAttachment", (Func<long[]>) (() =>
      {
        this.CreateAttachmentInputValidation(context, attachmentRequestModel, projectRef, runId, false);
        ArgumentUtility.CheckForNull<int[]>(resultIds, nameof (resultIds), "Test Results");
        ArgumentUtility.CheckGreaterThanZero((float) ((IEnumerable<int>) resultIds).Count<int>(), "resultIdCounts", "Test Results");
        ((IEnumerable<int>) resultIds).ForEach<int>((Action<int>) (r => ArgumentUtility.CheckGreaterThanZero((float) r, "resultId", "Test Results")));
        context.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementAttachmentsService.CreateTestSubResultJsonAttachment invoked with runId: {0}", (object) runId);
        this.ValidateTestResultsParameters(context, projectRef, runId, resultIds);
        int num = 0;
        long streamLength = 0;
        string str = attachmentRequestModel.FileName;
        Dictionary<string, object> dictionary = new Dictionary<string, object>()
        {
          {
            "RunId",
            (object) runId.ToString()
          },
          {
            "isInLogStore",
            (object) shouldUploadToLogStore.ToString()
          },
          {
            "Size in bytes",
            (object) attachmentRequestModel.Stream?.Length.ToString()
          },
          {
            "AzureStorageRequestId",
            (object) context.RequestContext.ActivityId
          }
        };
        try
        {
          using (new SimpleTimer(context.RequestContext, string.Format("Upload SubResult Attachment for RunId: {0}", (object) runId), dictionary, 200L, alwaysPublishTelemetry: true))
          {
            if (shouldUploadToLogStore)
              str = this.UploadAttachmentToLogStore(context, attachmentRequestModel.Stream, projectRef, runId);
            else
              num = this.UploadAttachment(context, attachmentRequestModel, projectRef, out streamLength);
          }
        }
        finally
        {
          CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) dictionary);
          TelemetryLogger.Instance.PublishData(context.RequestContext, "UploadAttachment", cid);
        }
        List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
        DateTime utcNow = DateTime.UtcNow;
        foreach (int resultId in resultIds)
          resultAttachmentList.Add(new TestResultAttachment()
          {
            TestRunId = runId,
            TestResultId = resultId,
            SessionId = 0,
            FileName = str,
            ActionPath = (string) null,
            IterationId = 0,
            CreationDate = utcNow,
            SubResultId = 0,
            AttachmentType = TestResultsConstants.TestSubResultJsonAttachmentType,
            Comment = attachmentRequestModel.Comment,
            FileId = num,
            IsComplete = true,
            Length = streamLength
          });
        if (this.ShouldUploadToLogStoreTable(context, runId, shouldUploadToLogStore))
        {
          context.RequestContext.Trace(1015003, TraceLevel.Info, "TestManagement", "BusinessLayer", "Storing in logstore table runId = {0} and shouldUploadToLogStore = {1}", (object) runId, (object) shouldUploadToLogStore);
          return context.RequestContext.GetService<ITeamFoundationLogStoreAttachmentService>().CreateLogStoreAttachmentMapping(context, projectRef, resultAttachmentList.ToArray());
        }
        int[] source = TestResultAttachment.Create(context, resultAttachmentList.ToArray(), projectRef.Name, false, isAttachmentUploadComplete: true);
        if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.AllowNegativeAttachmentIds"))
        {
          if (source == null || source.Length < resultAttachmentList.Count || ((IEnumerable<int>) source).Where<int>((Func<int, bool>) (a => a < 1)).Any<int>())
            throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestAttachmentIdNotObtained));
        }
        else if (source == null || source.Length < resultAttachmentList.Count)
          throw new Exception(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestAttachmentIdNotObtained));
        return ((IEnumerable<int>) source).Select<int, long>((Func<int, long>) (item => (long) item)).ToArray<long>();
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public Stream GetTestIterationAttachment(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int testRunId,
      int testResultId,
      int attachmentId,
      int iterationId,
      out string attachmentFileName,
      out CompressionType attachmentCompressionType)
    {
      string fileName = string.Empty;
      CompressionType compressionType = CompressionType.None;
      Stream iterationAttachment = this.ExecuteAction<Stream>(context.RequestContext, "TeamFoundationTestAttachmentsService.GetTestIterationAttachment", (Func<Stream>) (() =>
      {
        List<TestResultAttachment> resultAttachmentList = TestResultAttachment.Query(context, testRunId, testResultId, 0, projectRef.Name, attachmentId: attachmentId);
        if (resultAttachmentList.IsNullOrEmpty<TestResultAttachment>())
          resultAttachmentList = context.RequestContext.GetService<ITeamFoundationAttachmentService>().GetIterationAttachmentsFromDb(context, projectRef, testRunId, testResultId, attachmentId, iterationId);
        if (resultAttachmentList == null || !resultAttachmentList.Any<TestResultAttachment>())
          return (Stream) null;
        context.RequestContext.UpdateTimeToFirstPage();
        fileName = resultAttachmentList[0].FileName;
        if (resultAttachmentList[0].FileId != 0 && resultAttachmentList[0].FileId != -1)
          return TestResultAttachment.GetAttachmentStream(context, resultAttachmentList[0].FileId, out compressionType);
        context.RequestContext.Trace(1015921, TraceLevel.Info, "TestManagement", "AttachmentHandler", "Converted the attachments to stream for RunId: {0} , ResultId: {1} , IterationId: {2}, attachmentId: {3}", (object) testRunId, (object) testResultId, (object) iterationId, (object) attachmentId);
        return this.GetAttachmentStreamHelper(context, projectRef, testRunId, testResultId, 0, attachmentId, fileName, resultAttachmentList[0].AttachmentType);
      }), 1015110, "TestManagement", "AttachmentHandler");
      attachmentFileName = fileName;
      attachmentCompressionType = compressionType;
      return iterationAttachment;
    }

    public Stream GetTestAttachment(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int testRunId,
      int testResultId,
      int attachmentId,
      out string attachmentFileName,
      out CompressionType attachmentCompressionType,
      int subResultId = 0)
    {
      string fileName = string.Empty;
      CompressionType compressionType = CompressionType.None;
      Stream testAttachment = this.ExecuteAction<Stream>(context.RequestContext, "TeamFoundationTestAttachmentsService.GetTestAttachment", (Func<Stream>) (() =>
      {
        List<TestResultAttachment> testAttachments = this.GetTestAttachments(context, projectRef, testRunId, testResultId, subResultId, attachmentId);
        if (testAttachments == null || !testAttachments.Any<TestResultAttachment>())
          return (Stream) null;
        context.RequestContext.UpdateTimeToFirstPage();
        fileName = testAttachments[0].FileName;
        return testAttachments[0].FileId != 0 && testAttachments[0].FileId != -1 ? TestResultAttachment.GetAttachmentStream(context, testAttachments[0].FileId, out compressionType) : this.GetAttachmentStreamHelper(context, projectRef, testRunId, testResultId, subResultId, attachmentId, fileName, testAttachments[0].AttachmentType);
      }), 1015110, "TestManagement", "AttachmentHandler");
      attachmentFileName = fileName;
      attachmentCompressionType = compressionType;
      return testAttachment;
    }

    public List<TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int runId,
      int resultId,
      int subResultId = 0,
      int attachmentId = 0)
    {
      return context.RequestContext.IsFeatureEnabled("TestManagement.Server.GetTestAttachmentsIndependentOfCreateLogStoreAttachmentFF") ? this.ExecuteAction<List<TestResultAttachment>>(context.RequestContext, "TeamFoundationTestAttachmentsService.GetTestAttachments", (Func<List<TestResultAttachment>>) (() =>
      {
        ITeamFoundationAttachmentService service = context.RequestContext.GetService<ITeamFoundationAttachmentService>();
        context.RequestContext.Trace(1015921, TraceLevel.Info, "TestManagement", "AttachmentHandler", "Get Attachments via new flow has been triggered for {0} , {1} , {2}", (object) runId, (object) resultId, (object) subResultId);
        TestManagementRequestContext context1 = context;
        TeamProjectReference projectRef1 = projectRef;
        int testRunId = runId;
        int testResultId = resultId;
        int attachmentId1 = attachmentId;
        int subResultId1 = subResultId;
        List<TestResultAttachment> attachmentsFromDb = service.GetAttachmentsFromDb(context1, projectRef1, testRunId, testResultId, attachmentId1, subResultId1, true);
        if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.UseIterationIdInLogStoreAttachmentMapper"))
        {
          foreach (TestResultAttachment resultAttachment in TestResultAttachment.Query(context, runId, resultId, 0, projectRef.Name, subResultId, attachmentId))
          {
            TestResultAttachment attachment = resultAttachment;
            if (!attachmentsFromDb.Any<TestResultAttachment>((Func<TestResultAttachment, bool>) (x => x.Id == attachment.Id && x.TestRunId == attachment.TestRunId)))
              attachmentsFromDb.Add(attachment);
          }
        }
        this.PublishTelemetryForGetTestAttachments(context, projectRef, attachmentsFromDb, runId, resultId, attachmentId, subResultId);
        return attachmentsFromDb;
      }), 1015110, "TestManagement", "AttachmentHandler") : this.ExecuteAction<List<TestResultAttachment>>(context.RequestContext, "TeamFoundationTestAttachmentsService.GetTestAttachments", (Func<List<TestResultAttachment>>) (() =>
      {
        if (context.IsFeatureEnabled("TestManagement.Server.LogStoreAttachmentTableForCreateAttachment") && context.RequestContext.ServiceHost.DeploymentServiceHost.IsHosted)
        {
          ITeamFoundationAttachmentService service = context.RequestContext.GetService<ITeamFoundationAttachmentService>();
          context.RequestContext.Trace(1015921, TraceLevel.Info, "TestManagement", "AttachmentHandler", "Get Attachments via new flow has been triggered for {0} , {1} , {2}", (object) runId, (object) resultId, (object) subResultId);
          TestManagementRequestContext context2 = context;
          TeamProjectReference projectRef2 = projectRef;
          int testRunId = runId;
          int testResultId = resultId;
          int attachmentId2 = attachmentId;
          int subResultId2 = subResultId;
          List<TestResultAttachment> attachmentsFromDb = service.GetAttachmentsFromDb(context2, projectRef2, testRunId, testResultId, attachmentId2, subResultId2, true);
          this.PublishTelemetryForGetTestAttachments(context, projectRef, attachmentsFromDb, runId, resultId, attachmentId, subResultId);
          return attachmentsFromDb;
        }
        List<TestResultAttachment> testAttachments = TestResultAttachment.Query(context, runId, resultId, 0, projectRef.Name, subResultId, attachmentId);
        if (testAttachments.IsNullOrEmpty<TestResultAttachment>())
        {
          ITeamFoundationAttachmentService service = context.RequestContext.GetService<ITeamFoundationAttachmentService>();
          context.RequestContext.Trace(1015921, TraceLevel.Info, "TestManagement", "AttachmentHandler", "Get Attachments via new flow has been triggered for {0} , {1} , {2}", (object) runId, (object) resultId, (object) subResultId);
          TestManagementRequestContext context3 = context;
          TeamProjectReference projectRef3 = projectRef;
          int testRunId = runId;
          int testResultId = resultId;
          int attachmentId3 = attachmentId;
          int subResultId3 = subResultId;
          List<TestResultAttachment> attachmentsFromDb = service.GetAttachmentsFromDb(context3, projectRef3, testRunId, testResultId, attachmentId3, subResultId3);
          this.PublishTelemetryForGetTestAttachments(context, projectRef, attachmentsFromDb, runId, resultId, attachmentId, subResultId);
          return attachmentsFromDb;
        }
        this.PublishTelemetryForGetTestAttachments(context, projectRef, testAttachments, runId, resultId, attachmentId, subResultId);
        return testAttachments;
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int runId,
      int resultId,
      int sessionId,
      int subResultId,
      int attachmentId)
    {
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>(context.RequestContext, "TeamFoundationTestAttachmentsService.GetTestAttachments", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>) (() =>
      {
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source = AttachmentContractConverter.Convert((IEnumerable<TestResultAttachment>) TestResultAttachment.Query(context, runId, resultId, 0, projectId, subResultId, attachmentId));
        return source == null ? (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null : source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>();
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      string projectName,
      int runId,
      int resultId,
      int sessionId,
      int subResultId,
      int attachmentId)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>(context.RequestContext, "TeamFoundationTestAttachmentsService.GetTestAttachments", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>) (() =>
      {
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source = AttachmentContractConverter.Convert((IEnumerable<TestResultAttachment>) TestResultAttachment.Query(context, runId, resultId, 0, projectName, subResultId, attachmentId));
        return source == null ? (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null : source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>();
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int attachmentId,
      bool getSiblingAttachments)
    {
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>(context.RequestContext, "TeamFoundationTestAttachmentsService.GetTestAttachments", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>) (() =>
      {
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source = AttachmentContractConverter.Convert((IEnumerable<TestResultAttachment>) TestResultAttachment.Query(context, projectId, attachmentId, getSiblingAttachments));
        return source == null ? (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null : source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>();
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachments(
      TestManagementRequestContext context,
      string projectName,
      int attachmentId,
      bool getSiblingAttachments)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>(context.RequestContext, "TeamFoundationTestAttachmentsService.GetTestAttachments", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>) (() =>
      {
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source = AttachmentContractConverter.Convert((IEnumerable<TestResultAttachment>) TestResultAttachment.Query(context, projectName, attachmentId, getSiblingAttachments));
        return source == null ? (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null : source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>();
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> GetTestAttachmentsByQuery(
      TestManagementRequestContext context,
      Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery query)
    {
      ArgumentUtility.CheckForNull<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.ResultsStoreQuery>(query, nameof (query), "Test Results");
      return this.ExecuteAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>(context.RequestContext, "TeamFoundationTestAttachmentsService.GetTestAttachmentsByQuery", (Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>>) (() =>
      {
        IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment> source = AttachmentContractConverter.Convert((IEnumerable<TestResultAttachment>) TestResultAttachment.Query2(context, ResultsStoreQueryContractConverter.Convert(query)));
        return source == null ? (List<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>) null : source.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.Legacy.TestResultAttachment>();
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    public List<bool> CheckIfActionRecordingExists(
      TestManagementRequestContext context,
      Guid projectId,
      IList<int> testCaseIds)
    {
      return this.ExecuteAction<List<bool>>(context.RequestContext, "TeamFoundationTestAttachmentsService.CheckIfActionRecordingExists", (Func<List<bool>>) (() => TestResultAttachment.CheckActionRecordingExists(context, testCaseIds.ToArray<int>(), projectId)), 1015110, "TestManagement", "AttachmentHandler");
    }

    public List<bool> CheckIfActionRecordingExists(
      TestManagementRequestContext context,
      string projectName,
      IList<int> testCaseIds)
    {
      ArgumentUtility.CheckForNull<string>(projectName, nameof (projectName), "Test Results");
      return this.ExecuteAction<List<bool>>(context.RequestContext, "TeamFoundationTestAttachmentsService.CheckIfActionRecordingExists", (Func<List<bool>>) (() => TestResultAttachment.CheckActionRecordingExists(context, testCaseIds.ToArray<int>(), projectName)), 1015110, "TestManagement", "AttachmentHandler");
    }

    public Stream GetAttachmentStream(
      TestManagementRequestContext context,
      string projectName,
      int attachmentId,
      out string attachmentName)
    {
      attachmentName = string.Empty;
      CompressionType compressionType = CompressionType.None;
      GuidAndString projectFromName = Validator.CheckAndGetProjectFromName(context, projectName);
      if (!context.SecurityManager.HasViewTestResultsPermission(context, projectFromName.String))
        return (Stream) null;
      context.RequestContext.UpdateTimeToFirstPage();
      Stream attachmentStream = TestResultAttachment.GetAttachmentStream(context, projectFromName.GuidId, attachmentId, out attachmentName, out compressionType);
      attachmentName = TeamFoundationTestManagementAttachmentsService.EscapeCharacters(attachmentName);
      if (string.Equals(Path.GetExtension(attachmentName), ".trmx", StringComparison.OrdinalIgnoreCase))
      {
        string videoFileName = this.GetVideoFileName(attachmentStream);
        if (string.IsNullOrEmpty(videoFileName))
          return attachmentStream;
        using (attachmentStream)
        {
          List<TestResultAttachment> attachments = this.GetAttachments(context, projectFromName.GuidId, attachmentId);
          if (attachments != null)
          {
            if (attachments.Any<TestResultAttachment>())
            {
              List<TestResultAttachment> list = attachments.Where<TestResultAttachment>((Func<TestResultAttachment, bool>) (a => string.Equals(a.FileName, videoFileName, StringComparison.OrdinalIgnoreCase))).ToList<TestResultAttachment>();
              if (list != null)
              {
                if (list.Any<TestResultAttachment>())
                  return TestResultAttachment.GetAttachmentStream(context, projectFromName.GuidId, list.First<TestResultAttachment>().Id, out attachmentName, out compressionType);
              }
            }
          }
        }
      }
      return attachmentStream;
    }

    public void DeleteTestAttachment(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int runId,
      int resultId,
      int attachmentId)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, nameof (projectRef), "Test Results");
      TestResultAttachmentIdentity attachmentIdentity = new TestResultAttachmentIdentity();
      attachmentIdentity.AttachmentId = attachmentId;
      attachmentIdentity.TestRunId = runId;
      attachmentIdentity.TestResultId = resultId;
      context.RequestContext.Trace(1015851, TraceLevel.Info, "TestManagement", "DeleteAttachment", "TeamFoundationTestManagementAttachmentsService.DeleteTestAttachment projectId = {0}, runId = {1}, resultId: {2}, attachmentId: {3}", (object) projectRef.Id, (object) runId, (object) resultId, (object) attachmentId);
      if (TestResultAttachment.Delete(context, new TestResultAttachmentIdentity[1]
      {
        attachmentIdentity
      }, projectRef.Name))
      {
        this.PublishTelemetryForDeleteAttachment(context, attachmentIdentity, projectRef.Name);
      }
      else
      {
        if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.AllowDeleteAttachmentFromLogStore") || !context.RequestContext.ServiceHost.DeploymentServiceHost.IsHosted || !this.TryDeleteTestAttachmentFromLogStore(context, projectRef, attachmentIdentity))
          return;
        this.PublishTelemetryForDeleteAttachmentFromLogStoreMapper(context, attachmentIdentity, projectRef.Name);
      }
    }

    public List<TestResultAttachment> CreateAttachmentIdMappingsForLogStore(
      TestManagementRequestContext context,
      string projectId,
      List<TestResultAttachment> attachments)
    {
      return TestResultAttachment.CreateAttachmentIdMappingsForLogStore(context, projectId, attachments);
    }

    public TestResultAttachment GetAttachmentIdMappingForLogStore(
      TestManagementRequestContext context,
      string projectId,
      TestResultAttachment attachment)
    {
      return TestResultAttachment.GetAttachmentIdMappingForLogStore(context, projectId, attachment);
    }

    internal void ValidateAttachmentSize(
      TestAttachmentRequestModel attachmentRequestModel,
      bool isBase64Encoded = true)
    {
      int num = isBase64Encoded ? 139810133 : 104857600;
      if (attachmentRequestModel.Stream.Length > num)
        throw new TestManagementValidationException(string.Format(ServerResources.FileUploadExceedMaxSize, (object) num));
    }

    protected virtual AttachmentUploadHelper GetAttachmentUploadHelperInstance() => new AttachmentUploadHelper();

    protected virtual bool SaveFileData(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int sessionId,
      int testResultId,
      int attachmentId,
      long uncompressedLength,
      long streamLength,
      byte[] hashValue,
      CompressionType compressionType,
      long offsetFrom,
      long offsetTo,
      int defaultAfnStripFlag,
      string fileName,
      Stream fileStream)
    {
      context.TraceVerbose("RestLayer", "TeamFoundationTestManagementAttachmentsService.SaveFileData - {0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}", (object) testResultId, (object) attachmentId, (object) fileName, (object) defaultAfnStripFlag, (object) uncompressedLength, (object) streamLength, (object) offsetFrom, (object) offsetTo);
      context.TraceVerbose("AttachmentHandler", "Compression Type: {0}", (object) compressionType);
      bool flag1 = false;
      bool flag2 = true;
      using (Stream stream = fileStream)
      {
        using (ByteArray byteArray = new ByteArray((int) Math.Min(stream.Length, 1048576L)))
        {
          if (!context.RequestContext.IsFeatureEnabled("TestManagement.Server.AllowNegativeAttachmentIds"))
            context.TraceAndDebugAssert("AttachmentHandler", attachmentId > 0, "attachmentId cannot be < 0");
          context.TraceAndDebugAssert("AttachmentHandler", offsetTo - offsetFrom == stream.Length, "attachment size mismatch");
          do
          {
            int count = (int) Math.Min(stream.Length - stream.Position, 1048576L);
            int contentLength = stream.Read(byteArray.Bytes, 0, count);
            flag1 |= TestResultAttachment.AppendAttachment(context, projectId, testRunId, testResultId, attachmentId, sessionId, uncompressedLength, streamLength, hashValue, compressionType, offsetFrom, byteArray.Bytes, contentLength, defaultAfnStripFlag);
            offsetFrom += (long) contentLength;
            if (flag2)
            {
              context.RequestContext.UpdateTimeToFirstPage();
              flag2 = false;
            }
          }
          while (stream.Position < stream.Length);
        }
      }
      return flag1;
    }

    private Stream GetAttachmentStreamHelper(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int testRunId,
      int testResultId,
      int subResultId,
      int attachmentId,
      string fileName,
      string attachmentType = "generalAttachment")
    {
      Stream logStoreContentStream = (Stream) new MemoryStream();
      TestLogReference testLogReference1 = new TestLogReference();
      TestLogReference testLogReference2 = new TestLogReference();
      bool flag = context.RequestContext.IsFeatureEnabled("TestManagement.Server.AttachmentsUniformPathEnabled");
      TestLogReference testLogReferenceDefault;
      TestLogReference testLogReferenceFallback;
      if (testResultId == 0)
      {
        if (flag)
        {
          testLogReferenceDefault = this.GetTestLogReferenceToDownloadAttachmentWithUniformPath(testRunId, testResultId, subResultId, attachmentType, fileName);
          testLogReferenceFallback = this.GetTestLogReferenceToDownloadAttachment(testRunId, TestAttachmentType.TestRun, true, fileName, attachmentId: attachmentId);
        }
        else
        {
          testLogReferenceDefault = this.GetTestLogReferenceToDownloadAttachment(testRunId, TestAttachmentType.TestRun, true, fileName, attachmentId: attachmentId);
          testLogReferenceFallback = this.GetTestLogReferenceToDownloadAttachmentWithUniformPath(testRunId, testResultId, subResultId, attachmentType, fileName);
        }
      }
      else if (subResultId == 0)
      {
        if (flag)
        {
          testLogReferenceDefault = this.GetTestLogReferenceToDownloadAttachmentWithUniformPath(testRunId, testResultId, subResultId, attachmentType, fileName);
          testLogReferenceFallback = this.GetTestLogReferenceToDownloadAttachment(testRunId, TestAttachmentType.TestResult, true, fileName, testResultId, attachmentId);
        }
        else
        {
          testLogReferenceDefault = this.GetTestLogReferenceToDownloadAttachment(testRunId, TestAttachmentType.TestResult, true, fileName, testResultId, attachmentId);
          testLogReferenceFallback = this.GetTestLogReferenceToDownloadAttachmentWithUniformPath(testRunId, testResultId, subResultId, attachmentType, fileName);
        }
      }
      else if (flag)
      {
        testLogReferenceDefault = this.GetTestLogReferenceToDownloadAttachmentWithUniformPath(testRunId, testResultId, subResultId, attachmentType, fileName);
        testLogReferenceFallback = this.GetTestLogReferenceToDownloadAttachment(testRunId, TestAttachmentType.TestSubResult, true, fileName, attachmentId: attachmentId);
      }
      else
      {
        testLogReferenceDefault = this.GetTestLogReferenceToDownloadAttachment(testRunId, TestAttachmentType.TestSubResult, true, fileName, attachmentId: attachmentId);
        testLogReferenceFallback = this.GetTestLogReferenceToDownloadAttachmentWithUniformPath(testRunId, testResultId, subResultId, attachmentType, fileName);
      }
      this.DownloadStreamHelper(context, projectRef, testLogReferenceDefault, testLogReferenceFallback, logStoreContentStream, attachmentId);
      context.RequestContext.Trace(1015921, TraceLevel.Info, "TestManagement", "AttachmentHandler", "Converted the attachments to stream for RunId: {0} , ResultId: {1} , SubResultId: {2}, attachmentId: {3}", (object) testRunId, (object) testResultId, (object) subResultId, (object) attachmentId);
      logStoreContentStream.Seek(0L, SeekOrigin.Begin);
      return logStoreContentStream;
    }

    private void DownloadStreamHelper(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      TestLogReference testLogReferenceDefault,
      TestLogReference testLogReferenceFallback,
      Stream logStoreContentStream,
      int attachmentId)
    {
      ITestLogStoreService service = context.RequestContext.GetService<ITestLogStoreService>();
      ProjectInfo projectFromGuid = context.ProjectServiceHelper.GetProjectFromGuid(projectRef.Id);
      service.DownloadToStream(context, projectFromGuid, testLogReferenceDefault, logStoreContentStream);
      if (logStoreContentStream.Length != 0L)
        return;
      context.RequestContext.TraceAlways(1015941, TraceLevel.Warning, "TestManagement", "DownloadAttachment", "TeamFoundationTestManagementAttachmentsService.GetAttachment: Download attachment failed with file path change enabled. ProjectName = {0}, TestRunId = {1}, TestResultId = {2}, TestSubResultId = {3}, AttachmentId = {4}, FileName = {5}", (object) projectRef.Name, (object) testLogReferenceDefault.RunId, (object) testLogReferenceDefault.ResultId, (object) testLogReferenceDefault.SubResultId, (object) attachmentId, (object) testLogReferenceDefault.FilePath);
      service.DownloadToStream(context, projectFromGuid, testLogReferenceFallback, logStoreContentStream);
    }

    private void PublishTelemetryForGetTestAttachments(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      List<TestResultAttachment> attachments,
      int runId,
      int resultId,
      int attachmentId,
      int subResultId)
    {
      string str1 = string.Join<int>(", ", (IEnumerable<int>) attachments.Select<TestResultAttachment, int>((Func<TestResultAttachment, int>) (a => a.Id)).ToList<int>());
      string str2 = string.Join<int>(", ", (IEnumerable<int>) attachments.Select<TestResultAttachment, int>((Func<TestResultAttachment, int>) (a => a.TestRunId)).ToList<int>());
      string str3 = string.Join<int>(", ", (IEnumerable<int>) attachments.Select<TestResultAttachment, int>((Func<TestResultAttachment, int>) (a => a.TestResultId)).ToList<int>());
      string str4 = string.Join<int>(", ", (IEnumerable<int>) attachments.Select<TestResultAttachment, int>((Func<TestResultAttachment, int>) (a => a.IterationId)).ToList<int>());
      string str5 = string.Join<int>(", ", (IEnumerable<int>) attachments.Select<TestResultAttachment, int>((Func<TestResultAttachment, int>) (a => a.SubResultId)).ToList<int>());
      bool flag = this.CompareInputAndOutputParams(runId, resultId, subResultId, attachmentId, attachments);
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "ProjectName",
          (object) projectRef.Name
        },
        {
          "InputRunId",
          (object) runId.ToString()
        },
        {
          "Input_ResultId",
          (object) resultId.ToString()
        },
        {
          "Input_SubResultId",
          (object) subResultId.ToString()
        },
        {
          "Input_AttachmentId",
          (object) attachmentId.ToString()
        },
        {
          "Output_RunIds",
          (object) str2
        },
        {
          "Output_ResultIds",
          (object) str3
        },
        {
          "Output_AttachmentIds",
          (object) str1
        },
        {
          "Output_IterationIds",
          (object) str4
        },
        {
          "Output_SubResultIds",
          (object) str5
        },
        {
          "ExpectedResultsFetchedFromQuery",
          (object) flag.ToString()
        }
      });
      TelemetryLogger.Instance.PublishData(context.RequestContext, "GetTestAttachments", cid);
    }

    private bool CompareInputAndOutputParams(
      int runId,
      int resultId,
      int subResultId,
      int attachmentId,
      List<TestResultAttachment> attachments)
    {
      foreach (TestResultAttachment attachment in attachments)
      {
        if (attachment.TestRunId != runId || attachment.TestResultId != resultId || attachment.SubResultId != subResultId || attachmentId != 0 && attachment.Id != attachmentId)
          return false;
      }
      return true;
    }

    private bool TryDeleteTestAttachmentFromLogStore(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      TestResultAttachmentIdentity attachmentIdentity)
    {
      ArgumentUtility.CheckForNull<TestResultAttachmentIdentity>(attachmentIdentity, nameof (attachmentIdentity), "Test Results");
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, nameof (projectRef), "Test Results");
      context.RequestContext.Trace(1015851, TraceLevel.Info, "TestManagement", "DeleteAttachment", "TeamFoundationTestManagementAttachmentsService.TryDeleteTestAttachmentFromLogStore invoked with runId: {0}, resultId: {1}, attachmentId: {2}", (object) attachmentIdentity.TestRunId, (object) attachmentIdentity.TestResultId, (object) attachmentIdentity.AttachmentId);
      if (context.RequestContext.IsFeatureEnabled("TestManagement.Server.AttachmentsUniformPathEnabled"))
      {
        List<TestResultAttachment> testAttachments = this.GetTestAttachments(context, projectRef, attachmentIdentity.TestRunId, attachmentIdentity.TestResultId, 0, attachmentIdentity.AttachmentId);
        if (testAttachments == null || !testAttachments.Any<TestResultAttachment>())
          return false;
        TestLogType testLogType = AttachmentsHelper.GetTestLogType(testAttachments[0].AttachmentType);
        if (testAttachments[0].FileId == 0 || testAttachments[0].FileId == -1)
        {
          List<string> enumerable = TestResultAttachment.DeleteAttachmentsFromLogStoreMapper(context, new TestResultAttachmentIdentity[1]
          {
            attachmentIdentity
          }, projectRef.Name);
          if (!enumerable.IsNullOrEmpty<string>())
          {
            foreach (string filename in enumerable)
            {
              bool flag = false;
              try
              {
                flag = this.DeleteAttachmentFromLogStore(context, projectRef, filename, testLogType, attachmentIdentity.TestRunId, attachmentIdentity.TestResultId);
              }
              catch (Exception ex)
              {
                context.RequestContext.TraceException("DeleteAttachment", ex);
              }
              finally
              {
                if (!flag)
                  context.RequestContext.TraceAlways(1015971, TraceLevel.Error, "TestManagement", "DeleteAttachment", "TeamFoundationTestManagementAttachmentsService.TryDeleteTestAttachmentFromLogStore: Deletion from blob failed. ProjectName = {0}, TestRunId = {1}, TestResultId = {2}, AttachmentId = {3}, FileName = {4}, testLogType = {5}", (object) projectRef.Name, (object) attachmentIdentity.TestRunId, (object) attachmentIdentity.TestResultId, (object) attachmentIdentity.AttachmentId, (object) filename, (object) testLogType.ToString());
                Dictionary<string, object> data = new Dictionary<string, object>();
                data.Add("ProjectName", (object) projectRef.Name);
                int num = attachmentIdentity.TestRunId;
                data.Add("TestRunId", (object) num.ToString());
                num = attachmentIdentity.TestResultId;
                data.Add("TestResultId", (object) num.ToString());
                data.Add("AttachmentId", (object) attachmentIdentity.AttachmentId.ToString());
                data.Add("FileName", (object) filename);
                data.Add("IsSuccessful", (object) flag.ToString());
                CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) data);
                TelemetryLogger.Instance.PublishData(context.RequestContext, nameof (TryDeleteTestAttachmentFromLogStore), cid);
              }
            }
            return true;
          }
        }
      }
      else
      {
        List<string> enumerable = TestResultAttachment.DeleteAttachmentsFromLogStoreMapper(context, new TestResultAttachmentIdentity[1]
        {
          attachmentIdentity
        }, projectRef.Name);
        if (!enumerable.IsNullOrEmpty<string>())
        {
          foreach (string filename in enumerable)
          {
            bool flag = false;
            try
            {
              flag = this.DeleteTestAttachmentFromLogStore(context, projectRef, attachmentIdentity.TestRunId, filename, TestLogType.System, attachmentIdentity.TestResultId, 0, 0);
            }
            catch (Exception ex)
            {
              context.RequestContext.TraceException("DeleteAttachment", ex);
            }
            finally
            {
              if (!flag)
                context.RequestContext.TraceAlways(1015161, TraceLevel.Error, "TestManagement", "DeleteAttachment", "TeamFoundationTestManagementAttachmentsService.TryDeleteTestAttachmentFromLogStore: Deletion from blob failed. ProjectName = {0}, TestRunId = {1}, TestResultId = {2}, AttachmentId = {3}, FileName = {4}", (object) projectRef.Name, (object) attachmentIdentity.TestRunId, (object) attachmentIdentity.TestResultId, (object) attachmentIdentity.AttachmentId, (object) filename);
              Dictionary<string, object> data = new Dictionary<string, object>();
              data.Add("ProjectName", (object) projectRef.Name);
              int num = attachmentIdentity.TestRunId;
              data.Add("TestRunId", (object) num.ToString());
              num = attachmentIdentity.TestResultId;
              data.Add("TestResultId", (object) num.ToString());
              data.Add("AttachmentId", (object) attachmentIdentity.AttachmentId.ToString());
              data.Add("FileName", (object) filename);
              data.Add("IsSuccessful", (object) flag.ToString());
              CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) data);
              TelemetryLogger.Instance.PublishData(context.RequestContext, nameof (TryDeleteTestAttachmentFromLogStore), cid);
            }
          }
          return true;
        }
      }
      return false;
    }

    private bool DeleteAttachmentFromLogStore(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      string filename,
      TestLogType testLogType,
      int runId,
      int resultId = 0,
      int subResultId = 0)
    {
      return this.ExecuteAction<bool>(context.RequestContext, "TeamFoundationTestAttachmentsService.DeleteTestAttachmentFromLogStore", (Func<bool>) (() =>
      {
        ArgumentUtility.CheckGreaterThanOrEqualToZero((float) subResultId, nameof (subResultId), "Test Results");
        context.RequestContext.GetService<IVssRegistryService>();
        this.ValidateTestRunParameters(context, projectRef, runId, resultId);
        context.TraceVerbose("BusinessLayer", "TeamFoundationTestManagementAttachmentsService.DeleteAttachmentFromLogStore invoked with runId: {0}, resultId: {1}, filename: {2}", (object) runId, (object) resultId, (object) filename);
        return context.RequestContext.GetService<ITestLogStoreService>().DeleteBlob(context, projectRef.Id, new TestLogReference()
        {
          RunId = runId,
          ResultId = resultId,
          SubResultId = subResultId,
          Scope = TestLogScope.Run,
          FilePath = filename,
          Type = testLogType
        });
      }), 1015110, "TestManagement", "AttachmentHandler");
    }

    private void PublishTelemetryForDeleteAttachmentFromLogStoreMapper(
      TestManagementRequestContext context,
      TestResultAttachmentIdentity attachmentIdentity,
      string projectName)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "ProjectName",
          (object) projectName
        },
        {
          "RunId",
          (object) attachmentIdentity.TestRunId.ToString()
        },
        {
          "ResultId",
          (object) attachmentIdentity.TestResultId.ToString()
        },
        {
          "AttachmentId",
          (object) attachmentIdentity.AttachmentId.ToString()
        }
      });
      TelemetryLogger.Instance.PublishData(context.RequestContext, "TryDeleteTestAttachmentFromLogStore", cid);
    }

    private void PublishTelemetryForDeleteAttachment(
      TestManagementRequestContext context,
      TestResultAttachmentIdentity attachmentIdentity,
      string projectName)
    {
      CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
      {
        {
          "ProjectName",
          (object) projectName
        },
        {
          "RunId",
          (object) attachmentIdentity.TestRunId.ToString()
        },
        {
          "ResultId",
          (object) attachmentIdentity.TestResultId.ToString()
        },
        {
          "AttachmentId",
          (object) attachmentIdentity.AttachmentId.ToString()
        }
      });
      TelemetryLogger.Instance.PublishData(context.RequestContext, "TestResultAttachment.Delete", cid);
    }

    private void ValidateTestRunParameters(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int runId,
      int resultId = 0,
      int iterationId = 0)
    {
      if (context.RequestContext.GetService<ITeamFoundationTestManagementRunService>().GetTestRunById(context, runId, projectRef) == null)
        throw new TestObjectNotFoundException(context.RequestContext, runId, ObjectTypes.TestRun);
      if (resultId <= 0)
        return;
      TestResultArtifacts testResultArtifacts = context.RequestContext.GetService<ITeamFoundationTestManagementResultService>().FetchTestResultArtifacts(context, runId, resultId, projectRef.Name);
      if (testResultArtifacts == null || testResultArtifacts.TestCaseResult == null)
        throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultNotFound, (object) runId, (object) resultId), ObjectTypes.TestResult);
    }

    private void ValidateTestResultsParameters(
      TestManagementRequestContext context,
      TeamProjectReference projectRef,
      int runId,
      int[] resultIds)
    {
      if (context.RequestContext.GetService<ITeamFoundationTestManagementRunService>().GetTestRunById(context, runId, projectRef) == null)
        throw new TestObjectNotFoundException(context.RequestContext, runId, ObjectTypes.TestRun);
      if (((IEnumerable<int>) resultIds).Count<int>() <= 0)
        return;
      IList<TestResultArtifacts> source = context.RequestContext.GetService<ITeamFoundationTestManagementResultService>().FetchTestResultsArtifacts(context, ((IEnumerable<int>) resultIds).Select<int, TestCaseResult>((Func<int, TestCaseResult>) (id =>
      {
        return new TestCaseResult()
        {
          TestRunId = runId,
          TestResultId = id
        };
      })).ToList<TestCaseResult>(), projectRef.Name);
      if (source == null || !source.Any<TestResultArtifacts>())
        throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultNotFound, (object) runId, (object) 0), ObjectTypes.TestResult);
      int index = 0;
      foreach (TestResultArtifacts testResultArtifacts in (IEnumerable<TestResultArtifacts>) source)
      {
        if (testResultArtifacts == null || testResultArtifacts.TestCaseResult == null)
          throw new TestObjectNotFoundException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.TestResultNotFound, (object) runId, (object) resultIds[index]), ObjectTypes.TestResult);
        ++index;
      }
    }

    private void CreateAttachmentInputValidation(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      int runId,
      bool isBase64Encoded = true)
    {
      ArgumentUtility.CheckForNull<TeamProjectReference>(projectRef, nameof (projectRef), "Test Results");
      ArgumentUtility.CheckForNull<TestAttachmentRequestModel>(attachmentRequestModel, nameof (attachmentRequestModel), "Test Results");
      ArgumentUtility.CheckForNull<string>(attachmentRequestModel.Stream, "Stream", "Test Results");
      ArgumentUtility.CheckStringForNullOrWhiteSpace(attachmentRequestModel.FileName, "FileName", "Test Results");
      ArgumentUtility.CheckGreaterThanZero((float) runId, nameof (runId), "Test Results");
      int index = attachmentRequestModel.FileName.IndexOfAny(Path.GetInvalidFileNameChars());
      if (index >= 0)
        throw new ArgumentException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidFileNameSpecified, (object) attachmentRequestModel.FileName[index]), "fileName").Expected("Test Results");
      this.ValidateAttachmentSize(attachmentRequestModel, isBase64Encoded);
    }

    private string GetAttachmentType(string attachmentType)
    {
      if (string.Equals(attachmentType, TestResultsConstants.TestSubResultJsonAttachmentType, StringComparison.OrdinalIgnoreCase))
        return TestResultsConstants.TestSubResultJsonAttachmentType;
      AttachmentType result = AttachmentType.GeneralAttachment;
      if (!string.IsNullOrEmpty(attachmentType) && !Enum.TryParse<AttachmentType>(attachmentType, true, out result))
        throw new InvalidPropertyException("AttachmentType", ServerResources.InvalidAttachmentType);
      return result.ToString();
    }

    private int UploadAttachment(
      TestManagementRequestContext context,
      TestAttachmentRequestModel attachmentRequestModel,
      TeamProjectReference projectRef,
      out long streamLength)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(attachmentRequestModel.Stream);
      streamLength = 0L;
      using (Stream content = (Stream) new MemoryStream(bytes, 0, bytes.Length, false))
      {
        if (content == null)
          throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentContent));
        content.Seek(0L, SeekOrigin.Begin);
        streamLength = content.Length;
        return TestResultAttachment.UploadAttachment(context, projectRef.Id, content);
      }
    }

    private string UploadAttachmentToLogStoreThroughCreateAttachment(
      TestManagementRequestContext context,
      string stream,
      TeamProjectReference projectRef,
      int runId,
      string userFileName,
      TestAttachmentType testAttachmentType,
      int resultId = 0,
      int subResultId = 0,
      int attachmentId = 0,
      TestLogType testLogType = TestLogType.GeneralAttachment)
    {
      string fileName = userFileName;
      if (string.IsNullOrEmpty(userFileName))
        fileName = string.Format(this.getFileNameForAttachmentType(testAttachmentType), (object) Guid.NewGuid());
      context.RequestContext.Trace(1015111, TraceLevel.Info, "TestManagement", nameof (TeamFoundationTestManagementAttachmentsService), "TeamFoundationTestManagementAttachmentsService.CreateTestAttachment invoked with runId: {0} and FileName: {1}", (object) runId, (object) fileName);
      ITestLogStoreService service = context.RequestContext.GetService<ITestLogStoreService>();
      bool flag1 = context.RequestContext.IsFeatureEnabled("TestManagement.Server.AttachmentsUniformPathEnabled");
      string str;
      TestLogReference attachmentTestLogReference;
      if (flag1)
      {
        str = fileName;
        attachmentTestLogReference = new TestLogReference()
        {
          RunId = runId,
          ResultId = resultId,
          SubResultId = subResultId,
          FilePath = str,
          Type = testLogType,
          Scope = TestLogScope.Run
        };
      }
      else
      {
        str = this.getFilePathToStoreAttachmentInLogStore(testAttachmentType, true, fileName, attachmentId);
        attachmentTestLogReference = new TestLogReference()
        {
          RunId = runId,
          ResultId = resultId,
          FilePath = str,
          Type = TestLogType.System,
          Scope = TestLogScope.Run
        };
      }
      TestLogStatusWithFileName statusWithFileName = new TestLogStatusWithFileName();
      bool flag2 = false;
      try
      {
        byte[] buffer = Convert.FromBase64String(stream);
        using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length))
        {
          if (memoryStream == null)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentContent));
          if (flag1)
          {
            TestLogStatusWithFileName withFallbackName = service.CreateBlobWithFallbackName(context, projectRef.Id, attachmentTestLogReference, (Stream) memoryStream, new Dictionary<string, string>());
            if (withFallbackName != null)
            {
              if (withFallbackName.Status == TestLogStatusCode.Success)
              {
                flag2 = true;
                fileName = withFallbackName.FileName;
              }
            }
          }
          else
            flag2 = service.CreateBlob(context, projectRef.Id, attachmentTestLogReference, (Stream) memoryStream, new Dictionary<string, string>());
        }
      }
      catch (Exception ex)
      {
        context.RequestContext.Trace(1015920, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlob - CreateBlob failed, ExceptionHit: {0} ", (object) ex.Message);
      }
      finally
      {
        CustomerIntelligenceData cid = new CustomerIntelligenceData((IDictionary<string, object>) new Dictionary<string, object>()
        {
          {
            "TestRunId",
            (object) runId.ToString()
          },
          {
            "TestResultId",
            (object) resultId.ToString()
          },
          {
            "FileName",
            (object) fileName
          },
          {
            "FilePath",
            (object) str
          },
          {
            "IsSuccessful",
            (object) flag2.ToString()
          }
        });
        TelemetryLogger.Instance.PublishData(context.RequestContext, nameof (UploadAttachmentToLogStoreThroughCreateAttachment), cid);
      }
      if (!flag2)
      {
        context.RequestContext.TraceAlways(1015112, TraceLevel.Error, "TestManagement", nameof (TeamFoundationTestManagementAttachmentsService), "TeamFoundationTestManagementAttachmentsService.CreateTestAttachment failed with runId: {0}, resultId: {1}, FileName: {2}", (object) runId, (object) resultId, (object) fileName);
        throw new Exception("CreateBlob returned false for uploading the attachment");
      }
      return fileName;
    }

    private string getFilePathToStoreAttachmentInLogStore(
      TestAttachmentType testAttachmentType,
      bool shouldUseAttachmentIdInPath,
      string fileName,
      int attachmentId = 0)
    {
      return shouldUseAttachmentIdInPath ? string.Format("{0}{1}/{2}", (object) AttachmentsHelper.GetFilePathForAttachmentType(testAttachmentType), (object) attachmentId, (object) fileName) : AttachmentsHelper.GetFilePathForAttachmentType(testAttachmentType) + fileName;
    }

    private bool UploadAttachmentToLogStoreThroughCreateAttachmentV2(
      TestManagementRequestContext context,
      string stream,
      TeamProjectReference projectRef,
      int runId,
      string userFileName,
      TestAttachmentType testAttachmentType,
      TestLogType testLogType = TestLogType.System)
    {
      context.RequestContext.Trace(1015111, TraceLevel.Info, "TestManagement", nameof (TeamFoundationTestManagementAttachmentsService), "TeamFoundationTestManagementAttachmentsService.CreateTestAttachment invoked with runId: {0} and FileName: {1}", (object) runId, (object) userFileName);
      ITestLogStoreService service = context.RequestContext.GetService<ITestLogStoreService>();
      string str = AttachmentsHelper.GetFilePathForAttachmentType(testAttachmentType) + userFileName;
      TestLogReference attachmentTestLogReference = new TestLogReference()
      {
        RunId = runId,
        FilePath = str,
        Type = testLogType,
        Scope = TestLogScope.Run
      };
      bool createAttachmentV2 = false;
      try
      {
        byte[] buffer = Convert.FromBase64String(stream);
        using (MemoryStream memoryStream = new MemoryStream(buffer, 0, buffer.Length))
        {
          if (memoryStream == null)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentContent));
          createAttachmentV2 = service.CreateBlob(context, projectRef.Id, attachmentTestLogReference, (Stream) memoryStream, new Dictionary<string, string>());
        }
      }
      catch (Exception ex)
      {
        context.RequestContext.Trace(1015920, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlob - CreateBlob failed, ExceptionHit: {0} ", (object) ex.Message);
      }
      if (!createAttachmentV2)
      {
        context.RequestContext.Trace(1015112, TraceLevel.Error, "TestManagement", nameof (TeamFoundationTestManagementAttachmentsService), "TeamFoundationTestManagementAttachmentsService.CreateTestAttachment failed with runId: {0} and FileName: {1}", (object) runId, (object) userFileName);
        throw new Exception("CreateBlob returned false for uploading the attachment");
      }
      return createAttachmentV2;
    }

    private string UploadAttachmentToLogStore(
      TestManagementRequestContext context,
      string stream,
      TeamProjectReference projectRef,
      int runId)
    {
      string logStore = string.Format(TestResultsConstants.TestSubResultV2LogStoreFileName, (object) Guid.NewGuid());
      ITestLogStoreService service = context.RequestContext.GetService<ITestLogStoreService>();
      TestLogReference attachmentTestLogReference = new TestLogReference();
      attachmentTestLogReference.RunId = runId;
      attachmentTestLogReference.FilePath = TestResultsConstants.TestSubResultV2LogStoreFilePath + logStore;
      attachmentTestLogReference.Type = TestLogType.System;
      attachmentTestLogReference.Scope = TestLogScope.Run;
      bool flag = false;
      try
      {
        byte[] bytes = Encoding.UTF8.GetBytes(stream);
        using (MemoryStream memoryStream = new MemoryStream(bytes, 0, bytes.Length))
        {
          if (memoryStream == null)
            throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentContent));
          flag = service.CreateBlob(context, projectRef.Id, attachmentTestLogReference, (Stream) memoryStream, new Dictionary<string, string>());
        }
      }
      catch (Exception ex)
      {
        context.RequestContext.Trace(1015676, TraceLevel.Error, "TestManagement", "LogStorage", "CreateBlob - CreateBlob failed, ExceptionHit: {0} ", (object) ex.Message);
      }
      if (!flag)
        throw new Exception("CreateBlob returned false");
      return logStore;
    }

    private string getFileNameForAttachmentType(TestAttachmentType testAttachmentType)
    {
      switch (testAttachmentType)
      {
        case TestAttachmentType.TestRun:
          return TestResultsConstants.TestRunLogStoreFileName;
        case TestAttachmentType.TestResult:
          return TestResultsConstants.TestResultLogStoreFileName;
        case TestAttachmentType.TestSubResult:
          return TestResultsConstants.TestSubResultLogStoreFileNameNotJson;
        default:
          return TestResultsConstants.TestRunLogStoreFileName;
      }
    }

    private long getStreamSize(string stream)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(stream);
      using (MemoryStream memoryStream = new MemoryStream(bytes, 0, bytes.Length))
        return memoryStream != null ? memoryStream.Length : throw new InvalidPropertyException(string.Format((IFormatProvider) CultureInfo.CurrentCulture, ServerResources.InvalidAttachmentContent));
    }

    private TestLogReference GetTestLogReferenceToDownloadAttachment(
      int runId,
      TestAttachmentType testAttachmentType,
      bool enableFilePathChangeInStoringLogStoreAttachments,
      string fileName = "",
      int resultId = 0,
      int attachmentId = 0)
    {
      return new TestLogReference()
      {
        RunId = runId,
        Scope = TestLogScope.Run,
        FilePath = this.getFilePathToStoreAttachmentInLogStore(testAttachmentType, enableFilePathChangeInStoringLogStoreAttachments, fileName, attachmentId),
        Type = TestLogType.System,
        ResultId = resultId
      };
    }

    private TestLogReference GetTestLogReferenceToDownloadAttachmentWithUniformPath(
      int runId,
      int resultId,
      int subResultId,
      string attachmentType,
      string fileName)
    {
      TestLogType testLogType = AttachmentsHelper.GetTestLogType(attachmentType);
      return new TestLogReference()
      {
        RunId = runId,
        Scope = TestLogScope.Run,
        FilePath = fileName,
        Type = testLogType,
        ResultId = resultId,
        SubResultId = subResultId
      };
    }

    private static string EscapeCharacters(string attachmentName)
    {
      if (attachmentName != null)
      {
        try
        {
          attachmentName = Uri.EscapeDataString(attachmentName);
        }
        catch (UriFormatException ex)
        {
        }
      }
      return attachmentName;
    }

    private string GetVideoFileName(Stream attachmentStream)
    {
      try
      {
        foreach (XContainer xcontainer in XElement.Load(attachmentStream).Elements((XName) "Files").Select<XElement, XElement>((Func<XElement, XElement>) (fileElement => fileElement)))
        {
          XElement xelement;
          if ((xelement = xcontainer.Element((XName) "File")) != null)
            return xelement.Attribute((XName) "name").Value;
        }
      }
      catch (Exception ex)
      {
      }
      return string.Empty;
    }

    private List<TestResultAttachment> GetAttachments(
      TestManagementRequestContext tcmRequestContext,
      Guid projectGuid,
      int attachmentId)
    {
      List<TestResultAttachment> attachments;
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create(tcmRequestContext))
        attachments = managementDatabase.QueryAttachments(tcmRequestContext, projectGuid, attachmentId, true, out string _);
      if (attachments != null && attachments.Any<TestResultAttachment>())
        TestResultAttachment.SignAttachmentObjects(tcmRequestContext.RequestContext, attachments);
      return attachments;
    }

    private bool ShouldUploadToLogStoreTable(
      TestManagementRequestContext context,
      int runId,
      bool shouldUploadToLogStore)
    {
      context.RequestContext.Trace(1015003, TraceLevel.Info, "TestManagement", "BusinessLayer", "ShouldUploadToLogStoreTable runId = {0} and shouldUploadToLogStore = {1}", (object) runId, (object) shouldUploadToLogStore);
      return shouldUploadToLogStore && context.IsFeatureEnabled("TestManagement.Server.LogStoreAttachmentTableForSubResult");
    }
  }
}
