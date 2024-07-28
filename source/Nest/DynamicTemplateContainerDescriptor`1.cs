// Decompiled with JetBrains decompiler
// Type: Nest.DynamicTemplateContainerDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DynamicTemplateContainerDescriptor<T> : 
    IsADictionaryDescriptorBase<DynamicTemplateContainerDescriptor<T>, IDynamicTemplateContainer, string, IDynamicTemplate>
    where T : class
  {
    public DynamicTemplateContainerDescriptor()
      : base((IDynamicTemplateContainer) new DynamicTemplateContainer())
    {
    }

    public DynamicTemplateContainerDescriptor<T> DynamicTemplate(
      string name,
      Func<DynamicTemplateDescriptor<T>, IDynamicTemplate> selector)
    {
      return this.Assign(name, selector != null ? selector(new DynamicTemplateDescriptor<T>()) : (IDynamicTemplate) null);
    }
  }
}
