// Decompiled with JetBrains decompiler
// Type: Nest.SizeFieldDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SizeFieldDescriptor : 
    DescriptorBase<SizeFieldDescriptor, ISizeField>,
    ISizeField,
    IFieldMapping
  {
    bool? ISizeField.Enabled { get; set; }

    public SizeFieldDescriptor Enabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<ISizeField, bool?>) ((a, v) => a.Enabled = v));
  }
}
