// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Common.IIndexingUnitChangeEventSelector
// Assembly: Microsoft.VisualStudio.Services.Search.Common, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 8E09DCBA-148E-4EB7-BB73-B53B030BE93E
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Common.dll

using Microsoft.TeamFoundation.Framework.Common;
using Microsoft.VisualStudio.Services.Search.Common.Entities;
using System.Collections.Generic;

namespace Microsoft.VisualStudio.Services.Search.Common
{
  public interface IIndexingUnitChangeEventSelector
  {
    IDictionary<JobPriorityLevel, List<IndexingUnitChangeEventDetails>> GetChangeEventsToBeProcessed(
      ExecutionContext executionContext,
      int indexingUnitId,
      IEnumerable<IndexingUnitChangeEventDetails> changeEvents);
  }
}
