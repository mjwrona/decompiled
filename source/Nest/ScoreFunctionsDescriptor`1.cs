// Decompiled with JetBrains decompiler
// Type: Nest.ScoreFunctionsDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class ScoreFunctionsDescriptor<T> : 
    DescriptorPromiseBase<ScoreFunctionsDescriptor<T>, IList<IScoreFunction>>
    where T : class
  {
    public ScoreFunctionsDescriptor()
      : base((IList<IScoreFunction>) new List<IScoreFunction>())
    {
    }

    public ScoreFunctionsDescriptor<T> Gauss(
      Func<GaussDecayFunctionDescriptor<double?, double?, T>, IDecayFunction<double?, double?>> selector)
    {
      return this.Assign<Func<GaussDecayFunctionDescriptor<double?, double?, T>, IDecayFunction<double?, double?>>>(selector, (Action<IList<IScoreFunction>, Func<GaussDecayFunctionDescriptor<double?, double?, T>, IDecayFunction<double?, double?>>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new GaussDecayFunctionDescriptor<double?, double?, T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> GaussDate(
      Func<GaussDecayFunctionDescriptor<DateMath, Time, T>, IDecayFunction<DateMath, Time>> selector)
    {
      return this.Assign<Func<GaussDecayFunctionDescriptor<DateMath, Time, T>, IDecayFunction<DateMath, Time>>>(selector, (Action<IList<IScoreFunction>, Func<GaussDecayFunctionDescriptor<DateMath, Time, T>, IDecayFunction<DateMath, Time>>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new GaussDecayFunctionDescriptor<DateMath, Time, T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> GaussGeoLocation(
      Func<GaussDecayFunctionDescriptor<GeoLocation, Distance, T>, IDecayFunction<GeoLocation, Distance>> selector)
    {
      return this.Assign<Func<GaussDecayFunctionDescriptor<GeoLocation, Distance, T>, IDecayFunction<GeoLocation, Distance>>>(selector, (Action<IList<IScoreFunction>, Func<GaussDecayFunctionDescriptor<GeoLocation, Distance, T>, IDecayFunction<GeoLocation, Distance>>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new GaussDecayFunctionDescriptor<GeoLocation, Distance, T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> Linear(
      Func<LinearDecayFunctionDescriptor<double?, double?, T>, IDecayFunction<double?, double?>> selector)
    {
      return this.Assign<Func<LinearDecayFunctionDescriptor<double?, double?, T>, IDecayFunction<double?, double?>>>(selector, (Action<IList<IScoreFunction>, Func<LinearDecayFunctionDescriptor<double?, double?, T>, IDecayFunction<double?, double?>>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new LinearDecayFunctionDescriptor<double?, double?, T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> LinearDate(
      Func<LinearDecayFunctionDescriptor<DateMath, Time, T>, IDecayFunction<DateMath, Time>> selector)
    {
      return this.Assign<Func<LinearDecayFunctionDescriptor<DateMath, Time, T>, IDecayFunction<DateMath, Time>>>(selector, (Action<IList<IScoreFunction>, Func<LinearDecayFunctionDescriptor<DateMath, Time, T>, IDecayFunction<DateMath, Time>>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new LinearDecayFunctionDescriptor<DateMath, Time, T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> LinearGeoLocation(
      Func<LinearDecayFunctionDescriptor<GeoLocation, Distance, T>, IDecayFunction<GeoLocation, Distance>> selector)
    {
      return this.Assign<Func<LinearDecayFunctionDescriptor<GeoLocation, Distance, T>, IDecayFunction<GeoLocation, Distance>>>(selector, (Action<IList<IScoreFunction>, Func<LinearDecayFunctionDescriptor<GeoLocation, Distance, T>, IDecayFunction<GeoLocation, Distance>>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new LinearDecayFunctionDescriptor<GeoLocation, Distance, T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> Exponential(
      Func<ExponentialDecayFunctionDescriptor<double?, double?, T>, IDecayFunction<double?, double?>> selector)
    {
      return this.Assign<Func<ExponentialDecayFunctionDescriptor<double?, double?, T>, IDecayFunction<double?, double?>>>(selector, (Action<IList<IScoreFunction>, Func<ExponentialDecayFunctionDescriptor<double?, double?, T>, IDecayFunction<double?, double?>>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new ExponentialDecayFunctionDescriptor<double?, double?, T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> ExponentialDate(
      Func<ExponentialDecayFunctionDescriptor<DateMath, Time, T>, IDecayFunction<DateMath, Time>> selector)
    {
      return this.Assign<Func<ExponentialDecayFunctionDescriptor<DateMath, Time, T>, IDecayFunction<DateMath, Time>>>(selector, (Action<IList<IScoreFunction>, Func<ExponentialDecayFunctionDescriptor<DateMath, Time, T>, IDecayFunction<DateMath, Time>>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new ExponentialDecayFunctionDescriptor<DateMath, Time, T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> ExponentialGeoLocation(
      Func<ExponentialDecayFunctionDescriptor<GeoLocation, Distance, T>, IDecayFunction<GeoLocation, Distance>> selector)
    {
      return this.Assign<Func<ExponentialDecayFunctionDescriptor<GeoLocation, Distance, T>, IDecayFunction<GeoLocation, Distance>>>(selector, (Action<IList<IScoreFunction>, Func<ExponentialDecayFunctionDescriptor<GeoLocation, Distance, T>, IDecayFunction<GeoLocation, Distance>>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new ExponentialDecayFunctionDescriptor<GeoLocation, Distance, T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> ScriptScore(
      Func<ScriptScoreFunctionDescriptor<T>, IScriptScoreFunction> selector)
    {
      return this.Assign<Func<ScriptScoreFunctionDescriptor<T>, IScriptScoreFunction>>(selector, (Action<IList<IScoreFunction>, Func<ScriptScoreFunctionDescriptor<T>, IScriptScoreFunction>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new ScriptScoreFunctionDescriptor<T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> FieldValueFactor(
      Func<FieldValueFactorFunctionDescriptor<T>, IFieldValueFactorFunction> selector)
    {
      return this.Assign<Func<FieldValueFactorFunctionDescriptor<T>, IFieldValueFactorFunction>>(selector, (Action<IList<IScoreFunction>, Func<FieldValueFactorFunctionDescriptor<T>, IFieldValueFactorFunction>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new FieldValueFactorFunctionDescriptor<T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> RandomScore(
      Func<RandomScoreFunctionDescriptor<T>, IRandomScoreFunction> selector = null)
    {
      return this.Assign<Func<RandomScoreFunctionDescriptor<T>, IRandomScoreFunction>>(selector, (Action<IList<IScoreFunction>, Func<RandomScoreFunctionDescriptor<T>, IRandomScoreFunction>>) ((a, v) => a.AddIfNotNull<IScoreFunction>((IScoreFunction) v.InvokeOrDefault<RandomScoreFunctionDescriptor<T>, IRandomScoreFunction>(new RandomScoreFunctionDescriptor<T>()))));
    }

    public ScoreFunctionsDescriptor<T> Weight(
      Func<WeightFunctionDescriptor<T>, IWeightFunction> selector)
    {
      return this.Assign<Func<WeightFunctionDescriptor<T>, IWeightFunction>>(selector, (Action<IList<IScoreFunction>, Func<WeightFunctionDescriptor<T>, IWeightFunction>>) ((a, v) => a.AddIfNotNull<IScoreFunction>(v != null ? (IScoreFunction) v(new WeightFunctionDescriptor<T>()) : (IScoreFunction) null)));
    }

    public ScoreFunctionsDescriptor<T> Weight(double weight) => this.Assign<double>(weight, (Action<IList<IScoreFunction>, double>) ((a, v) =>
    {
      IList<IScoreFunction> list = a;
      list.AddIfNotNull<IScoreFunction>((IScoreFunction) new WeightFunction()
      {
        Weight = new double?(v)
      });
    }));
  }
}
