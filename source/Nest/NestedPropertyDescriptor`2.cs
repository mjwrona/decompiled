// Decompiled with JetBrains decompiler
// Type: Nest.NestedPropertyDescriptor`2
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public class NestedPropertyDescriptor<TParent, TChild> : 
    ObjectPropertyDescriptorBase<NestedPropertyDescriptor<TParent, TChild>, INestedProperty, TParent, TChild>,
    INestedProperty,
    IObjectProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where TParent : class
    where TChild : class
  {
    public NestedPropertyDescriptor()
      : base(FieldType.Nested)
    {
    }

    bool? INestedProperty.IncludeInParent { get; set; }

    bool? INestedProperty.IncludeInRoot { get; set; }

    public NestedPropertyDescriptor<TParent, TChild> IncludeInParent(bool? includeInParent = true) => this.Assign<bool?>(includeInParent, (Action<INestedProperty, bool?>) ((a, v) => a.IncludeInParent = v));

    public NestedPropertyDescriptor<TParent, TChild> IncludeInRoot(bool? includeInRoot = true) => this.Assign<bool?>(includeInRoot, (Action<INestedProperty, bool?>) ((a, v) => a.IncludeInRoot = v));
  }
}
