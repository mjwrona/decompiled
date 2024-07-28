// Decompiled with JetBrains decompiler
// Type: Nest.ScriptScoreQueryDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class ScriptScoreQueryDescriptor<T> : 
    QueryDescriptorBase<ScriptScoreQueryDescriptor<T>, IScriptScoreQuery>,
    IScriptScoreQuery,
    IQuery
    where T : class
  {
    protected override bool Conditionless => ScriptScoreQuery.IsConditionless((IScriptScoreQuery) this);

    QueryContainer IScriptScoreQuery.Query { get; set; }

    IScript IScriptScoreQuery.Script { get; set; }

    public ScriptScoreQueryDescriptor<T> Query(
      Func<QueryContainerDescriptor<T>, QueryContainer> selector)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(selector, (Action<IScriptScoreQuery, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Query = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }

    public ScriptScoreQueryDescriptor<T> Script(Func<ScriptDescriptor, IScript> selector) => this.Assign<Func<ScriptDescriptor, IScript>>(selector, (Action<IScriptScoreQuery, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));
  }
}
