// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package.FeedIndexingUnitHelper
// Assembly: Microsoft.VisualStudio.Services.Search.Server.Jobs, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 23ACAECB-A4CB-4AA5-8366-092C41D8D4A8
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Microsoft.VisualStudio.Services.Search.Server.Jobs.dll

using Microsoft.VisualStudio.Services.Search.Common;
using Microsoft.VisualStudio.Services.Search.Server.Jobs.Definitions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Microsoft.VisualStudio.Services.Search.Server.Jobs.Operations.Package
{
  internal static class FeedIndexingUnitHelper
  {
    internal static List<IndexingUnit> GetFeedIndexingUnits(
      CoreIndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit)
    {
      return indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(indexingExecutionContext.RequestContext, indexingUnit.IndexingUnitId, -1).Where<IndexingUnit>((Func<IndexingUnit, bool>) (IU => !IU.Properties.IsDisabled)).ToList<IndexingUnit>();
    }

    internal static List<IndexingUnit> GetFeedIndexingUnitsToBeDeleted(
      CoreIndexingExecutionContext indexingExecutionContext,
      IndexingUnit indexingUnit)
    {
      return indexingExecutionContext.IndexingUnitDataAccess.GetIndexingUnitsWithGivenParent(indexingExecutionContext.RequestContext, indexingUnit.IndexingUnitId, -1).Where<IndexingUnit>((Func<IndexingUnit, bool>) (IU => IU.Properties.IsDisabled)).ToList<IndexingUnit>();
    }
  }
}
