// Decompiled with JetBrains decompiler
// Type: Nest.SynonymTokenFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class SynonymTokenFilter : TokenFilterBase, ISynonymTokenFilter, ITokenFilter
  {
    public SynonymTokenFilter()
      : base("synonym")
    {
    }

    public bool? Expand { get; set; }

    public SynonymFormat? Format { get; set; }

    public bool? Lenient { get; set; }

    public IEnumerable<string> Synonyms { get; set; }

    public string SynonymsPath { get; set; }

    public string Tokenizer { get; set; }

    public bool? Updateable { get; set; }
  }
}
