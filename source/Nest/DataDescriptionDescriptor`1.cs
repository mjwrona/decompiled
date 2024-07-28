// Decompiled with JetBrains decompiler
// Type: Nest.DataDescriptionDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Linq.Expressions;

namespace Nest
{
  public class DataDescriptionDescriptor<T> : 
    DescriptorBase<DataDescriptionDescriptor<T>, IDataDescription>,
    IDataDescription
  {
    string IDataDescription.Format { get; set; }

    Field IDataDescription.TimeField { get; set; }

    string IDataDescription.TimeFormat { get; set; }

    public DataDescriptionDescriptor<T> Format(string format) => this.Assign<string>(format, (Action<IDataDescription, string>) ((a, v) => a.Format = v));

    public DataDescriptionDescriptor<T> TimeField(Field timeField) => this.Assign<Field>(timeField, (Action<IDataDescription, Field>) ((a, v) => a.TimeField = v));

    public DataDescriptionDescriptor<T> TimeField<TValue>(Expression<Func<T, TValue>> objectPath) => this.Assign<Expression<Func<T, TValue>>>(objectPath, (Action<IDataDescription, Expression<Func<T, TValue>>>) ((a, v) => a.TimeField = (Field) (Expression) v));

    public DataDescriptionDescriptor<T> TimeFormat(string timeFormat) => this.Assign<string>(timeFormat, (Action<IDataDescription, string>) ((a, v) => a.TimeFormat = v));
  }
}
