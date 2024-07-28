// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.Server.WebAccess.TestManagement.TestResultHelper
// Assembly: Microsoft.TeamFoundation.Server.WebAccess.TestManagement, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 2E4165D5-898A-42D9-B816-9FABF135E4DA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.TeamFoundation.Server.WebAccess.TestManagement.dll

using Microsoft.Azure.Devops.Work.RemoteServices;
using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using Microsoft.TeamFoundation.TestManagement.Common.Internal;
using Microsoft.TeamFoundation.TestManagement.Server;
using Microsoft.TeamFoundation.TestManagement.Server.TcmService;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using Microsoft.TeamFoundation.WorkItemTracking.Server;
using Microsoft.TeamFoundation.WorkItemTracking.WebApi.Models;
using Microsoft.VisualStudio.Services.Common;
using Microsoft.VisualStudio.Services.TestResults.WebApi;
using Microsoft.VisualStudio.Services.WebApi;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;

namespace Microsoft.TeamFoundation.Server.WebAccess.TestManagement
{
  internal class TestResultHelper : TestHelperBase
  {
    private const int c_maximumFileSize = 104857600;
    private ResultsHelper m_resultsHelper;

    internal TestResultHelper(TestManagerRequestContext testContext)
      : base(testContext)
    {
    }

    internal ResultUpdateResponseModel[] Update(ResultUpdateRequestModel[] updateRequests)
    {
      if (updateRequests == null)
        return (ResultUpdateResponseModel[]) null;
      ResultUpdateResponseModel[] updatedResults = (ResultUpdateResponseModel[]) null;
      this.TestContext.TestRequestContext.RequestContext.Trace(1015000, TraceLevel.Info, "TestManagement", "WebService", "Size of update request: {0}", (object) updateRequests.Length);
      if (!this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        if (!this.TryUpdatedResultsInTCM(updateRequests, out updatedResults))
          updatedResults = this.UpdateInternal(updateRequests);
      }
      else
        updatedResults = this.UpdatedResultsInTCMAsync(updateRequests);
      IEnumerable<int> source = ((IEnumerable<ResultUpdateRequestModel>) updateRequests).Select<ResultUpdateRequestModel, int>((Func<ResultUpdateRequestModel, int>) (request => request.TestCaseResult.PlanId)).Distinct<int>();
      if (source.Count<int>() == 1)
      {
        int[] array = ((IEnumerable<ResultUpdateRequestModel>) updateRequests).Select<ResultUpdateRequestModel, int>((Func<ResultUpdateRequestModel, int>) (request => request.TestCaseResult.TestPointId)).ToArray<int>();
        byte outcome = ((IEnumerable<ResultUpdateRequestModel>) updateRequests).Select<ResultUpdateRequestModel, byte>((Func<ResultUpdateRequestModel, byte>) (request => request.TestCaseResult.Outcome)).First<byte>();
        new TestPlansHelper(this.TestContext).SyncTestPoints(((IEnumerable<ResultUpdateRequestModel>) updateRequests).Select<ResultUpdateRequestModel, bool>((Func<ResultUpdateRequestModel, bool>) (request => request.TestCaseResult.UseTeamSettings)).FirstOrDefault<bool>(), new TestOutcomeUpdateRequest(this.TestContext.ProjectName, source.First<int>(), array, (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) outcome, this.TestContext.TfsRequestContext.GetUserId()));
      }
      return updatedResults;
    }

    internal static Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] CreateTestCaseResults(
      TestResultCreationRequestModel[] testResultCreationRequestModels,
      Guid runBy,
      int firstResultId,
      int testPlanId)
    {
      return ((IEnumerable<TestResultCreationRequestModel>) testResultCreationRequestModels).Select<TestResultCreationRequestModel, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>((Func<TestResultCreationRequestModel, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) (result =>
      {
        return new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult()
        {
          TestCaseId = result.TestCaseId,
          Id = new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier()
          {
            TestResultId = firstResultId++
          },
          TestPointId = result.TestPointId,
          ConfigurationId = result.ConfigurationId,
          ConfigurationName = result.ConfigurationName,
          Owner = result.Owner,
          RunBy = runBy,
          State = 3,
          Outcome = 0,
          DateStarted = DateTime.Now,
          DateCompleted = DateTime.Now,
          TestPlanId = testPlanId
        };
      })).ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
    }

