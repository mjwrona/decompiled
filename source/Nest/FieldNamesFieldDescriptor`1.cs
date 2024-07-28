// Decompiled with JetBrains decompiler
// Type: Nest.FieldNamesFieldDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class FieldNamesFieldDescriptor<T> : 
    DescriptorBase<FieldNamesFieldDescriptor<T>, IFieldNamesField>,
    IFieldNamesField,
    IFieldMapping
  {
    bool? IFieldNamesField.Enabled { get; set; }

    public FieldNamesFieldDescriptor<T> Enabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<IFieldNamesField, bool?>) ((a, v) => a.Enabled = v));
  }
}
