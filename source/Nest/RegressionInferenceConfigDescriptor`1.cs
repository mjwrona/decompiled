// Decompiled with JetBrains decompiler
// Type: Nest.RegressionInferenceConfigDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class RegressionInferenceConfigDescriptor<T> : 
    DescriptorBase<RegressionInferenceConfigDescriptor<T>, IRegressionInferenceConfig>,
    IRegressionInferenceConfig
  {
    Field IRegressionInferenceConfig.ResultsField { get; set; }

    public RegressionInferenceConfigDescriptor<T> ResultsField(Field field) => this.Assign<Field>(field, (Action<IRegressionInferenceConfig, Field>) ((a, v) => a.ResultsField = v));

    public RegressionInferenceConfigDescriptor<T> ResultsField<TValue>(
      Expression<Func<T, TValue>> objectPath)
    {
      return this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IRegressionInferenceConfig, Expression<Func<T, TValue>>>) ((a, v) => a.ResultsField = (Field) (Expression) v));
    }
  }
}
