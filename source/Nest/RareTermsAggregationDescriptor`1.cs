// Decompiled with JetBrains decompiler
// Type: Nest.RareTermsAggregationDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class RareTermsAggregationDescriptor<T> : 
    BucketAggregationDescriptorBase<RareTermsAggregationDescriptor<T>, IRareTermsAggregation, T>,
    IRareTermsAggregation,
    IBucketAggregation,
    IAggregation
    where T : class
  {
    TermsExclude IRareTermsAggregation.Exclude { get; set; }

    Nest.Field IRareTermsAggregation.Field { get; set; }

    TermsInclude IRareTermsAggregation.Include { get; set; }

    long? IRareTermsAggregation.MaximumDocumentCount { get; set; }

    object IRareTermsAggregation.Missing { get; set; }

    double? IRareTermsAggregation.Precision { get; set; }

    public RareTermsAggregationDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IRareTermsAggregation, Nest.Field>) ((a, v) => a.Field = v));

    public RareTermsAggregationDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> field) => this.Assign<Expression<Func<T, TValue>>>(field, (Action<IRareTermsAggregation, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public RareTermsAggregationDescriptor<T> MaximumDocumentCount(long? maximumDocumentCount) => this.Assign<long?>(maximumDocumentCount, (Action<IRareTermsAggregation, long?>) ((a, v) => a.MaximumDocumentCount = v));

    public RareTermsAggregationDescriptor<T> Include(long partition, long numberOfPartitions) => this.Assign<TermsInclude>(new TermsInclude(partition, numberOfPartitions), (Action<IRareTermsAggregation, TermsInclude>) ((a, v) => a.Include = v));

    public RareTermsAggregationDescriptor<T> Include(string includePattern) => this.Assign<TermsInclude>(new TermsInclude(includePattern), (Action<IRareTermsAggregation, TermsInclude>) ((a, v) => a.Include = v));

    public RareTermsAggregationDescriptor<T> Include(IEnumerable<string> values) => this.Assign<TermsInclude>(new TermsInclude(values), (Action<IRareTermsAggregation, TermsInclude>) ((a, v) => a.Include = v));

    public RareTermsAggregationDescriptor<T> Exclude(string excludePattern) => this.Assign<TermsExclude>(new TermsExclude(excludePattern), (Action<IRareTermsAggregation, TermsExclude>) ((a, v) => a.Exclude = v));

    public RareTermsAggregationDescriptor<T> Exclude(IEnumerable<string> values) => this.Assign<TermsExclude>(new TermsExclude(values), (Action<IRareTermsAggregation, TermsExclude>) ((a, v) => a.Exclude = v));

    public RareTermsAggregationDescriptor<T> Missing(object missing) => this.Assign<object>(missing, (Action<IRareTermsAggregation, object>) ((a, v) => a.Missing = v));

    public RareTermsAggregationDescriptor<T> Precision(double? precision) => this.Assign<double?>(precision, (Action<IRareTermsAggregation, double?>) ((a, v) => a.Precision = v));
  }
}
