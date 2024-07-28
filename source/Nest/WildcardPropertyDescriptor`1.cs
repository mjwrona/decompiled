// Decompiled with JetBrains decompiler
// Type: Nest.WildcardPropertyDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class WildcardPropertyDescriptor<T> : 
    PropertyDescriptorBase<WildcardPropertyDescriptor<T>, IWildcardProperty, T>,
    IWildcardProperty,
    IProperty,
    IFieldMapping
    where T : class
  {
    public WildcardPropertyDescriptor()
      : base(FieldType.Wildcard)
    {
    }

    int? IWildcardProperty.IgnoreAbove { get; set; }

    string IWildcardProperty.NullValue { get; set; }

    public WildcardPropertyDescriptor<T> IgnoreAbove(int? ignoreAbove) => this.Assign<int?>(ignoreAbove, (Action<IWildcardProperty, int?>) ((a, v) => a.IgnoreAbove = v));

    public WildcardPropertyDescriptor<T> NullValue(string nullValue) => this.Assign<string>(nullValue, (Action<IWildcardProperty, string>) ((a, v) => a.NullValue = v));
  }
}
