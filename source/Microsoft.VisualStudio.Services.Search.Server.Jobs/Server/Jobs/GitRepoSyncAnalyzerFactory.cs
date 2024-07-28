// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.GitRepoSyncAnalyzerFactory
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using Microsoft.VisualStudio.Services.Search.Server.EventHandler;
using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs
{
  public class GitRepoSyncAnalyzerFactory
  {
    public GitRepoSyncAnalyzer GetGitRepoSyncAnalyzer(
      ExecutionContext executionContext,
      TraceMetaData traceMetaData,
      IIndexingUnitChangeEventHandler indexingUnitChangeEventHandler,
      IEntityType entityType)
    {
      switch (entityType.Name)
      {
        case "Code":
          return (GitRepoSyncAnalyzer) new CodeGitRepoSyncAnalyzer(executionContext, traceMetaData, indexingUnitChangeEventHandler);
        case "Wiki":
          return (GitRepoSyncAnalyzer) new WikiRepoSyncAnalyzer(executionContext, traceMetaData, indexingUnitChangeEventHandler);
        default:
          throw new NotSupportedException(FormattableString.Invariant(FormattableStringFactory.Create("EntityType {0} is not supported", (object) entityType.Name)));
      }
    }
  }
}
