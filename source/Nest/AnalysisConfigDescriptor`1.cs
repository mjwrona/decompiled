// Decompiled with JetBrains decompiler
// Type: Nest.AnalysisConfigDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Nest
{
  public class AnalysisConfigDescriptor<T> : 
    DescriptorBase<AnalysisConfigDescriptor<T>, IAnalysisConfig>,
    IAnalysisConfig
    where T : class
  {
    Time IAnalysisConfig.BucketSpan { get; set; }

    Field IAnalysisConfig.CategorizationFieldName { get; set; }

    IEnumerable<string> IAnalysisConfig.CategorizationFilters { get; set; }

    IEnumerable<IDetector> IAnalysisConfig.Detectors { get; set; }

    Fields IAnalysisConfig.Influencers { get; set; }

    Time IAnalysisConfig.Latency { get; set; }

    bool? IAnalysisConfig.MultivariateByFields { get; set; }

    Field IAnalysisConfig.SummaryCountFieldName { get; set; }

    public AnalysisConfigDescriptor<T> BucketSpan(Time bucketSpan) => this.Assign<Time>(bucketSpan, (Action<IAnalysisConfig, Time>) ((a, v) => a.BucketSpan = v));

    public AnalysisConfigDescriptor<T> CategorizationFieldName(Field field) => this.Assign<Field>(field, (Action<IAnalysisConfig, Field>) ((a, v) => a.CategorizationFieldName = v));

    public AnalysisConfigDescriptor<T> CategorizationFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IAnalysisConfig, Expression<Func<T, TValue>>>) ((a, v) => a.CategorizationFieldName = (Field) (Expression) v));
    }

    public AnalysisConfigDescriptor<T> CategorizationFilters(IEnumerable<string> filters) => this.Assign<IEnumerable<string>>(filters, (Action<IAnalysisConfig, IEnumerable<string>>) ((a, v) => a.CategorizationFilters = v));

    public AnalysisConfigDescriptor<T> CategorizationFilters(params string[] filters) => this.Assign<string[]>(filters, (Action<IAnalysisConfig, string[]>) ((a, v) => a.CategorizationFilters = (IEnumerable<string>) v));

    public AnalysisConfigDescriptor<T> Detectors(
      Func<DetectorsDescriptor<T>, IPromise<IEnumerable<IDetector>>> selector)
    {
      return this.Assign<IEnumerable<IDetector>>(selector.InvokeOrDefault<DetectorsDescriptor<T>, IPromise<IEnumerable<IDetector>>>(new DetectorsDescriptor<T>()).Value, (Action<IAnalysisConfig, IEnumerable<IDetector>>) ((a, v) => a.Detectors = v));
    }

    public AnalysisConfigDescriptor<T> Influencers(
      Func<FieldsDescriptor<T>, IPromise<Fields>> fields)
    {
      return this.Assign<Func<FieldsDescriptor<T>, IPromise<Fields>>>(fields, (Action<IAnalysisConfig, Func<FieldsDescriptor<T>, IPromise<Fields>>>) ((a, v) => a.Influencers = v != null ? v(new FieldsDescriptor<T>())?.Value : (Fields) null));
    }

    public AnalysisConfigDescriptor<T> Influencers(Fields fields) => this.Assign<Fields>(fields, (Action<IAnalysisConfig, Fields>) ((a, v) => a.Influencers = v));

    public AnalysisConfigDescriptor<T> Latency(Time latency) => this.Assign<Time>(latency, (Action<IAnalysisConfig, Time>) ((a, v) => a.Latency = v));

    public AnalysisConfigDescriptor<T> MultivariateByFields(bool? multivariateByFields = true) => this.Assign<bool?>(multivariateByFields, (Action<IAnalysisConfig, bool?>) ((a, v) => a.MultivariateByFields = v));

    public AnalysisConfigDescriptor<T> SummaryCountFieldName(Field summaryCountFieldName) => this.Assign<Field>(summaryCountFieldName, (Action<IAnalysisConfig, Field>) ((a, v) => a.SummaryCountFieldName = v));

    public AnalysisConfigDescriptor<T> SummaryCountFieldName<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IAnalysisConfig, Expression<Func<T, TValue>>>) ((a, v) => a.SummaryCountFieldName = (Field) (Expression) v));
    }
  }
}
