// Decompiled with JetBrains decompiler
// Type: Nest.TextIndexPrefixesDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class TextIndexPrefixesDescriptor : 
    DescriptorBase<TextIndexPrefixesDescriptor, ITextIndexPrefixes>,
    ITextIndexPrefixes
  {
    int? ITextIndexPrefixes.MaxCharacters { get; set; }

    int? ITextIndexPrefixes.MinCharacters { get; set; }

    public TextIndexPrefixesDescriptor MinCharacters(int? min) => this.Assign<int?>(min, (Action<ITextIndexPrefixes, int?>) ((a, v) => a.MinCharacters = v));

    public TextIndexPrefixesDescriptor MaxCharacters(int? max) => this.Assign<int?>(max, (Action<ITextIndexPrefixes, int?>) ((a, v) => a.MaxCharacters = v));
  }
}
