// Decompiled with JetBrains decompiler
// Type: Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders.IndexingUnitWithSize
// Assembly: Microsoft.VisualStudio.Services.Search.Indexer, Version=19.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: 167B1EA6-4D18-408E-89C6-597B8290976F
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Plugins\Microsoft.VisualStudio.Services.Search.Indexer.dll

using Microsoft.VisualStudio.Services.Search.Shared.Platform.Primitives.Telemetry;
using System;
using System.Runtime.CompilerServices;

namespace Microsoft.VisualStudio.Services.Search.Indexer.Routing.RoutingProviders
{
  internal class IndexingUnitWithSize
  {
    public Microsoft.VisualStudio.Services.Search.Common.IndexingUnit IndexingUnit { get; set; }

    public long ActualInitialSize { get; set; }

    public int ActualInitialDocCount { get; set; }

    public int CurrentEstimatedDocumentCount { get; set; }

    public int EstimatedGrowth { get; set; }

    public int TotalSize
    {
      get
      {
        long totalSize = (long) this.CurrentEstimatedDocumentCount + (long) this.EstimatedGrowth;
        if (totalSize <= (long) int.MaxValue)
          return (int) totalSize;
        Tracer.TraceError(1082055, "Indexing Pipeline", "Indexer", FormattableString.Invariant(FormattableStringFactory.Create("Found an indexing unit with unexpected size. EstimatedDocumentCount :{0}, EstimatedGrowth: {1}", (object) this.CurrentEstimatedDocumentCount, (object) this.EstimatedGrowth)));
        return int.MaxValue;
      }
    }

    public bool SizeEstimationSuccessful { get; }

    internal IndexingUnitWithSize(
      Microsoft.VisualStudio.Services.Search.Common.IndexingUnit indexingUnit,
      int currentEstimatedDocumentCount,
      int estimatedGrowth,
      bool sizeEstimationSuccessful)
    {
      this.IndexingUnit = indexingUnit;
      this.CurrentEstimatedDocumentCount = currentEstimatedDocumentCount;
      this.EstimatedGrowth = estimatedGrowth;
      this.SizeEstimationSuccessful = sizeEstimationSuccessful;
    }

    public override int GetHashCode() => this.IndexingUnit.GetHashCode();

    public override string ToString() => this.IndexingUnit.ToString();

    public override bool Equals(object obj)
    {
      if (base.Equals(obj))
        return true;
      return obj is IndexingUnitWithSize indexingUnitWithSize && this.IndexingUnit.Equals((object) indexingUnitWithSize.IndexingUnit);
    }

    public virtual bool IsEmpty()
    {
      if (this.SizeEstimationSuccessful)
      {
        if (this.IndexingUnit.IndexingUnitType == "Git_Repository")
          return this.ActualInitialSize == 0L;
        if (this.IndexingUnit.IndexingUnitType == "TFVC_Repository" || this.IndexingUnit.IndexingUnitType == "CustomRepository" || this.IndexingUnit.IndexingUnitType == "ScopedIndexingUnit")
          return this.ActualInitialDocCount == 0;
      }
      return false;
    }
  }
}
