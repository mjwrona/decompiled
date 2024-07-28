// Decompiled with JetBrains decompiler
// Type: Nest.LowercaseTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class LowercaseTokenFilterDescriptor : 
    TokenFilterDescriptorBase<LowercaseTokenFilterDescriptor, ILowercaseTokenFilter>,
    ILowercaseTokenFilter,
    ITokenFilter
  {
    protected override string Type => "lowercase";

    string ILowercaseTokenFilter.Language { get; set; }

    public LowercaseTokenFilterDescriptor Language(string language) => this.Assign<string>(language, (Action<ILowercaseTokenFilter, string>) ((a, v) => a.Language = v));
  }
}
