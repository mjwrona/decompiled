// Decompiled with JetBrains decompiler
// Type: Nest.KuromojiIterationMarkCharFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class KuromojiIterationMarkCharFilterDescriptor : 
    CharFilterDescriptorBase<KuromojiIterationMarkCharFilterDescriptor, IKuromojiIterationMarkCharFilter>,
    IKuromojiIterationMarkCharFilter,
    ICharFilter
  {
    protected override string Type => "kuromoji_iteration_mark";

    bool? IKuromojiIterationMarkCharFilter.NormalizeKana { get; set; }

    bool? IKuromojiIterationMarkCharFilter.NormalizeKanji { get; set; }

    public KuromojiIterationMarkCharFilterDescriptor NormalizeKanji(bool? normalize = true) => this.Assign<bool?>(normalize, (Action<IKuromojiIterationMarkCharFilter, bool?>) ((a, v) => a.NormalizeKanji = v));

    public KuromojiIterationMarkCharFilterDescriptor NormalizeKana(bool? normalize = true) => this.Assign<bool?>(normalize, (Action<IKuromojiIterationMarkCharFilter, bool?>) ((a, v) => a.NormalizeKana = v));
  }
}
