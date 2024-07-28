// Decompiled with JetBrains decompiler
// Type: Nest.ScriptScoreFunctionDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ScriptScoreFunctionDescriptor<T> : 
    FunctionScoreFunctionDescriptorBase<ScriptScoreFunctionDescriptor<T>, IScriptScoreFunction, T>,
    IScriptScoreFunction,
    IScoreFunction
    where T : class
  {
    IScript IScriptScoreFunction.Script { get; set; }

    public ScriptScoreFunctionDescriptor<T> Script(Func<ScriptDescriptor, IScript> selector) => this.Assign<Func<ScriptDescriptor, IScript>>(selector, (Action<IScriptScoreFunction, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
  }
}
