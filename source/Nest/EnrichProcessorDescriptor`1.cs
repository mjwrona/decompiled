// Decompiled with JetBrains decompiler
// Type: Nest.EnrichProcessorDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class EnrichProcessorDescriptor<T> : 
    ProcessorDescriptorBase<EnrichProcessorDescriptor<T>, IEnrichProcessor>,
    IEnrichProcessor,
    IProcessor
    where T : class
  {
    protected override string Name => "enrich";

    Nest.Field IEnrichProcessor.Field { get; set; }

    Nest.Field IEnrichProcessor.TargetField { get; set; }

    string IEnrichProcessor.PolicyName { get; set; }

    bool? IEnrichProcessor.IgnoreMissing { get; set; }

    bool? IEnrichProcessor.Override { get; set; }

    int? IEnrichProcessor.MaxMatches { get; set; }

    GeoShapeRelation? IEnrichProcessor.ShapeRelation { get; set; }

    public EnrichProcessorDescriptor<T> Field(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IEnrichProcessor, Nest.Field>) ((a, v) => a.Field = v));

    public EnrichProcessorDescriptor<T> Field<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IEnrichProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.Field = (Nest.Field) (Expression) v));

    public EnrichProcessorDescriptor<T> PolicyName(string policyName) => this.Assign<string>(policyName, (Action<IEnrichProcessor, string>) ((a, v) => a.PolicyName = v));

    public EnrichProcessorDescriptor<T> TargetField(Nest.Field field) => this.Assign<Nest.Field>(field, (Action<IEnrichProcessor, Nest.Field>) ((a, v) => a.TargetField = v));

    public EnrichProcessorDescriptor<T> TargetField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IEnrichProcessor, Expression<Func<T, TValue>>>) ((a, v) => a.TargetField = (Nest.Field) (Expression) v));

    public EnrichProcessorDescriptor<T> IgnoreMissing(bool? ignoreMissing = true) => this.Assign<bool?>(ignoreMissing, (Action<IEnrichProcessor, bool?>) ((a, v) => a.IgnoreMissing = v));

    public EnrichProcessorDescriptor<T> Override(bool? @override = true) => this.Assign<bool?>(@override, (Action<IEnrichProcessor, bool?>) ((a, v) => a.Override = v));

    public EnrichProcessorDescriptor<T> MaxMatches(int? maxMatches) => this.Assign<int?>(maxMatches, (Action<IEnrichProcessor, int?>) ((a, v) => a.MaxMatches = v));

    public EnrichProcessorDescriptor<T> ShapeRelation(GeoShapeRelation? relation) => this.Assign<GeoShapeRelation?>(relation, (Action<IEnrichProcessor, GeoShapeRelation?>) ((a, v) => a.ShapeRelation = v));
  }
}
