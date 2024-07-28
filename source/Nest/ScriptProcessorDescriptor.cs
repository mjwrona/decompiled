// Decompiled with JetBrains decompiler
// Type: Nest.ScriptProcessorDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ScriptProcessorDescriptor : 
    ProcessorDescriptorBase<ScriptProcessorDescriptor, IScriptProcessor>,
    IScriptProcessor,
    IProcessor
  {
    protected override string Name => "script";

    string IScriptProcessor.Id { get; set; }

    string IScriptProcessor.Lang { get; set; }

    Dictionary<string, object> IScriptProcessor.Params { get; set; }

    string IScriptProcessor.Source { get; set; }

    public ScriptProcessorDescriptor Lang(string lang) => this.Assign<string>(lang, (Action<IScriptProcessor, string>) ((a, v) => a.Lang = v));

    public ScriptProcessorDescriptor Id(string id) => this.Assign<string>(id, (Action<IScriptProcessor, string>) ((a, v) => a.Id = v));

    public ScriptProcessorDescriptor Source(string source) => this.Assign<string>(source, (Action<IScriptProcessor, string>) ((a, v) => a.Source = v));

    public ScriptProcessorDescriptor Params(Dictionary<string, object> scriptParams) => this.Assign<Dictionary<string, object>>(scriptParams, (Action<IScriptProcessor, Dictionary<string, object>>) ((a, v) => a.Params = v));

    public ScriptProcessorDescriptor Params(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> paramsSelector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(paramsSelector, (Action<IScriptProcessor, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Params = v != null ? (Dictionary<string, object>) v(new FluentDictionary<string, object>()) : (Dictionary<string, object>) null));
    }
  }
}
