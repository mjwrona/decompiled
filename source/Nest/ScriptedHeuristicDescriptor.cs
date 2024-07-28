// Decompiled with JetBrains decompiler
// Type: Nest.ScriptedHeuristicDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ScriptedHeuristicDescriptor : 
    DescriptorBase<ScriptedHeuristicDescriptor, IScriptedHeuristic>,
    IScriptedHeuristic
  {
    IScript IScriptedHeuristic.Script { get; set; }

    public ScriptedHeuristicDescriptor Script(string script) => this.Assign<string>(script, (Action<IScriptedHeuristic, string>) ((a, v) => a.Script = (IScript) (InlineScript) v));

    public ScriptedHeuristicDescriptor Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IScriptedHeuristic, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
  }
}
