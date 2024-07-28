// Decompiled with JetBrains decompiler
// Type: Nest.SearchInputDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SearchInputDescriptor : 
    DescriptorBase<SearchInputDescriptor, ISearchInput>,
    ISearchInput,
    IInput
  {
    IEnumerable<string> ISearchInput.Extract { get; set; }

    ISearchInputRequest ISearchInput.Request { get; set; }

    Time ISearchInput.Timeout { get; set; }

    public SearchInputDescriptor Request(
      Func<SearchInputRequestDescriptor, ISearchInputRequest> selector)
    {
      return this.Assign<Func<SearchInputRequestDescriptor, ISearchInputRequest>>(selector, (Action<ISearchInput, Func<SearchInputRequestDescriptor, ISearchInputRequest>>) ((a, v) => a.Request = v != null ? v.InvokeOrDefault<SearchInputRequestDescriptor, ISearchInputRequest>(new SearchInputRequestDescriptor()) : (ISearchInputRequest) null));
    }

    public SearchInputDescriptor Extract(IEnumerable<string> extract) => this.Assign<IEnumerable<string>>(extract, (Action<ISearchInput, IEnumerable<string>>) ((a, v) => a.Extract = v));

    public SearchInputDescriptor Extract(params string[] extract) => this.Assign<string[]>(extract, (Action<ISearchInput, string[]>) ((a, v) => a.Extract = (IEnumerable<string>) v));

    public SearchInputDescriptor Timeout(Time timeout) => this.Assign<Time>(timeout, (Action<ISearchInput, Time>) ((a, v) => a.Timeout = v));
  }
}
