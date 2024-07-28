// Decompiled with JetBrains decompiler
// Type: Nest.StemmerTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class StemmerTokenFilterDescriptor : 
    TokenFilterDescriptorBase<StemmerTokenFilterDescriptor, IStemmerTokenFilter>,
    IStemmerTokenFilter,
    ITokenFilter
  {
    protected override string Type => "stemmer";

    string IStemmerTokenFilter.Language { get; set; }

    public StemmerTokenFilterDescriptor Language(string language) => this.Assign<string>(language, (Action<IStemmerTokenFilter, string>) ((a, v) => a.Language = v));
  }
}
