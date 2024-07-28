// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.YamlAttributeOverridesInspector
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Core;
using YamlDotNet.Serialization.TypeInspectors;

namespace YamlDotNet.Serialization
{
  public sealed class YamlAttributeOverridesInspector : TypeInspectorSkeleton
  {
    private readonly ITypeInspector innerTypeDescriptor;
    private readonly YamlAttributeOverrides overrides;

    public YamlAttributeOverridesInspector(
      ITypeInspector innerTypeDescriptor,
      YamlAttributeOverrides overrides)
    {
      this.innerTypeDescriptor = innerTypeDescriptor;
      this.overrides = overrides;
    }

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container) => this.overrides == null ? this.innerTypeDescriptor.GetProperties(type, container) : this.innerTypeDescriptor.GetProperties(type, container).Select<IPropertyDescriptor, IPropertyDescriptor>((Func<IPropertyDescriptor, IPropertyDescriptor>) (p => (IPropertyDescriptor) new YamlAttributeOverridesInspector.OverridePropertyDescriptor(p, this.overrides, type)));

    public sealed class OverridePropertyDescriptor : IPropertyDescriptor
    {
      private readonly IPropertyDescriptor baseDescriptor;
      private readonly YamlAttributeOverrides overrides;
      private readonly Type classType;

      public OverridePropertyDescriptor(
        IPropertyDescriptor baseDescriptor,
        YamlAttributeOverrides overrides,
        Type classType)
      {
        this.baseDescriptor = baseDescriptor;
        this.overrides = overrides;
        this.classType = classType;
      }

      public string Name => this.baseDescriptor.Name;

      public bool CanWrite => this.baseDescriptor.CanWrite;

      public Type Type => this.baseDescriptor.Type;

      public Type TypeOverride
      {
        get => this.baseDescriptor.TypeOverride;
        set => this.baseDescriptor.TypeOverride = value;
      }

      public int Order
      {
        get => this.baseDescriptor.Order;
        set => this.baseDescriptor.Order = value;
      }

      public ScalarStyle ScalarStyle
      {
        get => this.baseDescriptor.ScalarStyle;
        set => this.baseDescriptor.ScalarStyle = value;
      }

      public void Write(object target, object value) => this.baseDescriptor.Write(target, value);

      public T GetCustomAttribute<T>() where T : Attribute => this.overrides.GetAttribute<T>(this.classType, this.Name) ?? this.baseDescriptor.GetCustomAttribute<T>();

      public IObjectDescriptor Read(object target) => this.baseDescriptor.Read(target);
    }
  }
}
