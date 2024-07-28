// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase73
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase73 : TestManagementDatabase72
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
      SqlDataReader reader = this.ExecuteReader();
      TestManagementDatabase.QueryLogStoreAttachmentsColumns attachmentsColumns = new TestManagementDatabase.QueryLogStoreAttachmentsColumns();
      while (reader.Read())
        resultAttachmentList.Add(attachmentsColumns.Bind(reader));
      return resultAttachmentList;
    }

    internal override Dictionary<TestCaseResultIdentifier, List<long>> CreateLogStoreAttachmentMapping(
      Guid projectId,
      IEnumerable<TestResultAttachment> attachments)
    {
      if (attachments == null || !attachments.Any<TestResultAttachment>())
        return new Dictionary<TestCaseResultIdentifier, List<long>>();
      this.PrepareStoredProcedure("prc_CreateLogStoreAttachmentMapping");
      this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
      this.BindLogStoreAttachmentTable("@attachments", attachments);
      SqlDataReader reader = this.ExecuteReader();
      if (!reader.Read())
        throw new UnexpectedDatabaseResultException("prc_CreateLogStoreAttachmentMapping");
      List<long> longList1 = new List<long>();
      TestManagementDatabase.CreateAttachmentColumns attachmentColumns = new TestManagementDatabase.CreateAttachmentColumns();
      do
      {
        longList1.Add(attachmentColumns.AttachmentId.GetInt64((IDataReader) reader));
      }
      while (reader.Read());
      Dictionary<TestCaseResultIdentifier, List<long>> attachmentMapping = new Dictionary<TestCaseResultIdentifier, List<long>>();
      int num = 0;
      foreach (TestResultAttachment attachment in attachments)
      {
        TestCaseResultIdentifier key = new TestCaseResultIdentifier(attachment.TestRunId, attachment.TestResultId);
        List<long> longList2;
        if (!attachmentMapping.TryGetValue(key, out longList2))
        {
          longList2 = new List<long>();
          attachmentMapping.Add(key, longList2);
        }
        longList2.Add(longList1[num++]);
      }
      return attachmentMapping;
    }

    internal TestManagementDatabase73(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase73()
    {
    }
  }
}
