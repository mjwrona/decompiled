// Decompiled with JetBrains decompiler
// Type: Nest.IcuCollationTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IcuCollationTokenFilterDescriptor : 
    TokenFilterDescriptorBase<IcuCollationTokenFilterDescriptor, IIcuCollationTokenFilter>,
    IIcuCollationTokenFilter,
    ITokenFilter
  {
    protected override string Type => "icu_collation";

    IcuCollationAlternate? IIcuCollationTokenFilter.Alternate { get; set; }

    IcuCollationCaseFirst? IIcuCollationTokenFilter.CaseFirst { get; set; }

    bool? IIcuCollationTokenFilter.CaseLevel { get; set; }

    string IIcuCollationTokenFilter.Country { get; set; }

    IcuCollationDecomposition? IIcuCollationTokenFilter.Decomposition { get; set; }

    bool? IIcuCollationTokenFilter.HiraganaQuaternaryMode { get; set; }

    string IIcuCollationTokenFilter.Language { get; set; }

    bool? IIcuCollationTokenFilter.Numeric { get; set; }

    IcuCollationStrength? IIcuCollationTokenFilter.Strength { get; set; }

    string IIcuCollationTokenFilter.VariableTop { get; set; }

    string IIcuCollationTokenFilter.Variant { get; set; }

    public IcuCollationTokenFilterDescriptor Language(string language) => this.Assign<string>(language, (Action<IIcuCollationTokenFilter, string>) ((a, v) => a.Language = v));

    public IcuCollationTokenFilterDescriptor Country(string country) => this.Assign<string>(country, (Action<IIcuCollationTokenFilter, string>) ((a, v) => a.Country = v));

    public IcuCollationTokenFilterDescriptor Variant(string variant) => this.Assign<string>(variant, (Action<IIcuCollationTokenFilter, string>) ((a, v) => a.Variant = v));

    public IcuCollationTokenFilterDescriptor Strength(IcuCollationStrength? strength) => this.Assign<IcuCollationStrength?>(strength, (Action<IIcuCollationTokenFilter, IcuCollationStrength?>) ((a, v) => a.Strength = v));

    public IcuCollationTokenFilterDescriptor Decomposition(IcuCollationDecomposition? decomposition) => this.Assign<IcuCollationDecomposition?>(decomposition, (Action<IIcuCollationTokenFilter, IcuCollationDecomposition?>) ((a, v) => a.Decomposition = v));

    public IcuCollationTokenFilterDescriptor Alternate(IcuCollationAlternate? alternate) => this.Assign<IcuCollationAlternate?>(alternate, (Action<IIcuCollationTokenFilter, IcuCollationAlternate?>) ((a, v) => a.Alternate = v));

    public IcuCollationTokenFilterDescriptor CaseFirst(IcuCollationCaseFirst? caseFirst) => this.Assign<IcuCollationCaseFirst?>(caseFirst, (Action<IIcuCollationTokenFilter, IcuCollationCaseFirst?>) ((a, v) => a.CaseFirst = v));

    public IcuCollationTokenFilterDescriptor CaseLevel(bool? caseLevel = true) => this.Assign<bool?>(caseLevel, (Action<IIcuCollationTokenFilter, bool?>) ((a, v) => a.CaseLevel = v));

    public IcuCollationTokenFilterDescriptor Numeric(bool? numeric = true) => this.Assign<bool?>(numeric, (Action<IIcuCollationTokenFilter, bool?>) ((a, v) => a.Numeric = v));

    public IcuCollationTokenFilterDescriptor HiraganaQuaternaryMode(bool? mode = true) => this.Assign<bool?>(mode, (Action<IIcuCollationTokenFilter, bool?>) ((a, v) => a.HiraganaQuaternaryMode = v));

    public IcuCollationTokenFilterDescriptor VariableTop(string variableTop) => this.Assign<string>(variableTop, (Action<IIcuCollationTokenFilter, string>) ((a, v) => a.VariableTop = v));
  }
}
