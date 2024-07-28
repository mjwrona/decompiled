// Decompiled with JetBrains decompiler
// Type: Nest.TermsCompositeAggregationSourceDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TermsCompositeAggregationSourceDescriptor<T> : 
    CompositeAggregationSourceDescriptorBase<TermsCompositeAggregationSourceDescriptor<T>, ITermsCompositeAggregationSource, T>,
    ITermsCompositeAggregationSource,
    ICompositeAggregationSource
  {
    public TermsCompositeAggregationSourceDescriptor(string name)
      : base(name, "terms")
    {
    }

    IScript ITermsCompositeAggregationSource.Script { get; set; }

    public TermsCompositeAggregationSourceDescriptor<T> Script(
      Func<ScriptDescriptor, IScript> selector)
    {
      return this.Assign<Func<ScriptDescriptor, IScript>>(selector, (Action<ITermsCompositeAggregationSource, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
    }
  }
}
