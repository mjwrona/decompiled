// Decompiled with JetBrains decompiler
// Type: Nest.HunspellTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class HunspellTokenFilter : TokenFilterBase, IHunspellTokenFilter, ITokenFilter
  {
    public HunspellTokenFilter()
      : base("hunspell")
    {
    }

    public bool? Dedup { get; set; }

    public string Dictionary { get; set; }

    public string Locale { get; set; }

    public bool? LongestOnly { get; set; }
  }
}
