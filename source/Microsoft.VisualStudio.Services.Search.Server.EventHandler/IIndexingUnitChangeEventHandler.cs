// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.EventHandler.IIndexingUnitChangeEventHandler
// Assembly: Microsoft.VisualStudio.Services.Search.Server.EventHandler, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 86A812E9-C14F-422E-83C2-D709899BDEBA
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.EventHandler.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Server.EventHandler
{
  public interface IIndexingUnitChangeEventHandler
  {
    IndexingUnitChangeEvent HandleEvent(
      ExecutionContext executionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent);

    IndexingUnitChangeEvent HandleEventWithAddingEventWhenNeeded(
      ExecutionContext executionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent);

    IndexingUnitChangeEvent HandleEvent(
      ExecutionContext executionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent,
      bool scopeChangeEventProcessingToIndexingUnit);

    IList<IndexingUnitChangeEvent> HandleEvents(
      ExecutionContext executionContext,
      IList<IndexingUnitChangeEvent> indexingUnitChangeEventList);

    IndexingUnitChangeEvent HandleEventWithAddingEventWhenNeededForFinalize(
      ExecutionContext executionContext,
      IndexingUnitChangeEvent indexingUnitChangeEvent);

    IList<IndexingUnitChangeEvent> HandleEventsChildIUCEsForFinalize(
      ExecutionContext executionContext,
      IList<IndexingUnitChangeEvent> indexingUnitChangeEventList);

    void HandleEventForEntity(ExecutionContext executionContext, IEntityType entityType);
  }
}
