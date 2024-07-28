// Decompiled with JetBrains decompiler
// Type: Nest.KuromojiTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class KuromojiTokenizerDescriptor : 
    TokenizerDescriptorBase<KuromojiTokenizerDescriptor, IKuromojiTokenizer>,
    IKuromojiTokenizer,
    ITokenizer
  {
    protected override string Type => "kuromoji_tokenizer";

    bool? IKuromojiTokenizer.DiscardPunctuation { get; set; }

    bool? IKuromojiTokenizer.DiscardCompoundToken { get; set; }

    KuromojiTokenizationMode? IKuromojiTokenizer.Mode { get; set; }

    int? IKuromojiTokenizer.NBestCost { get; set; }

    string IKuromojiTokenizer.NBestExamples { get; set; }

    string IKuromojiTokenizer.UserDictionary { get; set; }

    IEnumerable<string> IKuromojiTokenizer.UserDictionaryRules { get; set; }

    public KuromojiTokenizerDescriptor Mode(KuromojiTokenizationMode? mode) => this.Assign<KuromojiTokenizationMode?>(mode, (Action<IKuromojiTokenizer, KuromojiTokenizationMode?>) ((a, v) => a.Mode = v));

    public KuromojiTokenizerDescriptor DiscardPunctuation(bool? discard = true) => this.Assign<bool?>(discard, (Action<IKuromojiTokenizer, bool?>) ((a, v) => a.DiscardPunctuation = v));

    public KuromojiTokenizerDescriptor DiscardCompoundToken(bool? discard = true) => this.Assign<bool?>(discard, (Action<IKuromojiTokenizer, bool?>) ((a, v) => a.DiscardCompoundToken = v));

    public KuromojiTokenizerDescriptor UserDictionary(string userDictionary) => this.Assign<string>(userDictionary, (Action<IKuromojiTokenizer, string>) ((a, v) => a.UserDictionary = v));

    public KuromojiTokenizerDescriptor NBestExamples(string examples) => this.Assign<string>(examples, (Action<IKuromojiTokenizer, string>) ((a, v) => a.NBestExamples = v));

    public KuromojiTokenizerDescriptor NBestCost(int? cost) => this.Assign<int?>(cost, (Action<IKuromojiTokenizer, int?>) ((a, v) => a.NBestCost = v));

    public KuromojiTokenizerDescriptor UserDictionaryRules(IEnumerable<string> rules) => this.Assign<IEnumerable<string>>(rules, (Action<IKuromojiTokenizer, IEnumerable<string>>) ((a, v) => a.UserDictionaryRules = rules));

    public KuromojiTokenizerDescriptor UserDictionaryRules(params string[] rules) => this.Assign<string[]>(rules, (Action<IKuromojiTokenizer, string[]>) ((a, v) => a.UserDictionaryRules = (IEnumerable<string>) rules));
  }
}
