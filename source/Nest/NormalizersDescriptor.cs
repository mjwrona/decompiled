// Decompiled with JetBrains decompiler
// Type: Nest.NormalizersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class NormalizersDescriptor : 
    IsADictionaryDescriptorBase<NormalizersDescriptor, INormalizers, string, INormalizer>
  {
    public NormalizersDescriptor()
      : base((INormalizers) new Normalizers())
    {
    }

    public NormalizersDescriptor UserDefined(string name, INormalizer analyzer) => this.Assign(name, analyzer);

    public NormalizersDescriptor Custom(
      string name,
      Func<CustomNormalizerDescriptor, ICustomNormalizer> selector)
    {
      return this.Assign(name, selector != null ? (INormalizer) selector(new CustomNormalizerDescriptor()) : (INormalizer) null);
    }
  }
}
