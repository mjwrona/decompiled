// Decompiled with JetBrains decompiler
// Type: Nest.GoogleNormalizedDistanceHeuristicDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class GoogleNormalizedDistanceHeuristicDescriptor : 
    DescriptorBase<GoogleNormalizedDistanceHeuristicDescriptor, IGoogleNormalizedDistanceHeuristic>,
    IGoogleNormalizedDistanceHeuristic
  {
    bool? IGoogleNormalizedDistanceHeuristic.BackgroundIsSuperSet { get; set; }

    public GoogleNormalizedDistanceHeuristicDescriptor BackgroundIsSuperSet(
      bool? backroundIsSuperSet = true)
    {
      return this.Assign<bool?>(backroundIsSuperSet, (Action<IGoogleNormalizedDistanceHeuristic, bool?>) ((a, v) => a.BackgroundIsSuperSet = v));
    }
  }
}
