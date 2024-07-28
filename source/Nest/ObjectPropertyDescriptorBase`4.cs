// Decompiled with JetBrains decompiler
// Type: Nest.ObjectPropertyDescriptorBase`4
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Diagnostics;

namespace Nest
{
  [DebuggerDisplay("{DebugDisplay}")]
  public abstract class ObjectPropertyDescriptorBase<TDescriptor, TInterface, TParent, TChild> : 
    CorePropertyDescriptorBase<TDescriptor, TInterface, TParent>,
    IObjectProperty,
    ICoreProperty,
    IProperty,
    IFieldMapping
    where TDescriptor : ObjectPropertyDescriptorBase<TDescriptor, TInterface, TParent, TChild>, TInterface
    where TInterface : class, IObjectProperty
    where TParent : class
    where TChild : class
  {
    protected ObjectPropertyDescriptorBase()
      : base(FieldType.Object)
    {
    }

    protected ObjectPropertyDescriptorBase(FieldType fieldType)
      : base(fieldType)
    {
    }

    Union<bool, DynamicMapping> IObjectProperty.Dynamic { get; set; }

    bool? IObjectProperty.Enabled { get; set; }

    IProperties IObjectProperty.Properties { get; set; }

    public TDescriptor Dynamic(Union<bool, DynamicMapping> dynamic) => this.Assign<Union<bool, DynamicMapping>>(dynamic, (Action<TInterface, Union<bool, DynamicMapping>>) ((a, v) => a.Dynamic = v));

    public TDescriptor Dynamic(bool dynamic = true) => this.Assign<bool>(dynamic, (Action<TInterface, bool>) ((a, v) => a.Dynamic = (Union<bool, DynamicMapping>) v));

    public TDescriptor Enabled(bool? enabled = true) => this.Assign<bool?>(enabled, (Action<TInterface, bool?>) ((a, v) => a.Enabled = v));

    public TDescriptor Properties(
      Func<PropertiesDescriptor<TChild>, IPromise<IProperties>> selector)
    {
      return this.Assign<Func<PropertiesDescriptor<TChild>, IPromise<IProperties>>>(selector, (Action<TInterface, Func<PropertiesDescriptor<TChild>, IPromise<IProperties>>>) ((a, v) => a.Properties = v != null ? v(new PropertiesDescriptor<TChild>(a.Properties))?.Value : (IProperties) null));
    }

    public TDescriptor AutoMap(IPropertyVisitor visitor = null, int maxRecursion = 0) => this.Assign<IProperties>(this.Self.Properties.AutoMap<TChild>(visitor, maxRecursion), (Action<TInterface, IProperties>) ((a, v) => a.Properties = v));

    public TDescriptor AutoMap(int maxRecursion) => this.AutoMap((IPropertyVisitor) null, maxRecursion);
  }
}
