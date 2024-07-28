// Decompiled with JetBrains decompiler
// Type: Nest.AnalyzeCharFiltersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class AnalyzeCharFiltersDescriptor : 
    DescriptorPromiseBase<AnalyzeCharFiltersDescriptor, AnalyzeCharFilters>
  {
    public AnalyzeCharFiltersDescriptor()
      : base(new AnalyzeCharFilters())
    {
    }

    public AnalyzeCharFiltersDescriptor Name(string tokenFilter) => this.Assign<string>(tokenFilter, (Action<AnalyzeCharFilters, string>) ((a, v) => a.AddIfNotNull<Union<string, ICharFilter>>((Union<string, ICharFilter>) v)));

    private AnalyzeCharFiltersDescriptor AssignIfNotNull(ICharFilter filter) => this.Assign<ICharFilter>(filter, (Action<AnalyzeCharFilters, ICharFilter>) ((a, v) =>
    {
      if (v == null)
        return;
      a.Add(v);
    }));

    public AnalyzeCharFiltersDescriptor PatternReplace(
      Func<PatternReplaceCharFilterDescriptor, IPatternReplaceCharFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ICharFilter) selector(new PatternReplaceCharFilterDescriptor()) : (ICharFilter) null);
    }

    public AnalyzeCharFiltersDescriptor HtmlStrip(
      Func<HtmlStripCharFilterDescriptor, IHtmlStripCharFilter> selector = null)
    {
      return this.AssignIfNotNull((ICharFilter) selector.InvokeOrDefault<HtmlStripCharFilterDescriptor, IHtmlStripCharFilter>(new HtmlStripCharFilterDescriptor()));
    }

    public AnalyzeCharFiltersDescriptor Mapping(
      Func<MappingCharFilterDescriptor, IMappingCharFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ICharFilter) selector(new MappingCharFilterDescriptor()) : (ICharFilter) null);
    }

    public AnalyzeCharFiltersDescriptor KuromojiIterationMark(
      Func<KuromojiIterationMarkCharFilterDescriptor, IKuromojiIterationMarkCharFilter> selector = null)
    {
      return this.AssignIfNotNull(selector != null ? (ICharFilter) selector.InvokeOrDefault<KuromojiIterationMarkCharFilterDescriptor, IKuromojiIterationMarkCharFilter>(new KuromojiIterationMarkCharFilterDescriptor()) : (ICharFilter) null);
    }

    public AnalyzeCharFiltersDescriptor IcuNormalization(
      Func<IcuNormalizationCharFilterDescriptor, IIcuNormalizationCharFilter> selector)
    {
      return this.AssignIfNotNull(selector != null ? (ICharFilter) selector(new IcuNormalizationCharFilterDescriptor()) : (ICharFilter) null);
    }
  }
}
