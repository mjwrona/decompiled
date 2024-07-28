// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Analytics.Transform.ITransformService
// Assembly: Microsoft.VisualStudio.Services.Analytics.Server, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 9C06769D-4EB9-467A-8965-10A4FD97C7AD
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Analytics.Server.dll

using Microsoft.TeamFoundation.Framework.Server;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Analytics.Transform
{
  [DefaultServiceImplementation(typeof (TransformService))]
  public interface ITransformService : IVssFrameworkService
  {
    void QueueTransform(IVssRequestContext requestContext, int maxDelaySeconds = 0);

    bool TransformNext(
      IVssRequestContext requestContext,
      ICollection<TransformResult> results = null,
      int? partitionDBConcurrencyCounter = null,
      PreTransformAction preTransformAction = null,
      int selectionOffset = 0);

    void ReprocessFromTable(IVssRequestContext requestContext, string tableName);

    void ReprocessFromProcessBatch(
      IVssRequestContext requestContext,
      long batchId,
      string triggerTableName,
      bool fromExistingState,
      bool createDependentWork,
      bool delayReworkPerAttemptHistory,
      bool ignoreWhenConsecutiveSprocFailures);

    void ReprocessFromUncorrectedBatches(
      IVssRequestContext requestContext,
      string operationSproc,
      bool fromExistingState,
      bool createDependentWork,
      bool delayReworkPerAttemptHistory);

    void GenerateCalendar(IVssRequestContext requestContext);

    CleanupDeletedTableResult CleanupDeletedTable(
      IVssRequestContext requestContext,
      string tableName,
      bool continueToNextTable);

    int TransformTimeoutSeconds { get; set; }
  }
}
