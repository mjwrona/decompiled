// Decompiled with JetBrains decompiler
// Type: Nest.SearchInputRequestDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using System;
using System.Collections.Generic;

namespace Nest
{
  public class SearchInputRequestDescriptor : 
    DescriptorBase<SearchInputRequestDescriptor, ISearchInputRequest>,
    ISearchInputRequest
  {
    ISearchRequest ISearchInputRequest.Body { get; set; }

    IEnumerable<IndexName> ISearchInputRequest.Indices { get; set; }

    IIndicesOptions ISearchInputRequest.IndicesOptions { get; set; }

    Elasticsearch.Net.SearchType? ISearchInputRequest.SearchType { get; set; }

    ISearchTemplateRequest ISearchInputRequest.Template { get; set; }

    public SearchInputRequestDescriptor Indices(IEnumerable<IndexName> indices) => this.Assign<IEnumerable<IndexName>>(indices, (Action<ISearchInputRequest, IEnumerable<IndexName>>) ((a, v) => a.Indices = v));

    public SearchInputRequestDescriptor Indices(params IndexName[] indices) => this.Assign<IndexName[]>(indices, (Action<ISearchInputRequest, IndexName[]>) ((a, v) => a.Indices = (IEnumerable<IndexName>) v));

    public SearchInputRequestDescriptor Indices<T>() => this.Assign<IndexName[]>(new IndexName[1]
    {
      (IndexName) typeof (T)
    }, (Action<ISearchInputRequest, IndexName[]>) ((a, v) => a.Indices = (IEnumerable<IndexName>) v));

    public SearchInputRequestDescriptor SearchType(Elasticsearch.Net.SearchType? searchType) => this.Assign<Elasticsearch.Net.SearchType?>(searchType, (Action<ISearchInputRequest, Elasticsearch.Net.SearchType?>) ((a, v) => a.SearchType = v));

    public SearchInputRequestDescriptor IndicesOptions(
      Func<IndicesOptionsDescriptor, IIndicesOptions> selector)
    {
      return this.Assign<IIndicesOptions>(selector(new IndicesOptionsDescriptor()), (Action<ISearchInputRequest, IIndicesOptions>) ((a, v) => a.IndicesOptions = v));
    }

    public SearchInputRequestDescriptor Body<T>(Func<SearchDescriptor<T>, ISearchRequest> selector) where T : class => this.Assign<Func<SearchDescriptor<T>, ISearchRequest>>(selector, (Action<ISearchInputRequest, Func<SearchDescriptor<T>, ISearchRequest>>) ((a, v) => a.Body = v != null ? v.InvokeOrDefault<SearchDescriptor<T>, ISearchRequest>(new SearchDescriptor<T>()) : (ISearchRequest) null));

    public SearchInputRequestDescriptor Template<T>(
      Func<SearchTemplateDescriptor<T>, ISearchTemplateRequest> selector)
      where T : class
    {
      return this.Assign<Func<SearchTemplateDescriptor<T>, ISearchTemplateRequest>>(selector, (Action<ISearchInputRequest, Func<SearchTemplateDescriptor<T>, ISearchTemplateRequest>>) ((a, v) => a.Template = v != null ? v.InvokeOrDefault<SearchTemplateDescriptor<T>, ISearchTemplateRequest>(new SearchTemplateDescriptor<T>()) : (ISearchTemplateRequest) null));
    }
  }
}
