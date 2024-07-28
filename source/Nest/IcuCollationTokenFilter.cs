// Decompiled with JetBrains decompiler
// Type: Nest.IcuCollationTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class IcuCollationTokenFilter : TokenFilterBase, IIcuCollationTokenFilter, ITokenFilter
  {
    public IcuCollationTokenFilter()
      : base("icu_collation")
    {
    }

    public IcuCollationAlternate? Alternate { get; set; }

    public IcuCollationCaseFirst? CaseFirst { get; set; }

    public bool? CaseLevel { get; set; }

    public string Country { get; set; }

    public IcuCollationDecomposition? Decomposition { get; set; }

    public bool? HiraganaQuaternaryMode { get; set; }

    public string Language { get; set; }

    public bool? Numeric { get; set; }

    public IcuCollationStrength? Strength { get; set; }

    public string VariableTop { get; set; }

    public string Variant { get; set; }
  }
}
