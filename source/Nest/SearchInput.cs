// Decompiled with JetBrains decompiler
// Type: Nest.SearchInput
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System.Collections.Generic;

namespace Nest
{
  public class SearchInput : InputBase, ISearchInput, IInput
  {
    public IEnumerable<string> Extract { get; set; }

    public ISearchInputRequest Request { get; set; }

    public Time Timeout { get; set; }

    internal override void WrapInContainer(IInputContainer container) => container.Search = (ISearchInput) this;
  }
}
