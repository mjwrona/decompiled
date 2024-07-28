// Decompiled with JetBrains decompiler
// Type: Nest.PatternTokenizer
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

namespace Nest
{
  public class PatternTokenizer : TokenizerBase, IPatternTokenizer, ITokenizer
  {
    public PatternTokenizer() => this.Type = "pattern";

    public string Flags { get; set; }

    public int? Group { get; set; }

    public string Pattern { get; set; }
  }
}
