// Decompiled with JetBrains decompiler
// Type: YamlDotNet.Serialization.TypeInspectors.CachedTypeInspector
// Assembly: YamlDotNet, Version=5.0.0.0, Culture=neutral, PublicKeyToken=ec19458f3c15af5e
// MVID: 5F9DD5C4-A41D-46B2-A793-8157A0D55AB5
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\YamlDotNet.dll

using System;
using System.Collections.Generic;

namespace YamlDotNet.Serialization.TypeInspectors
{
  public sealed class CachedTypeInspector : TypeInspectorSkeleton
  {
    private readonly ITypeInspector innerTypeDescriptor;
    private readonly Dictionary<Type, List<IPropertyDescriptor>> cache = new Dictionary<Type, List<IPropertyDescriptor>>();

    public CachedTypeInspector(ITypeInspector innerTypeDescriptor) => this.innerTypeDescriptor = innerTypeDescriptor != null ? innerTypeDescriptor : throw new ArgumentNullException(nameof (innerTypeDescriptor));

    public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
    {
      List<IPropertyDescriptor> properties;
      if (!this.cache.TryGetValue(type, out properties))
      {
        properties = new List<IPropertyDescriptor>(this.innerTypeDescriptor.GetProperties(type, container));
        this.cache.Add(type, properties);
      }
      return (IEnumerable<IPropertyDescriptor>) properties;
    }
  }
}
