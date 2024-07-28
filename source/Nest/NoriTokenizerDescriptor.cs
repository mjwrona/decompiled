// Decompiled with JetBrains decompiler
// Type: Nest.NoriTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class NoriTokenizerDescriptor : 
    TokenizerDescriptorBase<NoriTokenizerDescriptor, INoriTokenizer>,
    INoriTokenizer,
    ITokenizer
  {
    protected override string Type => "nori_tokenizer";

    NoriDecompoundMode? INoriTokenizer.DecompoundMode { get; set; }

    string INoriTokenizer.UserDictionary { get; set; }

    IEnumerable<string> INoriTokenizer.UserDictionaryRules { get; set; }

    bool? INoriTokenizer.DiscardPunctuation { get; set; }

    public NoriTokenizerDescriptor DecompoundMode(NoriDecompoundMode? mode) => this.Assign<NoriDecompoundMode?>(mode, (Action<INoriTokenizer, NoriDecompoundMode?>) ((a, v) => a.DecompoundMode = v));

    public NoriTokenizerDescriptor UserDictionary(string path) => this.Assign<string>(path, (Action<INoriTokenizer, string>) ((a, v) => a.UserDictionary = v));

    public NoriTokenizerDescriptor UserDictionaryRules(params string[] rules) => this.Assign<string[]>(rules, (Action<INoriTokenizer, string[]>) ((a, v) => a.UserDictionaryRules = (IEnumerable<string>) v));

    public NoriTokenizerDescriptor UserDictionaryRules(IEnumerable<string> rules) => this.Assign<IEnumerable<string>>(rules, (Action<INoriTokenizer, IEnumerable<string>>) ((a, v) => a.UserDictionaryRules = v));

    public NoriTokenizerDescriptor DiscardPunctuation(bool? discard = true) => this.Assign<bool?>(discard, (Action<INoriTokenizer, bool?>) ((a, v) => a.DiscardPunctuation = v));
  }
}
