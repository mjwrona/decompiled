// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.TestManagementDatabase77
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using System;
using System.Data;
using System.Data.SqlClient;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public class TestManagementDatabase77 : TestManagementDatabase76
  {
    internal TestManagementDatabase77(string connectionString, int partitionId)
      : base(connectionString, partitionId)
    {
    }

    public TestManagementDatabase77()
    {
    }

    public override void QueueDeleteRunsByRetentionSettings(
      TestManagementRequestContext context,
      Guid projectId,
      DateTime currentUtcDate,
      Guid deletedBy,
      int runsDeletionBatchSize,
      int automatedTestRetentionDuration,
      int manualTestRetentionDuration,
      out int automatedRunsDeleted,
      out int manualRunsDeleted,
      bool isTcmService,
      bool isOnpremService,
      int queueDeleteRunsByRetentionSettingsSprocExecTimeOutInSec)
    {
      try
      {
        context.TraceEnter("Database", "ResultRetentionDatabase.QueueDeleteRunsByRetentionSettings");
        this.PrepareStoredProcedure("TestResult.prc_QueueDeleteRunsByRetentionSettings", queueDeleteRunsByRetentionSettingsSprocExecTimeOutInSec);
        this.BindInt("@dataspaceId", this.GetDataspaceIdWithLazyInitialization(projectId));
        this.BindInt("@automatedDuration", automatedTestRetentionDuration);
        this.BindInt("@manualDuration", manualTestRetentionDuration);
        this.BindDateTime("@currentUtcDate", currentUtcDate);
        this.BindGuid("@deletedBy", deletedBy);
        this.BindInt("@runsDeletionBatchSize", runsDeletionBatchSize);
        this.BindBoolean("@isTcmService", isTcmService);
        this.BindBoolean("@isOnpremService", isOnpremService);
        SqlDataReader reader = this.ExecuteReader();
        automatedRunsDeleted = 0;
        manualRunsDeleted = 0;
        if (!reader.Read())
          return;
        automatedRunsDeleted = new SqlColumnBinder("AutomatedRunsDeleted").GetInt32((IDataReader) reader);
        manualRunsDeleted = new SqlColumnBinder("ManualRunsDeleted").GetInt32((IDataReader) reader);
      }
      finally
      {
        context.TraceLeave("Database", "ResultRetentionDatabase.QueueDeleteRunsByRetentionSettings");
      }
    }
  }
}
