// Decompiled with JetBrains decompiler
// Type: Nest.DelimitedPayloadTokenFilterDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class DelimitedPayloadTokenFilterDescriptor : 
    TokenFilterDescriptorBase<DelimitedPayloadTokenFilterDescriptor, IDelimitedPayloadTokenFilter>,
    IDelimitedPayloadTokenFilter,
    ITokenFilter
  {
    protected override string Type => "delimited_payload";

    char? IDelimitedPayloadTokenFilter.Delimiter { get; set; }

    DelimitedPayloadEncoding? IDelimitedPayloadTokenFilter.Encoding { get; set; }

    public DelimitedPayloadTokenFilterDescriptor Delimiter(char? delimiter) => this.Assign<char?>(delimiter, (Action<IDelimitedPayloadTokenFilter, char?>) ((a, v) => a.Delimiter = v));

    public DelimitedPayloadTokenFilterDescriptor Encoding(DelimitedPayloadEncoding? encoding) => this.Assign<DelimitedPayloadEncoding?>(encoding, (Action<IDelimitedPayloadTokenFilter, DelimitedPayloadEncoding?>) ((a, v) => a.Encoding = v));
  }
}
