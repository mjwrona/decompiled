// Decompiled with JetBrains decompiler
// Type: Nest.PhraseSuggesterDescriptor`1
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nest
{
  public class PhraseSuggesterDescriptor<T> : 
    SuggestDescriptorBase<PhraseSuggesterDescriptor<T>, IPhraseSuggester, T>,
    IPhraseSuggester,
    ISuggester
    where T : class
  {
    IPhraseSuggestCollate IPhraseSuggester.Collate { get; set; }

    double? IPhraseSuggester.Confidence { get; set; }

    IEnumerable<IDirectGenerator> IPhraseSuggester.DirectGenerator { get; set; }

    bool? IPhraseSuggester.ForceUnigrams { get; set; }

    int? IPhraseSuggester.GramSize { get; set; }

    IPhraseSuggestHighlight IPhraseSuggester.Highlight { get; set; }

    double? IPhraseSuggester.MaxErrors { get; set; }

    double? IPhraseSuggester.RealWordErrorLikelihood { get; set; }

    char? IPhraseSuggester.Separator { get; set; }

    int? IPhraseSuggester.ShardSize { get; set; }

    SmoothingModelContainer IPhraseSuggester.Smoothing { get; set; }

    string IPhraseSuggester.Text { get; set; }

    int? IPhraseSuggester.TokenLimit { get; set; }

    public PhraseSuggesterDescriptor<T> Text(string text) => this.Assign<string>(text, (Action<IPhraseSuggester, string>) ((a, v) => a.Text = v));

    public PhraseSuggesterDescriptor<T> ShardSize(int? shardSize) => this.Assign<int?>(shardSize, (Action<IPhraseSuggester, int?>) ((a, v) => a.ShardSize = v));

    public PhraseSuggesterDescriptor<T> GramSize(int? gramSize) => this.Assign<int?>(gramSize, (Action<IPhraseSuggester, int?>) ((a, v) => a.GramSize = v));

    public PhraseSuggesterDescriptor<T> Confidence(double? confidence) => this.Assign<double?>(confidence, (Action<IPhraseSuggester, double?>) ((a, v) => a.Confidence = v));

    public PhraseSuggesterDescriptor<T> MaxErrors(double? maxErrors) => this.Assign<double?>(maxErrors, (Action<IPhraseSuggester, double?>) ((a, v) => a.MaxErrors = v));

    public PhraseSuggesterDescriptor<T> Separator(char? separator) => this.Assign<char?>(separator, (Action<IPhraseSuggester, char?>) ((a, v) => a.Separator = v));

    public PhraseSuggesterDescriptor<T> DirectGenerator(
      params Func<DirectGeneratorDescriptor<T>, IDirectGenerator>[] generators)
    {
      return this.Assign<List<IDirectGenerator>>(((IEnumerable<Func<DirectGeneratorDescriptor<T>, IDirectGenerator>>) generators).Select<Func<DirectGeneratorDescriptor<T>, IDirectGenerator>, IDirectGenerator>((Func<Func<DirectGeneratorDescriptor<T>, IDirectGenerator>, IDirectGenerator>) (g => g(new DirectGeneratorDescriptor<T>()))).ToList<IDirectGenerator>(), (Action<IPhraseSuggester, List<IDirectGenerator>>) ((a, v) => a.DirectGenerator = (IEnumerable<IDirectGenerator>) v));
    }

    public PhraseSuggesterDescriptor<T> RealWordErrorLikelihood(double? realWordErrorLikelihood) => this.Assign<double?>(realWordErrorLikelihood, (Action<IPhraseSuggester, double?>) ((a, v) => a.RealWordErrorLikelihood = v));

    public PhraseSuggesterDescriptor<T> Highlight(
      Func<PhraseSuggestHighlightDescriptor, IPhraseSuggestHighlight> selector)
    {
      return this.Assign<Func<PhraseSuggestHighlightDescriptor, IPhraseSuggestHighlight>>(selector, (Action<IPhraseSuggester, Func<PhraseSuggestHighlightDescriptor, IPhraseSuggestHighlight>>) ((a, v) => a.Highlight = v != null ? v(new PhraseSuggestHighlightDescriptor()) : (IPhraseSuggestHighlight) null));
    }

    public PhraseSuggesterDescriptor<T> Collate(
      Func<PhraseSuggestCollateDescriptor<T>, IPhraseSuggestCollate> selector)
    {
      return this.Assign<Func<PhraseSuggestCollateDescriptor<T>, IPhraseSuggestCollate>>(selector, (Action<IPhraseSuggester, Func<PhraseSuggestCollateDescriptor<T>, IPhraseSuggestCollate>>) ((a, v) => a.Collate = v != null ? v(new PhraseSuggestCollateDescriptor<T>()) : (IPhraseSuggestCollate) null));
    }

    public PhraseSuggesterDescriptor<T> Smoothing(
      Func<SmoothingModelContainerDescriptor, SmoothingModelContainer> selector)
    {
      return this.Assign<Func<SmoothingModelContainerDescriptor, SmoothingModelContainer>>(selector, (Action<IPhraseSuggester, Func<SmoothingModelContainerDescriptor, SmoothingModelContainer>>) ((a, v) => a.Smoothing = v != null ? v(new SmoothingModelContainerDescriptor()) : (SmoothingModelContainer) null));
    }

    public PhraseSuggesterDescriptor<T> TokenLimit(int? tokenLimit) => this.Assign<int?>(tokenLimit, (Action<IPhraseSuggester, int?>) ((a, v) => a.TokenLimit = v));

    public PhraseSuggesterDescriptor<T> ForceUnigrams(bool? forceUnigrams = true) => this.Assign<bool?>(forceUnigrams, (Action<IPhraseSuggester, bool?>) ((a, v) => a.ForceUnigrams = v));
  }
}
