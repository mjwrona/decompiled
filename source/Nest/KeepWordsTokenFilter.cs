// Decompiled with JetBrains decompiler
// Type: Nest.KeepWordsTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class KeepWordsTokenFilter : TokenFilterBase, IKeepWordsTokenFilter, ITokenFilter
  {
    public KeepWordsTokenFilter()
      : base("keep")
    {
    }

    public IEnumerable<string> KeepWords { get; set; }

    public bool? KeepWordsCase { get; set; }

    public string KeepWordsPath { get; set; }
  }
}
