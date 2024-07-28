// Decompiled with JetBrains decompiler
// Type: Nest.IcuTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class IcuTokenizerDescriptor : 
    TokenizerDescriptorBase<IcuTokenizerDescriptor, IIcuTokenizer>,
    IIcuTokenizer,
    ITokenizer
  {
    protected override string Type => "icu_tokenizer";

    string IIcuTokenizer.RuleFiles { get; set; }

    public IcuTokenizerDescriptor RuleFiles(string ruleFiles) => this.Assign<string>(ruleFiles, (Action<IIcuTokenizer, string>) ((a, v) => a.RuleFiles = v));
  }
}
