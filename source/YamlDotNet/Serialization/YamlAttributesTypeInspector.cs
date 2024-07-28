// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.YamlAttributesTypeInspector
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;
using YamlDotNet.Serialization.TypeInspectors;

namespace YamlDotNet.Serialization
{
  public sealed class YamlAttributesTypeInspector : TypeInspectorSkeleton
  {
    private readonly ITypeInspector innerTypeDescriptor;

    public YamlAttributesTypeInspector(ITypeInspector innerTypeDescriptor) => this.innerTypeDescriptor = innerTypeDescriptor;

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container) => (IEnumerable<IPropertyDescriptor>) this.innerTypeDescriptor.GetProperties(type, container).Where<IPropertyDescriptor>((Func<IPropertyDescriptor, bool>) (p => p.GetCustomAttribute<YamlIgnoreAttribute>() == null)).Select<IPropertyDescriptor, IPropertyDescriptor>((Func<IPropertyDescriptor, IPropertyDescriptor>) (p =>
    {
      PropertyDescriptor properties = new PropertyDescriptor(p);
      YamlMemberAttribute customAttribute = p.GetCustomAttribute<YamlMemberAttribute>();
      if (customAttribute != null)
      {
        if ((object) customAttribute.SerializeAs != null)
          properties.TypeOverride = customAttribute.SerializeAs;
        properties.Order = customAttribute.Order;
        properties.ScalarStyle = customAttribute.ScalarStyle;
        if (customAttribute.Alias != null)
          properties.Name = customAttribute.Alias;
      }
      return (IPropertyDescriptor) properties;
    })).OrderBy<IPropertyDescriptor, int>((Func<IPropertyDescriptor, int>) (p => p.Order));
  }
}
