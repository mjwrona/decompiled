// Decompiled with JetBrains decompiler
// Type: Nest.MappingsDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  [Obsolete("MappingsDescriptor is obsolete since 7.x Elasticsearch no longer treats mappings as a dictionary. Please use TypeMappingsDescriptor")]
  public class MappingsDescriptor : IDescriptor
  {
    [Obsolete("MappingsDescriptor is obsolete please call Map() on the parent descriptor")]
    public ITypeMapping Map<T>(
      Func<TypeMappingDescriptor<T>, ITypeMapping> selector)
      where T : class
    {
      return (ITypeMapping) new PreventMappingMultipleTypesDescriptor(selector != null ? selector(new TypeMappingDescriptor<T>()) : (ITypeMapping) null);
    }

    [Obsolete("MappingsDescriptor is obsolete please call Map() on the parent descriptor")]
    public ITypeMapping Map(
      Func<TypeMappingDescriptor<object>, ITypeMapping> selector)
    {
      return (ITypeMapping) new PreventMappingMultipleTypesDescriptor(selector != null ? selector(new TypeMappingDescriptor<object>()) : (ITypeMapping) null);
    }

    [Obsolete("Types are gone from Elasticsearch 7.x the first argument is completely ignored please remove it as we will in 8.x")]
    public ITypeMapping Map<T>(
      object type,
      Func<TypeMappingDescriptor<T>, ITypeMapping> selector)
      where T : class
    {
      return (ITypeMapping) new PreventMappingMultipleTypesDescriptor(selector != null ? selector(new TypeMappingDescriptor<T>()) : (ITypeMapping) null);
    }

    [Obsolete("Types are gone from Elasticsearch 7.x the first argument is completely ignored please remove it as we will in 8.x")]
    public ITypeMapping Map(
      object type,
      Func<TypeMappingDescriptor<object>, ITypeMapping> selector)
    {
      return (ITypeMapping) new PreventMappingMultipleTypesDescriptor(selector != null ? selector(new TypeMappingDescriptor<object>()) : (ITypeMapping) null);
    }
  }
}
