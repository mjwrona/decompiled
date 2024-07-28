// Decompiled with JetBrains decompiler
// Type: Nest.HistogramPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class HistogramPropertyDescriptor<T> : 
    PropertyDescriptorBase<HistogramPropertyDescriptor<T>, IHistogramProperty, T>,
    IHistogramProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    bool? IHistogramProperty.IgnoreMalformed { get; set; }

    public HistogramPropertyDescriptor()
      : base(FieldType.Histogram)
    {
    }

    public HistogramPropertyDescriptor<T> IgnoreMalformed(bool? ignoreMalformed = true) => this.Assign<bool?>(ignoreMalformed, (Action<IHistogramProperty, bool?>) ((a, v) => a.IgnoreMalformed = v));
  }
}
