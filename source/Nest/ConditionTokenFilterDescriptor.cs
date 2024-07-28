// Decompiled with JetBrains decompiler
// Type: Nest.ConditionTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ConditionTokenFilterDescriptor : 
    TokenFilterDescriptorBase<ConditionTokenFilterDescriptor, IConditionTokenFilter>,
    IConditionTokenFilter,
    ITokenFilter
  {
    protected override string Type => "condition";

    IScript IConditionTokenFilter.Script { get; set; }

    IEnumerable<string> IConditionTokenFilter.Filters { get; set; }

    public ConditionTokenFilterDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IConditionTokenFilter, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public ConditionTokenFilterDescriptor Script(string predicate) => this.Assign<InlineScript>(new InlineScript(predicate), (Action<IConditionTokenFilter, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public ConditionTokenFilterDescriptor Filters(params string[] filters) => this.Assign<string[]>(filters, (Action<IConditionTokenFilter, string[]>) ((a, v) => a.Filters = (IEnumerable<string>) v));

    public ConditionTokenFilterDescriptor Filters(IEnumerable<string> filters) => this.Assign<IEnumerable<string>>(filters, (Action<IConditionTokenFilter, IEnumerable<string>>) ((a, v) => a.Filters = v));
  }
}