    internal static List<TestResultCreationResponseModel> CreateTestResultCreationResponseModels(
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] results)
    {
      return ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) results).Select<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestResultCreationResponseModel>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, TestResultCreationResponseModel>) (result => new TestResultCreationResponseModel(result))).ToList<TestResultCreationResponseModel>();
    }

    internal static TestResultAttachment CreateTestResultAttachment(
      int testRunId,
      int testResultId,
      int iterationId,
      HttpPostedFileBase file)
    {
      if (file.ContentLength > 104857600)
        throw new LegacyValidationException(string.Format(TestManagementServerResources.FileUploadedExceededMaxSize, (object) 104857600));
      TestResultAttachment resultAttachment = new TestResultAttachment();
      resultAttachment.AttachmentType = "GeneralAttachment";
      string fileName;
      string str = fileName = Path.GetFileName(file.FileName);
      resultAttachment.FileName = fileName;
      resultAttachment.Comment = str;
      resultAttachment.Length = (long) file.ContentLength;
      resultAttachment.TestRunId = testRunId;
      resultAttachment.TestResultId = testResultId;
      resultAttachment.IterationId = iterationId;
      resultAttachment.TmiRunId = Guid.Empty;
      return resultAttachment;
    }

    internal List<object> CreateAndUploadTestResultAttachments(
      HttpFileCollectionBase filesToUpload,
      int testRunId,
      int testResultId,
      int iterationId,
      string actionPath)
    {
      List<object> resultAttachments = new List<object>();
      for (int index = 0; index < filesToUpload.Count; ++index)
      {
        HttpPostedFileBase file = filesToUpload[index];
        TestResultAttachment resultAttachment = TestResultHelper.CreateTestResultAttachment(testRunId, testResultId, iterationId, file);
        resultAttachment.ActionPath = string.IsNullOrEmpty(actionPath) ? (string) null : actionPath;
        int[] testAttachmentIds = this.CreateTestAttachmentIds(new TestResultAttachment[1]
        {
          resultAttachment
        }, false);
        resultAttachment.Id = testAttachmentIds[0];
        resultAttachments.Add((object) new
        {
          Id = testAttachmentIds[0],
          Name = Path.GetFileName(file.FileName),
          Size = file.ContentLength
        });
        this.UploadAttachment(testRunId, testResultId, resultAttachment, file);
      }
      return resultAttachments;
    }

    internal int[] CreateTestAttachmentIds(
      TestResultAttachment[] attachments,
      bool areSessionAttachments)
    {
      return TestResultAttachment.Create((TestManagementRequestContext) this.TestContext.TestRequestContext, attachments, this.TestContext.ProjectName, areSessionAttachments);
    }

    internal void UploadAttachment(
      int testRunId,
      int testResultId,
      TestResultAttachment attachment,
      HttpPostedFileBase file)
    {
      GuidAndString projectFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName);
      bool flag = true;
      using (Stream inputStream = file.InputStream)
      {
        using (ByteArray byteArray = new ByteArray((int) Math.Min(inputStream.Length, 1048576L)))
        {
          int offsetFrom = 0;
          do
          {
            int contentLength = inputStream.Read(byteArray.Bytes, 0, 1048576);
            TestResultAttachment.AppendAttachment((TestManagementRequestContext) this.TestContext.TestRequestContext, projectFromName.GuidId, testRunId, testResultId, attachment.Id, 0, attachment.Length, attachment.Length, (byte[]) null, CompressionType.None, (long) offsetFrom, byteArray.Bytes, contentLength, 0);
            offsetFrom += contentLength;
            if (flag)
            {
              this.TestContext.TestRequestContext.RequestContext.UpdateTimeToFirstPage();
              flag = false;
            }
          }
          while (inputStream.Position < inputStream.Length);
        }
      }
    }

    internal void DeleteAttachment(int attachmentId, int testRunId, int testResultId)
    {
      if (testResultId != 0 ? this.TestContext.TestRequestContext.TcmServiceHelper.TryDeleteTestResultAttachment(this.TestContext.TfsRequestContext, this.TestContext.CurrentProjectGuid, testRunId, testResultId, attachmentId) : this.TestContext.TestRequestContext.TcmServiceHelper.TryDeleteTestRunAttachment(this.TestContext.TfsRequestContext, this.TestContext.CurrentProjectGuid, testRunId, attachmentId))
        return;
      TestResultAttachment.Delete((TestManagementRequestContext) this.TestContext.TestRequestContext, new TestResultAttachmentIdentity[1]
      {
        TestResultHelper.CreateTestResultAttachmentIdentity(attachmentId, testRunId, testResultId)
      }, this.TestContext.ProjectName);
    }

    internal static void SaveTestResults(
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult[] testResults,
      TestManagerRequestContext testContext)
    {
      ResultUpdateRequest[] array = ((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) testResults).Select<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, ResultUpdateRequest>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, ResultUpdateRequest>) (result => new ResultUpdateRequest()
      {
        TestCaseResult = result,
        TestResultId = result.TestResultId,
        TestRunId = result.TestRunId
      })).ToArray<ResultUpdateRequest>();
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.Update((TestManagementRequestContext) testContext.TestRequestContext, array, testContext.ProjectName);
    }

    internal List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> EndTestCaseResults(
      TestRunModel runModel,
      TestManagerRequestContext testContext)
    {
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier> excessIds = new List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>();
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> testCaseResultList1 = Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.QueryByRun((TestManagementRequestContext) testContext.TestRequestContext, runModel.TestRunId, int.MaxValue, out excessIds, testContext.ProjectName, false, out List<TestActionResult> _, out List<TestResultParameter> _, out List<TestResultAttachment> _);
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> testCaseResultList2 = new List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult testCaseResult in testCaseResultList1)
      {
        if (TestResultHelper.ShouldEndTestCaseResult((Microsoft.TeamFoundation.TestManagement.Client.TestOutcome) testCaseResult.Outcome, (TestResultState) testCaseResult.State))
        {
          testCaseResult.State = (byte) 5;
          testCaseResultList2.Add(testCaseResult);
        }
      }
      TestResultHelper.SaveTestResults(testCaseResultList2.ToArray(), testContext);
      return testCaseResultList1;
    }

    public bool TryEndTestCaseResultsInTCM(
      TestManagerRequestContext testContext,
      TestRunModel runModel,
      out List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> serverTestResults)
    {
      serverTestResults = new List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
      Guid projectGuidFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) testContext.TestRequestContext, testContext.ProjectName);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results;
      if (!testContext.TestRequestContext.TcmServiceHelper.TryGetTestResults(testContext.TfsRequestContext, projectGuidFromName, runModel.TestRunId, out results))
        return false;
      this.PopulateServerTestResults(testContext, projectGuidFromName, runModel, out serverTestResults, results);
      return true;
    }

    internal List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> EndTestCaseResultsInTCMAsync(
      TestManagerRequestContext testContext,
      TestRunModel runModel)
    {
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> serverTestResults = new List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
      Guid projectId = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) testContext.TestRequestContext, testContext.ProjectName);
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults = TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.GetTestResultsAsync(projectId, runModel.TestRunId, new ResultDetails?(), new int?(), new int?(), (IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestOutcome>) null, new bool?(), (object) null, new CancellationToken())?.Result));
      this.PopulateServerTestResults(testContext, projectId, runModel, out serverTestResults, testCaseResults);
      return serverTestResults;
    }

    internal List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> PopulateServerTestResults(
      TestManagerRequestContext testContext,
      Guid projectId,
      TestRunModel runModel,
      out List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> serverTestResults,
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResults)
    {
      serverTestResults = new List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultsToSave = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult1 in testCaseResults)
      {
        Microsoft.TeamFoundation.TestManagement.Client.TestOutcome result1;
        Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(testCaseResult1.Outcome, out result1);
        TestResultState result2;
        Enum.TryParse<TestResultState>(testCaseResult1.State, out result2);
        if (TestResultHelper.ShouldEndTestCaseResult(result1, result2))
        {
          testCaseResult1.State = "Completed";
          testCaseResultsToSave.Add(testCaseResult1);
          result2 = TestResultState.Completed;
        }
        List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> testCaseResultList = serverTestResults;
        Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult testCaseResult2 = new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult();
        testCaseResult2.State = (byte) result2;
        testCaseResult2.Outcome = (byte) result1;
        testCaseResult2.TestRunId = runModel.TestRunId;
        testCaseResult2.TestResultId = testCaseResult1.Id;
        testCaseResult2.TestPlanId = testCaseResult1.TestPlan != null ? int.Parse(testCaseResult1.TestPlan.Id) : 0;
        testCaseResult2.TestPointId = testCaseResult1.TestPoint != null ? int.Parse(testCaseResult1.TestPoint.Id) : 0;
        testCaseResultList.Add(testCaseResult2);
      }
      if (testCaseResultsToSave.Count > 0)
      {
        if (!this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
          testContext.TestRequestContext.TcmServiceHelper.TryUpdateTestResults(testContext.TfsRequestContext, projectId, runModel.TestRunId, testCaseResultsToSave.ToArray(), out testCaseResults);
        else
          testCaseResults = TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.UpdateTestResultsAsync(testCaseResultsToSave.ToArray(), projectId, runModel.TestRunId, (object) null, new CancellationToken())?.Result));
        TestPointOutcomeUpdateRequestConverter.UpdateWebApiResult((IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResultsToSave, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResults);
        testContext.TestRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(testContext.TfsRequestContext, testContext.ProjectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testCaseResultsToSave);
      }
      return testCaseResultsToSave;
    }

    internal List<TestCaseResultWithActionResultModel> GetTestCaseResults(
      int testRunId,
      int[] testCaseResultIds)
    {
      ArgumentUtility.CheckForNull<int[]>(testCaseResultIds, nameof (testCaseResultIds), this.TestContext.TfsRequestContext.ServiceName);
      if (testCaseResultIds.Length == 1)
        return new List<TestCaseResultWithActionResultModel>()
        {
          this.GetTestCaseResultWithActionResultModel(testRunId, testCaseResultIds[0])
        };
      List<TestCaseResultIdAndRev> idAndRevs = new List<TestCaseResultIdAndRev>();
      for (int index = 0; index < testCaseResultIds.Length; ++index)
        idAndRevs.Add(new TestCaseResultIdAndRev(new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(testRunId, testCaseResultIds[index]), 0));
      List<TestActionResult> actionResults;
      List<TestResultParameter> testResultParmeters;
      List<TestResultAttachment> testResultAttachments;
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> testCaseResults = this.GetTestCaseResults(idAndRevs, out actionResults, out testResultParmeters, out testResultAttachments);
      if (testCaseResults.Count > 0)
        return this.ConvertServerTestResultToMvcModel(testCaseResults, actionResults, testResultParmeters, testResultAttachments);
      throw new Microsoft.TeamFoundation.TestManagement.Server.TestObjectNotFoundException(TestManagementServerResources.TestCaseResultNotFoundError, ObjectTypes.TestResult);
    }

    internal List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> GetTestCaseResults(
      List<TestCaseResultIdAndRev> idAndRevs,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> testResultParmeters,
      out List<TestResultAttachment> testResultAttachments)
    {
      actionResults = new List<TestActionResult>();
      testResultParmeters = new List<TestResultParameter>();
      testResultAttachments = new List<TestResultAttachment>();
      TestResultsQuery query = this.GetTestResultsQuery(idAndRevs);
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> results = new List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
      TestResultsQuery results1 = (TestResultsQuery) null;
      if (!this.TestContext.TestRequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        this.TestContext.TestRequestContext.TcmServiceHelper.TryGetTestResultsByQuery(this.TestContext.TfsRequestContext, this.TestContext.CurrentProjectGuid, query, out results1);
        if (results1 != null)
          results1.Results.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>().ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (result => results.Add(this.ConvertToServerTestCaseResult(result))));
      }
      else
      {
        TestResultsHttpClient testResultsHttpClientWithSqlReadOnly = this.TestResultsHttpClientWithSqlReadOnly("TestManagement.Server.QueryResults.EnableSqlReadReplica");
        TestResultsQuery testResultsQuery = TestManagementController.InvokeAction<TestResultsQuery>((Func<TestResultsQuery>) (() => testResultsHttpClientWithSqlReadOnly.GetTestResultsByQueryAsync(query, this.TestContext.CurrentProjectGuid, (object) null, new CancellationToken())?.Result));
        ResultsFilter resultsFilter = query.ResultsFilter;
        if ((resultsFilter != null ? (resultsFilter.ExecutedIn != Service.Tfs ? 1 : 0) : 1) != 0)
          testResultsQuery.Results.ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>().ForEach((Action<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (result => results.Add(this.ConvertToServerTestCaseResult(result))));
      }
      if (results.Count == 0)
      {
        List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> collection = Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, idAndRevs.ToArray(), this.TestContext.ProjectName, true, (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null, out actionResults, out testResultParmeters, out testResultAttachments);
        results.AddRange((IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) collection);
      }
      return results;
    }

    private TestCaseResultWithActionResultModel GetTestCaseResultWithActionResultModel(
      int testRunId,
      int testCaseResultId)
    {
      if (!this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFSLegacy"))
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result;
        if (this.TestContext.TestRequestContext.TcmServiceHelper.TryGetTestResultById(this.TestContext.TfsRequestContext, this.TestContext.CurrentProjectGuid, testRunId, testCaseResultId, true, false, false, out result))
        {
          TestCaseResultWithActionResultModel mvcModel = this.ConvertWebApiTestResultToMvcModel(result);
          if (this.TestContext.TestRequestContext.TcmServiceHelper.IsTestRunInTfs(testRunId))
          {
            List<TestResultAttachment> attachments;
            Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, new List<TestCaseResultIdAndRev>()
            {
              new TestCaseResultIdAndRev(new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(testRunId, testCaseResultId), 0)
            }.ToArray(), this.TestContext.ProjectName, true, (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null, out List<TestActionResult> _, out List<TestResultParameter> _, out attachments);
            IEnumerable<TestAttachmentModel> collection = attachments.Select<TestResultAttachment, TestAttachmentModel>((Func<TestResultAttachment, TestAttachmentModel>) (attachment => new TestAttachmentModel(attachment)));
            mvcModel.TestActionResultDetailsModel.Attachments.AddRange(collection);
          }
          return mvcModel;
        }
        List<TestActionResult> actionResults;
        List<TestResultParameter> parameters;
        List<TestResultAttachment> attachments1;
        return this.ConvertServerTestResultToMvcModel(Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, new List<TestCaseResultIdAndRev>()
        {
          new TestCaseResultIdAndRev(new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(testRunId, testCaseResultId), 0)
        }.ToArray(), this.TestContext.ProjectName, true, (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null, out actionResults, out parameters, out attachments1), actionResults, parameters, attachments1).FirstOrDefault<TestCaseResultWithActionResultModel>();
      }
      ResultDetails detailsToInclude = RestApiHelper.IncludeVariableToResultDetails(true, false, false);
      return this.ConvertWebApiTestResultToMvcModel(TestManagementController.InvokeAction<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) (() => this.TestResultsHttpClient.GetTestResultByIdAsync(this.TestContext.CurrentProjectGuid, testRunId, testCaseResultId, new ResultDetails?(detailsToInclude))?.Result)));
    }

    internal static List<TestRunAttachmentModel> GetAttachments(
      TestManagerRequestContext testContext,
      int testRunId,
      int testResultId = 0)
    {
      List<TestRunAttachmentModel> attachmentModels = new List<TestRunAttachmentModel>();
      if (!TestResultHelper.TryGetAttachmentsFromTCM(testContext, testRunId, out attachmentModels, testResultId))
        attachmentModels = TestResultHelper.GetAttachmentsInternal(testContext, testRunId, testResultId);
      return attachmentModels;
    }

    private static List<TestRunAttachmentModel> GetAttachmentsInternal(
      TestManagerRequestContext testContext,
      int testRunId,
      int testResultId = 0)
    {
      List<TestRunAttachmentModel> runAttachmentModelList = new List<TestRunAttachmentModel>();
      return TestResultAttachment.Query((TestManagementRequestContext) testContext.TestRequestContext, testRunId, testResultId, 0, testContext.ProjectName).Select<TestResultAttachment, TestRunAttachmentModel>((Func<TestResultAttachment, TestRunAttachmentModel>) (attachment => new TestRunAttachmentModel(attachment.FileName, attachment.Length, attachment.CreationDate, attachment.Comment, attachment.Id, attachment.AttachmentType))).ToList<TestRunAttachmentModel>();
    }

    private static bool TryGetAttachmentsFromTCM(
      TestManagerRequestContext testContext,
      int testRunId,
      out List<TestRunAttachmentModel> attachmentModels,
      int testResultId = 0)
    {
      attachmentModels = (List<TestRunAttachmentModel>) null;
      Guid projectGuidFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) testContext.TestRequestContext, testContext.ProjectName);
      List<TestAttachment> attachments;
      int num = testResultId <= 0 ? (testContext.TestRequestContext.TcmServiceHelper.TryGetTestRunAttachments(testContext.TfsRequestContext, projectGuidFromName, testRunId, out attachments) ? 1 : 0) : (testContext.TestRequestContext.TcmServiceHelper.TryGetTestResultAttachments(testContext.TfsRequestContext, projectGuidFromName, testRunId, testResultId, out attachments) ? 1 : 0);
      if (num == 0)
        return num != 0;
      attachmentModels = attachments.Select<TestAttachment, TestRunAttachmentModel>((Func<TestAttachment, TestRunAttachmentModel>) (attachment => new TestRunAttachmentModel(attachment.FileName, attachment.Size, attachment.CreatedDate, attachment.Comment, attachment.Id, attachment.AttachmentType.ToString()))).ToList<TestRunAttachmentModel>();
      return num != 0;
    }

    internal List<TestCaseResultModel> GetTestCaseResultForTestCaseId(int testCaseId)
    {
      TestManagementRequestContext testRequestContext = (TestManagementRequestContext) this.TestContext.TestRequestContext;
      if (testRequestContext.IsFeatureEnabled("TestManagement.Server.TestResultsForTestCaseInTesthub"))
        return this.QueryTestResultsForTestCaseId(testCaseId);
      ResultsStoreQuery resultsStoreQuery = this.TestContext.GetResultsStoreQuery(string.Format("SELECT * FROM TestResult WHERE TestCaseId = {0} ORDER BY CreationDate desc", (object) testCaseId));
      return this.ProcessTestCaseResults(Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.QueryWithSuiteDetails(testRequestContext, resultsStoreQuery, 200));
    }

    private List<TestCaseResultModel> QueryTestResultsForTestCaseId(int testCaseId) => this.QueryTestResultsForTestCaseIdInternal2(testCaseId);

    private List<TestCaseResultModel> QueryTestResultsForTestCaseIdInternal2(int testCaseId)
    {
      TestResultsQuery testResultsQuery = new TestResultsQuery()
      {
        ResultsFilter = new ResultsFilter()
        {
          AutomatedTestName = string.Empty,
          TestCaseId = testCaseId
        }
      };
      GuidAndString projectId = new GuidAndString(this.TestContext.ProjectName, this.TestContext.CurrentProjectGuid);
      List<TestCaseResultModel> list;
      if (!this.TestContext.TestRequestContext.RequestContext.IsFeatureEnabled("TestManagement.Server.EnableDirectTCMS2SCallFromTFS"))
      {
        list = this.ResultsHelper.GetTestResults(projectId, testResultsQuery).Results.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResultModel>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResultModel>) (result => new TestCaseResultModel(result))).ToList<TestCaseResultModel>();
      }
      else
      {
        new ResultsHelper((TestManagementRequestContext) this.TestContext.TestRequestContext).CheckForViewTestResultPermission(projectId.GuidId);
        TestResultsQuery testResultsQuery1 = TestManagementController.InvokeAction<TestResultsQuery>((Func<TestResultsQuery>) (() => this.TestResultsHttpClient.GetTestResultsByQueryAsync(testResultsQuery, projectId.GuidId, (object) null, new CancellationToken())?.Result));
        this.TestContext.TestRequestContext.RequestContext.CheckPermissionToReadPublicIdentityInfo();
        list = testResultsQuery1.Results.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResultModel>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, TestCaseResultModel>) (result => new TestCaseResultModel(result))).ToList<TestCaseResultModel>();
      }
      this.PopulateSuiteAndConfigurationDetails(list);
      this.PopulatePlanAndSuiteNames(list);
      return list;
    }

    private void PopulateSuiteAndConfigurationDetails(List<TestCaseResultModel> results)
    {
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source1 = new List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
      using (TestManagementDatabase managementDatabase = TestManagementDatabase.Create((TestManagementRequestContext) this.TestContext.TestRequestContext))
      {
        IEnumerable<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> source2 = results.Select<TestCaseResultModel, Tuple<int, int, int>>((Func<TestCaseResultModel, Tuple<int, int, int>>) (result => Tuple.Create<int, int, int>(result.PlanId, result.TestPointId, result.ConfigurationId))).Distinct<Tuple<int, int, int>>().Select<Tuple<int, int, int>, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>((Func<Tuple<int, int, int>, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) (tuple => new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult()
        {
          TestPlanId = tuple.Item1,
          TestPointId = tuple.Item2,
          ConfigurationId = tuple.Item3
        }));
        source1 = managementDatabase.FetchSuiteAndConfigurationDetails(this.TestContext.CurrentProjectGuid, source2.ToList<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>());
      }
      Dictionary<Tuple<int, int, int>, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> resultsToSuiteAndConfigDetails = source1.ToDictionary<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, Tuple<int, int, int>, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, Tuple<int, int, int>>) (details => Tuple.Create<int, int, int>(details.TestPlanId, details.TestPointId, details.ConfigurationId)), (Func<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>) (details => details));
      results.RemoveAll((Predicate<TestCaseResultModel>) (result => !resultsToSuiteAndConfigDetails.ContainsKey(Tuple.Create<int, int, int>(result.PlanId, result.TestPointId, result.ConfigurationId))));
      foreach (TestCaseResultModel result in results)
      {
        Tuple<int, int, int> key = Tuple.Create<int, int, int>(result.PlanId, result.TestPointId, result.ConfigurationId);
        Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult testCaseResult = resultsToSuiteAndConfigDetails[key];
        result.ConfigurationName = testCaseResult.ConfigurationName;
        result.SuiteId = testCaseResult.TestSuiteId;
        result.SuiteName = testCaseResult.SuiteName;
      }
    }

    internal List<TestResolutionStateModel> GetTestResolutionStates() => Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, 0, this.TestContext.ProjectName).Select<Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState, TestResolutionStateModel>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestResolutionState, TestResolutionStateModel>) (res => new TestResolutionStateModel(res))).ToList<TestResolutionStateModel>();

    internal List<TestFailureTypeModel> GetTestFailureTypes() => Microsoft.TeamFoundation.TestManagement.Server.TestFailureType.Query((TestManagementRequestContext) this.TestContext.TestRequestContext, -1, this.TestContext.ProjectName).Select<Microsoft.TeamFoundation.TestManagement.Server.TestFailureType, TestFailureTypeModel>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestFailureType, TestFailureTypeModel>) (falt => new TestFailureTypeModel(falt))).ToList<TestFailureTypeModel>();

    private List<TestCaseResultModel> ProcessTestCaseResults(List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> testCaseResults)
    {
      List<TestCaseResultModel> results = new List<TestCaseResultModel>();
      if (testCaseResults != null)
      {
        foreach (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult testCaseResult in testCaseResults)
          results.Add(new TestCaseResultModel(testCaseResult));
      }
      this.PopulateSuiteAndConfigurationDetails(results);
      this.PopulatePlanAndSuiteNames(results);
      this.PopulateDurationInMs(results);
      return results;
    }

    private void PopulateDurationInMs(List<TestCaseResultModel> results)
    {
      foreach (TestCaseResultModel result in results)
        result.Duration = (long) new TimeSpan(result.Duration).TotalMilliseconds;
    }

    internal TestResultsQuery GetTestResultsQuery(List<TestCaseResultIdAndRev> idAndRevs)
    {
      TestResultsQuery query = new TestResultsQuery();
      query.Results = (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      idAndRevs.ForEach((Action<TestCaseResultIdAndRev>) (idAndRev =>
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult testCaseResult = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
        {
          Id = idAndRev.Id.TestResultId,
          TestRun = new ShallowReference()
        };
        testCaseResult.TestRun.Id = idAndRev.Id.TestRunId.ToString();
        query.Results.Add(testCaseResult);
      }));
      QueryAdapterFactory queryAdapterFactory = new QueryAdapterFactory(this.TestContext);
      query.Fields = (IList<string>) (queryAdapterFactory.GetAdapter("TestResult") as TestResultQueryAdapter).GetDefaultColumnReferenceNames();
      return query;
    }

    internal Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult ConvertToServerTestCaseResult(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult result)
    {
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult serverTestCaseResult = new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult();
      serverTestCaseResult.TestCaseTitle = result.TestCaseTitle;
      int result1;
      int.TryParse(result.TestRun.Id, out result1);
      serverTestCaseResult.Id = new Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier(result1, result.Id);
      int result2;
      if (result.TestCase != null && int.TryParse(result.TestCase.Id, out result2))
        serverTestCaseResult.TestCaseId = result2;
      Microsoft.TeamFoundation.TestManagement.Client.TestOutcome result3;
      if (Enum.TryParse<Microsoft.TeamFoundation.TestManagement.Client.TestOutcome>(result.Outcome, out result3))
        serverTestCaseResult.Outcome = (byte) result3;
      serverTestCaseResult.Priority = (byte) result.Priority;
      serverTestCaseResult.Duration = (long) result.DurationInMs * 10000L;
      serverTestCaseResult.ConfigurationName = result.Configuration?.Name;
      int result4;
      if (int.TryParse(result.Configuration?.Id, out result4))
        serverTestCaseResult.ConfigurationId = result4;
      serverTestCaseResult.ComputerName = result.ComputerName;
      serverTestCaseResult.ErrorMessage = result.ErrorMessage;
      serverTestCaseResult.OwnerName = result.Owner != null ? result.Owner.DisplayName : string.Empty;
      serverTestCaseResult.Owner = Guid.Empty;
      Guid result5;
      if (Guid.TryParse(result.Owner?.Id, out result5))
        serverTestCaseResult.Owner = result5;
      return serverTestCaseResult;
    }

    internal static Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult ConvertTestResultToWebApiModel(
      TestManagerRequestContext context,
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult testCaseResult)
    {
      return new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult()
      {
        TestCase = new ShallowReference()
        {
          Id = testCaseResult.TestCaseId.ToString()
        },
        TestPoint = new ShallowReference()
        {
          Id = testCaseResult.TestPointId.ToString()
        },
        Configuration = new ShallowReference()
        {
          Id = testCaseResult.ConfigurationId.ToString(),
          Name = testCaseResult.ConfigurationName
        },
        Owner = new IdentityRef()
        {
          Id = testCaseResult.Owner.ToString(),
          DisplayName = context.Identities.GetUserDisplayName(testCaseResult.Owner)
        },
        RunBy = new IdentityRef()
        {
          Id = testCaseResult.RunBy.ToString(),
          DisplayName = context.Identities.GetUserDisplayName(testCaseResult.RunBy)
        },
        Id = testCaseResult.TestResultId,
        AutomatedTestName = testCaseResult.AutomatedTestName,
        AutomatedTestStorage = testCaseResult.AutomatedTestStorage,
        AutomatedTestId = testCaseResult.AutomatedTestId,
        AutomatedTestType = testCaseResult.AutomatedTestType,
        AutomatedTestTypeId = testCaseResult.AutomatedTestTypeId,
        Priority = (int) testCaseResult.Priority,
        TestCaseTitle = testCaseResult.TestCaseTitle,
        TestCaseRevision = testCaseResult.TestCaseRevision
      };
    }

    internal static Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult ConvertResultUpdateRequestToWebApiModel(
      ResultUpdateRequestModel resultUpdateRequestModel)
    {
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult webApiModel = new Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult();
      webApiModel.TestRun = new ShallowReference()
      {
        Id = resultUpdateRequestModel.TestRunId.ToString()
      };
      webApiModel.Id = resultUpdateRequestModel.TestResultId;
      if (resultUpdateRequestModel.TestCaseResult != null)
      {
        if (resultUpdateRequestModel.TestCaseResult.TestCaseId > 0)
          webApiModel.TestCase = new ShallowReference()
          {
            Id = resultUpdateRequestModel.TestCaseResult.TestCaseId.ToString()
          };
        if (resultUpdateRequestModel.TestCaseResult.ConfigurationId > 0)
          webApiModel.Configuration = new ShallowReference()
          {
            Id = resultUpdateRequestModel.TestCaseResult.ConfigurationId.ToString(),
            Name = resultUpdateRequestModel.TestCaseResult.ConfigurationName
          };
        if (resultUpdateRequestModel.TestCaseResult.TestPointId > int.MinValue)
          webApiModel.TestPoint = new ShallowReference()
          {
            Id = resultUpdateRequestModel.TestCaseResult.TestPointId.ToString()
          };
        if (resultUpdateRequestModel.TestCaseResult.PlanId > 0)
          webApiModel.TestPlan = new ShallowReference()
          {
            Id = resultUpdateRequestModel.TestCaseResult.PlanId.ToString()
          };
        if (resultUpdateRequestModel.TestCaseResult.State > (byte) 0)
          webApiModel.State = Enum.GetName(typeof (TestResultState), (object) resultUpdateRequestModel.TestCaseResult.State);
        webApiModel.Priority = (int) resultUpdateRequestModel.TestCaseResult.Priority;
        if (!string.IsNullOrEmpty(resultUpdateRequestModel.TestCaseResult.ComputerName))
          webApiModel.ComputerName = resultUpdateRequestModel.TestCaseResult.ComputerName;
        if (resultUpdateRequestModel.TestCaseResult.Owner != Guid.Empty)
          webApiModel.Owner = new IdentityRef()
          {
            Id = resultUpdateRequestModel.TestCaseResult.Owner.ToString()
          };
        if (resultUpdateRequestModel.TestCaseResult.RunBy != Guid.Empty)
          webApiModel.RunBy = new IdentityRef()
          {
            Id = resultUpdateRequestModel.TestCaseResult.RunBy.ToString(),
            DisplayName = resultUpdateRequestModel.TestCaseResult.RunByName
          };
        if (!string.IsNullOrEmpty(resultUpdateRequestModel.TestCaseResult.TestCaseTitle))
          webApiModel.TestCaseTitle = resultUpdateRequestModel.TestCaseResult.TestCaseTitle;
        if (resultUpdateRequestModel.TestCaseResult.TestCaseRevision > 0)
          webApiModel.TestCaseRevision = resultUpdateRequestModel.TestCaseResult.TestCaseRevision;
        if (resultUpdateRequestModel.TestCaseResult.FailureType > (byte) 0)
          webApiModel.FailureType = resultUpdateRequestModel.TestCaseResult.FailureTypeString;
        webApiModel.ResolutionStateId = resultUpdateRequestModel.TestCaseResult.ResolutionStateId;
        if (resultUpdateRequestModel.TestCaseResult.Outcome > (byte) 0)
          webApiModel.Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) resultUpdateRequestModel.TestCaseResult.Outcome);
        if (!string.IsNullOrEmpty(resultUpdateRequestModel.TestCaseResult.ErrorMessage))
          webApiModel.ErrorMessage = resultUpdateRequestModel.TestCaseResult.ErrorMessage;
        if (!string.IsNullOrEmpty(resultUpdateRequestModel.TestCaseResult.Comment))
          webApiModel.Comment = resultUpdateRequestModel.TestCaseResult.Comment;
        if (resultUpdateRequestModel.TestCaseResult.DateStarted != new DateTime())
          webApiModel.StartedDate = resultUpdateRequestModel.TestCaseResult.DateStarted;
        if (resultUpdateRequestModel.TestCaseResult.DateCompleted != new DateTime())
          webApiModel.CompletedDate = resultUpdateRequestModel.TestCaseResult.DateCompleted;
        if (resultUpdateRequestModel.TestCaseResult.Duration > 0L)
          webApiModel.DurationInMs = (double) resultUpdateRequestModel.TestCaseResult.Duration;
      }
      if (resultUpdateRequestModel.ActionResults != null && ((IEnumerable<TestActionResultModel>) resultUpdateRequestModel.ActionResults).Any<TestActionResultModel>())
      {
        webApiModel.IterationDetails = new List<TestIterationDetailsModel>();
        IEnumerable<IGrouping<int, TestActionResultModel>> groupings = ((IEnumerable<TestActionResultModel>) resultUpdateRequestModel.ActionResults).GroupBy<TestActionResultModel, int>((Func<TestActionResultModel, int>) (ar => ar.IterationId));
        TestResultParameterModel[] parameters = resultUpdateRequestModel.Parameters;
        IEnumerable<IGrouping<int, TestResultParameterModel>> source = parameters != null ? ((IEnumerable<TestResultParameterModel>) parameters).GroupBy<TestResultParameterModel, int>((Func<TestResultParameterModel, int>) (param => param.IterationId)) : (IEnumerable<IGrouping<int, TestResultParameterModel>>) null;
        foreach (IGrouping<int, TestActionResultModel> grouping1 in groupings)
        {
          IGrouping<int, TestActionResultModel> actionResultsGroup = grouping1;
          TestIterationDetailsModel iterationDetailsModel = new TestIterationDetailsModel()
          {
            ActionResults = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel>()
          };
          iterationDetailsModel.Id = actionResultsGroup.Key;
          foreach (TestActionResultModel actionResultModel1 in (IEnumerable<TestActionResultModel>) actionResultsGroup)
          {
            if (!string.IsNullOrEmpty(actionResultModel1.ActionPath))
            {
              List<Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel> actionResults = iterationDetailsModel.ActionResults;
              Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel actionResultModel2 = new Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel();
              actionResultModel2.IterationId = actionResultModel1.IterationId;
              actionResultModel2.ActionPath = actionResultModel1.ActionPath;
              Microsoft.TeamFoundation.TestManagement.WebApi.SharedStepModel sharedStepModel;
              if (actionResultModel1.SharedStepId <= 0)
              {
                sharedStepModel = (Microsoft.TeamFoundation.TestManagement.WebApi.SharedStepModel) null;
              }
              else
              {
                sharedStepModel = new Microsoft.TeamFoundation.TestManagement.WebApi.SharedStepModel();
                sharedStepModel.Id = actionResultModel1.SharedStepId;
                sharedStepModel.Revision = actionResultModel1.SharedStepRevision;
              }
              actionResultModel2.SharedStepModel = sharedStepModel;
              actionResultModel2.Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) actionResultModel1.Outcome);
              actionResultModel2.ErrorMessage = actionResultModel1.ErrorMessage;
              actionResultModel2.Comment = actionResultModel1.Comment;
              actionResultModel2.StartedDate = actionResultModel1.DateStarted;
              actionResultModel2.CompletedDate = actionResultModel1.DateCompleted;
              actionResultModel2.DurationInMs = (double) actionResultModel1.Duration;
              actionResults.Add(actionResultModel2);
            }
            else
            {
              iterationDetailsModel.Outcome = Enum.GetName(typeof (Microsoft.TeamFoundation.TestManagement.Client.TestOutcome), (object) actionResultModel1.Outcome);
              iterationDetailsModel.ErrorMessage = actionResultModel1.ErrorMessage;
              iterationDetailsModel.Comment = actionResultModel1.Comment;
              iterationDetailsModel.StartedDate = actionResultModel1.DateStarted;
              iterationDetailsModel.CompletedDate = actionResultModel1.DateCompleted;
              iterationDetailsModel.DurationInMs = (double) actionResultModel1.Duration;
            }
          }
          IGrouping<int, TestResultParameterModel> grouping2 = source.Where<IGrouping<int, TestResultParameterModel>>((Func<IGrouping<int, TestResultParameterModel>, bool>) (pg => pg.Key == actionResultsGroup.Key)).FirstOrDefault<IGrouping<int, TestResultParameterModel>>();
          if (grouping2 != null)
          {
            iterationDetailsModel.Parameters = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestResultParameterModel>();
            foreach (TestResultParameterModel resultParameterModel in (IEnumerable<TestResultParameterModel>) grouping2)
            {
              if (!string.IsNullOrEmpty(resultParameterModel.ActionPath))
                iterationDetailsModel.Parameters.Add(new Microsoft.TeamFoundation.TestManagement.WebApi.TestResultParameterModel()
                {
                  IterationId = resultParameterModel.IterationId,
                  ActionPath = resultParameterModel.ActionPath,
                  ParameterName = resultParameterModel.ParameterName,
                  Value = resultParameterModel.Expected,
                  DataType = resultParameterModel.DataType
                });
            }
          }
          webApiModel.IterationDetails.Add(iterationDetailsModel);
        }
      }
      return webApiModel;
    }

    private TestCaseResultWithActionResultModel ConvertWebApiTestResultToMvcModel(
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult webApiResult)
    {
      int id = webApiResult.Id;
      int result;
      int.TryParse(webApiResult.TestRun.Id, out result);
      TestCaseResultWithActionResultModel mvcModel1 = new TestCaseResultWithActionResultModel();
      mvcModel1.TestCaseResult = new TestCaseResultModel(webApiResult);
      TestResultHelper.PopulateDataRowCountsInTestResults((IEnumerable<TestCaseResultModel>) new List<TestCaseResultModel>()
      {
        mvcModel1.TestCaseResult
      }, this.TestContext);
      (List<TestActionResultModel> ActionResults, List<TestResultParameterModel> ParameterResults, List<TestAttachmentModel> Attachments) mvcModel2 = TestResultHelper.ConvertWebApiIterationDetailsToMvcModel(result, id, webApiResult);
      mvcModel1.TestActionResultDetailsModel = new TestActionResultDetailsModel(mvcModel2.ActionResults, mvcModel2.ParameterResults, mvcModel2.Attachments);
      return mvcModel1;
    }

    private List<TestCaseResultWithActionResultModel> ConvertServerTestResultToMvcModel(
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> results,
      List<TestActionResult> actionResults,
      List<TestResultParameter> testResultParmeters,
      List<TestResultAttachment> testResultAttachments)
    {
      List<TestCaseResultModel> testCaseResults = new List<TestCaseResultModel>();
      List<TestCaseResultWithActionResultModel> mvcModel = new List<TestCaseResultWithActionResultModel>();
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult result in results)
        testCaseResults.Add(new TestCaseResultModel(result));
      TestResultHelper.PopulateDataRowCountsInTestResults((IEnumerable<TestCaseResultModel>) testCaseResults, this.TestContext);
      foreach (TestCaseResultModel testCaseResultModel in testCaseResults)
        mvcModel.Add(new TestCaseResultWithActionResultModel()
        {
          TestActionResultDetailsModel = TestResultHelper.GetTestActionResultDetailsModel(actionResults, testResultParmeters, testResultAttachments),
          TestCaseResult = testCaseResultModel
        });
      return mvcModel;
    }

    internal static (List<TestActionResultModel> ActionResults, List<TestResultParameterModel> ParameterResults, List<TestAttachmentModel> Attachments) ConvertWebApiIterationDetailsToMvcModel(
      int runId,
      int resultId,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult webApiResult)
    {
      List<TestActionResultModel> actionResultModelList = new List<TestActionResultModel>();
      List<TestResultParameterModel> resultParameterModelList = new List<TestResultParameterModel>();
      List<TestAttachmentModel> testAttachmentModelList = new List<TestAttachmentModel>();
      foreach (TestIterationDetailsModel iterationDetail in webApiResult.IterationDetails)
      {
        Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel actionResultModel = new Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel();
        actionResultModel.IterationId = iterationDetail.Id;
        actionResultModel.Outcome = iterationDetail.Outcome;
        actionResultModel.ActionPath = string.Empty;
        actionResultModel.ErrorMessage = iterationDetail.ErrorMessage;
        actionResultModel.Comment = iterationDetail.Comment;
        actionResultModel.StartedDate = iterationDetail.StartedDate;
        actionResultModel.CompletedDate = iterationDetail.CompletedDate;
        actionResultModel.DurationInMs = iterationDetail.DurationInMs;
        Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel actionResult1 = actionResultModel;
        actionResultModelList.Add(new TestActionResultModel(runId, resultId, actionResult1));
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestActionResultModel actionResult2 in iterationDetail.ActionResults)
          actionResultModelList.Add(new TestActionResultModel(runId, resultId, actionResult2));
        foreach (Microsoft.TeamFoundation.TestManagement.WebApi.TestResultParameterModel parameter in iterationDetail.Parameters)
        {
          resultParameterModelList.Add(new TestResultParameterModel(runId, resultId, parameter));
          parameter.ActionPath = string.Empty;
          resultParameterModelList.Add(new TestResultParameterModel(runId, resultId, parameter));
        }
        foreach (TestCaseResultAttachmentModel attachment in iterationDetail.Attachments)
          testAttachmentModelList.Add(new TestAttachmentModel(runId, resultId, attachment));
      }
      return (actionResultModelList, resultParameterModelList, testAttachmentModelList);
    }

    private void PopulatePlanAndSuiteNames(List<TestCaseResultModel> results)
    {
      Dictionary<int, string> testPlanNames = this.GetTestPlanNames(results.Select<TestCaseResultModel, int>((Func<TestCaseResultModel, int>) (r => r.PlanId)).Distinct<int>());
      foreach (TestCaseResultModel result in results)
      {
        result.PlanName = testPlanNames[result.PlanId];
        if (!string.IsNullOrEmpty(result.SuiteName))
          result.SuiteName = result.SuiteName.Equals(TestManagementServerResources.RootSuiteTitle, StringComparison.OrdinalIgnoreCase) ? result.PlanName : result.SuiteName;
      }
    }

    private Dictionary<int, string> GetTestPlanNames(IEnumerable<int> testPlanIds)
    {
      Dictionary<int, string> testPlanNames = new Dictionary<int, string>();
      List<Microsoft.TeamFoundation.TestManagement.Server.TestPlan> source = Microsoft.TeamFoundation.TestManagement.Server.TestPlan.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, testPlanIds.Select<int, IdAndRev>((Func<int, IdAndRev>) (planId => new IdAndRev()
      {
        Id = planId
      })).ToArray<IdAndRev>(), new List<int>(), this.TestContext.ProjectName, false, false);
      foreach (int testPlanId in testPlanIds)
      {
        int planId = testPlanId;
        Microsoft.TeamFoundation.TestManagement.Server.TestPlan testPlan = source.Where<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>((Func<Microsoft.TeamFoundation.TestManagement.Server.TestPlan, bool>) (p => p.PlanId == planId)).FirstOrDefault<Microsoft.TeamFoundation.TestManagement.Server.TestPlan>();
        testPlanNames[planId] = testPlan == null ? (string) null : testPlan.Name;
      }
      return testPlanNames;
    }

    private static TestActionResultDetailsModel GetTestActionResultDetailsModel(
      List<TestActionResult> actionResults,
      List<TestResultParameter> resultParameters,
      List<TestResultAttachment> resultAttachments)
    {
      List<TestActionResultModel> list1 = actionResults.Select<TestActionResult, TestActionResultModel>((Func<TestActionResult, TestActionResultModel>) (actionResult => new TestActionResultModel(actionResult))).ToList<TestActionResultModel>();
      List<TestResultParameterModel> list2 = resultParameters.Select<TestResultParameter, TestResultParameterModel>((Func<TestResultParameter, TestResultParameterModel>) (parameter => new TestResultParameterModel(parameter))).ToList<TestResultParameterModel>();
      List<TestAttachmentModel> list3 = resultAttachments.Select<TestResultAttachment, TestAttachmentModel>((Func<TestResultAttachment, TestAttachmentModel>) (attachment => new TestAttachmentModel(attachment))).ToList<TestAttachmentModel>();
      List<TestResultParameterModel> parameters = list2;
      List<TestAttachmentModel> attachments = list3;
      return new TestActionResultDetailsModel(list1, parameters, attachments);
    }

    private static bool ShouldEndTestCaseResult(Microsoft.TeamFoundation.TestManagement.Client.TestOutcome testOutcome, TestResultState state) => testOutcome != Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Unspecified && testOutcome != Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.None && testOutcome != Microsoft.TeamFoundation.TestManagement.Client.TestOutcome.Paused && state != TestResultState.Completed;

    private static TestResultAttachmentIdentity CreateTestResultAttachmentIdentity(
      int attachmentId,
      int testRunId,
      int testResultId)
    {
      return new TestResultAttachmentIdentity()
      {
        AttachmentId = attachmentId,
        TestRunId = testRunId,
        TestResultId = testResultId
      };
    }

    internal void AssociateWorkItems(
      TestCaseResultIdentifierModel[] testCaseResultIdentifierModels,
      string[] workItemUris,
      string projectName)
    {
      Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.CreateAssociatedWorkItems((TestManagementRequestContext) this.TestContext.TestRequestContext, ((IEnumerable<TestCaseResultIdentifierModel>) testCaseResultIdentifierModels).Select<TestCaseResultIdentifierModel, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>((Func<TestCaseResultIdentifierModel, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) (resultIdModel => resultIdModel.CreateFromModel())).ToArray<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>(), workItemUris, projectName);
    }

    internal static void PopulateDataRowCountsInTestResults(
      IEnumerable<TestCaseResultModel> testCaseResults,
      TestManagerRequestContext TestContext)
    {
      Dictionary<int, WorkItemIdRevisionPair> idAndRevisionMap = TestResultHelper.GetTestCaseIdToIdAndRevisionMap(testCaseResults);
      Dictionary<int, WorkItemIdRevisionPair>.ValueCollection values = idAndRevisionMap.Values;
      IEnumerable<WorkItem> workItems = TestContext.TfsRequestContext.GetService<IWorkItemRemotableService>().GetWorkItems(TestContext.TfsRequestContext, (IEnumerable<int>) idAndRevisionMap.Keys, (IEnumerable<string>) new string[2]
      {
        "System.Id",
        WorkItemFieldNames.DataField
      });
      Dictionary<int, int> dictionary1;
      if (!(TestContext.TestRequestContext.WorkItemFieldDataHelper is WorkItemFieldDataHelper itemFieldDataHelper))
      {
        dictionary1 = (Dictionary<int, int>) null;
      }
      else
      {
        TfsTestManagementRequestContext testRequestContext = TestContext.TestRequestContext;
        IEnumerable<WorkItem> fieldData = workItems;
        IEnumerable<int> testCaseIds = values.Select<WorkItemIdRevisionPair, int>((Func<WorkItemIdRevisionPair, int>) (testCaseid => testCaseid.Id));
        dictionary1 = itemFieldDataHelper.GetDataRowCountMap((TestManagementRequestContext) testRequestContext, fieldData, testCaseIds);
      }
      Dictionary<int, int> dictionary2 = dictionary1;
      foreach (TestCaseResultModel testCaseResult in testCaseResults)
      {
        if (testCaseResult.TestCaseId > 0)
        {
          testCaseResult.DataRowCount = 0;
          int num = 0;
          if (dictionary2 != null && dictionary2.TryGetValue(testCaseResult.TestCaseId, out num))
            testCaseResult.DataRowCount = num;
        }
      }
    }

    private static Dictionary<int, WorkItemIdRevisionPair> GetTestCaseIdToIdAndRevisionMap(
      IEnumerable<TestCaseResultModel> testCaseResults)
    {
      return testCaseResults.GroupBy<TestCaseResultModel, int>((Func<TestCaseResultModel, int>) (tcr => tcr.TestCaseId)).Select<IGrouping<int, TestCaseResultModel>, TestCaseResultModel>((Func<IGrouping<int, TestCaseResultModel>, TestCaseResultModel>) (tcrg => tcrg.First<TestCaseResultModel>())).ToDictionary<TestCaseResultModel, int, WorkItemIdRevisionPair>((Func<TestCaseResultModel, int>) (tcr => tcr.TestCaseId), (Func<TestCaseResultModel, WorkItemIdRevisionPair>) (tcr => new WorkItemIdRevisionPair()
      {
        Id = tcr.TestCaseId,
        Revision = tcr.TestCaseRevision
      }));
    }

    private ResultUpdateResponseModel[] UpdateResults(
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> results,
      ResultUpdateRequestModel[] resultUpdateRequestModels)
    {
      TestResultHelper.UpdateResultFromModel(this.CreateResultMap(results), resultUpdateRequestModels);
      return ((IEnumerable<ResultUpdateResponse>) Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.Update((TestManagementRequestContext) this.TestContext.TestRequestContext, ((IEnumerable<ResultUpdateRequestModel>) resultUpdateRequestModels).Select<ResultUpdateRequestModel, ResultUpdateRequest>((Func<ResultUpdateRequestModel, ResultUpdateRequest>) (update => update.CreateFromModel())).ToArray<ResultUpdateRequest>(), this.TestContext.ProjectName)).Select<ResultUpdateResponse, ResultUpdateResponseModel>((Func<ResultUpdateResponse, ResultUpdateResponseModel>) (response => new ResultUpdateResponseModel(response))).ToArray<ResultUpdateResponseModel>();
    }

    private ResultUpdateResponseModel[] UpdateInternal(ResultUpdateRequestModel[] updateRequests) => this.UpdateResults(Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult.Fetch((TestManagementRequestContext) this.TestContext.TestRequestContext, TestResultHelper.GetResultIdsToFetch(((IEnumerable<ResultUpdateRequestModel>) updateRequests).Select<ResultUpdateRequestModel, TestCaseResultModel>((Func<ResultUpdateRequestModel, TestCaseResultModel>) (req => req.TestCaseResult)).ToArray<TestCaseResultModel>()), this.TestContext.ProjectName, false, (List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier>) null, out List<TestActionResult> _, out List<TestResultParameter> _, out List<TestResultAttachment> _), updateRequests);

    private bool TryUpdatedResultsInTCM(
      ResultUpdateRequestModel[] resultUpdateRequestModels,
      out ResultUpdateResponseModel[] updatedResults)
    {
      updatedResults = (ResultUpdateResponseModel[]) null;
      Guid projectGuidFromName = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] fromResultRequest = this.GetTestCaseResultsFromResultRequest(resultUpdateRequestModels);
      int testRunId = resultUpdateRequestModels[0].TestRunId;
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> updatedResults1;
      if (!this.TestContext.TestRequestContext.TcmServiceHelper.TryUpdateTestResults(this.TestContext.TfsRequestContext, projectGuidFromName, testRunId, ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) fromResultRequest).ToArray<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(), out updatedResults1))
        return false;
      updatedResults = this.UpdateLastDateAndPointTable(updatedResults1, updatedResults, fromResultRequest);
      return true;
    }

    private ResultUpdateResponseModel[] UpdatedResultsInTCMAsync(
      ResultUpdateRequestModel[] resultUpdateRequestModels)
    {
      ResultUpdateResponseModel[] updatedResults = (ResultUpdateResponseModel[]) null;
      Guid projectId = Microsoft.TeamFoundation.TestManagement.Server.Validator.CheckAndGetProjectGuidFromName((TestManagementRequestContext) this.TestContext.TestRequestContext, this.TestContext.ProjectName);
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] testResultsToUpdate = this.GetTestCaseResultsFromResultRequest(resultUpdateRequestModels);
      int testRunId = resultUpdateRequestModels[0].TestRunId;
      return this.UpdateLastDateAndPointTable(TestManagementController.InvokeAction<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>((Func<List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>>) (() => this.TestResultsHttpClient.UpdateTestResultsAsync(testResultsToUpdate, projectId, testRunId, (object) null, new CancellationToken())?.Result)), updatedResults, testResultsToUpdate);
    }

    private Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] GetTestCaseResultsFromResultRequest(
      ResultUpdateRequestModel[] resultUpdateRequestModels)
    {
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> testCaseResultList = new List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>();
      if (resultUpdateRequestModels != null && resultUpdateRequestModels.Length != 0)
      {
        foreach (ResultUpdateRequestModel updateRequestModel in resultUpdateRequestModels)
        {
          if (updateRequestModel != null)
            testCaseResultList.Add(TestResultHelper.ConvertResultUpdateRequestToWebApiModel(updateRequestModel));
        }
      }
      return testCaseResultList.ToArray();
    }

    private ResultUpdateResponseModel[] UpdateLastDateAndPointTable(
      List<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult> results,
      ResultUpdateResponseModel[] updatedResults,
      Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult[] testResultsToUpdate)
    {
      PlannedResultsTCMServiceHelper tcmServiceHelper = this.TestContext.TestRequestContext.PlannedTestingTCMServiceHelper;
      TestPointOutcomeUpdateRequestConverter.UpdateWebApiResult((IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) ((IEnumerable<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testResultsToUpdate).ToList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>(), (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) results);
      this.TestContext.TestRequestContext.TestPointOutcomeHelper.UpdateTestPointOutcomeFromWebApi(this.TestContext.TfsRequestContext, this.TestContext.ProjectName, (IList<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult>) testResultsToUpdate);
      updatedResults = results.Select<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, ResultUpdateResponseModel>((Func<Microsoft.TeamFoundation.TestManagement.WebApi.TestCaseResult, ResultUpdateResponseModel>) (result => new ResultUpdateResponseModel(new ResultUpdateResponse()
      {
        Revision = result.Revision
      }))).ToArray<ResultUpdateResponseModel>();
      return updatedResults;
    }

    private static TestCaseResultIdAndRev[] GetResultIdsToFetch(TestCaseResultModel[] testResults) => ((IEnumerable<TestCaseResultModel>) testResults).Select<TestCaseResultModel, TestCaseResultIdAndRev>((Func<TestCaseResultModel, TestCaseResultIdAndRev>) (result => new TestCaseResultIdAndRev(result.Id.CreateFromModel(), 0))).ToArray<TestCaseResultIdAndRev>();

    private static void UpdateResultFromModel(
      Dictionary<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> resultMap,
      ResultUpdateRequestModel[] resultUpdateRequestModels)
    {
      foreach (ResultUpdateRequestModel updateRequestModel in resultUpdateRequestModels)
      {
        Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier fromModel = updateRequestModel.TestCaseResult.Id.CreateFromModel();
        if (resultMap.ContainsKey(fromModel))
          resultMap[fromModel] = updateRequestModel.TestCaseResult.UpdateFromModel(resultMap[fromModel]);
      }
    }

    private Dictionary<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> CreateResultMap(
      List<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> results)
    {
      Dictionary<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult> resultMap = new Dictionary<Microsoft.TeamFoundation.TestManagement.Server.TestCaseResultIdentifier, Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult>();
      foreach (Microsoft.TeamFoundation.TestManagement.Server.TestCaseResult result in results)
        resultMap[result.Id] = result;
      return resultMap;
    }

    internal ResultsHelper ResultsHelper
    {
      get
      {
        if (this.m_resultsHelper == null)
          this.m_resultsHelper = new ResultsHelper((TestManagementRequestContext) this.TestContext.TestRequestContext);
        return this.m_resultsHelper;
      }
    }

    internal TestResultsHttpClient TestResultsHttpClient => this.TestContext.TestRequestContext.RequestContext.GetClient<TestResultsHttpClient>();

    internal TestResultsHttpClient TestResultsHttpClientWithSqlReadOnly(string FeatureFlag) => this.TestContext.TestRequestContext.RequestContext.GetClient<TestResultsHttpClient>(this.TestContext.TestRequestContext.RequestContext.GetHttpClientOptionsForEventualReadConsistencyLevel(FeatureFlag));
  }
}
