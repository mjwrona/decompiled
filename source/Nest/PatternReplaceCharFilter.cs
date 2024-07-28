// Decompiled with JetBrains decompiler
// Type: Nest.PatternReplaceCharFilter
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class PatternReplaceCharFilter : CharFilterBase, IPatternReplaceCharFilter, ICharFilter
  {
    public PatternReplaceCharFilter()
      : base("pattern_replace")
    {
    }

    public string Flags { get; set; }

    public string Pattern { get; set; }

    public string Replacement { get; set; }
  }
}
