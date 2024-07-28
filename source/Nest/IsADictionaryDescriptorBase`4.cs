// Decompiled with JetBrains decompiler
// Type: Nest.IsADictionaryDescriptorBase`4
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public abstract class IsADictionaryDescriptorBase<TDescriptor, TInterface, TKey, TValue> : 
    DescriptorPromiseBase<TDescriptor, TInterface>
    where TDescriptor : IsADictionaryDescriptorBase<TDescriptor, TInterface, TKey, TValue>
    where TInterface : class, IIsADictionary<TKey, TValue>
  {
    protected IsADictionaryDescriptorBase(TInterface instance)
      : base(instance)
    {
    }

    protected TDescriptor Assign(TKey key, TValue value)
    {
      this.PromisedValue.Add(key, value);
      return this.Self;
    }
  }
}
