// Decompiled with JetBrains decompiler
// Type: Nest.ElisionTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class ElisionTokenFilter : TokenFilterBase, IElisionTokenFilter, ITokenFilter
  {
    public ElisionTokenFilter()
      : base("elision")
    {
    }

    public IEnumerable<string> Articles { get; set; }

    public bool? ArticlesCase { get; set; }
  }
}
