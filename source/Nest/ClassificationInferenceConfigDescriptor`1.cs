// Decompiled with JetBrains decompiler
// Type: Nest.ClassificationInferenceConfigDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class ClassificationInferenceConfigDescriptor<T> : 
    DescriptorBase<ClassificationInferenceConfigDescriptor<T>, IClassificationInferenceConfig>,
    IClassificationInferenceConfig
  {
    Field IClassificationInferenceConfig.ResultsField { get; set; }

    int? IClassificationInferenceConfig.NumTopClasses { get; set; }

    Field IClassificationInferenceConfig.TopClassesResultsField { get; set; }

    public ClassificationInferenceConfigDescriptor<T> ResultsField(Field field) => this.Assign<Field>(field, (Action<IClassificationInferenceConfig, Field>) ((a, v) => a.ResultsField = v));

    public ClassificationInferenceConfigDescriptor<T> ResultsField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IClassificationInferenceConfig, Expression<Func<T, TValue>>>) ((a, v) => a.ResultsField = (Field) (Expression) v));
    }

    public ClassificationInferenceConfigDescriptor<T> NumTopClasses(int? numTopClasses) => this.Assign<int?>(numTopClasses, (Action<IClassificationInferenceConfig, int?>) ((a, v) => a.NumTopClasses = v));

    public ClassificationInferenceConfigDescriptor<T> TopClassesResultsField(Field field) => this.Assign<Field>(field, (Action<IClassificationInferenceConfig, Field>) ((a, v) => a.TopClassesResultsField = v));

    public ClassificationInferenceConfigDescriptor<T> TopClassesResultsField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IClassificationInferenceConfig, Expression<Func<T, TValue>>>) ((a, v) => a.TopClassesResultsField = (Field) (Expression) v));
    }
  }
}
