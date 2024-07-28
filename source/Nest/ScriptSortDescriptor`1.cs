// Decompiled with JetBrains decompiler
// Type: Nest.ScriptSortDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ScriptSortDescriptor<T> : 
    SortDescriptorBase<ScriptSortDescriptor<T>, IScriptSort, T>,
    IScriptSort,
    ISort
    where T : class
  {
    protected override Field SortKey => (Field) "_script";

    IScript IScriptSort.Script { get; set; }

    string IScriptSort.Type { get; set; }

    public virtual ScriptSortDescriptor<T> Type(string type) => this.Assign<string>(type, (Action<IScriptSort, string>) ((a, v) => a.Type = v));

    public ScriptSortDescriptor<T> Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<IScriptSort, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
  }
}
