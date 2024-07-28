// Decompiled with JetBrains decompiler
// Type: Nest.TemplateDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TemplateDescriptor : DescriptorBase<TemplateDescriptor, ITemplate>, ITemplate
  {
    IAliases ITemplate.Aliases { get; set; }

    ITypeMapping ITemplate.Mappings { get; set; }

    IIndexSettings ITemplate.Settings { get; set; }

    public TemplateDescriptor Settings(
      Func<IndexSettingsDescriptor, IPromise<IIndexSettings>> selector)
    {
      return this.Assign<Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>(selector, (Action<ITemplate, Func<IndexSettingsDescriptor, IPromise<IIndexSettings>>>) ((a, v) => a.Settings = v != null ? v(new IndexSettingsDescriptor())?.Value : (IIndexSettings) null));
    }

    public TemplateDescriptor Mappings<T>(
      Func<TypeMappingDescriptor<T>, ITypeMapping> selector)
      where T : class
    {
      return this.Assign<Func<TypeMappingDescriptor<T>, ITypeMapping>>(selector, (Action<ITemplate, Func<TypeMappingDescriptor<T>, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new TypeMappingDescriptor<T>()) : (ITypeMapping) null));
    }

    public TemplateDescriptor Mappings(
      Func<TypeMappingDescriptor<object>, ITypeMapping> selector)
    {
      return this.Assign<Func<TypeMappingDescriptor<object>, ITypeMapping>>(selector, (Action<ITemplate, Func<TypeMappingDescriptor<object>, ITypeMapping>>) ((a, v) => a.Mappings = v != null ? v(new TypeMappingDescriptor<object>()) : (ITypeMapping) null));
    }

    public TemplateDescriptor Aliases(
      Func<AliasesDescriptor, IPromise<IAliases>> selector)
    {
      return this.Assign<Func<AliasesDescriptor, IPromise<IAliases>>>(selector, (Action<ITemplate, Func<AliasesDescriptor, IPromise<IAliases>>>) ((a, v) => a.Aliases = v != null ? v(new AliasesDescriptor())?.Value : (IAliases) null));
    }
  }
}
