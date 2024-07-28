// Decompiled with JetBrains decompiler
// Type: Nest.SuggestFuzziness
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class SuggestFuzziness : ISuggestFuzziness
  {
    public IFuzziness Fuzziness { get; set; }

    public int? MinLength { get; set; }

    public int? PrefixLength { get; set; }

    public bool? Transpositions { get; set; }

    public bool? UnicodeAware { get; set; }
  }
}
