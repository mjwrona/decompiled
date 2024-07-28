// Decompiled with JetBrains decompiler
// Type: Nest.StandardTokenizerDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class StandardTokenizerDescriptor : 
    TokenizerDescriptorBase<StandardTokenizerDescriptor, IStandardTokenizer>,
    IStandardTokenizer,
    ITokenizer
  {
    protected override string Type => "standard";

    int? IStandardTokenizer.MaxTokenLength { get; set; }

    public StandardTokenizerDescriptor MaxTokenLength(int? maxLength) => this.Assign<int?>(maxLength, (Action<IStandardTokenizer, int?>) ((a, v) => a.MaxTokenLength = v));
  }
}
