// Decompiled with JetBrains decompiler
// Type: Nest.MatrixAggregationDescriptorBase`3
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public abstract class MatrixAggregationDescriptorBase<TMatrixAggregation, TMatrixAggregationInterface, T> : 
    DescriptorBase<TMatrixAggregation, TMatrixAggregationInterface>,
    IMatrixAggregation,
    IAggregation
    where TMatrixAggregation : MatrixAggregationDescriptorBase<TMatrixAggregation, TMatrixAggregationInterface, T>, TMatrixAggregationInterface, IMatrixStatsAggregation
    where TMatrixAggregationInterface : class, IMatrixAggregation
    where T : class
  {
    Nest.Fields IMatrixAggregation.Fields { get; set; }

    IDictionary<string, object> IAggregation.Meta { get; set; }

    IDictionary<Nest.Field, double> IMatrixAggregation.Missing { get; set; }

    string IAggregation.Name { get; set; }

    public TMatrixAggregation Field(Nest.Fields fields) => this.Assign<Nest.Fields>(fields, (Action<TMatrixAggregationInterface, Nest.Fields>) ((a, v) => a.Fields = v));

    public TMatrixAggregation Fields(Func<FieldsDescriptor<T>, IPromise<Nest.Fields>> fields) => this.Assign<Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>(fields, (Action<TMatrixAggregationInterface, Func<FieldsDescriptor<T>, IPromise<Nest.Fields>>>) ((a, v) => a.Fields = v != null ? v(new FieldsDescriptor<T>())?.Value : (Nest.Fields) null));

    public TMatrixAggregation Missing(
      Func<FluentDictionary<Nest.Field, double>, FluentDictionary<Nest.Field, double>> selector)
    {
      return this.Assign<Func<FluentDictionary<Nest.Field, double>, FluentDictionary<Nest.Field, double>>>(selector, (Action<TMatrixAggregationInterface, Func<FluentDictionary<Nest.Field, double>, FluentDictionary<Nest.Field, double>>>) ((a, v) => a.Missing = v != null ? (IDictionary<Nest.Field, double>) v(new FluentDictionary<Nest.Field, double>()) : (IDictionary<Nest.Field, double>) null));
    }

    public TMatrixAggregation Meta(
      Func<FluentDictionary<string, object>, FluentDictionary<string, object>> selector)
    {
      return this.Assign<Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>(selector, (Action<TMatrixAggregationInterface, Func<FluentDictionary<string, object>, FluentDictionary<string, object>>>) ((a, v) => a.Meta = v != null ? (IDictionary<string, object>) v(new FluentDictionary<string, object>()) : (IDictionary<string, object>) null));
    }
  }
}
