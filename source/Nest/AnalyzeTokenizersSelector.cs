// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeTokenizersSelector
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AnalyzeTokenizersSelector : SelectorBase
  {
    public ITokenizer EdgeNGram(
      Func<EdgeNGramTokenizerDescriptor, IEdgeNGramTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new EdgeNGramTokenizerDescriptor());
    }

    public ITokenizer NGram(
      Func<NGramTokenizerDescriptor, INGramTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new NGramTokenizerDescriptor());
    }

    public ITokenizer Keyword(
      Func<KeywordTokenizerDescriptor, IKeywordTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new KeywordTokenizerDescriptor());
    }

    public ITokenizer Letter(
      Func<LetterTokenizerDescriptor, ILetterTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new LetterTokenizerDescriptor());
    }

    public ITokenizer Lowercase(
      Func<LowercaseTokenizerDescriptor, ILowercaseTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new LowercaseTokenizerDescriptor());
    }

    public ITokenizer PathHierarchy(
      Func<PathHierarchyTokenizerDescriptor, IPathHierarchyTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new PathHierarchyTokenizerDescriptor());
    }

    public ITokenizer Pattern(
      Func<PatternTokenizerDescriptor, IPatternTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new PatternTokenizerDescriptor());
    }

    public ITokenizer Standard(
      Func<StandardTokenizerDescriptor, IStandardTokenizer> selector = null)
    {
      return (ITokenizer) selector.InvokeOrDefault<StandardTokenizerDescriptor, IStandardTokenizer>(new StandardTokenizerDescriptor());
    }

    public ITokenizer UaxEmailUrl(
      Func<UaxEmailUrlTokenizerDescriptor, IUaxEmailUrlTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new UaxEmailUrlTokenizerDescriptor());
    }

    public ITokenizer Whitespace(
      Func<WhitespaceTokenizerDescriptor, IWhitespaceTokenizer> selector = null)
    {
      return (ITokenizer) selector.InvokeOrDefault<WhitespaceTokenizerDescriptor, IWhitespaceTokenizer>(new WhitespaceTokenizerDescriptor());
    }

    public ITokenizer Kuromoji(
      Func<KuromojiTokenizerDescriptor, IKuromojiTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new KuromojiTokenizerDescriptor());
    }

    public ITokenizer Icu(
      Func<IcuTokenizerDescriptor, IIcuTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new IcuTokenizerDescriptor());
    }

    public ITokenizer Nori(
      Func<NoriTokenizerDescriptor, INoriTokenizer> selector)
    {
      return (ITokenizer) selector(new NoriTokenizerDescriptor());
    }

    public ITokenizer CharGroup(
      Func<CharGroupTokenizerDescriptor, ICharGroupTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new CharGroupTokenizerDescriptor());
    }

    public ITokenizer SimplePattern(
      Func<SimplePatternTokenizerDescriptor, ISimplePatternTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new SimplePatternTokenizerDescriptor());
    }

    public ITokenizer SimplePatternSplit(
      Func<SimplePatternSplitTokenizerDescriptor, ISimplePatternSplitTokenizer> selector)
    {
      return selector == null ? (ITokenizer) null : (ITokenizer) selector(new SimplePatternSplitTokenizerDescriptor());
    }
  }
}
