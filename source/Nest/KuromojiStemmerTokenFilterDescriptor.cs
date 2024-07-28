// Decompiled with JetBrains decompiler
// Type: Nest.KuromojiStemmerTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class KuromojiStemmerTokenFilterDescriptor : 
    TokenFilterDescriptorBase<KuromojiStemmerTokenFilterDescriptor, IKuromojiStemmerTokenFilter>,
    IKuromojiStemmerTokenFilter,
    ITokenFilter
  {
    protected override string Type => "kuromoji_stemmer";

    int? IKuromojiStemmerTokenFilter.MinimumLength { get; set; }

    public KuromojiStemmerTokenFilterDescriptor MinimumLength(int? minimumLength) => this.Assign<int?>(minimumLength, (Action<IKuromojiStemmerTokenFilter, int?>) ((a, v) => a.MinimumLength = v));
  }
}
