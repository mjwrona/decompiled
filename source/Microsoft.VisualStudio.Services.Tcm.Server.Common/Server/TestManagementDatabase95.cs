// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase95
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase95 : TestManagementDatabase94
  {
    internal override List<int> CreateTestAttachmentsInLogStoreMapper(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      Dictionary<TestCaseResultIdentifier, List<int>> dictionary = new Dictionary<TestCaseResultIdentifier, List<int>>();
      try
      {
        this.RequestContext.TraceEnter(1015852, "TestManagement", "Database", "TestManagementDatabase.CreateTestAttachmentsInLogStoreMapper");
        this.AssignTempIdToAttachments(attachments, true);
        this.PrepareStoredProcedure("TestResult.prc_CreateAttachmentsInLogStoreMapper");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindTestResultAttachmentTypeTable3("@attachments", attachments);
        SqlDataReader reader = this.ExecuteReader();
        if (!reader.Read())
          throw new UnexpectedDatabaseResultException("prc_CreateAttachmentsInLogStoreMapper");
        List<int> inLogStoreMapper = new List<int>();
        TestManagementDatabase.CreateAttachmentColumns attachmentColumns = new TestManagementDatabase.CreateAttachmentColumns();
        do
        {
          inLogStoreMapper.Add(attachmentColumns.AttachmentId.GetInt32((IDataReader) reader));
        }
        while (reader.Read());
        return inLogStoreMapper;
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceError("Database", "prc_CreateAttachmentsInLogStoreMapper threw exception. ProjectId: {0}, Exception message: {1}", (object) projectId, (object) ex.Message);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(1015852, "TestManagement", "Database", "TestManagementDatabase.CreateTestAttachmentsInLogStoreMapper");
      }
    }

    internal override void UpdateAttachmentInLogStoreAttachmentMapper(
      Guid projectId,
      int runId,
      int resultId,
      int attachmentId,
      long uncompressedLength,
      string filename)
    {
      try
      {
        this.RequestContext.TraceEnter(1015852, "TestManagement", "Database", "TestManagementDatabase.UpdateAttachmentInLogStoreAttachmentMapper");
        this.PrepareStoredProcedure("TestResult.prc_UpdateAttachmentInLogStoreAttachmentMapper");
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@testRunId", runId);
        this.BindInt("@testResultId", resultId);
        this.BindInt("@attachmentId", attachmentId);
        this.BindString("@filename", filename, 256, false, SqlDbType.NVarChar);
        this.BindLong("@uncompressedLength", uncompressedLength);
        this.ExecuteNonQuery();
      }
      catch (Exception ex)
      {
        this.RequestContext.TraceError("Database", "prc_UpdateAttachmentInLogStoreAttachmentMapper threw exception. ProjectId: {0}, Exception message: {1}", (object) projectId, (object) ex.Message);
        throw;
      }
      finally
      {
        this.RequestContext.TraceLeave(1015852, "TestManagement", "Database", "TestManagementDatabase.UpdateAttachmentInLogStoreAttachmentMapper");
      }
    }

    internal TestManagementDatabase95(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase95()
    {
    }

    public override int DeleteFlakyTestData(
      Guid projectId,
      int batchSize,
      int deleteFlakyTestDataSprocExecTimeOutInSec)
    {
      try
      {
        this.RequestContext.TraceEnter(1015102, "TestManagement", "Database", "TestResultDatabase.DeleteFlakyTestData");
        this.PrepareStoredProcedure("TestResult.prc_DeleteFlakyTestData", deleteFlakyTestDataSprocExecTimeOutInSec);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@batchSize", batchSize);
        return (int) this.ExecuteScalar();
      }
      catch (SqlException ex)
      {
        this.MapException(ex);
      }
      finally
      {
        this.RequestContext.TraceLeave(1015102, "TestManagement", "Database", "TestResultDatabase.DeleteFlakyTestData");
        this.RequestContext.TraceAlways(1015102, TraceLevel.Info, "TestManagement", "Database", string.Format("TestResultDatabase.DeleteFlakyTestData: ProjectId={0}, BatchSize={1}", (object) projectId, (object) batchSize));
      }
      return 0;
    }
  }
}
