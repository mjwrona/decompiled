// Decompiled with JetBrains decompiler
// Type: Nest.KuromojiPartOfSpeechTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class KuromojiPartOfSpeechTokenFilterDescriptor : 
    TokenFilterDescriptorBase<KuromojiPartOfSpeechTokenFilterDescriptor, IKuromojiPartOfSpeechTokenFilter>,
    IKuromojiPartOfSpeechTokenFilter,
    ITokenFilter
  {
    protected override string Type => "kuromoji_part_of_speech";

    IEnumerable<string> IKuromojiPartOfSpeechTokenFilter.StopTags { get; set; }

    public KuromojiPartOfSpeechTokenFilterDescriptor StopTags(IEnumerable<string> stopTags) => this.Assign<IEnumerable<string>>(stopTags, (Action<IKuromojiPartOfSpeechTokenFilter, IEnumerable<string>>) ((a, v) => a.StopTags = v));

    public KuromojiPartOfSpeechTokenFilterDescriptor StopTags(params string[] stopTags) => this.Assign<string[]>(stopTags, (Action<IKuromojiPartOfSpeechTokenFilter, string[]>) ((a, v) => a.StopTags = (IEnumerable<string>) v));
  }
}
