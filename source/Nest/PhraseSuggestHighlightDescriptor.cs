// Decompiled with JetBrains decompiler
// Type: Nest.PhraseSuggestHighlightDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class PhraseSuggestHighlightDescriptor : 
    DescriptorBase<PhraseSuggestHighlightDescriptor, IPhraseSuggestHighlight>,
    IPhraseSuggestHighlight
  {
    string IPhraseSuggestHighlight.PostTag { get; set; }

    string IPhraseSuggestHighlight.PreTag { get; set; }

    public PhraseSuggestHighlightDescriptor PreTag(string preTag) => this.Assign<string>(preTag, (Action<IPhraseSuggestHighlight, string>) ((a, v) => a.PreTag = v));

    public PhraseSuggestHighlightDescriptor PostTag(string postTag) => this.Assign<string>(postTag, (Action<IPhraseSuggestHighlight, string>) ((a, v) => a.PostTag = v));
  }
}
