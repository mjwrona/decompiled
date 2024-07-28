// Decompiled with JetBrains decompiler
// Type: Nest.RankFeaturesPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class RankFeaturesPropertyDescriptor<T> : 
    PropertyDescriptorBase<RankFeaturesPropertyDescriptor<T>, IRankFeaturesProperty, T>,
    IRankFeaturesProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public RankFeaturesPropertyDescriptor()
      : base(FieldType.RankFeatures)
    {
    }

    bool? IRankFeaturesProperty.PositiveScoreImpact { get; set; }

    public RankFeaturesPropertyDescriptor<T> PositiveScoreImpact(bool? positiveScoreImpact = true) => this.Assign<bool?>(positiveScoreImpact, (Action<IRankFeaturesProperty, bool?>) ((a, v) => a.PositiveScoreImpact = v));
  }
}
