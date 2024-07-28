// Decompiled with JetBrains decompiler
// Type: Nest.ShingleTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class ShingleTokenFilter : TokenFilterBase, IShingleTokenFilter, ITokenFilter
  {
    public ShingleTokenFilter()
      : base("shingle")
    {
    }

    public string FillerToken { get; set; }

    public int? MaxShingleSize { get; set; }

    public int? MinShingleSize { get; set; }

    public bool? OutputUnigrams { get; set; }

    public bool? OutputUnigramsIfNoShingles { get; set; }

    public string TokenSeparator { get; set; }
  }
}
