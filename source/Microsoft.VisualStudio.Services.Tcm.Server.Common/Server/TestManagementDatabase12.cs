// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase12
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase12 : TestManagementDatabase11
  {
    internal override Dictionary<TestCaseResultIdentifier, List<int>> CreateAttachments(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments,
      bool areSessionAttachments,
      bool changeCounterInterval = false)
    {
      this.AssignTempIdToAttachments(attachments, changeCounterInterval);
      this.PrepareStoredProcedure("TestResult.prc_CreateAttachments");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestResultAttachmentTypeTable2("@attachments", attachments);
      this.BindBoolean("@areSessionAttachments", areSessionAttachments);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException("prc_CreateAttachments");
      List<int> intList1 = new List<int>();
      TestManagementDatabase.CreateAttachmentColumns attachmentColumns = new TestManagementDatabase.CreateAttachmentColumns();
      do
      {
        intList1.Add(attachmentColumns.AttachmentId.GetInt32((IDataReader) reader));
      }
      while (reader.Read());
      Dictionary<TestCaseResultIdentifier, List<int>> attachments1 = new Dictionary<TestCaseResultIdentifier, List<int>>();
      int num = 0;
      foreach (TestResultAttachment attachment in attachments)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(attachment.TestRunId, attachment.TestResultId);
        List<int> intList2;
        if (!attachments1.TryGetValue(key, out intList2))
        {
          intList2 = new List<int>();
          attachments1.Add(key, intList2);
        }
        intList2.Add(intList1[num++]);
      }
      return attachments1;
    }

    internal override void DeleteAttachments(
      Guid projectId,
      IEnumerable<TestResultAttachmentIdentity> attachments)
    {
      try
      {
        this.RequestContext.TraceEnter(1015851, "TestManagement", "Database", "TestManagementDatabase.DeleteAttachments");
        this.PrepareStoredProcedure("TestResult.prc_DeleteAttachments");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindTestResultAttachmentIdentityTypeTable("@attachments", attachments);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceError("Database", "TestResult.DeleteAttachments threw exception. ProjectId: {0}, Exception message: {1}", (object) projectId, (object) ex.Message);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(1015851, "TestManagement", "Database", "TestManagementDatabase.DeleteAttachments");
      }
    }

    internal override void UpdateDefaultStrip(
      Guid projectId,
      int testRunId,
      int testResultId,
      int testCaseId)
    {
      this.PrepareStoredProcedure("TestManagement.prc_UpdateDefaultStrip");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@testCaseId", testCaseId);
      this.ExecuteNonQuery();
    }

    internal TestManagementDatabase12(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase12()
    {
    }

    internal override void CreateAssociatedWorkItems(
      TestCaseResultIdentifier[] identifiers,
      string[] workItemUris,
      Guid projectId)
    {
      this.PrepareStoredProcedure("TestResult.prc_CreateAssociatedWorkItems");
      this.BindInt("@dataspaceId", projectId.Equals(Guid.Empty) ? 0 : this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindTestCaseResultWorkItemLinkTypeTable("@workItemLinksTable", (IEnumerable<KeyValuePair<TestCaseResultIdentifier, string>>) this.GetListOfUris<TestCaseResultIdentifier>(identifiers, workItemUris));
      this.ExecuteNonQuery();
    }

    public override List<TestCaseResult> FetchTestResultsWithSuiteDetails(
      TestCaseResultIdAndRev[] resultIds,
      Guid projectId)
    {
      try
      {
        this.RequestContext.TraceEnter(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResults");
        this.PrepareStoredProcedure("TestManagement.prc_FetchTestResultsWithSuiteDetails");
        this.BindTestCaseResultIdAndRevTypeTable("@idsTable", (IEnumerable<TestCaseResultIdAndRev>) resultIds);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@maxCommentSize", TestManagementServiceUtility.GetMaxLengthForResultComment(this.RequestContext));
        List<TestCaseResult> testCaseResultList = new List<TestCaseResult>();
        TestManagementDatabase.FetchTestResultsColumns testResultsColumns = new TestManagementDatabase.FetchTestResultsColumns();
        using (SqlDataReader reader = this.ExecuteReader())
        {
          List<string> stringList = new List<string>();
          while (reader.Read())
            testCaseResultList.Add(testResultsColumns.bind(reader));
        }
        return testCaseResultList;
      }
      finally
      {
        this.RequestContext.TraceLeave(0, "TestManagement", "Database", "TestResultDatabase.FetchTestResultsWithSuiteDetails");
      }
    }
  }
}
