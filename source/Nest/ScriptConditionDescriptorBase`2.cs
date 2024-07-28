// Decompiled with JetBrains decompiler
// Type: Nest.ScriptConditionDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public abstract class ScriptConditionDescriptorBase<TDescriptor, TInterface> : 
    DescriptorBase<TDescriptor, TInterface>,
    IScriptCondition,
    ICondition
    where TDescriptor : ScriptConditionDescriptorBase<TDescriptor, TInterface>, TInterface, IScriptCondition
    where TInterface : class, IScriptCondition
  {
    string IScriptCondition.Lang { get; set; }

    IDictionary<string, object> IScriptCondition.Params { get; set; }

    public TDescriptor Lang(string lang) => this.Assign<string>(lang, (Action<TInterface, string>) ((a, v) => a.Lang = v));

    public TDescriptor Params(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsDictionary)
    {
      return this.Assign<FluentDictionary<string, object>>(paramsDictionary(new FluentDictionary<string, object>()), (Action<TInterface, FluentDictionary<string, object>>) ((a, v) => a.Params = (IDictionary<string, object>) v));
    }

    public TDescriptor Params(Dictionary<string, object> paramsDictionary) => this.Assign<Dictionary<string, object>>(paramsDictionary, (Action<TInterface, Dictionary<string, object>>) ((a, v) => a.Params = (IDictionary<string, object>) v));
  }
}
