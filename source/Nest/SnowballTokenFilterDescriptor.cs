// Decompiled with JetBrains decompiler
// Type: Nest.SnowballTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SnowballTokenFilterDescriptor : 
    TokenFilterDescriptorBase<SnowballTokenFilterDescriptor, ISnowballTokenFilter>,
    ISnowballTokenFilter,
    ITokenFilter
  {
    protected override string Type => "snowball";

    SnowballLanguage? ISnowballTokenFilter.Language { get; set; }

    public SnowballTokenFilterDescriptor Language(SnowballLanguage? language) => this.Assign<SnowballLanguage?>(language, (Action<ISnowballTokenFilter, SnowballLanguage?>) ((a, v) => a.Language = v));
  }
}
