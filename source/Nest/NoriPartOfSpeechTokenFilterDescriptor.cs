// Decompiled with JetBrains decompiler
// Type: Nest.NoriPartOfSpeechTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class NoriPartOfSpeechTokenFilterDescriptor : 
    TokenFilterDescriptorBase<NoriPartOfSpeechTokenFilterDescriptor, INoriPartOfSpeechTokenFilter>,
    INoriPartOfSpeechTokenFilter,
    ITokenFilter
  {
    protected override string Type => "nori_part_of_speech";

    IEnumerable<string> INoriPartOfSpeechTokenFilter.StopTags { get; set; }

    public NoriPartOfSpeechTokenFilterDescriptor StopTags(IEnumerable<string> stopTags) => this.Assign<IEnumerable<string>>(stopTags, (Action<INoriPartOfSpeechTokenFilter, IEnumerable<string>>) ((a, v) => a.StopTags = v));

    public NoriPartOfSpeechTokenFilterDescriptor StopTags(params string[] stopTags) => this.Assign<string[]>(stopTags, (Action<INoriPartOfSpeechTokenFilter, string[]>) ((a, v) => a.StopTags = (IEnumerable<string>) v));
  }
}
