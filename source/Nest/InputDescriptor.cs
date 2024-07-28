// Decompiled with JetBrains decompiler
// Type: Nest.InputDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;

namespace Nest
{
  public class InputDescriptor : InputContainer
  {
    private InputDescriptor Assign<TValue>(TValue value, Action<IInputContainer, TValue> assigner) => Fluent.Assign<InputDescriptor, IInputContainer, TValue>(this, value, assigner);

    public InputDescriptor Search(Func<SearchInputDescriptor, ISearchInput> selector) => this.Assign<Func<SearchInputDescriptor, ISearchInput>>(selector, (Action<IInputContainer, Func<SearchInputDescriptor, ISearchInput>>) ((a, v) => a.Search = v(new SearchInputDescriptor())));

    public InputDescriptor Http(Func<HttpInputDescriptor, IHttpInput> selector) => this.Assign<Func<HttpInputDescriptor, IHttpInput>>(selector, (Action<IInputContainer, Func<HttpInputDescriptor, IHttpInput>>) ((a, v) => a.Http = v(new HttpInputDescriptor())));

    public InputDescriptor Simple(Func<SimpleInputDescriptor, ISimpleInput> selector) => this.Assign<Func<SimpleInputDescriptor, ISimpleInput>>(selector, (Action<IInputContainer, Func<SimpleInputDescriptor, ISimpleInput>>) ((a, v) => a.Simple = v(new SimpleInputDescriptor())));

    public InputDescriptor Chain(Func<ChainInputDescriptor, IChainInput> selector) => this.Assign<Func<ChainInputDescriptor, IChainInput>>(selector, (Action<IInputContainer, Func<ChainInputDescriptor, IChainInput>>) ((a, v) => a.Chain = v(new ChainInputDescriptor())));
  }
}
