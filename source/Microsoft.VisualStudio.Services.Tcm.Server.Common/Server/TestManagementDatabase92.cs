// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase92
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase92 : TestManagementDatabase91
  {
    internal override Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachmentMappingV2(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      if (attachments == null || !attachments.Any<TestResultAttachment>())
        return new Dictionary<TestCaseResultIdentifier, List<int>>();
      this.PrepareStoredProcedure("prc_CreateLogStoreAttachmentMappingV3");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindLogStoreAttachmentMappingV2Table("@attachments", attachments);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException("prc_CreateLogStoreAttachmentMappingV3");
      List<int> intList1 = new List<int>();
      TestManagementDatabase.CreateAttachmentColumns attachmentColumns = new TestManagementDatabase.CreateAttachmentColumns();
      do
      {
        intList1.Add(attachmentColumns.AttachmentId.GetInt32((IDataReader) reader));
      }
      while (reader.Read());
      Dictionary<TestCaseResultIdentifier, List<int>> attachmentMappingV2 = new Dictionary<TestCaseResultIdentifier, List<int>>();
      int num = 0;
      foreach (TestResultAttachment attachment in attachments)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(attachment.TestRunId, attachment.TestResultId);
        List<int> intList2;
        if (!attachmentMappingV2.TryGetValue(key, out intList2))
        {
          intList2 = new List<int>();
          attachmentMappingV2.Add(key, intList2);
        }
        intList2.Add(intList1[num++]);
      }
      return attachmentMappingV2;
    }

    internal override List<TestResultAttachment> QueryAttachmentsV2(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int subResultId = 0,
      bool shouldFallBackToOldAttachment = false)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "LogStorage", nameof (QueryAttachmentsV2));
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      this.PrepareStoredProcedure("prc_QueryLogStoreAttachmentsV3");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindNullableInt("@attachmentId", attachmentId, 0);
      this.BindInt("@subResultId", subResultId);
      this.BindBoolean("@shouldFallBackToOldAttachment", shouldFallBackToOldAttachment);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryLogStoreAttachmentsColumnsV2 attachmentsColumnsV2 = new TestManagementDatabase.QueryLogStoreAttachmentsColumnsV2();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumnsV2.Bind(reader));
      this.RequestContext.TraceLeave(0, "TestManagement", "LogStorage", nameof (QueryAttachmentsV2));
      return resultAttachmentList;
    }

    internal override List<TestResultAttachment> QueryIterationAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int iterationId)
    {
      this.RequestContext.TraceEnter(0, "TestManagement", "LogStorage", nameof (QueryIterationAttachments));
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      this.PrepareStoredProcedure("prc_QueryLogStoreAttachmentsByIterationId");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindNullableInt("@attachmentId", attachmentId, 0);
      this.BindInt("@iterationId", iterationId);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryLogStoreAttachmentsColumnsV3 attachmentsColumnsV3 = new TestManagementDatabase.QueryLogStoreAttachmentsColumnsV3();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumnsV3.Bind(reader));
      this.RequestContext.TraceLeave(0, "TestManagement", "LogStorage", nameof (QueryIterationAttachments));
      return resultAttachmentList;
    }

    internal TestManagementDatabase92(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase92()
    {
    }

    internal override TestCaseResult FetchTestResultV2(
      TestCaseResultIdentifier resultId,
      Guid projectId,
      bool includeActionResults,
      out List<TestActionResult> actionResults,
      out List<TestResultParameter> parameters,
      out List<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResultV2");
        this.PrepareStoredProcedure("TestResult.prc_FetchTestResultV2");
        this.BindBoolean("@includeActionResults", includeActionResults);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", resultId.TestRunId);
        this.BindInt("@testResultId", resultId.TestResultId);
        this.BindInt("@maxCommentSize", TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext));
        TestCaseResult testCaseResult = new TestCaseResult();
        TestManagementDatabase.FetchTestResultsColumns testResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          while (reader.Read())
            testCaseResult = testResultsColumns.bind(reader);
          if (!reader.NextResult())
            throw new UnexpectedDatabaseResultException("TestResult.prc_FetchTestResultV2");
          testCaseResult.CustomFields = new List<TestExtensionField>();
          TestManagementDatabase.FetchTestResultsExColumns resultsExColumns = new TestManagementDatabase.FetchTestResultsExColumns();
          while (reader.Read())
          {
            Tuple<int, int, TestExtensionField> tuple = resultsExColumns.bind(reader);
            string name = tuple.Item3.Field.Name;
            if (name != null)
            {
              if (string.Equals("StackTrace", name, StringComparison.OrdinalIgnoreCase))
              {
                testCaseResult.StackTrace = tuple.Item3;
                continue;
              }
              if (string.Equals("FailingSince", name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string jsonString)
                {
                  FailingSince convertedObject = (FailingSince) null;
                  if (TestResultHelper.TryJsonConvertWithRetry<FailingSince>(jsonString, out convertedObject, true))
                  {
                    testCaseResult.FailingSince = convertedObject;
                    continue;
                  }
                  continue;
                }
                continue;
              }
              if (string.Equals("Comment", name, StringComparison.OrdinalIgnoreCase))
              {
                testCaseResult.Comment = tuple.Item3.Value as string;
                continue;
              }
              if (string.Equals("ErrorMessage", name, StringComparison.OrdinalIgnoreCase))
              {
                testCaseResult.ErrorMessage = tuple.Item3.Value as string;
                continue;
              }
              if (string.Equals("UnsanitizedTestCaseTitle", name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str)
                {
                  testCaseResult.TestCaseTitle = str;
                  continue;
                }
                continue;
              }
              if (string.Equals("UnsanitizedAutomatedTestName", name, StringComparison.OrdinalIgnoreCase))
              {
                if (tuple.Item3.Value is string str)
                {
                  testCaseResult.AutomatedTestName = str;
                  continue;
                }
                continue;
              }
              if (string.Equals("MaxReservedSubResultId", name, StringComparison.OrdinalIgnoreCase))
              {
                testCaseResult.SubResultCount = (int) tuple.Item3.Value;
                continue;
              }
              if (string.Equals("TestResultGroupType", name, StringComparison.OrdinalIgnoreCase))
              {
                testCaseResult.ResultGroupType = (ResultGroupType) TestManagementServiceUtility.ValidateAndGetEnumValue<ResultGroupType>((string) tuple.Item3.Value, ResultGroupType.None);
                continue;
              }
            }
            testCaseResult.CustomFields.Add(tuple.Item3);
          }
          if (includeActionResults)
          {
            actionResults = new List<TestActionResult>();
            parameters = new List<TestResultParameter>();
            attachments = new List<TestResultAttachment>();
            this.ReadTestActionResults(actionResults, parameters, attachments, reader, "prc_FetchTestResultV2");
            if (!reader.NextResult())
              throw new UnexpectedDatabaseResultException("prc_FetchTestResultV2");
            TestManagementDatabase.QueryLogStoreAttachmentsColumnsV3 attachmentsColumnsV3 = new TestManagementDatabase.QueryLogStoreAttachmentsColumnsV3();
            while (reader.Read())
              attachments.Add(attachmentsColumnsV3.Bind(reader));
          }
          else
          {
            actionResults = (List<TestActionResult>) null;
            parameters = (List<TestResultParameter>) null;
            attachments = (List<TestResultAttachment>) null;
          }
        }
        return testCaseResult;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResultV2");
      }
    }
  }
}
