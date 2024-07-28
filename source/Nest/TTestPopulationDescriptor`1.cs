// Decompiled with JetBrains decompiler
// Type: Nest.TTestPopulationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class TTestPopulationDescriptor<T> : 
    DescriptorBase<TTestPopulationDescriptor<T>, ITTestPopulation>,
    ITTestPopulation
    where T : class
  {
    Nest.Field ITTestPopulation.Field { get; set; }

    IScript ITTestPopulation.Script { get; set; }

    QueryContainer ITTestPopulation.Filter { get; set; }

    public TTestPopulationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<ITTestPopulation, Nest.Field>) ((a, v) => a.Field = v));

    public TTestPopulationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<ITTestPopulation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public TTestPopulationDescriptor<T> Script(string script) => this.Assign<InlineScript>((InlineScript) script, (Action<ITTestPopulation, InlineScript>) ((a, v) => a.Script = (IScript) v));

    public TTestPopulationDescriptor<T> Script(Func<ScriptDescriptor, IScript> scriptSelector) => this.Assign<Func<ScriptDescriptor, IScript>>(scriptSelector, (Action<ITTestPopulation, Func<ScriptDescriptor, IScript>>) ((a, v) => a.Script = v != null ? v(new ScriptDescriptor()) : (IScript) null));

    public TTestPopulationDescriptor<T> Filter(
      Func<QueryContainerDescriptor<T>, QueryContainer> filter)
    {
      return this.Assign<Func<QueryContainerDescriptor<T>, QueryContainer>>(filter, (Action<ITTestPopulation, Func<QueryContainerDescriptor<T>, QueryContainer>>) ((a, v) => a.Filter = v != null ? v(new QueryContainerDescriptor<T>()) : (QueryContainer) null));
    }
  }
}
