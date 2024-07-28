// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase87
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase87 : TestManagementDatabase86
  {
    internal override bool GetAppendAttachmentStatusForUploadedFileV2(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int sessionId,
      int attachmentId,
      bool isFileUploadComplete)
    {
      this.PrepareStoredProcedure("TestResult.prc_AppendAttachmentV2");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@attachmentId", attachmentId);
      this.BindBoolean("@isComplete", isFileUploadComplete);
      this.BindInt("@sessionId", sessionId);
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new TestManagementDatabase.AppendAttachmentColumns().CoverageChanged.GetBoolean((IDataReader) reader, false) : throw new UnexpectedDatabaseResultException("prc_AppendAttachmentV2");
    }

    internal override List<TestResultAttachment> QueryAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int subResultId = 0,
      bool shouldFallBackToOldAttachment = false)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      this.PrepareStoredProcedure("prc_QueryLogStoreAttachmentsV2");
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
      return resultAttachmentList;
    }

    internal override Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachmentMapping(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      if (attachments == null || !attachments.Any<TestResultAttachment>())
        return new Dictionary<TestCaseResultIdentifier, List<int>>();
      this.PrepareStoredProcedure("prc_CreateLogStoreAttachmentMappingV2");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindLogStoreAttachmentMappingTable("@attachments", attachments);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException("prc_CreateLogStoreAttachmentMapping");
      List<int> intList1 = new List<int>();
      TestManagementDatabase.CreateAttachmentColumns attachmentColumns = new TestManagementDatabase.CreateAttachmentColumns();
      do
      {
        intList1.Add(attachmentColumns.AttachmentId.GetInt32((IDataReader) reader));
      }
      while (reader.Read());
      Dictionary<TestCaseResultIdentifier, List<int>> attachmentMapping = new Dictionary<TestCaseResultIdentifier, List<int>>();
      int num = 0;
      foreach (TestResultAttachment attachment in attachments)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(attachment.TestRunId, attachment.TestResultId);
        List<int> intList2;
        if (!attachmentMapping.TryGetValue(key, out intList2))
        {
          intList2 = new List<int>();
          attachmentMapping.Add(key, intList2);
        }
        intList2.Add(intList1[num++]);
      }
      return attachmentMapping;
    }

    internal TestManagementDatabase87(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase87()
    {
    }

    public override int DeleteStaleTestCaseReferenceByProject(
      int batchSize,
      int staleTestCaseRefSprocExecTimeOutInSec,
      Guid projectId,
      int daysOlderToCleanupStaleTestCaseRef)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.DeleteStaleTestCaseReferenceByProject");
        this.PrepareStoredProcedure("TestResult.prc_DeleteStaleTestCaseReferenceByProject", staleTestCaseRefSprocExecTimeOutInSec);
        this.BindInt("@batchSize", batchSize);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@daysOlderToCleanupStaleTestCaseRef", daysOlderToCleanupStaleTestCaseRef);
        return (int) this.ExecuteScalar();
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.DeleteStaleTestCaseReferenceByProject");
        this.RequestContext.TraceAlways(1015919, TraceLevel.Info, "TestManagement", "Database", string.Format("TestResultDatabase.DeleteStaleTestCaseReferenceByProject: ProjectId={0}, BatchSize={1}, ", (object) projectId, (object) batchSize) + string.Format("DaysOlderToCleanupStaleTestCaseRef={0}", (object) daysOlderToCleanupStaleTestCaseRef));
      }
    }
  }
}
