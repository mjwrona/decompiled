// Decompiled with JetBrains decompiler
// Type: Nest.BulkDescriptor
// Assembly: Nest, Version=7.0.0.0, Culture=neutral, PublicKeyToken=96c599bbe3e70f5d
// MVID: CCE7C15C-052B-4528-A6A5-137560B7864B
// Assembly location: C:\Program Files\Azure DevOps Server 2022\Application Tier\Web Services\bin\Nest.dll

using Elasticsearch.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Nest
{
  public class BulkDescriptor : 
    RequestDescriptorBase<BulkDescriptor, BulkRequestParameters, IBulkRequest>,
    IBulkRequest,
    IRequest<BulkRequestParameters>,
    IRequest
  {
    internal override ApiUrls ApiUrls => ApiUrlsLookups.NoNamespaceBulk;

    public BulkDescriptor()
    {
    }

    public BulkDescriptor(IndexName index)
      : base((Func<RouteValues, RouteValues>) (r => r.Optional(nameof (index), (IUrlParameter) index)))
    {
    }

    IndexName IBulkRequest.Index => this.Self.RouteValues.Get<IndexName>("index");

    public BulkDescriptor Index(IndexName index) => this.Assign<IndexName>(index, (Action<IBulkRequest, IndexName>) ((a, v) => a.RouteValues.Optional(nameof (index), (IUrlParameter) v)));

    public BulkDescriptor Index<TOther>() where TOther : class => this.Assign<Type>(typeof (TOther), (Action<IBulkRequest, Type>) ((a, v) => a.RouteValues.Optional("index", (IUrlParameter) (IndexName) v)));

    public BulkDescriptor Pipeline(string pipeline) => this.Qs(nameof (pipeline), (object) pipeline);

    public BulkDescriptor Refresh(Elasticsearch.Net.Refresh? refresh) => this.Qs(nameof (refresh), (object) refresh);

    public BulkDescriptor RequireAlias(bool? requirealias = true) => this.Qs("require_alias", (object) requirealias);

    public BulkDescriptor Routing(Nest.Routing routing) => this.Qs(nameof (routing), (object) routing);

    public BulkDescriptor SourceEnabled(bool? sourceenabled = true) => this.Qs("_source", (object) sourceenabled);

    public BulkDescriptor SourceExcludes(Fields sourceexcludes) => this.Qs("_source_excludes", (object) sourceexcludes);

    public BulkDescriptor SourceExcludes<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs("_source_excludes", fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public BulkDescriptor SourceIncludes(Fields sourceincludes) => this.Qs("_source_includes", (object) sourceincludes);

    public BulkDescriptor SourceIncludes<T>(params Expression<Func<T, object>>[] fields) where T : class => this.Qs("_source_includes", fields != null ? (object) ((IEnumerable<Expression<Func<T, object>>>) fields).Select<Expression<Func<T, object>>, Field>((Func<Expression<Func<T, object>>, Field>) (e => (Field) (Expression) e)) : (object) (IEnumerable<Field>) null);

    public BulkDescriptor Timeout(Time timeout) => this.Qs(nameof (timeout), (object) timeout);

    public BulkDescriptor TypeQueryString(string typequerystring) => this.Qs("type", (object) typequerystring);

    public BulkDescriptor WaitForActiveShards(string waitforactiveshards) => this.Qs("wait_for_active_shards", (object) waitforactiveshards);

    BulkOperationsCollection<IBulkOperation> IBulkRequest.Operations { get; set; } = new BulkOperationsCollection<IBulkOperation>();

    public BulkDescriptor Create<T>(
      Func<BulkCreateDescriptor<T>, IBulkCreateOperation<T>> bulkCreateSelector)
      where T : class
    {
      return this.AddOperation(bulkCreateSelector != null ? (IBulkOperation) bulkCreateSelector(new BulkCreateDescriptor<T>()) : (IBulkOperation) null);
    }

    public BulkDescriptor CreateMany<T>(
      IEnumerable<T> objects,
      Func<BulkCreateDescriptor<T>, T, IBulkCreateOperation<T>> bulkCreateSelector = null)
      where T : class
    {
      return this.AddOperations<T, BulkCreateDescriptor<T>, IBulkCreateOperation<T>>(objects, bulkCreateSelector, (Func<T, BulkCreateDescriptor<T>>) (o => new BulkCreateDescriptor<T>().Document(o)));
    }

    public BulkDescriptor Index<T>(
      Func<BulkIndexDescriptor<T>, IBulkIndexOperation<T>> bulkIndexSelector)
      where T : class
    {
      return this.AddOperation(bulkIndexSelector != null ? (IBulkOperation) bulkIndexSelector(new BulkIndexDescriptor<T>()) : (IBulkOperation) null);
    }

    public BulkDescriptor IndexMany<T>(
      IEnumerable<T> objects,
      Func<BulkIndexDescriptor<T>, T, IBulkIndexOperation<T>> bulkIndexSelector = null)
      where T : class
    {
      return this.AddOperations<T, BulkIndexDescriptor<T>, IBulkIndexOperation<T>>(objects, bulkIndexSelector, (Func<T, BulkIndexDescriptor<T>>) (o => new BulkIndexDescriptor<T>().Document(o)));
    }

    public BulkDescriptor DeleteMany<T>(
      IEnumerable<T> objects,
      Func<BulkDeleteDescriptor<T>, T, IBulkDeleteOperation<T>> bulkDeleteSelector = null)
      where T : class
    {
      return this.AddOperations<T, BulkDeleteDescriptor<T>, IBulkDeleteOperation<T>>(objects, bulkDeleteSelector, (Func<T, BulkDeleteDescriptor<T>>) (o => new BulkDeleteDescriptor<T>().Document(o)));
    }

    public BulkDescriptor DeleteMany<T>(
      IEnumerable<string> ids,
      Func<BulkDeleteDescriptor<T>, string, IBulkDeleteOperation<T>> bulkDeleteSelector = null)
      where T : class
    {
      return this.AddOperations<string, BulkDeleteDescriptor<T>, IBulkDeleteOperation<T>>(ids, bulkDeleteSelector, (Func<string, BulkDeleteDescriptor<T>>) (id => new BulkDeleteDescriptor<T>().Id((Id) id)));
    }

    public BulkDescriptor DeleteMany<T>(
      IEnumerable<long> ids,
      Func<BulkDeleteDescriptor<T>, long, IBulkDeleteOperation<T>> bulkDeleteSelector = null)
      where T : class
    {
      return this.AddOperations<long, BulkDeleteDescriptor<T>, IBulkDeleteOperation<T>>(ids, bulkDeleteSelector, (Func<long, BulkDeleteDescriptor<T>>) (id => new BulkDeleteDescriptor<T>().Id((Id) id)));
    }

    public BulkDescriptor Delete<T>(
      T obj,
      Func<BulkDeleteDescriptor<T>, IBulkDeleteOperation<T>> bulkDeleteSelector = null)
      where T : class
    {
      return this.AddOperation((IBulkOperation) bulkDeleteSelector.InvokeOrDefault<BulkDeleteDescriptor<T>, IBulkDeleteOperation<T>>(new BulkDeleteDescriptor<T>().Document(obj)));
    }

    public BulkDescriptor Delete<T>(
      Func<BulkDeleteDescriptor<T>, IBulkDeleteOperation<T>> bulkDeleteSelector)
      where T : class
    {
      return this.AddOperation(bulkDeleteSelector != null ? (IBulkOperation) bulkDeleteSelector(new BulkDeleteDescriptor<T>()) : (IBulkOperation) null);
    }

    public BulkDescriptor UpdateMany<T>(
      IEnumerable<T> objects,
      Func<BulkUpdateDescriptor<T, T>, T, IBulkUpdateOperation<T, T>> bulkUpdateSelector)
      where T : class
    {
      return this.AddOperations<T, BulkUpdateDescriptor<T, T>, IBulkUpdateOperation<T, T>>(objects, bulkUpdateSelector, (Func<T, BulkUpdateDescriptor<T, T>>) (o => new BulkUpdateDescriptor<T, T>().IdFrom(o)));
    }

    public BulkDescriptor UpdateMany<T, TPartialDocument>(
      IEnumerable<T> objects,
      Func<BulkUpdateDescriptor<T, TPartialDocument>, T, IBulkUpdateOperation<T, TPartialDocument>> bulkUpdateSelector)
      where T : class
      where TPartialDocument : class
    {
      return this.AddOperations<T, BulkUpdateDescriptor<T, TPartialDocument>, IBulkUpdateOperation<T, TPartialDocument>>(objects, bulkUpdateSelector, (Func<T, BulkUpdateDescriptor<T, TPartialDocument>>) (o => new BulkUpdateDescriptor<T, TPartialDocument>().IdFrom(o)));
    }

    public BulkDescriptor Update<T>(
      Func<BulkUpdateDescriptor<T, T>, IBulkUpdateOperation<T, T>> bulkUpdateSelector)
      where T : class
    {
      return this.Update<T, T>(bulkUpdateSelector);
    }

    public BulkDescriptor Update<T, TPartialDocument>(
      Func<BulkUpdateDescriptor<T, TPartialDocument>, IBulkUpdateOperation<T, TPartialDocument>> bulkUpdateSelector)
      where T : class
      where TPartialDocument : class
    {
      return this.AddOperation(bulkUpdateSelector != null ? (IBulkOperation) bulkUpdateSelector(new BulkUpdateDescriptor<T, TPartialDocument>()) : (IBulkOperation) null);
    }

    public BulkDescriptor AddOperation(IBulkOperation operation) => this.Assign<IBulkOperation>(operation, (Action<IBulkRequest, IBulkOperation>) ((a, v) => a.Operations.AddIfNotNull<IBulkOperation>(v)));

    private BulkDescriptor AddOperations<T, TDescriptor, TInterface>(
      IEnumerable<T> objects,
      Func<TDescriptor, T, TInterface> bulkIndexSelector,
      Func<T, TDescriptor> defaultSelector)
      where TDescriptor : class, TInterface
      where TInterface : class, IBulkOperation
    {
      if (objects == null)
        return this;
      List<T> list = objects.ToList<T>();
      List<TInterface> interfaceList = new List<TInterface>(list.Count<T>());
      foreach (T obj in list)
      {
        TInterface @interface = bulkIndexSelector.InvokeOrDefault<TDescriptor, T, TInterface>(defaultSelector(obj), obj);
        if ((object) @interface != null)
          interfaceList.Add(@interface);
      }
      return this.Assign<List<TInterface>>(interfaceList, (Action<IBulkRequest, List<TInterface>>) ((a, v) => a.Operations.AddRange((IEnumerable<IBulkOperation>) v)));
    }
  }
}
