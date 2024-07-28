// Decompiled with JetBrains decompiler
// Type: Nest.AnalysisDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Runtime.Serialization;

namespace Nest
{
  [DataContract]
  public class AnalysisDescriptor : DescriptorBase<AnalysisDescriptor, IAnalysis>, IAnalysis
  {
    IAnalyzers IAnalysis.Analyzers { get; set; }

    ICharFilters IAnalysis.CharFilters { get; set; }

    INormalizers IAnalysis.Normalizers { get; set; }

    ITokenFilters IAnalysis.TokenFilters { get; set; }

    ITokenizers IAnalysis.Tokenizers { get; set; }

    public AnalysisDescriptor Analyzers(
      Func<AnalyzersDescriptor, IPromise<IAnalyzers>> selector)
    {
      return this.Assign<Func<AnalyzersDescriptor, IPromise<IAnalyzers>>>(selector, (Action<IAnalysis, Func<AnalyzersDescriptor, IPromise<IAnalyzers>>>) ((a, v) => a.Analyzers = v != null ? v(new AnalyzersDescriptor())?.Value : (IAnalyzers) null));
    }

    public AnalysisDescriptor CharFilters(
      Func<CharFiltersDescriptor, IPromise<ICharFilters>> selector)
    {
      return this.Assign<Func<CharFiltersDescriptor, IPromise<ICharFilters>>>(selector, (Action<IAnalysis, Func<CharFiltersDescriptor, IPromise<ICharFilters>>>) ((a, v) => a.CharFilters = v != null ? v(new CharFiltersDescriptor())?.Value : (ICharFilters) null));
    }

    public AnalysisDescriptor TokenFilters(
      Func<TokenFiltersDescriptor, IPromise<ITokenFilters>> selector)
    {
      return this.Assign<Func<TokenFiltersDescriptor, IPromise<ITokenFilters>>>(selector, (Action<IAnalysis, Func<TokenFiltersDescriptor, IPromise<ITokenFilters>>>) ((a, v) => a.TokenFilters = v != null ? v(new TokenFiltersDescriptor())?.Value : (ITokenFilters) null));
    }

    public AnalysisDescriptor Tokenizers(
      Func<TokenizersDescriptor, IPromise<ITokenizers>> selector)
    {
      return this.Assign<Func<TokenizersDescriptor, IPromise<ITokenizers>>>(selector, (Action<IAnalysis, Func<TokenizersDescriptor, IPromise<ITokenizers>>>) ((a, v) => a.Tokenizers = v != null ? v(new TokenizersDescriptor())?.Value : (ITokenizers) null));
    }

    public AnalysisDescriptor Normalizers(
      Func<NormalizersDescriptor, IPromise<INormalizers>> selector)
    {
      return this.Assign<Func<NormalizersDescriptor, IPromise<INormalizers>>>(selector, (Action<IAnalysis, Func<NormalizersDescriptor, IPromise<INormalizers>>>) ((a, v) => a.Normalizers = v != null ? v(new NormalizersDescriptor())?.Value : (INormalizers) null));
    }
  }
}
