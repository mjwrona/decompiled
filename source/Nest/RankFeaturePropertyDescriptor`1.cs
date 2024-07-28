// Decompiled with JetBrains decompiler
// Type: Nest.RankFeaturePropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class RankFeaturePropertyDescriptor<T> : 
    PropertyDescriptorBase<RankFeaturePropertyDescriptor<T>, IRankFeatureProperty, T>,
    IRankFeatureProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public RankFeaturePropertyDescriptor()
      : base(FieldType.RankFeature)
    {
    }

    bool? IRankFeatureProperty.PositiveScoreImpact { get; set; }

    public RankFeaturePropertyDescriptor<T> PositiveScoreImpact(bool? positiveScoreImpact = true) => this.Assign<bool?>(positiveScoreImpact, (Action<IRankFeatureProperty, bool?>) ((a, v) => a.PositiveScoreImpact = v));
  }
}
