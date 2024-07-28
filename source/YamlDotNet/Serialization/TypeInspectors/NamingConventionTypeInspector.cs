// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.TypeInspectors.NamingConventionTypeInspector
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace YamlDotNet.Serialization.TypeInspectors
{
  public sealed class NamingConventionTypeInspector : TypeInspectorSkeleton
  {
    private readonly ITypeInspector innerTypeDescriptor;
    private readonly INamingConvention namingConvention;

    public NamingConventionTypeInspector(
      ITypeInspector innerTypeDescriptor,
      INamingConvention namingConvention)
    {
      this.innerTypeDescriptor = innerTypeDescriptor != null ? innerTypeDescriptor : throw new ArgumentNullException(nameof (innerTypeDescriptor));
      this.namingConvention = namingConvention != null ? namingConvention : throw new ArgumentNullException(nameof (namingConvention));
    }

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container) => this.innerTypeDescriptor.GetProperties(type, container).Select<IPropertyDescriptor, IPropertyDescriptor>((Func<IPropertyDescriptor, IPropertyDescriptor>) (p =>
    {
      YamlMemberAttribute customAttribute = p.GetCustomAttribute<YamlMemberAttribute>();
      if (customAttribute != null && !customAttribute.ApplyNamingConventions)
        return p;
      return (IPropertyDescriptor) new PropertyDescriptor(p)
      {
        Name = this.namingConvention.Apply(p.Name)
      };
    }));
  }
}
