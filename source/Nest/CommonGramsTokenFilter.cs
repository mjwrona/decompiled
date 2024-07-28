// Decompiled with JetBrains decompiler
// Type: Nest.CommonGramsTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class CommonGramsTokenFilter : TokenFilterBase, ICommonGramsTokenFilter, ITokenFilter
  {
    public CommonGramsTokenFilter()
      : base("common_grams")
    {
    }

    public IEnumerable<string> CommonWords { get; set; }

    public string CommonWordsPath { get; set; }

    public bool? IgnoreCase { get; set; }

    public bool? QueryMode { get; set; }
  }
}
