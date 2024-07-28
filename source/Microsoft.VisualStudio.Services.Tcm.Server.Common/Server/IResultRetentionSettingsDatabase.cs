// Decompiled with JetBrains decompiler
// Type: Microsoft.TeamFoundation.TestManagement.Server.IResultRetentionSettingsDatabase
// Assembly: Microsoft.VisualStudio.Services.Tcm.Server.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 7631C286-897C-44D1-A133-A0BB6CC047F3
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Tcm.Server.Common.dll

using Microsoft.TeamFoundation.Framework.Server;
using Microsoft.TeamFoundation.TestManagement.WebApi;
using System;

namespace Microsoft.TeamFoundation.TestManagement.Server
{
  [CLSCompliant(false)]
  public interface IResultRetentionSettingsDatabase
  {
    ResultRetentionSettings CreateResultRetentionSettings(
      IVssRequestContext context,
      Guid projectId,
      ResultRetentionSettings settings);

    ResultRetentionSettings GetResultRetentionSettings(IVssRequestContext context, Guid projectId);

    ResultRetentionSettings UpdateResultRetentionSettings(
      IVssRequestContext context,
      Guid projectId,
      ResultRetentionSettings settings);

    void QueueDeleteRunsByRetentionSettings(
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
      int queueDeleteRunsByRetentionSettingsSprocExecTimeOutInSec);
  }
}
