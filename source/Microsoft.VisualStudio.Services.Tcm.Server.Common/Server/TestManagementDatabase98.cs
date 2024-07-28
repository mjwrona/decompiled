// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase98
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
  public class TestManagementDatabase98 : TestManagementDatabase97
  {
    internal override bool GetAppendAttachmentStatusForUploadedFileInLogStore(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int sessionId,
      int attachmentId,
      bool isFileUploadComplete)
    {
      this.PrepareStoredProcedure("TestResult.prc_AppendLogStoreAttachment");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@attachmentId", attachmentId);
      this.BindBoolean("@isComplete", isFileUploadComplete);
      this.BindInt("@sessionId", sessionId);
      SqlDataReader reader = this.ExecuteReader();
      return reader.Read() ? new TestManagementDatabase.AppendAttachmentColumns().CoverageChanged.GetBoolean((IDataReader) reader, false) : throw new UnexpectedDatabaseResultException("TestResult.prc_AppendLogStoreAttachment");
    }

    internal override List<TestResultAttachment> QueryUploadedAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int attachmentId,
      int subResultId = 0,
      bool shouldFallBackToOldAttachment = false)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      this.PrepareStoredProcedure("prc_QueryUploadedLogStoreAttachments");
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

    internal TestManagementDatabase98(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase98()
    {
    }
  }
}
