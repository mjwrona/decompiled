// Decompiled with JetBrains decompiler
// Type: Nest.PredicateTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PredicateTokenFilterDescriptor : 
    TokenFilterDescriptorBase<PredicateTokenFilterDescriptor, IPredicateTokenFilter>,
    IPredicateTokenFilter,
    ITokenFilter
  {
    protected override string Type => "predicate_token_filter";

    IScript IPredicateTokenFilter.Script { get; set; }

    public PredicateTokenFilterDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IPredicateTokenFilter, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public PredicateTokenFilterDescriptor Script(string predicate) => this.Assign<InlineScript>(new InlineScript(predicate), (Action<IPredicateTokenFilter, InlineScript>) ((a, v) => a.Script = (IScript) v));
  }
}
