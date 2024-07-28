// Decompiled with JetBrains decompiler
// Type: Nest.SimplePatternTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SimplePatternTokenizerDescriptor : 
    TokenizerDescriptorBase<SimplePatternTokenizerDescriptor, ISimplePatternTokenizer>,
    ISimplePatternTokenizer,
    ITokenizer
  {
    protected override string Type => "simple_pattern";

    string ISimplePatternTokenizer.Pattern { get; set; }

    public SimplePatternTokenizerDescriptor Pattern(string pattern) => this.Assign<string>(pattern, (Action<ISimplePatternTokenizer, string>) ((a, v) => a.Pattern = v));
  }
}
