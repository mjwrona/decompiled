// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.ICorePipelineFailureHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 10D2EBC4-B606-4155-939F-EEB226A80181
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions.dll

using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions
{
  public interface ICorePipelineFailureHandler
  {
    IEntityType SupportedEntityType { get; }

    int Weight { get; }

    bool HandleError(
      CoreIndexingExecutionContext coreIndexingExecutionContext,
      Exception exception);

    int HandleItemLevelErrors(
      CoreIndexingExecutionContext coreIndexingExecutionContext);
  }
}
