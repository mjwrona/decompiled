// Decompiled with JetBrains decompiler
// Type: Nest.ScriptQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ScriptQueryDescriptor<T> : 
    QueryDescriptorBase<ScriptQueryDescriptor<T>, IScriptQuery>,
    IScriptQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => ScriptQuery.IsConditionless((IScriptQuery) this);

    IScript IScriptQuery.Script { get; set; }

    public ScriptQueryDescriptor<T> Script(Func<ScriptDescriptor, IScript> selector) => this.Assign<Func<ScriptDescriptor, IScript>>(selector, (Action<IScriptQuery, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
  }
}
