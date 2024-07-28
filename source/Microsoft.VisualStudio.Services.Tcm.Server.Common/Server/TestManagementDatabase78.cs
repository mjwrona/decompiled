// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase78
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using System;
using System.Collections.Generic;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase78 : TestManagementDatabase77
  {
    internal override List<TestResultAttachment> QueryLogStoreAttachments(
      TestManagementRequestContext context,
      Guid projectId,
      int testRunId,
      int testResultId,
      int subResultId = 0,
      bool shouldFallBackToOldAttachment = false)
    {
      List<TestResultAttachment> resultAttachmentList = new List<TestResultAttachment>();
      this.PrepareStoredProcedure("prc_QueryLogStoreAttachments");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindInt("@testRunId", testRunId);
      this.BindInt("@testResultId", testResultId);
      this.BindInt("@subResultId", subResultId);
      this.BindBoolean("@shouldFallBackToOldAttachment", shouldFallBackToOldAttachment);
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryLogStoreAttachmentsColumns attachmentsColumns = new TestManagementDatabase.QueryLogStoreAttachmentsColumns();
      if (reader.HasRows)
      {
        while (reader.Read())
          resultAttachmentList.Add(attachmentsColumns.Bind(reader));
      }
      else if (reader.NextResult() && reader.HasRows)
      {
        while (reader.Read())
          resultAttachmentList.Add(attachmentsColumns.Bind(reader));
      }
      return resultAttachmentList;
    }

    internal TestManagementDatabase78(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase78()
    {
    }
  }
}
