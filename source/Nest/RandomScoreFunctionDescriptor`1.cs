// Decompiled with JetBrains decompiler
// Type: Nest.RandomScoreFunctionDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class RandomScoreFunctionDescriptor<T> : 
    FunctionScoreFunctionDescriptorBase<RandomScoreFunctionDescriptor<T>, IRandomScoreFunction, T>,
    IRandomScoreFunction,
    IScoreFunction
    where T : class
  {
    Nest.Field IRandomScoreFunction.Field { get; set; }

    Union<long, string> IRandomScoreFunction.Seed { get; set; }

    public RandomScoreFunctionDescriptor<T> Seed(long? seed) => this.Assign<long?>(seed, (Action<IRandomScoreFunction, long?>) ((a, v) =>
    {
      IRandomScoreFunction randomScoreFunction = a;
      long? nullable = v;
      Union<long, string> valueOrDefault = nullable.HasValue ? (Union<long, string>) nullable.GetValueOrDefault() : (Union<long, string>) null;
      randomScoreFunction.Seed = valueOrDefault;
    }));

    public RandomScoreFunctionDescriptor<T> Seed(string seed) => this.Assign<string>(seed, (Action<IRandomScoreFunction, string>) ((a, v) => a.Seed = (Union<long, string>) v));

    public RandomScoreFunctionDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IRandomScoreFunction, Nest.Field>) ((a, v) => a.Field = v));

    public RandomScoreFunctionDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IRandomScoreFunction, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));
  }
}
