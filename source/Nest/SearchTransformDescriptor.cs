// Decompiled with JetBrains decompiler
// Type: Nest.SearchTransformDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class SearchTransformDescriptor : 
    DescriptorBase<SearchTransformDescriptor, ISearchTransform>,
    ISearchTransform,
    ITransform
  {
    ISearchInputRequest ISearchTransform.Request { get; set; }

    Time ISearchTransform.Timeout { get; set; }

    public SearchTransformDescriptor Request(
      Func<SearchInputRequestDescriptor, ISearchInputRequest> selector)
    {
      return this.Assign<ISearchInputRequest>(selector.InvokeOrDefault<SearchInputRequestDescriptor, ISearchInputRequest>(new SearchInputRequestDescriptor()), (Action<ISearchTransform, ISearchInputRequest>) ((a, v) => a.Request = v));
    }

    public SearchTransformDescriptor Timeout(Time timeout) => this.Assign<Time>(timeout, (Action<ISearchTransform, Time>) ((a, v) => a.Timeout = v));
  }
}
