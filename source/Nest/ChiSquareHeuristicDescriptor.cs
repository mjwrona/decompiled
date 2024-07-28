// Decompiled with JetBrains decompiler
// Type: Nest.ChiSquareHeuristicDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ChiSquareHeuristicDescriptor : 
    DescriptorBase<ChiSquareHeuristicDescriptor, IChiSquareHeuristic>,
    IChiSquareHeuristic
  {
    bool? IChiSquareHeuristic.BackgroundIsSuperSet { get; set; }

    bool? IChiSquareHeuristic.IncludeNegatives { get; set; }

    public ChiSquareHeuristicDescriptor BackgroundIsSuperSet(bool? backgroundIsSuperSet = true) => this.Assign<bool?>(backgroundIsSuperSet, (Action<IChiSquareHeuristic, bool?>) ((a, v) => a.BackgroundIsSuperSet = v));

    public ChiSquareHeuristicDescriptor IncludeNegatives(bool? includeNegatives = true) => this.Assign<bool?>(includeNegatives, (Action<IChiSquareHeuristic, bool?>) ((a, v) => a.IncludeNegatives = v));
  }
}
