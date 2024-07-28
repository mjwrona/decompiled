// Decompiled with JetBrains decompiler
// Type: Nest.TokenFiltersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TokenFiltersDescriptor : 
    IsADictionaryDescriptorBase<TokenFiltersDescriptor, ITokenFilters, string, ITokenFilter>
  {
    public TokenFiltersDescriptor()
      : base((ITokenFilters) new TokenFilters())
    {
    }

    public TokenFiltersDescriptor UserDefined(string name, ITokenFilter analyzer) => this.Assign(name, analyzer);

    public TokenFiltersDescriptor DictionaryDecompounder(
      string name,
      Func<DictionaryDecompounderTokenFilterDescriptor, IDictionaryDecompounderTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new DictionaryDecompounderTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor HyphenationDecompounder(
      string name,
      Func<HyphenationDecompounderTokenFilterDescriptor, IHyphenationDecompounderTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new HyphenationDecompounderTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor EdgeNGram(
      string name,
      Func<EdgeNGramTokenFilterDescriptor, IEdgeNGramTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new EdgeNGramTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Phonetic(
      string name,
      Func<PhoneticTokenFilterDescriptor, IPhoneticTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new PhoneticTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Shingle(
      string name,
      Func<ShingleTokenFilterDescriptor, IShingleTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new ShingleTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Stop(
      string name,
      Func<StopTokenFilterDescriptor, IStopTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new StopTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Synonym(
      string name,
      Func<SynonymTokenFilterDescriptor, ISynonymTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new SynonymTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor SynonymGraph(
      string name,
      Func<SynonymGraphTokenFilterDescriptor, ISynonymGraphTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new SynonymGraphTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor WordDelimiter(
      string name,
      Func<WordDelimiterTokenFilterDescriptor, IWordDelimiterTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new WordDelimiterTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor WordDelimiterGraph(
      string name,
      Func<WordDelimiterGraphTokenFilterDescriptor, IWordDelimiterGraphTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new WordDelimiterGraphTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor AsciiFolding(
      string name,
      Func<AsciiFoldingTokenFilterDescriptor, IAsciiFoldingTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new AsciiFoldingTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor CommonGrams(
      string name,
      Func<CommonGramsTokenFilterDescriptor, ICommonGramsTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new CommonGramsTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor DelimitedPayload(
      string name,
      Func<DelimitedPayloadTokenFilterDescriptor, IDelimitedPayloadTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new DelimitedPayloadTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Elision(
      string name,
      Func<ElisionTokenFilterDescriptor, IElisionTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new ElisionTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Hunspell(
      string name,
      Func<HunspellTokenFilterDescriptor, IHunspellTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new HunspellTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor KeepTypes(
      string name,
      Func<KeepTypesTokenFilterDescriptor, IKeepTypesTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new KeepTypesTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor KeepWords(
      string name,
      Func<KeepWordsTokenFilterDescriptor, IKeepWordsTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new KeepWordsTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor KeywordMarker(
      string name,
      Func<KeywordMarkerTokenFilterDescriptor, IKeywordMarkerTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new KeywordMarkerTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor KStem(
      string name,
      Func<KStemTokenFilterDescriptor, IKStemTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<KStemTokenFilterDescriptor, IKStemTokenFilter>(new KStemTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor Length(
      string name,
      Func<LengthTokenFilterDescriptor, ILengthTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new LengthTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor LimitTokenCount(
      string name,
      Func<LimitTokenCountTokenFilterDescriptor, ILimitTokenCountTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new LimitTokenCountTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Lowercase(
      string name,
      Func<LowercaseTokenFilterDescriptor, ILowercaseTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<LowercaseTokenFilterDescriptor, ILowercaseTokenFilter>(new LowercaseTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor NGram(
      string name,
      Func<NGramTokenFilterDescriptor, INGramTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new NGramTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor PatternCapture(
      string name,
      Func<PatternCaptureTokenFilterDescriptor, IPatternCaptureTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new PatternCaptureTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor PatternReplace(
      string name,
      Func<PatternReplaceTokenFilterDescriptor, IPatternReplaceTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new PatternReplaceTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor PorterStem(
      string name,
      Func<PorterStemTokenFilterDescriptor, IPorterStemTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<PorterStemTokenFilterDescriptor, IPorterStemTokenFilter>(new PorterStemTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor Reverse(
      string name,
      Func<ReverseTokenFilterDescriptor, IReverseTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<ReverseTokenFilterDescriptor, IReverseTokenFilter>(new ReverseTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor Snowball(
      string name,
      Func<SnowballTokenFilterDescriptor, ISnowballTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new SnowballTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Stemmer(
      string name,
      Func<StemmerTokenFilterDescriptor, IStemmerTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new StemmerTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Predicate(
      string name,
      Func<PredicateTokenFilterDescriptor, IPredicateTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new PredicateTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Condition(
      string name,
      Func<ConditionTokenFilterDescriptor, IConditionTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new ConditionTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor StemmerOverride(
      string name,
      Func<StemmerOverrideTokenFilterDescriptor, IStemmerOverrideTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new StemmerOverrideTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Trim(
      string name,
      Func<TrimTokenFilterDescriptor, ITrimTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<TrimTokenFilterDescriptor, ITrimTokenFilter>(new TrimTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor Truncate(
      string name,
      Func<TruncateTokenFilterDescriptor, ITruncateTokenFilter> selector)
    {
      return this.Assign(name, selector != null ? (ITokenFilter) selector(new TruncateTokenFilterDescriptor()) : (ITokenFilter) null);
    }

    public TokenFiltersDescriptor Unique(
      string name,
      Func<UniqueTokenFilterDescriptor, IUniqueTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<UniqueTokenFilterDescriptor, IUniqueTokenFilter>(new UniqueTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor Uppercase(
      string name,
      Func<UppercaseTokenFilterDescriptor, IUppercaseTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<UppercaseTokenFilterDescriptor, IUppercaseTokenFilter>(new UppercaseTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor Fingerprint(
      string name,
      Func<FingerprintTokenFilterDescriptor, IFingerprintTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<FingerprintTokenFilterDescriptor, IFingerprintTokenFilter>(new FingerprintTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor KuromojiStemmer(
      string name,
      Func<KuromojiStemmerTokenFilterDescriptor, IKuromojiStemmerTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<KuromojiStemmerTokenFilterDescriptor, IKuromojiStemmerTokenFilter>(new KuromojiStemmerTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor KuromojiReadingForm(
      string name,
      Func<KuromojiReadingFormTokenFilterDescriptor, IKuromojiReadingFormTokenFilter> selector)
    {
      return this.Assign(name, (ITokenFilter) selector(new KuromojiReadingFormTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor KuromojiPartOfSpeech(
      string name,
      Func<KuromojiPartOfSpeechTokenFilterDescriptor, IKuromojiPartOfSpeechTokenFilter> selector)
    {
      return this.Assign(name, (ITokenFilter) selector(new KuromojiPartOfSpeechTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor IcuCollation(
      string name,
      Func<IcuCollationTokenFilterDescriptor, IIcuCollationTokenFilter> selector)
    {
      return this.Assign(name, (ITokenFilter) selector(new IcuCollationTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor IcuFolding(
      string name,
      Func<IcuFoldingTokenFilterDescriptor, IIcuFoldingTokenFilter> selector)
    {
      return this.Assign(name, (ITokenFilter) selector(new IcuFoldingTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor IcuNormalization(
      string name,
      Func<IcuNormalizationTokenFilterDescriptor, IIcuNormalizationTokenFilter> selector)
    {
      return this.Assign(name, (ITokenFilter) selector(new IcuNormalizationTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor IcuTransform(
      string name,
      Func<IcuTransformTokenFilterDescriptor, IIcuTransformTokenFilter> selector)
    {
      return this.Assign(name, (ITokenFilter) selector(new IcuTransformTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor NoriPartOfSpeech(
      string name,
      Func<NoriPartOfSpeechTokenFilterDescriptor, INoriPartOfSpeechTokenFilter> selector)
    {
      return this.Assign(name, (ITokenFilter) selector(new NoriPartOfSpeechTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor Multiplexer(
      string name,
      Func<MultiplexerTokenFilterDescriptor, IMultiplexerTokenFilter> selector)
    {
      return this.Assign(name, (ITokenFilter) selector(new MultiplexerTokenFilterDescriptor()));
    }

    public TokenFiltersDescriptor RemoveDuplicates(
      string name,
      Func<RemoveDuplicatesTokenFilterDescriptor, IRemoveDuplicatesTokenFilter> selector = null)
    {
      return this.Assign(name, (ITokenFilter) selector.InvokeOrDefault<RemoveDuplicatesTokenFilterDescriptor, IRemoveDuplicatesTokenFilter>(new RemoveDuplicatesTokenFilterDescriptor()));
    }
  }
}
