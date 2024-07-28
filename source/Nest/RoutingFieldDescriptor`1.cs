// Decompiled with JetBrains decompiler
// Type: Nest.RoutingFieldDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class RoutingFieldDescriptor<T> : 
    DescriptorBase<RoutingFieldDescriptor<T>, IRoutingField>,
    IRoutingField,
    IFieldMapping
  {
    bool? IRoutingField.Required { get; set; }

    public RoutingFieldDescriptor<T> Required(bool? required = true) => this.Assign<bool?>(required, (Action<IRoutingField, bool?>) ((a, v) => a.Required = v));
  }
}
