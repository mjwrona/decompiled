// Decompiled with JetBrains decompiler
// Type: Nest.TokenizersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TokenizersDescriptor : 
    IsADictionaryDescriptorBase<TokenizersDescriptor, ITokenizers, string, ITokenizer>
  {
    public TokenizersDescriptor()
      : base((ITokenizers) new Tokenizers())
    {
    }

    public TokenizersDescriptor UserDefined(string name, ITokenizer analyzer) => this.Assign(name, analyzer);

    public TokenizersDescriptor EdgeNGram(
      string name,
      Func<EdgeNGramTokenizerDescriptor, IEdgeNGramTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new EdgeNGramTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor NGram(
      string name,
      Func<NGramTokenizerDescriptor, INGramTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new NGramTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor Keyword(
      string name,
      Func<KeywordTokenizerDescriptor, IKeywordTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new KeywordTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor Letter(
      string name,
      Func<LetterTokenizerDescriptor, ILetterTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new LetterTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor Lowercase(
      string name,
      Func<LowercaseTokenizerDescriptor, ILowercaseTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new LowercaseTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor PathHierarchy(
      string name,
      Func<PathHierarchyTokenizerDescriptor, IPathHierarchyTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new PathHierarchyTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor Pattern(
      string name,
      Func<PatternTokenizerDescriptor, IPatternTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new PatternTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor Standard(
      string name,
      Func<StandardTokenizerDescriptor, IStandardTokenizer> selector = null)
    {
      return this.Assign(name, (ITokenizer) selector.InvokeOrDefault<StandardTokenizerDescriptor, IStandardTokenizer>(new StandardTokenizerDescriptor()));
    }

    public TokenizersDescriptor UaxEmailUrl(
      string name,
      Func<UaxEmailUrlTokenizerDescriptor, IUaxEmailUrlTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new UaxEmailUrlTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor Whitespace(
      string name,
      Func<WhitespaceTokenizerDescriptor, IWhitespaceTokenizer> selector = null)
    {
      return this.Assign(name, (ITokenizer) selector.InvokeOrDefault<WhitespaceTokenizerDescriptor, IWhitespaceTokenizer>(new WhitespaceTokenizerDescriptor()));
    }

    public TokenizersDescriptor Kuromoji(
      string name,
      Func<KuromojiTokenizerDescriptor, IKuromojiTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new KuromojiTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor Icu(
      string name,
      Func<IcuTokenizerDescriptor, IIcuTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new IcuTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor Nori(
      string name,
      Func<NoriTokenizerDescriptor, INoriTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new NoriTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor CharGroup(
      string name,
      Func<CharGroupTokenizerDescriptor, ICharGroupTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new CharGroupTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor SimplePattern(
      string name,
      Func<SimplePatternTokenizerDescriptor, ISimplePatternTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new SimplePatternTokenizerDescriptor()) : (ITokenizer) null);
    }

    public TokenizersDescriptor SimplePatternSplit(
      string name,
      Func<SimplePatternSplitTokenizerDescriptor, ISimplePatternSplitTokenizer> selector)
    {
      return this.Assign(name, selector != null ? (ITokenizer) selector(new SimplePatternSplitTokenizerDescriptor()) : (ITokenizer) null);
    }
  }
}
