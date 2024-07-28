// Decompiled with JetBrains decompiler
// Type: Nest.MatrixStatsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class MatrixStatsAggregationDescriptor<T> : 
    MatrixAggregationDescriptorBase<MatrixStatsAggregationDescriptor<T>, IMatrixStatsAggregation, T>,
    IMatrixStatsAggregation,
    IMatrixAggregation,
    IAggregation
    where T : class
  {
    MatrixStatsMode? IMatrixStatsAggregation.Mode { get; set; }

    public MatrixStatsAggregationDescriptor<T> Mode(MatrixStatsMode? mode) => this.Assign<MatrixStatsMode?>(mode, (Action<IMatrixStatsAggregation, MatrixStatsMode?>) ((a, v) => a.Mode = v));
  }
}
