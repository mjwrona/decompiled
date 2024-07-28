// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase101
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.Client;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase101 : TestManagementDatabase100
  {
    internal override List<TestResultAttachment> CreateLogStoreTestAttachmentIdMappings(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      try
      {
        this.RequestContext.TraceEnter(1015853, "TestManagement", "Database", "TestManagementDatabase.CreateLogStoreTestAttachmentIdMappings");
        List<TestResultAttachment> attachmentIdMappings = new List<TestResultAttachment>();
        int testRunId = attachments.First<TestResultAttachment>().TestRunId;
        int testResultId = attachments.First<TestResultAttachment>().TestResultId;
        this.AssignTempIdToAttachments(attachments, true, 1);
        this.PrepareStoredProcedure("TestResult.prc_CreateLogStoreAttachmentIdMappings");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@inputBatchSize", attachments.Count<TestResultAttachment>());
        this.BindInt("@testRunId", testRunId);
        this.BindInt("@testResultId", testResultId);
        this.BindTestResultAttachmentTypeTable5("@attachments", attachments);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException("prc_CreateLogStoreAttachmentIdMappings");
        TestManagementDatabase.QueryAttachmentIdMappingColumns idMappingColumns = new TestManagementDatabase.QueryAttachmentIdMappingColumns();
        do
        {
          attachmentIdMappings.Add(idMappingColumns.Bind(reader));
        }
        while (reader.Read());
        return attachmentIdMappings;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceError("Database", "prc_CreateLogStoreAttachmentIdMappings threw exception. ProjectId: {0}, Exception message: {1}", (object) projectId, (object) ex.Message);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(1015853, "TestManagement", "Database", "TestManagementDatabase.CreateLogStoreTestAttachmentIdMappings");
      }
    }

    internal override TestResultAttachment GetLogStoreTestAttachmentIdMapping(
      Guid projectId,
      int runId,
      int resultId,
      int subResultId,
      int attachmentId)
    {
      try
      {
        this.RequestContext.TraceEnter(1015854, "TestManagement", "Database", "TestManagementDatabase.GetLogStoreTestAttachmentIdMapping");
        this.PrepareStoredProcedure("TestResult.prc_GetLogStoreAttachmentIdMapping");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", runId);
        this.BindInt("@testResultId", resultId);
        this.BindInt("@subResultId", subResultId);
        this.BindInt("@attachmentId", attachmentId);
        SqlDataReader reader = this.ExecuteReader();
        return reader.Read() ? new TestManagementDatabase.QueryAttachmentIdMappingColumns().Bind(reader) : throw new TestObjectNotFoundException(this.RequestContext, attachmentId, ObjectTypes.Attachment);
      }
      catch (TestObjectNotFoundException ex)
      {
        this.RequestContext.TraceError("Database", "prc_GetLogStoreAttachmentIdMapping threw exception. ProjectId: {0}, RunId: {1}, ResultId: {2}, SubResultId: {3}, attachmentId: {4} does not exist. Exception message: {5}", (object) projectId, (object) runId, (object) resultId, (object) subResultId, (object) attachmentId, (object) ex.Message);
        return new TestResultAttachment();
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceError("Database", "prc_GetLogStoreAttachmentIdMapping threw exception. ProjectId: {0}, RunId: {1}, ResultId: {2}, SubResultId: {3}, attachmentId: {4}, Exception message: {5}", (object) projectId, (object) runId, (object) resultId, (object) subResultId, (object) attachmentId, (object) ex.Message);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(1015854, "TestManagement", "Database", "TestManagementDatabase.GetLogStoreTestAttachmentIdMapping");
      }
    }

    internal TestManagementDatabase101(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase101()
    {
    }
  }
}
