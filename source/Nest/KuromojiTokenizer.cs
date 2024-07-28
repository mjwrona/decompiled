// Decompiled with JetBrains decompiler
// Type: Nest.KuromojiTokenizer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class KuromojiTokenizer : TokenizerBase, IKuromojiTokenizer, ITokenizer
  {
    public KuromojiTokenizer() => this.Type = "kuromoji_tokenizer";

    public bool? DiscardPunctuation { get; set; }

    public bool? DiscardCompoundToken { get; set; }

    public KuromojiTokenizationMode? Mode { get; set; }

    public int? NBestCost { get; set; }

    public string NBestExamples { get; set; }

    public string UserDictionary { get; set; }

    public IEnumerable<string> UserDictionaryRules { get; set; }
  }
}
