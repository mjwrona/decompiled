// Decompiled with JetBrains decompiler
// Type: Nest.CharFiltersDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class CharFiltersDescriptor : 
    IsADictionaryDescriptorBase<CharFiltersDescriptor, ICharFilters, string, ICharFilter>
  {
    public CharFiltersDescriptor()
      : base((ICharFilters) new CharFilters())
    {
    }

    public CharFiltersDescriptor UserDefined(string name, ICharFilter analyzer) => this.Assign(name, analyzer);

    public CharFiltersDescriptor PatternReplace(
      string name,
      Func<PatternReplaceCharFilterDescriptor, IPatternReplaceCharFilter> selector)
    {
      return this.Assign(name, selector != null ? (ICharFilter) selector(new PatternReplaceCharFilterDescriptor()) : (ICharFilter) null);
    }

    public CharFiltersDescriptor HtmlStrip(
      string name,
      Func<HtmlStripCharFilterDescriptor, IHtmlStripCharFilter> selector = null)
    {
      return this.Assign(name, (ICharFilter) selector.InvokeOrDefault<HtmlStripCharFilterDescriptor, IHtmlStripCharFilter>(new HtmlStripCharFilterDescriptor()));
    }

    public CharFiltersDescriptor Mapping(
      string name,
      Func<MappingCharFilterDescriptor, IMappingCharFilter> selector)
    {
      return this.Assign(name, selector != null ? (ICharFilter) selector(new MappingCharFilterDescriptor()) : (ICharFilter) null);
    }

    public CharFiltersDescriptor KuromojiIterationMark(
      string name,
      Func<KuromojiIterationMarkCharFilterDescriptor, IKuromojiIterationMarkCharFilter> selector = null)
    {
      return this.Assign(name, selector != null ? (ICharFilter) selector.InvokeOrDefault<KuromojiIterationMarkCharFilterDescriptor, IKuromojiIterationMarkCharFilter>(new KuromojiIterationMarkCharFilterDescriptor()) : (ICharFilter) null);
    }

    public CharFiltersDescriptor IcuNormalization(
      string name,
      Func<IcuNormalizationCharFilterDescriptor, IIcuNormalizationCharFilter> selector)
    {
      return this.Assign(name, selector != null ? (ICharFilter) selector(new IcuNormalizationCharFilterDescriptor()) : (ICharFilter) null);
    }
  }
}
