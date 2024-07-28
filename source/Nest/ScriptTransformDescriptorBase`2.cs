// Decompiled with JetBrains decompiler
// Type: Nest.ScriptTransformDescriptorBase`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public abstract class ScriptTransformDescriptorBase<TDescriptor, TInterface> : 
    DescriptorBase<TDescriptor, TInterface>,
    IScriptTransform,
    ITransform
    where TDescriptor : ScriptTransformDescriptorBase<TDescriptor, TInterface>, TInterface, IScriptTransform
    where TInterface : class, IScriptTransform
  {
    string IScriptTransform.Lang { get; set; }

    Dictionary<string, object> IScriptTransform.Params { get; set; }

    public TDescriptor Params(Dictionary<string, object> scriptParams) => this.Assign<Dictionary<string, object>>(scriptParams, (Action<TInterface, Dictionary<string, object>>) ((a, v) => a.Params = v));

    public TDescriptor Params(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsSelector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(paramsSelector, (Action<TInterface, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Params = v != null ? (Dictionary<string, object>) v(new FluentDictionary<string, object>()) : (Dictionary<string, object>) null));
    }

    public TDescriptor Lang(string lang) => this.Assign<string>(lang, (Action<TInterface, string>) ((a, v) => a.Lang = v));
  }
}
